using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using constructionOrgManagement.ViewModels;
using System;
using System.Linq;

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
    private void OnDropdownButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Parent is Panel parentPanel)
        {
            if (parentPanel.Parent is Panel grandParent)
            {
                foreach (var child in grandParent.Children)
                {
                    if (child is StackPanel panel && panel != parentPanel)
                    {
                        var submenu = panel.Children.OfType<StackPanel>()
                            .FirstOrDefault(p => p.Classes.Contains("submenu-panel"));
                        if (submenu != null) submenu.IsVisible = false;
                    }
                }
            }

            var currentSubmenu = parentPanel.Children.OfType<StackPanel>()
                .FirstOrDefault(p => p.Classes.Contains("submenu-panel"));
            if (currentSubmenu != null) currentSubmenu.IsVisible = !currentSubmenu.IsVisible;
        }
    }
}