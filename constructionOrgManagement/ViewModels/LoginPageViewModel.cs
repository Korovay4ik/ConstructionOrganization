using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using constructionOrgManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace constructionOrgManagement.ViewModels;

public partial class LoginPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PasswordChar))]
    [NotifyPropertyChangedFor(nameof(ShowPasswordText))]
    private bool _showPassword;

    public event Action<object, DatabaseConnectionSettings>? LoginSuccess;

    public string ShowPasswordText => ShowPassword ? "Скрыть" : "Показать";

    public char PasswordChar => ShowPassword ? '\0' : '●';

    [RelayCommand]
    private void ToggleShowPassword()
    {
        ShowPassword = !ShowPassword;
    }

    [RelayCommand]
    private void Login()
    {
        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
        {
            _currentNotification = CreateNotification("Предупреждение", "Поля логин и пароль должны быть заполнены.", NotificationManager, _currentNotification);
            return;
        }
        try
        {
            if (Username == "root") throw new ArgumentException();
            var settings = new DatabaseConnectionSettings
            {
                Host = "MySql-8.2",
                Database = "construction_organization",
                UserLogin = Username,
                UserPassword = Password
            };

            using var context = new ConstructionOrganizationContext(settings);
            context.Database.OpenConnection();
            context.Database.CloseConnection();

            LoginSuccess?.Invoke(this, settings);
        }
        catch (Exception)
        {
            _currentNotification = CreateNotification("Ошибка", "Неверный логин или пароль.", NotificationManager, _currentNotification);
        }
    }
}