#region Header

//
// Copyright 2003-2021 by Autodesk, Inc. 
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
//

#endregion // Header

using Autodesk.Revit.DB;
using RevitLookup.Views;
using Form = System.Windows.Forms.Form;

namespace RevitLookup.Core.RevitTypes
{
    /// <summary>
    ///     Snoop.Data class to hold and format an ElementSet value.
    /// </summary>
    public class ParameterSet : Data
    {
        private readonly Element _element;
        private readonly Autodesk.Revit.DB.ParameterSet _value;

        public ParameterSet(string label, Element elem, Autodesk.Revit.DB.ParameterSet val) : base(label)
        {
            _value = val;
            _element = elem;
        }

        public override bool HasDrillDown => _value is {IsEmpty: false};

        public override string StrValue()
        {
            return Utils.ObjToLabelStr(_value);
        }

        public override Form DrillDown()
        {
            if (_value is not {IsEmpty: false}) return null;
            var form = new Parameters(_element, _value);
            return form;
        }
    }
}