using Newtonsoft.Json;

namespace WalletWasabi.Gui.Rpc
{
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
}
