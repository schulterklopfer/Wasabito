using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WalletWasabi.Gui.Rpc;
using Xunit;

namespace WalletWasabi.Tests
{
	class TesteableRpcService : JsonRpcService
	{
		public void UnpublishedProcedure(){}

		[JsonRpcMethod("say", "repeats what you say.")]
		public string Echo(string text) => text;

		[JsonRpcMethod("substract", "Substracts two numbers.")]
		public int Substract(int minuend, int subtrahend) => minuend - subtrahend;

		[JsonRpcMethod("substractasync", "Substracts two numbers.")]
		public async Task<int> SubstractAsync(int minuend, int subtrahend) => await Task.FromResult( minuend - subtrahend );

		[JsonRpcMethod("writelog", "Write something in the log file.")]
		public void Log(string logEntry){}

		[JsonRpcMethod("fail", "Throws an exception.")]
		public void Failure() => throw new InvalidOperationException();

		[JsonRpcMethod("format", "Write something in the log file.")]
		public async Task FormatHardDriveAsync(string unit, CancellationTokenSource cts)
		{ 
			await Task.FromResult((JsonRpcResponse)null);
		}
	}

	public class RpcTests
	{
		[Theory]
		[InlineData( // Valid request with params by order
			"{\"jsonrpc\": \"2.0\", \"method\": \"substract\", \"params\": [42, 23], \"id\": 1}", 
			"{\"jsonrpc\":\"2.0\",\"result\":19,\"id\":\"1\"}")]
		[InlineData( // Valid request with params by name
			"{\"jsonrpc\": \"2.0\", \"method\": \"substract\", \"params\": {\"minuend\": 42, \"subtrahend\": 23}, \"id\": \"3\"}", 
			"{\"jsonrpc\":\"2.0\",\"result\":19,\"id\":\"3\"}")]
		[InlineData( // Valid request (Notification)
			"{\"jsonrpc\": \"2.0\", \"method\": \"substract\", \"params\": {\"minuend\": 42, \"subtrahend\": 23}}", 
			"")]
		[InlineData( // Invalid (broken) request
			"{\"jsonrpc\": \"2.0\", \"method\": \"substract\", \"id\", \"3\"}", 
			"{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-32700,\"message\":\"Parse error\"}}")]
		[InlineData( // Invalid (missing jsonrpc) request
			"{\"method\": \"subtract\", \"id\": \"2\" }", 
			"{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-32700,\"message\":\"Parse error\"}}")]
		[InlineData( // Invalid (missing method) request
			"{\"jsonrpc\": \"2.0\", \"params\": \"{}\", \"id\": \"2\" }", 
			"{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-32700,\"message\":\"Parse error\"}}")]
		[InlineData( // Invalid (wrong number of arguments) request
			"{\"jsonrpc\": \"2.0\", \"method\": \"substract\", \"params\": {\"subtrahend\": 23}, \"id\": \"2\"}", 
			"{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-32602,\"message\":\"A value for the 'minuend' is missing.\"},\"id\":\"2\"}")]
		[InlineData( // Invalid (wrong number of arguments) request
			"{\"jsonrpc\": \"2.0\", \"method\": \"substract\", \"params\": [23], \"id\": \"2\"}", 
			"{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-32602,\"message\":\"2 parameters were expected but 1 were received.\"},\"id\":\"2\"}")]
		[InlineData( // Valid request for void procedure
			"{\"jsonrpc\": \"2.0\", \"method\": \"writelog\", \"params\": [\"blah blah blah\"], \"id\": \"2\"}", 
			"{\"jsonrpc\":\"2.0\",\"id\":\"2\"}")]
		[InlineData( // Valid request for async procedure with cancellation token
			"{\"jsonrpc\": \"2.0\", \"method\": \"format\", \"params\": [\"c:\"], \"id\": \"2\"}", 
			"{\"jsonrpc\":\"2.0\",\"id\":\"2\"}")]		
		[InlineData( // Valid request but internal server error
			"{\"jsonrpc\": \"2.0\", \"method\": \"fail\", \"params\": [], \"id\": \"2\"}", 
			"{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-32603,\"message\":\"Internal error\"},\"id\":\"2\"}")]
		[InlineData( // Valid request with params by name
			"{\"jsonrpc\": \"2.0\", \"method\": \"substractasync\", \"params\": {\"minuend\": 42, \"subtrahend\": 23}, \"id\": \"3\"}", 
			"{\"jsonrpc\":\"2.0\",\"result\":19,\"id\":\"3\"}")]
 		public async Task ParsingRequestTestsAsync(string request, string expectedResponse)
		{
			var handler = new JsonRpcRequestHandler(new TesteableRpcService());
			
			var response = await handler.HandleAsync(request, new CancellationTokenSource());
			Assert.Equal(expectedResponse, response);
		}

		[Fact]
		public void ServiceMetadataTests()
		{

		}
	}
}