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

using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RevitLookup.Core;
using RevitLookup.Services.Contracts;
using RevitLookup.Services.Enums;
using RevitLookup.Views.Dialogs;
using Wpf.Ui.Common;
using Wpf.Ui.Contracts;
using Wpf.Ui.Controls;

namespace RevitLookup.ViewModels.Pages;

public sealed partial class DashboardViewModel : ObservableObject
{
    private readonly IContentDialogService _dialogService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISnackbarService _snackbarService;
    private readonly ISnoopService _snoopService;

    public DashboardViewModel(ISnoopService snoopService,
        IContentDialogService dialogService,
        ISnackbarService snackbarService,
        IServiceProvider serviceProvider)
    {
        _snoopService = snoopService;
        _serviceProvider = serviceProvider;
        _dialogService = dialogService;
        _snackbarService = snackbarService;
    }

    [RelayCommand]
    private async Task NavigateSnoopPage(string parameter)
    {
        if (!Validate()) return;

        switch (parameter)
        {
            case "view":
                await _snoopService.Snoop(SnoopableType.View);
                break;
            case "document":
                await _snoopService.Snoop(SnoopableType.Document);
                break;
            case "application":
                await _snoopService.Snoop(SnoopableType.Application);
                break;
            case "uiApplication":
                await _snoopService.Snoop(SnoopableType.UiApplication);
                break;
            case "database":
                await _snoopService.Snoop(SnoopableType.Database);
                break;
            case "dependents":
                await _snoopService.Snoop(SnoopableType.DependentElements);
                break;
            case "selection":
                await _snoopService.Snoop(SnoopableType.Selection);
                break;
            case "linked":
                await _snoopService.Snoop(SnoopableType.LinkedElement);
                break;
            case "face":
                await _snoopService.Snoop(SnoopableType.Face);
                break;
            case "edge":
                await _snoopService.Snoop(SnoopableType.Edge);
                break;
            case "point":
                await _snoopService.Snoop(SnoopableType.Point);
                break;
            case "subElement":
                await _snoopService.Snoop(SnoopableType.SubElement);
                break;
            case "components":
                await _snoopService.Snoop(SnoopableType.ComponentManager);
                break;
            case "performance":
                await _snoopService.Snoop(SnoopableType.PerformanceAdviser);
                break;
            case "updaters":
                await _snoopService.Snoop(SnoopableType.UpdaterRegistry);
                break;
            case "services":
                await _snoopService.Snoop(SnoopableType.Services);
                break;
            case "schemas":
                await _snoopService.Snoop(SnoopableType.Schemas);
                break;
            case "events":
                var service = (EventsViewModel) _serviceProvider.GetService(typeof(EventsViewModel));
                await service.Snoop(SnoopableType.Events);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(parameter), parameter);
        }
    }

    [RelayCommand]
    private async Task OpenDialog(string parameter)
    {
        if (!Validate()) return;

        var dialog = _dialogService.CreateDialog();
        switch (parameter)
        {
            case "parameters":
                dialog.Title = "BuiltIn Parameters";
                dialog.Content = new UnitsDialog(typeof(BuiltInParameter));
                dialog.DialogWidth = 800;
                dialog.DialogHeight = 600;
                await dialog.ShowAsync();
                break;
            case "categories":
                dialog.Title = "BuiltIn Categories";
                dialog.Content = new UnitsDialog(typeof(BuiltInCategory));
                dialog.DialogWidth = 800;
                dialog.DialogHeight = 600;
                await dialog.ShowAsync();
                break;
            case "forge":
                dialog.Title = "Forge Schema";
                dialog.Content = new UnitsDialog(typeof(ForgeTypeId));
                dialog.DialogWidth = 800;
                dialog.DialogHeight = 600;
                await dialog.ShowAsync();
                break;
            case "search":
                dialog = new SearchElementsDialog(_serviceProvider, _dialogService.GetContentPresenter());
                dialog.DialogWidth = 570;
                dialog.DialogHeight = 330;
                await dialog.ShowAsync();
                break;
        }
    }

    private bool Validate()
    {
        //TODO MOQ data skip validation
        // return true;
        if (RevitApi.UiDocument is not null) return true;

        _snackbarService.Show("Request denied", "There are no open documents", SymbolRegular.Warning24, ControlAppearance.Caution);
        return false;
    }
}