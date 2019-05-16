using System;
using System.Linq;
using WalletWasabi.Models;
using WalletWasabi.Services;

namespace WalletWasabi.Gui.Rpc
{
	public class WasabiJsonRpcService : JsonRpcService
	{	
		[JsonRpcMethod("listcoins", "returns the list of all coins in the walet.")]
		public JsonRpcResponse GetCoinList()
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
					anonymitySet = x.AnonymitySet
				}).ToArray());
		}
	}
}
