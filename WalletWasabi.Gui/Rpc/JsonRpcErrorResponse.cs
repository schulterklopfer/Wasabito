using Newtonsoft.Json;

namespace WalletWasabi.Gui.Rpc
{
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
