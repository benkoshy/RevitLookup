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

using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitLookup.Core;

public static class RevitApi
{
    public static UIApplication UiApplication { get; set; }
    public static Autodesk.Revit.ApplicationServices.Application Application => UiApplication.Application;
    public static UIDocument UiDocument => UiApplication.ActiveUIDocument;
    public static Document Document => UiApplication.ActiveUIDocument.Document;
    public static View ActiveView => UiApplication.ActiveUIDocument.ActiveView;

    public static UIControlledApplication CreateUiControlledApplication()
    {
        return (UIControlledApplication) Activator.CreateInstance(
            typeof(UIControlledApplication),
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new object[] {UiApplication},
            null);
    }
}