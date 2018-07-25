using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Markup.Xaml;
using AvalonStudio.Extensibility;
using AvalonStudio.Shell;
using AvalonStudio.Shell.Controls;
using WalletWasabi.Gui.Tabs.WalletManager;
using WalletWasabi.Models;

namespace WalletWasabi.Gui
{
	public class MainWindow : MetroWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			this.AttachDevTools();
		}

		private void InitializeComponent()
		{
			Activated += OnActivated;
			Closing += OnClosing;
			AvaloniaXamlLoader.Load(this);
		}

		private void OnActivated(object sender, EventArgs e)
		{
			Activated -= OnActivated;
			DisplayWalletManager();
		}

		private void DisplayWalletManager()
		{
			var isAnyWalletAvailable = Directory.Exists(Global.WalletsDir) && Directory.EnumerateFiles(Global.WalletsDir).Any();

			var walletManagerViewModel = new WalletManagerViewModel();
			IoC.Get<IShell>().AddDocument(walletManagerViewModel);

			if (isAnyWalletAvailable)
			{
				walletManagerViewModel.SelectLoadWallet();
			}
			else
			{
				walletManagerViewModel.SelectGenerateWallet();
			}
		}

		private async void OnClosing(object sender, CancelEventArgs e)
		{
			var ccjClient = Global.ChaumianClient;
			if(! ccjClient.IsRunning )
			{
				e.Cancel = true;
				return;
			}

			var inputRegistrableRound = ccjClient.State.GetRegistrableRoundOrDefault();
			if (inputRegistrableRound == null)
			{
				e.Cancel = true;
				return;
			}

			var enqueuedCoins = inputRegistrableRound.CoinsRegistered;

			if( enqueuedCoins.Count == 0)
			{
				e.Cancel = true;
				return;
			}

			var mustClose = await AskCloseAndDequeCoins(enqueuedCoins);
			e.Cancel = !mustClose;
		}

		private async Task<bool> AskCloseAndDequeCoins(IEnumerable<SmartCoin> coins)
		{
			var ccjClient = Global.ChaumianClient;
 			await ccjClient.DequeueCoinsFromMixAsync(coins.ToArray());
			return true;
		} 
	}
}
