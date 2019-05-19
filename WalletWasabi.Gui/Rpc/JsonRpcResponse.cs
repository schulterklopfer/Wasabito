using Newtonsoft.Json;

namespace WalletWasabi.Gui.Rpc
{
	public class JsonRpcResponse 
	{
		[JsonProperty("jsonrpc", Order = 0)]
		public string JsonRpc => "2.0";

		[JsonProperty("result", Order = 1)]
		public object Result { get; }

		[JsonProperty("error", Order = 1)]
		public object Error { get; }

		[JsonProperty("id", Order = 3)]
		public string Id { get; }

		public static JsonRpcResponse CreateResultResponse(string id, object result)
		{
			return new JsonRpcResponse(id, result, null);
		}
		public static JsonRpcResponse CreateErrorResponse(string id, object error)
		{
			return new JsonRpcResponse(id, null, error);
		}

		private JsonRpcResponse(string id, object result, object error)
		{
			Id = id;
			Result = result;
			Error = error;
		}

		public string ToJson()
		{
			var settings = new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Ignore
			};

			return JsonConvert.SerializeObject(this, settings);
		}
	}
}
