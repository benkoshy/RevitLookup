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

using CommunityToolkit.Mvvm.ComponentModel;
using RevitLookup.ViewModels.Objects;

namespace RevitLookup.ViewModels.Dialogs;

public sealed partial class UnitsViewModel : ObservableObject
{
    private readonly List<UnitInfo> _units;
    [ObservableProperty] private List<UnitInfo> _filteredUnits;
    [ObservableProperty] private string _searchText = string.Empty;

    public UnitsViewModel(List<UnitInfo> unitType)
    {
        _units = unitType;
        _filteredUnits = _units;
    }

    async partial void OnSearchTextChanged(string value)
    {
        if (string.IsNullOrEmpty(SearchText))
        {
            FilteredUnits = _units;
            return;
        }

        FilteredUnits = await Task.Run(() =>
        {
            var formattedText = value.ToLower().Trim();
            var searchResults = new List<UnitInfo>();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var family in _units)
                if (family.Label.ToLower().Contains(formattedText) || family.Unit.ToLower().Contains(formattedText))
                    searchResults.Add(family);

            return searchResults;
        });
    }
}