using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletWasabi.Models;
using WalletWasabi.Services;

namespace WalletWasabi.Gui.Rpc
{
	public class WasabiJsonRpcService
	{
		[JsonRpcMethod("listunspentcoins")]
		public object[] GetUnspentCoinList()
		{
			if(Global.WalletService == null)
			{
				throw new Exception("There is not wallet loaded.");
			}

			return Global.WalletService.Coins.Where(x=>x.Unspent).Select( x=> new {
					txid   = x.TransactionId.ToString(), 
					index  = x.Index, 
					amount = x.Amount.Satoshi, 
					anonymitySet = x.AnonymitySet,
					confirmed = x.Confirmed,
					label = x.Label,
					keyPath = x.HdPubKey.FullKeyPath.ToString(),
					address = x.HdPubKey.GetP2wpkhAddress(Global.Network).ToString()
				}).ToArray();
		}

		[JsonRpcMethod("getstatus")]
		public object GetStatus()
		{
			var sync = Global.Synchronizer;

			return new {
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
				};
		}

		[JsonRpcMethod("stop")]
		public async Task StopAsync()
		{
			await Global.StopAndExitAsync();
		}
	}
}
