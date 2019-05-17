using System;
using System.Linq;
using WalletWasabi.Models;
using WalletWasabi.Services;

namespace WalletWasabi.Gui.Rpc
{
	public class WasabiJsonRpcService : JsonRpcService
	{
		[JsonRpcMethod("listunspentcoins", "returns the list of all coins in the walet.")]
		public JsonRpcResponse GetUnspentCoinList()
		{
			if(Global.WalletService == null)
			{
				return Error(JsonRpcErrorCodes.InternalError, "There is not wallet loaded.");
			}

			return Result<object[]>(
				Global.WalletService.Coins.Where(x=>x.Unspent).Select( x=> new {
					txid   = x.TransactionId.ToString(), 
					index  = x.Index, 
					amount = x.Amount.Satoshi, 
					anonymitySet = x.AnonymitySet,
					confirmed = x.Confirmed,
					label = x.Label,
					keyPath = x.HdPubKey.FullKeyPath.ToString(),
					address = x.HdPubKey.GetP2wpkhAddress(Global.Network).ToString()
				}).ToArray());
		}

		[JsonRpcMethod("getstatus", "returns the list of all coins in the walet.")]
		public JsonRpcResponse GetStatus()
		{
			var sync = Global.Synchronizer;

			return Result<object>(
				new {
					torStatus = sync.TorStatus == TorStatus.NotRunning ? "Not running" : (sync.TorStatus == TorStatus.Running ? "Running" : "Turned off"), 
					backendStatus = sync.BackendStatus == BackendStatus.Connected ? "Connected" : "Disconnected",
					bestBlockchainHeight   = sync.BestBlockchainHeight, 
					network = sync.Network.Name, 
					exchangeRate = sync.UsdExchangeRate,
					peers = Global.Nodes.ConnectedNodes.Select(x=> new {
						isConnected = x.IsConnected,
						lastSeen = x.LastSeen,
						endpoint = x.Peer.Endpoint.ToString(),
						userAgent = x.PeerVersion.UserAgent,
					}).ToArray(),
					transactionInMempool = Global.WalletService.TransactionCache.Count,
				});
		}
	}
}
