using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WalletWasabi.Gui.Rpc
{
	public abstract class JsonRpcService
	{	
		protected JsonRpcResultResponse<TResult> Result<TResult>(TResult data)
		{
			return new JsonRpcResultResponse<TResult>(data);
		}

		protected JsonRpcErrorResponse Error(JsonRpcErrorCodes code, string message = default)
		{
			if (String.IsNullOrWhiteSpace(message))
				throw new ArgumentNullException(nameof(message));

			return new JsonRpcErrorResponse(new JsonRpcError(code, message));
		}


		private Dictionary<string, JsonRpcMethodMetadata> _methodsMap = 
				new Dictionary<string, JsonRpcMethodMetadata>();
		
		public bool TryGetMetadata(string methodName, out JsonRpcMethodMetadata metadata)
		{
			if( _methodsMap.Count == 0)
			{
				LoadServiceMetadata();
			}
			if( !_methodsMap.TryGetValue(methodName, out metadata))
			{
				metadata = null;
				return false;
			}
			return true;
		}

		private void LoadServiceMetadata()
		{
			foreach(var info in JsonRpcService.EnumetareServiceInfo(this))
			{
				_methodsMap.Add(info.Name, info);
			}
		}

		internal static IEnumerable<JsonRpcMethodMetadata> EnumetareServiceInfo(JsonRpcService service)
		{
			var serviceType = service.GetType();
			var publicMethods = serviceType.GetMethods();
			foreach(var methodInfo in publicMethods)
			{
				var attrs = methodInfo.GetCustomAttributes();
				foreach(Attribute attr in attrs)
				{
					if (attr is JsonRpcMethodAttribute)
					{
						var parameters = new List<(string name, Type type)>();
						foreach(var p in methodInfo.GetParameters())
						{
							parameters.Add((p.Name, p.ParameterType));
						}
						var jsonRpcMethodAttr = (JsonRpcMethodAttribute) attr;
						yield return new JsonRpcMethodMetadata(jsonRpcMethodAttr.Name, jsonRpcMethodAttr.Description, methodInfo, parameters);
					}
				}
			}
		}

	}
}
