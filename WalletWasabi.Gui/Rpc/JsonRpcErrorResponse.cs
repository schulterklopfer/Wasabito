using Newtonsoft.Json;

namespace WalletWasabi.Gui.Rpc
{
	/// <summary>
	/// Represents the response used to inform about an error situation.
	/// </summary>
	public class JsonRpcErrorResponse : JsonRpcResponse 
	{
		public JsonRpcErrorResponse(JsonRpcError result) 
			: base()
		{
			Result = result;
		}

		[JsonProperty("error", Order = 2)]
		public JsonRpcError Result { get; }
	}
}
