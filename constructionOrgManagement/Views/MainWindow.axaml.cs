using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using constructionOrgManagement.ViewModels;
using System;

namespace constructionOrgManagement.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
    private void ToggleTheme_Click(object sender, RoutedEventArgs e)
    {
        Application.Current!.RequestedThemeVariant = 
            Application.Current.ActualThemeVariant == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
    }
}