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
using System.Windows.Controls;
using System.Windows.Input;
using RevitLookup.ViewModels.Dialogs;
using RevitLookup.ViewModels.Objects;
using RevitLookup.Views.Extensions;

namespace RevitLookup.Views.Dialogs;

public sealed partial class UnitsDialog
{
    public UnitsDialog(List<UnitInfo> unitType)
    {
        InitializeComponent();
        DataContext = new UnitsViewModel(unitType);
    }

    private void OnRowLoaded(object sender, RoutedEventArgs routedEventArgs)
    {
        var element = (FrameworkElement) sender;
        var unitInfo = (UnitInfo) element.DataContext;
        CreateTreeContextMenu(unitInfo, element);
    }

    private void CreateTreeContextMenu(UnitInfo info, FrameworkElement row)
    {
        row.ContextMenu = new ContextMenu();
        row.ContextMenu.AddMenuItem(Resources["CopyMenuItem"])
            .SetHeader("Copy unit")
            .SetCommand(info, parameter => Clipboard.SetText(parameter.Unit))
            .SetShortcut(row, ModifierKeys.Control, Key.C);
        row.ContextMenu.AddMenuItem(Resources["CopyMenuItem"])
            .SetHeader("Copy label")
            .SetCommand(info, parameter => Clipboard.SetText(parameter.Label))
            .SetShortcut(row, ModifierKeys.Control | ModifierKeys.Shift, Key.C);
    }
}