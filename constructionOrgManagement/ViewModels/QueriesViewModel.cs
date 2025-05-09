using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using constructionOrgManagement.Models;
using constructionOrgManagement.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace constructionOrgManagement.ViewModels
{
    public partial class QueriesViewModel : ViewModelBase
    {
        [ObservableProperty] private ObservableCollection<Control> _queriesControls = [];
        [ObservableProperty] private ObservableCollection<object>? _currentQueryData;
        private List<DataGridColumnInfo> _dataGridColumns = [];
        [ObservableProperty] private string _currentQueryTitle = "Выполнение запросов";
        private readonly ConstructionOrganizationContext? _dbContext;
        private readonly Dictionary<string, object> _parameterValues = [];
        private Func<ConstructionOrganizationContext, Dictionary<string, object>, IQueryable>? _queryExecutor;

        public QueriesViewModel(ConstructionOrganizationContext dbContext)
        {
            _dbContext = dbContext;
            CurrentQueryData = null;
        }
        [RelayCommand]
        private void ExecuteQuery()
        {
            try
            {
                if (_dbContext == null || _queryExecutor == null) return;

                var query = _queryExecutor(_dbContext, _parameterValues) as IQueryable<object>;
                CurrentQueryData = [.. query?.ToList()!];
                QueriesView.Instance?.GenerateDataGridColumns(CurrentQueryData, _dataGridColumns);
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                Debug.WriteLine(ex);
            }
        }
        public void Initialize(QueryInfo queryInfo,
                         Func<ConstructionOrganizationContext, Dictionary<string, object>, IQueryable> queryExecutor)
        {
            CurrentQueryTitle = queryInfo.Title;
            _ = queryInfo.Description;
            _queryExecutor = queryExecutor;
            _dataGridColumns = queryInfo.Columns;
            CreateParameterControls(queryInfo.Parameters);
        }
        private void CreateParameterControls(List<QueryParameterInfo> parameters)
        {
            QueriesControls.Clear();
            _parameterValues.Clear();

            foreach (var param in parameters)
            {
                Control control = param.ParameterType switch
                {
                    Type t when t == typeof(DateTime) => CreateDatePicker(param),
                    Type t when t == typeof(int) || t == typeof(decimal) => CreateNumericUpDown(param),
                    Type t when t == typeof(string) && param.AvailableValues?.Count > 0 => CreateComboBox(param),
                    Type t when t == typeof(string) => CreateTextBox(param),
                    Type t when t == typeof(bool) => CreateCheckBox(param),
                    _ => CreateTextBox(param)
                };

                QueriesControls.Add(control);
            }
        }
        private StackPanel CreateDatePicker(QueryParameterInfo param)
        {
            var picker = new DatePicker
            {
                Margin = new Thickness(0, 0, 0, 10),
                SelectedDate = param.DefaultValue as DateTime? ?? DateTime.Today
            };

            picker.PropertyChanged += (s, e) =>
            {
                if (e.Property == DatePicker.SelectedDateProperty)
                    _parameterValues[param.Name] = picker.SelectedDate;
            };

            _parameterValues[param.Name] = picker.SelectedDate;
            return WrapWithLabel(picker, param.Name);
        }
        private StackPanel CreateNumericUpDown(QueryParameterInfo param)
        {
            var numeric = new NumericUpDown
            {
                Margin = new Thickness(0, 0, 0, 10),
                Value = Convert.ToDecimal(param.DefaultValue ?? 0)
            };

            numeric.PropertyChanged += (s, e) =>
            {
                if (e.Property == NumericUpDown.ValueProperty)
                    _parameterValues[param.Name] = numeric.Value;
            };

            _parameterValues[param.Name] = numeric.Value;
            return WrapWithLabel(numeric, param.Name);
        }
        private StackPanel CreateComboBox(QueryParameterInfo param)
        {
            var combo = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 10),
                ItemsSource = param.AvailableValues,
                SelectedItem = param.DefaultValue ?? param.AvailableValues?.FirstOrDefault()
            };

            combo.PropertyChanged += (s, e) =>
            {
                if (e.Property == ComboBox.SelectedItemProperty)
                    _parameterValues[param.Name] = combo.SelectedItem!;
            };

            _parameterValues[param.Name] = combo.SelectedItem!;
            return WrapWithLabel(combo, param.Name);
        }
        private StackPanel CreateCheckBox(QueryParameterInfo param)
        {
            var checkBox = new CheckBox
            {
                Margin = new Thickness(0, 0, 0, 10),
                IsChecked = param.DefaultValue as bool? ?? false
            };

            checkBox.PropertyChanged += (s, e) =>
            {
                if (e.Property == CheckBox.IsCheckedProperty)
                    _parameterValues[param.Name] = checkBox.IsChecked;
            };

            _parameterValues[param.Name] = checkBox.IsChecked;
            return WrapWithLabel(checkBox, param.Name);
        }
        private StackPanel CreateTextBox(QueryParameterInfo param)
        {
            var textBox = new TextBox
            {
                Margin = new Thickness(0, 0, 0, 10),
                Text = param.DefaultValue?.ToString() ?? string.Empty
            };

            textBox.PropertyChanged += (s, e) =>
            {
                if (e.Property == TextBox.TextProperty)
                    _parameterValues[param.Name] = textBox.Text;
            };

            _parameterValues[param.Name] = textBox.Text;
            return WrapWithLabel(textBox, param.Name);
        }
        private static StackPanel WrapWithLabel(Control control, string labelText)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
            stackPanel.Children.Add(new TextBlock { Text = labelText });
            stackPanel.Children.Add(control);
            return stackPanel;
        }
    }
    public class QueryParameterInfo
    {
        public required string Name { get; set; }
        public required Type ParameterType { get; set; }
        public required object DefaultValue { get; set; }
        public List<object>? AvailableValues { get; set; }
    }
    public class DataGridColumnInfo
    {
        public required string PropertyName { get; set; }
        public required string Header { get; set; }
        public string? FormatString { get; set; }
    }

    public class QueryInfo
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public List<QueryParameterInfo> Parameters { get; set; } = [];
        public List<DataGridColumnInfo> Columns { get; set; } = [];
    }
}
