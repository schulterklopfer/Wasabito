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
	public class JsonRpcRequestHandler
	{
		private JsonRpcService _service;
		private Dictionary<string, (string name, string description, MethodInfo methodInfo)> _methodsMap = 
				new Dictionary<string, (string name, string description, MethodInfo methodInfo)>();

		public JsonRpcRequestHandler(JsonRpcService service)
		{
			_service = service;
		}

		public async Task<string> HandleAsync(string body, CancellationTokenSource cts)
		{
			JsonRpcResponse response = null;

			if(!JsonRpcRequest.TryParse(body, out var jsonRpcRequest))
			{
				return Error(JsonRpcErrorCodes.ParseError, null, null);
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
					for (int i = 0; i < methodParameters.Count; i++)
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
							throw new InvalidParameterException($"A value for the '{param.name}' is missing.");
						}
						parameters.Add( jobj[param.name].ToObject(param.type));
					}
				}
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
					throw new InvalidParameterException($"{methodParameters.Count} parameters were expected but {parameters.Count} were received.");
				}
				var result =  prodecureMetadata.MethodInfo.Invoke(_service, parameters.ToArray());

				if (jsonRpcRequest.IsNotification) // the client is not interested in getting a response
				{
					return string.Empty;
				}

				response = prodecureMetadata.MethodInfo.IsAsync()
					? await (Task<JsonRpcResponse>)result
					: (JsonRpcResponse)result;
				response.Id = jsonRpcRequest.Id;
				return response.ToJson();
			}
			catch(InvalidParameterException e)
			{
				return Error(JsonRpcErrorCodes.InvalidParams, e.Message, jsonRpcRequest.Id);
			}
			catch(Exception)
			{
				return Error(JsonRpcErrorCodes.InternalError, null, jsonRpcRequest.Id);
			}
		}

		private string Error(JsonRpcErrorCodes code, string reason, string id)
		{
			var error = new JsonRpcError(code, reason);
			var response = new JsonRpcErrorResponse(error);
			response.Id = id;
			return response.ToJson();
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