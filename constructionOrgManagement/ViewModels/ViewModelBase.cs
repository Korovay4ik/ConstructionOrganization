using Avalonia.Notification;
using CommunityToolkit.Mvvm.ComponentModel;
using constructionOrgManagement.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Threading.Tasks;

namespace constructionOrgManagement.ViewModels;

public class ViewModelBase : ObservableObject
{
    public INotificationMessageManager NotificationManager { get; } = new NotificationMessageManager();
    protected INotificationMessage? _currentNotification;

    protected static INotificationMessage? CreateNotification(string badge, string message, INotificationMessageManager notificationManager, INotificationMessage? currentNotification)
    {
        notificationManager.Dismiss(currentNotification!);

        string accentColor, backColor, foreColor;
        switch (badge)
        {
            case "Успех":
                accentColor = "#4CAF50"; backColor = "#F1F8E9"; foreColor = "#1B5E20";
                break;
            case "Ошибка":
                accentColor = "#F44336"; backColor = "#FFEBEE"; foreColor = "#B71C1C";
                break;
            case "Предупреждение":
                accentColor = "#FFC107"; backColor = "#FFF8E1"; foreColor = "#FF6F00";
                break;
            default:
                accentColor = "#2196F3"; backColor = "#E3F2FD"; foreColor = "#0D47A1";
                break;
        }
        currentNotification = notificationManager.CreateMessage()
            .Accent(accentColor).Animates(true).Background(backColor).Foreground(foreColor)
            .HasBadge(badge).HasMessage(message)
            .Dismiss().WithButton("Закрыть", button => {
                notificationManager.Dismiss(currentNotification!);
                currentNotification = null;
            })
            .Dismiss().WithDelay(TimeSpan.FromSeconds(4)).Queue();
        return currentNotification;
    }
    protected static async Task<bool> ShowConfirmationDialog(string title, string message)
    {
        var result = await MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.YesNo, Icon.Warning).ShowAsync();

        return result == ButtonResult.Yes;
    }
    protected static object? CloneObject(object source)
    {
        var clone = Activator.CreateInstance(source.GetType());

        foreach (var property in source.GetType().GetProperties())
        {
            if (property.PropertyType.IsValueType ||
                property.PropertyType == typeof(string))
            {
                var value = property.GetValue(source);
                property.SetValue(clone, value);
            }
        }

        return clone;
    }
}
