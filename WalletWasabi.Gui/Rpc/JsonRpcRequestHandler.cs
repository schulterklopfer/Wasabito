using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WalletWasabi.Gui.Rpc
{
	///<summary>
	/// This class coordinates all the major steps in processing the RPC call.
	/// It parses the json request, parses the parameters, invoke the service
	/// methods and handles the errors.
	///</summary>
	public class JsonRpcRequestHandler
	{
		private readonly JsonRpcService _service;

		public JsonRpcRequestHandler(JsonRpcService service)
		{
			_service = service;
		}

		public async Task<string> HandleAsync(string body, CancellationTokenSource cts)
		{
			JsonRpcResponse response = null;

			if(!JsonRpcRequest.TryParse(body, out var jsonRpcRequest))
			{
				return new JsonRpcErrorResponse(JsonRpcErrorCodes.ParseError).ToJson();
			}
			var methodName = jsonRpcRequest.Method;

			if(!_service.TryGetMetadata(methodName, out var prodecureMetadata))
			{
				return Error(JsonRpcErrorCodes.MethodNotFound, $"'{methodName}' method not found.", jsonRpcRequest.Id);
			}

			try
			{
				var methodParameters = prodecureMetadata.Parameters;
				var parameters = new List<object>();

				if (jsonRpcRequest.Parameters is JArray jarr)
				{
					var count = methodParameters.Count < jarr.Count ? methodParameters.Count : jarr.Count;  
					for (int i = 0; i < count; i++)
					{
						parameters.Add( jarr[i].ToObject(methodParameters[i].type) );
					}
				}
				else if (jsonRpcRequest.Parameters is JObject jobj)
				{
					for (int i = 0; i < methodParameters.Count; i++)
					{
						var param = methodParameters[i];
						if(!jobj.ContainsKey(param.name))
						{
							return Error(JsonRpcErrorCodes.InvalidParams, 
								$"A value for the '{param.name}' is missing.", jsonRpcRequest.Id);
						}
						parameters.Add( jobj[param.name].ToObject(param.type));
					}
				}

				// Special case: if there is a missing parameter and the procedure is expecting a CancellationTokenSource
				// then pass the cts we have. This will allow us to cancel async requests when the server is stopped. 
				if (parameters.Count == methodParameters.Count -1)
				{
					var position = methodParameters.FindIndex(x=>x.type == typeof(CancellationTokenSource));
					if(position > -1)
					{
						parameters.Insert(position, cts);
					}
				}
				if (parameters.Count != methodParameters.Count)
				{
					return Error(JsonRpcErrorCodes.InvalidParams, 
						$"{methodParameters.Count} parameters were expected but {parameters.Count} were received.", jsonRpcRequest.Id);
				}
				var result =  prodecureMetadata.MethodInfo.Invoke(_service, parameters.ToArray());

				if (jsonRpcRequest.IsNotification) // the client is not interested in getting a response
				{
					return string.Empty;
				}

				response = prodecureMetadata.MethodInfo.IsAsync()
					? await (Task<JsonRpcResponse>)result
					: (JsonRpcResponse)result;
				response = response ?? new JsonRpcResponse(); // for methdos that return `void`
				response.Id = jsonRpcRequest.Id;
				return response.ToJson();
			}
			catch(Exception)
			{
				return Error(JsonRpcErrorCodes.InternalError, null, jsonRpcRequest.Id);
			}
		}

		private string Error(JsonRpcErrorCodes code, string reason, string id)
		{
			return id == null ? string.Empty : (new JsonRpcErrorResponse(code, reason, id).ToJson());
		}
	}

	internal static class MethodInfoExtensions
	{
		public static bool IsAsync(this MethodInfo mi)
		{
			Type attType = typeof(AsyncStateMachineAttribute);

			var attrib = (AsyncStateMachineAttribute)mi.GetCustomAttribute(attType);

			return (attrib != null);
		}
	}
}