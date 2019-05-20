using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using WalletWasabi.Models;
using WalletWasabi.Services;

namespace WalletWasabi.Gui.Rpc
{
	public class WasabiJsonRpcService
	{
		[JsonRpcMethod("listunspentcoins")]
		public object[] GetUnspentCoinList()
		{
			AssertWalletIsLoaded();
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

		[JsonRpcMethod("getwalletinfo")]
		public object WalletInfo()
		{
			AssertWalletIsLoaded();
			var km = Global.WalletService.KeyManager;
			return new {
				walletFile = Global.WalletService.KeyManager.FilePath,
				extendedAccountPublicKey = km.ExtPubKey.ToString(Global.Network),
				extendedAccountZpub = km.ExtPubKey.ToZpub(Global.Network),
				accountKeyPath = $"m/{km.AccountKeyPath.ToString()}",
				masterKeyFingerprint = km.MasterFingerprint.ToString(),
			};
		}

		[JsonRpcMethod("getnewaddress")]
		public object GenerateReceiveAddress(string label)
		{
			AssertWalletIsLoaded();
			if(string.IsNullOrWhiteSpace(label))
			{
				throw new Exception("A non-empty label is required.");
			}
			var hdkey = Global.WalletService.KeyManager
				.GenerateNewKey(label, KeyManagement.KeyState.Clean, isInternal: false);
			return new {
				address = hdkey.GetP2wpkhAddress(Global.Network).ToString(),
				keyPath = hdkey.FullKeyPath.ToString(),
				label = hdkey.Label,
				publicKey = hdkey.PubKey.ToHex(),
				p2wpkh = hdkey.P2wpkhScript.ToHex()
			};
		}

		[JsonRpcMethod("getstatus")]
		public object GetStatus()
		{
			var sync = Global.Synchronizer;

			return new {
					torStatus = sync.TorStatus == TorStatus.NotRunning ? "Not running" : (sync.TorStatus == TorStatus.Running ? "Running" : "Turned off"), 
					backendStatus = sync.BackendStatus == BackendStatus.Connected ? "Connected" : "Disconnected",
					bestBlockchainHeight = sync.BitcoinStore.HashChain.TipHeight.ToString(),
					bestBlockchainHash   = sync.BitcoinStore.HashChain.TipHash.ToString(),
					filtersCount = sync.BitcoinStore.HashChain.HashCount,
					filtersLeft = sync.BitcoinStore.HashChain.HashesLeft,
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
		private void AssertWalletIsLoaded()
		{
			if(Global.WalletService == null)
			{
				throw new Exception("There is not wallet loaded.");
			}
		}
	}
}
