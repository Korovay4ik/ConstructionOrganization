using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using constructionOrgManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace constructionOrgManagement.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private ViewModelBase? contentSwitcher;
        [ObservableProperty] private bool _isMenuVisible;
        private DataManipulationViewModel? _dataManipulationVM;
        private DataEditViewModel? _dataEditVM;
        private QueriesViewModel? _queriesVM;
        private readonly LoginPageViewModel _loginPageVM = new();
        private ConstructionOrganizationContext? _dbContext;

        public static MainWindowViewModel? Instance { get; private set; }

        public MainWindowViewModel()
        {
            Instance = this;
            ShowLoginPage();
        }
        [RelayCommand]
        public void SwitchContent(Type viewModelType)
        {
            ContentSwitcher = viewModelType switch
            {
                Type type when type == typeof(DataManipulationViewModel) => _dataManipulationVM,
                Type type when type == typeof(DataEditViewModel) => _dataEditVM,
                Type type when type == typeof(LoginPageViewModel) => _loginPageVM,
                Type type when type == typeof(QueriesViewModel) => _queriesVM,
                _ => throw new ArgumentException("Неизвестный тип."),
            };
        }
        public void ShowLoginPage()
        {
            IsMenuVisible = false;
            ContentSwitcher = _loginPageVM;

            _loginPageVM.LoginSuccess += OnLoginSuccess;
        }
        private void OnLoginSuccess(object sender, DatabaseConnectionSettings settings)
        {
            _dbContext = new ConstructionOrganizationContext(settings);

            try
            {
                if (!_dbContext.Database.CanConnect())
                {
                    throw new Exception("Не удалось подключиться к базе данных");
                }

                IsMenuVisible = true;
                LoadViewModel();
                SwitchContent(typeof(DataManipulationViewModel));
            }
            catch (Exception ex)
            {
                _currentNotification = CreateNotification("Ошибка", ex.InnerException?.ToString() ?? ex.ToString(), NotificationManager, _currentNotification);
                return;
            }
            _loginPageVM.LoginSuccess -= OnLoginSuccess;
        }
        [RelayCommand]
        private void Logout()
        {
            _loginPageVM.Username = string.Empty;
            _loginPageVM.Password = string.Empty;
            ShowLoginPage();
        }
        private void LoadViewModel()
        {
            if (_dbContext == null) return;
            _dataManipulationVM = new DataManipulationViewModel(_dbContext);
            _dataEditVM = new DataEditViewModel(_dbContext);
            _queriesVM = new QueriesViewModel(_dbContext);
        }
        public void ShowObjectScheduleQuery()
        {
            if (_dbContext == null) return;

            var queryInfo = new QueryInfo
            {
                Title = "График и смета на строительство объекта",
                Description = "Получение графика и сметы на строительство указанного объекта",
                Parameters =
            [
                new() {
                    Name = "Название объекта",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.Objects.OrderBy(o=>o.ObjectName).Select(o=>o.ObjectName)]
                },
                //new() {
                //    Name = "Дата начала периода",
                //    ParameterType = typeof(DateTime),
                //    DefaultValue = DateTime.Today.AddMonths(-1)
                //},
                //new() {
                //    Name = "Дата окончания периода",
                //    ParameterType = typeof(DateTime),
                //    DefaultValue = DateTime.Today
                //}
            ],
                Columns =
                [
                    new()
                    {
                        PropertyName = nameof(ViewObjectScheduleAndEstimate.ObjectName),
                        Header = "Объект",
                    }
                ]
            };

            SwitchContent(typeof(QueriesViewModel));
            if (ContentSwitcher is not QueriesViewModel queriesVm) return;
            queriesVm.Initialize(queryInfo, (db, parameters) =>
            {
                var objectName = (string)parameters["Название объекта"];
                //var startDate = (DateOnly)parameters["Дата начала периода"];
                //var endDate = (DateOnly)parameters["Дата окончания периода"];

                return db.Set<ViewObjectScheduleAndEstimate>()
                    .Where(x => x.ObjectName.Contains(objectName) /*&&
                                (x.PlannedStartDate >= startDate || x.ActualStartDate >= startDate) &&
                                (x.PlannedEndDate <= endDate || x.ActualEndDate <= endDate)*/);
            });
        }
    }
}
