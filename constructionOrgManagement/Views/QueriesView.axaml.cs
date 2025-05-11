using Avalonia.Controls;
using Avalonia.Data;
using constructionOrgManagement.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace constructionOrgManagement.Views;

public partial class QueriesView : UserControl
{
    public static QueriesView? Instance { get; private set; }
    public QueriesView()
    {
        Instance = this;
        InitializeComponent();

        DataContextChanged += OnDataContextChanged;
    }
    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is QueriesViewModel viewModel)
        {
            viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
    }
    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(QueriesViewModel.CurrentQueryData) &&
            DataContext is QueriesViewModel viewModel)
        {
            GenerateDataGridColumns(viewModel.CurrentQueryData!, viewModel.DataGridColumns);
        }
    }
    public void GenerateDataGridColumns(IEnumerable data, List<ViewModels.DataGridColumnInfo> dataGridColumns)
    {
        queriesDataGrid.Columns.Clear();

        if (data == null || !data.GetEnumerator().MoveNext())
            return;

        try
        {
            if (dataGridColumns?.Count > 0)
            {
                foreach (var columnInfo in dataGridColumns)
                {
                    var binding = new Binding(columnInfo.PropertyName)
                    {
                        StringFormat = columnInfo.FormatString,
                        TargetNullValue = columnInfo.NullValue,
                        FallbackValue = columnInfo.NullValue
                    };

                    var column = new DataGridTextColumn
                    {
                        Header = columnInfo.Header,
                        Binding = binding,
                        CanUserSort = true
                    };

                    queriesDataGrid.Columns.Add(column);
                }
            }
            else
            {
                var firstItem = data.Cast<object>().First();
                var properties = firstItem.GetType().GetProperties();

                foreach (var prop in properties)
                {
                    queriesDataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = prop.Name,
                        Binding = new Binding(prop.Name),
                        Width = DataGridLength.Auto
                    });
                }
            }
        }
        catch(Exception)
        {

        }
    }
}