﻿using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using WalletWasabi.Gui.Models;

namespace WalletWasabi.Gui.Controls.LockScreen
{
    internal class LockScreen : UserControl
    {
        private bool _isLocked;
        private LockScreenType _activeLockScreenType;
        private ContentControl LockScreenHost;
        private LockScreenImpl CurrentLockScreen;
        private CompositeDisposable ScreenImplDisposables;

        public static readonly DirectProperty<LockScreen, LockScreenType> ActiveLockScreenTypeProperty =
            AvaloniaProperty.RegisterDirect<LockScreen, LockScreenType>(nameof(ActiveLockScreenType),
                                                                        o => o.ActiveLockScreenType,
                                                                        (o, v) => o.ActiveLockScreenType = v);
        public LockScreenType ActiveLockScreenType
        {
            get => _activeLockScreenType;
            set => this.SetAndRaise(ActiveLockScreenTypeProperty, ref _activeLockScreenType, value);
        }

        public static readonly DirectProperty<LockScreen, bool> IsLockedProperty =
            AvaloniaProperty.RegisterDirect<LockScreen, bool>(nameof(IsLocked),
                                                              o => o.IsLocked,
                                                              (o, v) => o.IsLocked = v);
        public bool IsLocked
        {
            get => _isLocked;
            set => this.SetAndRaise(IsLockedProperty, ref _isLocked, value);
        }

        public LockScreen()
        {
            InitializeComponent();

            this.LockScreenHost = this.FindControl<ContentControl>("LockScreenHost");

            this.WhenAnyValue(x => x.ActiveLockScreenType)
                .Subscribe(OnActiveLockScreenTypeChanged);
        }

        private void OnActiveLockScreenTypeChanged(LockScreenType obj)
        {
            switch (obj)
            {
                case LockScreenType.SlideLock:
                    CurrentLockScreen = new SlideLock();
                    break;
                case LockScreenType.PINLock:
                    CurrentLockScreen = new PINLock();
                    break;
                default:
                    CurrentLockScreen = new SimpleLock();
                    break;
            }

            LockScreenHost.Content = CurrentLockScreen;

            ScreenImplDisposables?.Dispose();
            ScreenImplDisposables = new CompositeDisposable();

            this.WhenAnyValue(x => x.IsLocked)
                .BindTo(CurrentLockScreen, y => y.IsLocked)
                .DisposeWith(ScreenImplDisposables);

            CurrentLockScreen.WhenAnyValue(x => x.IsLocked)
                .BindTo(this, y => y.IsLocked)
                .DisposeWith(ScreenImplDisposables);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
