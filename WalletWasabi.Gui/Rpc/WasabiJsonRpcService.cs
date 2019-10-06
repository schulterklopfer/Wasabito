using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using WalletWasabi.Models;
using WalletWasabi.Models.TransactionBuilding;
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
					label = x.Label.ToString(),
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
				balance = _global.WalletService.Coins
							.Where(c => c.Unspent && !c.SpentAccordingToBackend)
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
				.GenerateNewKey(new SmartLabel(label), KeyManagement.KeyState.Clean, isInternal: false);
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

		public class Payment
		{
			public BitcoinAddress sendto;
			public Money amount;
			public string label;
			public bool subtractFee;
		}

		[JsonRpcMethod("send")]
		public async Task<object> SendTransaction(Payment[] payments, TxoRef[] coins, int feeTarget)
		{
			AssertWalletIsLoaded();
			var sync = _global.Synchronizer;
			var payment = new PaymentIntent(payments.Select(p => 
				new DestinationRequest(p.sendto.ScriptPubKey, MoneyRequest.Create(p.amount, p.subtractFee), new SmartLabel(p.label))));
			var feeStrategy = FeeStrategy.CreateFromConfirmationTarget(feeTarget);
			var password = string.Empty;
			var result = _global.WalletService.BuildTransaction(
				password, 
				payment, 
				feeStrategy, 
				allowUnconfirmed: true, 
				allowedInputs: coins);
			var smartTx = result.Transaction;

			// dequeue the coins we are going to spend
			TxoRef[] toDequeue = _global.WalletService.Coins
				.Where(x => x.CoinJoinInProgress && coins.Contains(x.GetTxoRef()))
				.Select(x => x.GetTxoRef())
				.ToArray();
			if (toDequeue.Any())
			{
				await _global.ChaumianClient.DequeueCoinsFromMixAsync(toDequeue, "Coin is used in a spending transaction built by the user.");
			}

			await _global.WalletService.SendTransactionAsync(smartTx);
			return new {
				txid = smartTx.Transaction.GetHash(),
				tx = smartTx.Transaction.ToHex()
			};
		}

		[JsonRpcMethod("stop")]
		public async Task StopAsync()
		{
			await _global.StopAndExitAsync();
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