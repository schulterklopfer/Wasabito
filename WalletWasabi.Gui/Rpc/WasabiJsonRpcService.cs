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
		private Global _global;

		public WasabiJsonRpcService(Global global)
		{
			_global = global;
		}
		
		[JsonRpcMethod("listunspentcoins")]
		public object[] GetUnspentCoinList()
		{
			AssertWalletIsLoaded();
			return _global.WalletService.Coins.Where(x=>x.Unspent).Select( x=> new {
					txid   = x.TransactionId.ToString(), 
					index  = x.Index, 
					amount = x.Amount.Satoshi, 
					anonymitySet = x.AnonymitySet,
					confirmed = x.Confirmed,
					label = x.Label,
					keyPath = x.HdPubKey.FullKeyPath.ToString(),
					address = x.HdPubKey.GetP2wpkhAddress(_global.Network).ToString()
				}).ToArray();
		}

		[JsonRpcMethod("getwalletinfo")]
		public object WalletInfo()
		{
			AssertWalletIsLoaded();
			var km = _global.WalletService.KeyManager;
			return new {
				walletFile = _global.WalletService.KeyManager.FilePath,
				extendedAccountPublicKey = km.ExtPubKey.ToString(_global.Network),
				extendedAccountZpub = km.ExtPubKey.ToZpub(_global.Network),
				accountKeyPath = $"m/{km.AccountKeyPath.ToString()}",
				masterKeyFingerprint = km.MasterFingerprint.ToString(),
				balance = Global.WalletService.Coins
							.Where(c => c.Unspent && !c.IsDust && !c.SpentAccordingToBackend)
							.Sum(c => c.Amount.Satoshi)
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
			var hdkey = _global.WalletService.KeyManager
				.GenerateNewKey(label, KeyManagement.KeyState.Clean, isInternal: false);
			return new {
				address = hdkey.GetP2wpkhAddress(_global.Network).ToString(),
				keyPath = hdkey.FullKeyPath.ToString(),
				label = hdkey.Label,
				publicKey = hdkey.PubKey.ToHex(),
				p2wpkh = hdkey.P2wpkhScript.ToHex()
			};
		}

		[JsonRpcMethod("getstatus")]
		public object GetStatus()
		{
			var sync = _global.Synchronizer;

			return new {
					torStatus = sync.TorStatus == TorStatus.NotRunning ? "Not running" : (sync.TorStatus == TorStatus.Running ? "Running" : "Turned off"), 
					backendStatus = sync.BackendStatus == BackendStatus.Connected ? "Connected" : "Disconnected",
					bestBlockchainHeight = sync.BitcoinStore.HashChain.TipHeight.ToString(),
					bestBlockchainHash   = sync.BitcoinStore.HashChain.TipHash.ToString(),
					filtersCount = sync.BitcoinStore.HashChain.HashCount,
					filtersLeft = sync.BitcoinStore.HashChain.HashesLeft,
					network = sync.Network.Name, 
					exchangeRate = sync.UsdExchangeRate,
					peers = _global.Nodes.ConnectedNodes.Select(x=> new {
						isConnected = x.IsConnected,
						lastSeen = x.LastSeen,
						endpoint = x.Peer.Endpoint.ToString(),
						userAgent = x.PeerVersion.UserAgent,
					}).ToArray(),
				};
		}

		[JsonRpcMethod("send")]
		public async Task SendTransaction(BitcoinAddress address, TxoRef[] outpoints, long amount, string label, int feeTarget)
		{
			AssertWalletIsLoaded();

			var sync = Global.Synchronizer;
			var operation = new WalletService.Operation(address.ScriptPubKey, amount, label);
			var password = string.Empty;
			var result = Global.WalletService.BuildTransaction(
				password, 
				new[] { operation }, 
				feeTarget, 
				allowUnconfirmed: true, 
				allowedInputs: outpoints);
			await Global.WalletService.SendTransactionAsync(result.Transaction);
		}


		[JsonRpcMethod("stop")]
		public async Task StopAsync()
		{
			await Global.StopAndExitAsync();
		}
		private void AssertWalletIsLoaded()
		{
			if(_global.WalletService == null)
			{
				throw new Exception("There is not wallet loaded.");
			}
		}
	}
}
