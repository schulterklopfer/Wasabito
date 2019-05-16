using Newtonsoft.Json;

namespace WalletWasabi.Gui.Rpc
{
	public class JsonRpcResponse 
	{
		[JsonProperty("jsonrpc", Order = 1)]
		public string JsonRpc => "2.0";

		[JsonProperty("id", Order = 3)]
		public string Id { get; internal set; }

		public JsonRpcResponse()
		{
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
