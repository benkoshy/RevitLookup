// Copyright 2003-2024 by Autodesk, Inc.
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

using System.Data;
using System.IO;
using System.Text;

namespace RevitLookup.ViewModels.Dialogs;

public sealed class FamilySizeTableEditDialogViewModel : DataTable
{
    private readonly FamilySizeTableManager _manager;
    private readonly string _tableName;
    
    public FamilySizeTableEditDialogViewModel(FamilySizeTableManager manager, string tableName)
    {
        _manager = manager;
        DeleteRowCommand = new RelayCommand<DataRow>(DeleteRow);
        DuplicateRowCommand = new RelayCommand<DataRow>(DuplicateRow);
        _tableName = tableName;
        var table = manager.GetSizeTable(tableName);
        CreateColumns(table);
        WriteRows(table);
    }
    
    public RelayCommand<DataRow> DeleteRowCommand { get; }
    public RelayCommand<DataRow> DuplicateRowCommand { get; }
    
    public FamilySizeTableEditDialogViewModel(FamilySizeTable table)
    {
        CreateColumns(table);
        WriteRows(table);
    }
    
    private void WriteRows(FamilySizeTable table)
    {
        for (var i = 0; i < table.NumberOfRows; i++)
        {
            var rowArray = new object[table.NumberOfColumns];
            for (var j = 0; j < table.NumberOfColumns; j++)
            {
                rowArray[j] = table.AsValueString(i, j);
            }
            
            Rows.Add(rowArray);
        }
    }
    
    private void CreateColumns(FamilySizeTable table)
    {
        for (var i = 0; i < table.NumberOfColumns; i++)
        {
            var header = table.GetColumnHeader(i);
            var specId = header.GetSpecTypeId();
            var typeId = table.GetColumnHeader(i).GetUnitTypeId();
            var headerName = table.GetColumnHeader(i).Name;
            
            var columnName = headerName;
            if (!specId.Empty())
            {
                columnName = $"{columnName}##{specId.ToSpecLabel().ToLowerInvariant()}";
            }
            
            if (!typeId.Empty())
            {
                columnName = $"{columnName}##{typeId.ToUnitLabel().ToLowerInvariant()}";
            }
            
            Columns.Add(new DataColumn(columnName, typeof(string)));
        }
    }
    
    public void SaveData()
    {
        var dirPath = Path.Combine(Path.GetTempPath(), "Size Tables");
        Directory.CreateDirectory(dirPath);
        var path = Path.Combine(dirPath, $"{_tableName}.csv");
        var writer = new StreamWriter(path);
        var header = new StringBuilder();
        for (var i = 1; i < Columns.Count; i++)
        {
            var column = Columns[i];
            var name = column.ColumnName.Replace(' ', '_');
            header.Append(",");
            header.Append(name);
        }
        
        writer.WriteLine(header);
        foreach (DataRow row in Rows)
        {
            var result = new StringBuilder();
            foreach (var value in row.ItemArray)
            {
                var recordValue = value.ToString();
                if (recordValue.Contains(','))
                {
                    recordValue = $"[{recordValue}]";
                }
                
                result.Append(recordValue);
                result.Append(",");
            }
            
            result.Remove(result.Length - 1, 1);
            writer.WriteLine(result);
        }
        
        writer.Close();
        Application.ActionEventHandler.Raise(_ =>
        {
            using var transaction = new Transaction(Context.Document, "Change size table");
            transaction.Start();
            _manager.ImportSizeTable(Context.Document, path, new FamilySizeTableErrorInfo());
            transaction.Commit();
            File.Delete(path);
        });
    }
    
    private void DeleteRow(DataRow row)
    {
        Rows.Remove(row);
    }
    
    private void DuplicateRow(DataRow row)
    {
        var index = Rows.IndexOf(row);
        var newRow = NewRow();
        newRow.ItemArray = (object[]) row.ItemArray.Clone();
        Rows.InsertAt(newRow, index + 1);
    }
}