using System;
using System.Collections.Generic;
using System.Reflection;
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
			LoadServiceInfo();
		}

		public string Handle(string body)
		{
			JsonRpcResponse response = null;

			if(!JsonRpcRequest.TryParse(body, out var jsonRpcRequest))
			{
				return Error(JsonRpcErrorCodes.ParseError, null, null);
			}
			var methodName = jsonRpcRequest.Method;

			if(!_methodsMap.TryGetValue(methodName, out var map))
			{
				return Error(JsonRpcErrorCodes.MethodNotFound, $"{methodName} method not found.", jsonRpcRequest.Id);
			}

			try
			{
				var p = jsonRpcRequest.Parameters != null && jsonRpcRequest.Parameters.HasValues 
					? new object[] { jsonRpcRequest.Parameters }
					: new object[0];
				response = (JsonRpcResponse)map.methodInfo.Invoke(_service, p);
			}
			catch(Exception)
			{
				return Error(JsonRpcErrorCodes.InternalError, null, jsonRpcRequest.Id);
			}
			
			return response.ToJson();
		}

		private string Error(JsonRpcErrorCodes code, string reason, string id)
		{
			var error = new JsonRpcError(code, reason);
			var response = new JsonRpcErrorResponse(error);
			response.Id = id;
			return response.ToJson();
		}

		private void LoadServiceInfo()
		{
			var serviceType = _service.GetType();
			var publicMethods = serviceType.GetMethods();
			foreach(var methodInfo in publicMethods)
			{
				var attrs = methodInfo.GetCustomAttributes();  
				foreach(Attribute attr in attrs)  
				{
					if (attr is JsonRpcMethodAttribute)
					{
						var jsonRpcMethodAttr = (JsonRpcMethodAttribute) attr;
						_methodsMap.Add(jsonRpcMethodAttr.Name, (jsonRpcMethodAttr.Name, jsonRpcMethodAttr.Description, methodInfo));
						Console.WriteLine($"{jsonRpcMethodAttr.Name} was added.");
					}
				}
			}
		}
	}
}