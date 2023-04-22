﻿// Copyright 2003-2023 by Autodesk, Inc.
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
// 
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.

using System.Windows;
using System.Windows.Interop;
using Microsoft.Extensions.DependencyInjection;
using RevitLookup.Core;
using RevitLookup.Services.Contracts;
using Wpf.Ui.Appearance;
using Wpf.Ui.Contracts;

namespace RevitLookup.Views;

public sealed partial class RevitLookupView : IWindow
{
    private readonly IServiceScope _serviceScope;

    public RevitLookupView(IServiceScopeFactory scopeFactory, ISettingsService settingsService)
    {
        Wpf.Ui.Application.Current = this;
        InitializeComponent();
        Theme.Apply(this, settingsService.Theme, settingsService.Background);

        _serviceScope = scopeFactory.CreateScope();
        var navigationService = _serviceScope.ServiceProvider.GetService<INavigationService>()!;
        var windowController = _serviceScope.ServiceProvider.GetService<IWindowController>()!;
        var dialogService = _serviceScope.ServiceProvider.GetService<IContentDialogService>()!;
        var snackbarService = _serviceScope.ServiceProvider.GetService<ISnackbarService>()!;

        windowController.SetControlledWindow(this);
        navigationService.SetNavigationControl(RootNavigation);
        dialogService.SetContentPresenter(RootContentDialog);

        snackbarService.SetSnackbarControl(RootSnackbar);
        snackbarService.Timeout = 3000;

        RootNavigation.TransitionDuration = settingsService.TransitionDuration;
        WindowBackdropType = settingsService.Background;

        Unloaded += UnloadServices;
        Activated += (sender, _) => Wpf.Ui.Application.Current = (Window) sender;
    }

    public IServiceProvider Scope => _serviceScope.ServiceProvider;

    public void Show(Window window)
    {
        Left = window.Left + 47;
        Top = window.Top + 49;
        this.Show(RevitApi.UiApplication.MainWindowHandle);
    }

    public void ShowAttached()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        this.Show(RevitApi.UiApplication.MainWindowHandle);
    }

    public void Initialize()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        var interopHelper = new WindowInteropHelper(this) {Owner = RevitApi.UiApplication.MainWindowHandle};
        interopHelper.EnsureHandle();
    }

    private void UnloadServices(object sender, RoutedEventArgs e)
    {
        _serviceScope.Dispose();
    }
}