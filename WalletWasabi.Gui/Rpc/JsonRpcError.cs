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

	/// <summary>
	/// When a rpc call encounters an error, the Response Object MUST contain the error 
	/// member with a value that is a Object with the following members.
	/// </summary>
	public class JsonRpcError
	{
		// Default error messages for standard JsonRpcErrorCodes 
		private static Dictionary<JsonRpcErrorCodes, string> Messages = new Dictionary<JsonRpcErrorCodes, string> {
			[JsonRpcErrorCodes.ParseError]     = "Parse error",
			[JsonRpcErrorCodes.InvalidRequest] = "Invalid Request",
			[JsonRpcErrorCodes.MethodNotFound] = "Method not found",
			[JsonRpcErrorCodes.InvalidParams]  = "Invalid params",
			[JsonRpcErrorCodes.InternalError]  = "Internal error",
		};

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="code">The error code to return. It can be one of the already defined or any other.</param> 
		/// <param name="message">The error message to use. In case null is passed, it uses the default descriptions.</param> 
		public  JsonRpcError(JsonRpcErrorCodes code, string message=null)
		{
			Code = (int)code;
			Message = message ?? GetMessage(code);
		}

		/// <summary>
		/// A Number that indicates the error type that occurred.
		/// This MUST be an integer.
		/// </summary>
		[JsonProperty("code", Order = 1)]
		public int Code { get; }

		/// <summary>
		/// A String providing a short description of the error.
		/// The message SHOULD be limited to a concise single sentence.
		/// </summary>
		[JsonProperty("message", Order = 2)]
		public string Message { get; }

		private string GetMessage(JsonRpcErrorCodes code)
		{
			if(Messages.TryGetValue(code, out var message))
			{
				return message;
			}
			return "Server error";
		}
	}
}
