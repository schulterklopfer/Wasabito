using Newtonsoft.Json;

namespace WalletWasabi.Gui.Rpc
{
	/// <summary>
	/// Represents the response used to inform about an error situation.
	/// </summary>
	public class JsonRpcErrorResponse : JsonRpcResponse 
	{
		public JsonRpcErrorResponse(JsonRpcErrorCodes code, string message=null, string id = null) 
			: base()
		{
			Result = new JsonRpcError(code, message);
			Id = id;
		}

		[JsonProperty("error", Order = 2)]
		public JsonRpcError Result { get; }
	}
}
