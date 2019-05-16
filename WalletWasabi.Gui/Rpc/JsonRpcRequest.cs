using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WalletWasabi.Gui.Rpc
{
	public class JsonRpcRequest
	{
		public static JsonRpcRequest Parse(string rawJson)
		{
			return JsonConvert.DeserializeObject<JsonRpcRequest>(rawJson);
		}

		public JsonRpcRequest(string id, string method)
			: this("2.0", id, method, null)
		{
		}

		[JsonConstructor]
		public  JsonRpcRequest(string jsonRpc, string id, string method, JToken parameters)
		{
			this.JsonRPC = jsonRpc;
			this.Id = id;
			this.Method = method;
			this.Parameters = parameters;
		}

		[JsonProperty("jsonrpc", Order = 1, Required = Required.Always)]
		public string JsonRPC { get; }

		[JsonProperty("id", Required = Required.Always)]
		public string Id { get; }

		[JsonProperty("method", Required = Required.Always)]
		public string Method { get; }

		[JsonProperty("params")]
		public JToken Parameters { get;  }
	}
}

