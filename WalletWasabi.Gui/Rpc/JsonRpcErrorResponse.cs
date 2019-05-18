using Newtonsoft.Json;

namespace WalletWasabi.Gui.Rpc
{
	/// <summary>
	/// Represents the response used to inform about an error situation.
	/// </summary>
	public class JsonRpcErrorResponse : JsonRpcResponse 
	{
		public JsonRpcErrorResponse(JsonRpcErrorCodes code, string message=null) 
			: base()
		{
			Result = new JsonRpcError(code, message);
		}

		[JsonProperty("error", Order = 2)]
		public JsonRpcError Result { get; }
	}
}
