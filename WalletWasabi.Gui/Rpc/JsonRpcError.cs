using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WalletWasabi.Gui.Rpc
{
	public enum JsonRpcErrorCodes
	{
		ParseError     = -32700,  // Invalid JSON was received by the server. An error occurred on the server while parsing the JSON text. 
		InvalidRequest = -32600,  // The JSON sent is not a valid Request object.
		MethodNotFound = -32601,  // The method does not exist / is not available.
		InvalidParams  = -32602,  // Invalid method parameter(s).
		InternalError  = -32603,  // Internal JSON-RPC error.
	}

	public class JsonRpcError
	{
		private static Dictionary<JsonRpcErrorCodes, string> Messages = new Dictionary<JsonRpcErrorCodes, string> {
			[JsonRpcErrorCodes.ParseError]     = "Parse error",
			[JsonRpcErrorCodes.InvalidRequest] = "Invalid Request",
			[JsonRpcErrorCodes.MethodNotFound] = "Method not found",
			[JsonRpcErrorCodes.InvalidParams]  = "Invalid params",
			[JsonRpcErrorCodes.InternalError]  = "Internal error",
		};

		public  JsonRpcError(JsonRpcErrorCodes code, string message=null)
		{
			Code = (int)code;
			if(message != null)
			{
				Message = message;
			}
			else
			{
				if(Messages.ContainsKey(code))
				{
					Message = Messages[code];
				}
				else
				{
					Message = "Server error";
				}
			}
		}

		public  JsonRpcError(int code, string message)
		{
			Code = code;
			Message = message;
		}

		[JsonProperty("code", Order = 1)]
		public int Code { get; }

		[JsonProperty("message", Order = 2)]
		public string Message { get; }
	}
}

