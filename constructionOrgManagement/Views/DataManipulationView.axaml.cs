using Avalonia.Controls;
using System.ComponentModel;
using System;
using constructionOrgManagement.ViewModels;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Data;

namespace constructionOrgManagement.Views;

public partial class DataManipulationView : UserControl
{
    public static DataManipulationView? Instance { get; private set; }
    public DataManipulationView()
    {
        InitializeComponent();
        Instance = this;

        DataContextChanged += OnDataContextChanged;
    }
    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is DataManipulationViewModel viewModel)
        {
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
    }
    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DataManipulationViewModel.CurrentData) &&
            DataContext is DataManipulationViewModel viewModel &&
            viewModel.CurrentData != null)
        {
            UpdateDataGridColumns(viewModel.CurrentData);
        }
    }
    private void UpdateDataGridColumns(List<object> data)
    {
        try
        {
            if (tableDataGrid == null) return;

            tableDataGrid.Columns.Clear();

            if (data.Count == 0) return;

            var properties = data[0].GetType()
                                    .GetProperties()
                                    .Where(p => !p.Name.Contains("Id", StringComparison.OrdinalIgnoreCase))
                                    .ToList();

            foreach (var prop in properties)
            {
                tableDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = prop.Name,
                    Binding = new Binding(prop.Name)
                });
            }
            
            tableDataGrid.ItemsSource = data;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка при обновлении колонок: {ex.Message}");
        }
    }
}