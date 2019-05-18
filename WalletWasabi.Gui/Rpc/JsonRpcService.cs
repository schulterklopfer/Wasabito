using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WalletWasabi.Gui.Rpc
{
	///<summary>
	/// Base class for service classes.
	///
	/// Provides two methods for responding to the clients with a **result** for valid
	/// requests or with **error** in case the request is invalid or there is a problem.
	///
	/// Also, it loads and serves information about the service. This is, it discovers
	/// (using reflection) the methods that have to be invoked and the parameters it
	/// receives. 
	///</summary>
	public abstract class JsonRpcService
	{
		/// <summary>
		/// Creates and returns a rpc response to be returned to the client.
		/// </summary>
		protected JsonRpcResultResponse<TResult> Result<TResult>(TResult data)
		{
			return new JsonRpcResultResponse<TResult>(data);
		}

		/// <summary>
		/// Creates and returns a rpc error response to be returned to the client.
		/// </summary>
		protected JsonRpcErrorResponse Error(JsonRpcErrorCodes code, string message = default)
		{
			if (String.IsNullOrWhiteSpace(message))
				throw new ArgumentNullException(nameof(message));

			return new JsonRpcErrorResponse(code, message);
		}


		/// Keeps the directory of procedures' metadata
		private Dictionary<string, JsonRpcMethodMetadata> _proceduresDirectory = 
				new Dictionary<string, JsonRpcMethodMetadata>();
		

		/// <summary>
		/// Tries to return the metadata for a given procedure name. 
		/// Returns true if found otherwise returns false.  
		/// </summary>
		public bool TryGetMetadata(string methodName, out JsonRpcMethodMetadata metadata)
		{
			if( _proceduresDirectory.Count == 0)
			{
				LoadServiceMetadata();
			}
			if( !_proceduresDirectory.TryGetValue(methodName, out metadata))
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
				_proceduresDirectory.Add(info.Name, info);
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
