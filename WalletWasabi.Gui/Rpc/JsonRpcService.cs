using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WalletWasabi.Gui.Rpc
{
	public abstract class JsonRpcService
	{	
		protected JsonRpcResultResponse<TResult> Result<TResult>(TResult data)
		{
			return new JsonRpcResultResponse<TResult>(data);
		}

		protected JsonRpcErrorResponse<JsonRpcError> Error(JsonRpcErrorCodes code, string message = default)
		{
			if (String.IsNullOrWhiteSpace(message))
				throw new ArgumentNullException(nameof(message));

			return new JsonRpcErrorResponse<JsonRpcError>(new JsonRpcError(code, message));
		}
	}
}
