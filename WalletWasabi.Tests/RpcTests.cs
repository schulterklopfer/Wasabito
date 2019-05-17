using System;
using System.Linq;
using Newtonsoft.Json;
using WalletWasabi.Gui.Rpc;
using Xunit;

namespace WalletWasabi.Tests
{
	public class RpcTests
	{
		[Fact]
		public void ParsingRequestTests()
		{
			var parsed = JsonRpcRequest.TryParse(
				"{\"jsonrpc\": \"2.0\", \"method\": \"subtract\", \"params\": [42, 23], \"id\": 1}",
				out var request);

			Assert.True(parsed);
			Assert.Equal("2.0", request.JsonRPC);
			Assert.Equal("subtract", request.Method);
			Assert.Equal("1", request.Id);
			Assert.NotNull(request.Parameters);
			Assert.Equal(42, (int)request.Parameters[0]);
			Assert.Equal(23, (int)request.Parameters[1]);

			parsed = JsonRpcRequest.TryParse(
				"{\"jsonrpc\": \"2.0\", \"method\": \"subtract\", \"params\": {\"subtrahend\": 23, \"minuend\": 42}, \"id\": 2}",
				out request);

			Assert.True(parsed);
			Assert.Equal("2.0", request.JsonRPC);
			Assert.Equal("subtract", request.Method);
			Assert.Equal("2", request.Id);
			Assert.NotNull(request.Parameters);
			Assert.Equal(23, request.Parameters.Value<int>("subtrahend"));
			Assert.Equal(42, request.Parameters.Value<int>("minuend"));
		}
	}
}