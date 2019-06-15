using Avalonia.Threading;
using AvalonStudio.Extensibility;
using AvalonStudio.MVVM;
using AvalonStudio.Shell;
using NBitcoin;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletWasabi.Gui.ViewModels;
using WalletWasabi.Helpers;
using WalletWasabi.KeyManagement;
using WalletWasabi.Logging;
using WalletWasabi.Models.ChaumianCoinJoin;
using WalletWasabi.Services;
using static WalletWasabi.Gui.Models.ShieldLevelHelper;

namespace WalletWasabi.Gui.Controls.WalletExplorer
{
	[Export(typeof(IExtension))]
	[Export]
	[ExportToolControl]
	[Shared]
	public class CoinJoinStatusViewModel : ToolViewModel, IActivatableExtension
	{
		public override Location DefaultLocation => Location.Right;

		private CompositeDisposable Disposables { get; set; }

		private CcjRoundPhase _phase;
		private Money _requiredBTC;
		private int _successfulRoundCount;
		private string _coordinatorFeePercent;
		private int _peersRegistered;
		private int _peersNeeded;

		public CcjRoundPhase Phase
		{
			get => _phase;
			set => this.RaiseAndSetIfChanged(ref _phase, value);
		}

		public Money RequiredBTC
		{
			get => _requiredBTC;
			set => this.RaiseAndSetIfChanged(ref _requiredBTC, value);
		}

		public string CoordinatorFeePercent
		{
			get => _coordinatorFeePercent;
			set => this.RaiseAndSetIfChanged(ref _coordinatorFeePercent, value);
		}

		public int PeersRegistered
		{
			get => _peersRegistered;
			set => this.RaiseAndSetIfChanged(ref _peersRegistered, value);
		}

		public int PeersNeeded
		{
			get => _peersNeeded;
			set => this.RaiseAndSetIfChanged(ref _peersNeeded, value);
		}
		public int SuccessfulRoundCount
		{
			get => _successfulRoundCount;
			set => this.RaiseAndSetIfChanged(ref _successfulRoundCount, value);
		}

		public CoinJoinStatusViewModel()
		{
			Title = "Coinjoin Status";
			
			Task.Run(async()=>{
				while(Global.ChaumianClient?.State == null)
				{
					await Task.Delay(1000);
				}
				var registrableRound = Global.ChaumianClient.State.GetRegistrableRoundOrDefault();

				UpdateRequiredBtcLabel(registrableRound);
				CoordinatorFeePercent = registrableRound?.State?.CoordinatorFeePercent.ToString() ?? "0.003";

				CcjClientRound mostAdvancedRound = Global.ChaumianClient?.State?.GetMostAdvancedRoundOrDefault();

				if (mostAdvancedRound != default)
				{
					SuccessfulRoundCount = mostAdvancedRound.State.SuccessfulRoundCount;
					Phase = mostAdvancedRound.State.Phase;
					PeersRegistered = mostAdvancedRound.State.RegisteredPeerCount;
					PeersNeeded = mostAdvancedRound.State.RequiredPeerCount;
				}
				else
				{
					SuccessfulRoundCount = -1;
					Phase = CcjRoundPhase.InputRegistration;
					PeersRegistered = 0;
					PeersNeeded = 100;
				}
			});
		}

		private void UpdateStates()
		{
			var registrableRound = Global.ChaumianClient.State.GetRegistrableRoundOrDefault();
			if (registrableRound != default)
			{
				CoordinatorFeePercent = registrableRound.State.CoordinatorFeePercent.ToString();
				UpdateRequiredBtcLabel(registrableRound);
			}
			var mostAdvancedRound = Global.ChaumianClient.State.GetMostAdvancedRoundOrDefault();
			if (mostAdvancedRound != default)
			{
				SuccessfulRoundCount = mostAdvancedRound.State.SuccessfulRoundCount;
				if (!Global.ChaumianClient.State.IsInErrorState)
				{
					Phase = mostAdvancedRound.State.Phase;
				}
				this.RaisePropertyChanged(nameof(Phase));
				PeersRegistered = mostAdvancedRound.State.RegisteredPeerCount;
				PeersNeeded = mostAdvancedRound.State.RequiredPeerCount;
			}
		}

		private void UpdateRequiredBtcLabel(CcjClientRound registrableRound)
#pragma warning restore CS0618 // Type or member is obsolete
		{
			var ws = Global.WalletService;
			if(ws == null) return;

			if (registrableRound == default)
			{
				if (RequiredBTC == default)
				{
					RequiredBTC = Money.Zero;
				}
			}
			else
			{
				var queued = ws.Coins.Where(x => x.CoinJoinInProgress);
				if (queued.Any())
				{
					RequiredBTC = registrableRound.State.CalculateRequiredAmount(Global.ChaumianClient.State.GetAllQueuedCoinAmounts().ToArray());
				}
				else
				{
					var available = ws.Coins.Where(x => x.Confirmed && !x.Unavailable);
					if (available.Any())
					{
						RequiredBTC = registrableRound.State.CalculateRequiredAmount(available.Where(x => x.AnonymitySet < Global.Config.PrivacyLevelStrong).Select(x => x.Amount).ToArray());
					}
					else
					{
						RequiredBTC = registrableRound.State.CalculateRequiredAmount();
					}
				}
			}
		}
		public void BeforeActivation()
		{
		}

		public void Activation()
		{
			IoC.Get<IShell>().MainPerspective.AddOrSelectTool(this);
		}
	}
}