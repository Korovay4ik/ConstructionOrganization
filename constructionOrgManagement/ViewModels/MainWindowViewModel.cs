using CommunityToolkit.Mvvm.ComponentModel;
using constructionOrgManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace constructionOrgManagement.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private ViewModelBase? contentSwitcher;
        private readonly DataManipulationViewModel _dataManipulationVM = new();
        private readonly DataEditViewModel _dataEditVM = new();
        
        public static MainWindowViewModel? Instance { get; private set; }

        public MainWindowViewModel()
        {
            Instance = this;

            Task.Run(() =>
            {
                using var tempContext = new ConstructionOrganizationContext();
                tempContext.Database.OpenConnection();
                tempContext.Database.CloseConnection();
            });
        }
        public void SwitchContent(Type viewModelType)
        {
            ContentSwitcher = viewModelType switch
            {
                Type type when type == typeof(DataManipulationViewModel) => _dataManipulationVM,
                Type type when type == typeof(DataEditViewModel) => _dataEditVM,
                _ => throw new ArgumentException("Неизвестный тип."),
            };
        }
    }
}
