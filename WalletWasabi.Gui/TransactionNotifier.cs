using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using WalletWasabi.Logging;
using WalletWasabi.Models;

namespace WalletWasabi.Gui
{
	internal class TransactionNotifier
	{
		public readonly static TransactionNotifier Current = new TransactionNotifier();

		private TransactionNotifier()
		{
		}

		public void Start()
		{
			Global.WalletService.CoinReceived += OnCoinReceived;
		}

		public void Stop()
		{
			Global.WalletService.CoinReceived -= OnCoinReceived;
		}

		private void OnCoinReceived(object sender, SmartCoin coin)
		{
			try
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					Process.Start(new ProcessStartInfo
					{
						FileName = "notify-send",
						Arguments = $"\"Wasabi\" \"Received {coin.Amount.ToString(false, true)} BTC\"",
						CreateNoWindow = true
					});
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				{
					Process.Start(new ProcessStartInfo
					{
						FileName = "osascript",
						Arguments = $"-e display notification \"Received {coin.Amount.ToString(false, true)} BTC\" with title \"Wasabi\"",
						CreateNoWindow = true
					});
				}
			}
			catch (Exception ex)
			{
				Logger.LogWarning<TransactionNotifier>(ex);
			}
		}
	}
}