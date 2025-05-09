using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Threading;
using constructionOrgManagement.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using static Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal.MySqlJsonTableExpression;

namespace constructionOrgManagement.Views;

public partial class QueriesView : UserControl
{
    public static QueriesView? Instance { get; private set; }
    public QueriesView()
    {
        Instance = this;
        InitializeComponent();
    }
    public void GenerateDataGridColumns(IEnumerable data, List<ViewModels.DataGridColumnInfo> dataGridColumns)
    {
        queriesDataGrid.Columns.Clear();

        if (data == null || !data.GetEnumerator().MoveNext())
            return;

        if (dataGridColumns?.Count > 0)
        {
            foreach (var columnInfo in dataGridColumns)
            {
                var column = new DataGridTextColumn
                {
                    Header = columnInfo.Header,
                    Binding = new Binding(columnInfo.PropertyName)
                    //{
                    //    StringFormat = columnInfo.FormatString
                    //}
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
        queriesDataGrid.ItemsSource = data as IList ?? data.Cast<object>().ToList();
    }
}