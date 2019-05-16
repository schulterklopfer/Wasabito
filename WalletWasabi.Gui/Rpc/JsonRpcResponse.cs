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

	public class JsonRpcResultResponse<TResult> : JsonRpcResponse 
	{
		public JsonRpcResultResponse(TResult result = default) 
			: base()
		{
			Result = result;
		}

		[JsonProperty("result", Order = 2)]
		public TResult Result { get; }
	}

	public class JsonRpcErrorResponse<TError> : JsonRpcResponse 
	{
		public JsonRpcErrorResponse(TError result = default) 
			: base()
		{
			Result = result;
		}

		[JsonProperty("error", Order = 2)]
		public TError Result { get; }
	}
}
