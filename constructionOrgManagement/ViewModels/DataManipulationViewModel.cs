using Avalonia.Controls;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using constructionOrgManagement.Models;
using constructionOrgManagement.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace constructionOrgManagement.ViewModels
{
    
    public partial class DataManipulationViewModel : ViewModelBase
    {
        private string? _selectedTable;
        private static ConstructionOrganizationContext? _dbContext;

        [ObservableProperty] private List<string>? _availableTables;

        [ObservableProperty] private string? _filterText;

        [ObservableProperty] private List<object>? _currentData;
        [ObservableProperty] private List<object>? _originalData;

        [ObservableProperty] private ObservableCollection<object>? _filterColumns = [];
        [ObservableProperty] private string? _filterColumn = null;
        public string? SelectedTable
        {
            get => _selectedTable;
            set
            {
                if (_selectedTable != value)
                {
                    _selectedTable = value;
                    OnPropertyChanged(nameof(SelectedTable));
                    RefreshData();
                    UpdateFilterColumns();
                }
            }
        }
        partial void OnFilterTextChanged(string? value)
        {
            if (CurrentData == null || OriginalData == null) return;

            if (string.IsNullOrWhiteSpace(FilterText))
            {
                CurrentData = OriginalData.ToList();
                return;
            }

            List<object> filteredItems = [];
            if (FilterColumn == null || FilterColumn == "Все поля")
            {
                filteredItems = OriginalData
                    .Where(item => item.GetType().GetProperties()
                    .Where(prop => !prop.Name.Contains("id", StringComparison.OrdinalIgnoreCase))
                        .Any(prop =>
                        {
                            var value = prop.GetValue(item)?.ToString();
                            return value != null && value.Contains(FilterText, StringComparison.OrdinalIgnoreCase);
                        }))
                    .ToList();
            }
            else
            {
                filteredItems = OriginalData.Where(item =>
                                {
                                    var prop = item.GetType().GetProperty(FilterColumn);
                                    if (prop == null) return false;

                                    var propValue = prop.GetValue(item)?.ToString();
                                    return propValue != null &&
                                           propValue.Contains(FilterText, StringComparison.OrdinalIgnoreCase);
                                }).ToList();
            }

            CurrentData = filteredItems;
        }
        partial void OnFilterColumnChanged(string? value)
        {
            if (!string.IsNullOrWhiteSpace(FilterText)) OnFilterTextChanged(FilterText);
        }

        public DataManipulationViewModel(ConstructionOrganizationContext dbContext)
        {
            AvailableTables =
            [
                "Brigade",
                //"BrigadeComposition",
                "BuildingMaterial",
                "ConstructionDepartment",
                "Contract",
                "Customer",
                "DepartmentEquipment",
                "Employee",
                "EmployeeAttribute",
                "EmployeeCategory",
                "Estimate",
                "Object",
                "ObjectCategory",
                "ObjectCharacteristic",
                "ObjectEquipment",
                //"ObjectMaster",
                "OrganizationEquipment",
                "Site",
                "SpecificEmployeeAttribute",
                "SpecificObjectCharacteristic",
                "WorkSchedule",
                "WorkType",
                "WorkTypeForCategory"
            ];
            _dbContext = dbContext;
        }
        private List<object>? GetEnhancedData(string? tableName)
        {
            if (_dbContext == null) return [];
            try
            {
                if (string.IsNullOrEmpty(tableName)) return [];

                var dbSetProperty = _dbContext.GetType().GetProperty(tableName + "s") ?? _dbContext.GetType().GetProperty(tableName[..^1] + "ies");
                if (dbSetProperty == null) return null;

                if (dbSetProperty.GetValue(_dbContext) is not IEnumerable<object> dbSet) return null;

                var items = dbSet.ToList();
                if (items.Count == 0) return [];

                return tableName switch
                {
                    "Employee" => GetEmployeeData(items),
                    "Brigade" => GetBrigadeData(items),
                    "Contract" => GetContractData(items),
                    "Object" => GetObjectData(items),
                    "Site" => GetSiteData(items),
                    "ConstructionDepartment" => GetConstructionDepartmentData(items),
                    "OrganizationEquipment" => GetOrganizationEquipmentData(items),
                    "BuildingMaterial" => GetBuildingMaterialData(items),
                    "Customer" => GetCustomerData(items),
                    "DepartmentEquipment" => GetDepartmentEquipmentData(items),
                    "EmployeeAttribute" => GetEmployeeAttributeData(items),
                    "EmployeeCategory" => GetEmployeeCategoryData(items),
                    "Estimate" => GetEstimateData(items),
                    "ObjectCategory" => GetObjectCategoryData(items),
                    "ObjectCharacteristic" => GetObjectCharacteristicData(items),
                    "ObjectEquipment" => GetObjectEquipmentData(items),
                    "SpecificEmployeeAttribute" => GetSpecificEmployeeAttributeData(items),
                    "SpecificObjectCharacteristic" => GetSpecificObjectCharacteristicData(items),
                    "WorkSchedule" => GetWorkScheduleData(items),
                    "WorkType" => GetWorkTypeData(items),
                    "WorkTypeForCategory" => GetWorkTypeForCategoryData(items),
                    _ => GetDefaultData(items)
                };
            }
            catch (Exception ex)
            {
                _currentNotification = CreateNotification("Ошибка", $"{ex}", NotificationManager, _currentNotification);
                return [];
            }
        }
        public static string? GetFullName(Employee? employee)
        {
            if (employee == null) return null;
            return string.Join(" ", employee.Surname, employee.Name, employee.Patronymic ?? string.Empty);
        }
        private void UpdateFilterColumns()
        {
            if (OriginalData == null || FilterColumns == null || OriginalData.Count == 0) return;

            FilterColumns.Clear();

            var properties = OriginalData[0].GetType()
                                    .GetProperties()
                                    .Where(p => !p.Name.Contains("Id", StringComparison.OrdinalIgnoreCase))
                                    .ToList();

            foreach (var prop in properties)
            {
                FilterColumns.Add(prop.Name);
            }
            FilterColumns.Add("Все поля");
        }
        [RelayCommand]
        private void RefreshData()
        {
            CurrentData = GetEnhancedData(SelectedTable);
            OriginalData = CurrentData;
            OnFilterColumnChanged(FilterColumn);
        }
        [RelayCommand]
        private void Edit(object selectedItem)
        {
            if (selectedItem == null || _dbContext == null) return;

            var originalEntity = FindOriginalEntity(selectedItem, GetEntityType(SelectedTable));
            if (originalEntity is Models.Object)
            {
                _dbContext.Entry(originalEntity).Collection("MasterEmployees").Load();
                _dbContext.Entry(originalEntity).Collection("SpecificObjectCharacteristics").Load();
                _dbContext.Entry(originalEntity).Collection("Estimates").Load();
                _dbContext.Entry(originalEntity).Collection("ObjectEquipments").Load();
            }
            else if (originalEntity is Brigade) _dbContext.Entry(originalEntity).Collection("Workers").Load();
            else if (originalEntity is Employee) _dbContext.Entry(originalEntity).Collection("SpecificEmployeeAttributes").Load(); 
            else if (originalEntity is ObjectCategory) _dbContext.Entry(originalEntity).Collection("WorkTypeForCategories").Load(); 
            else if (originalEntity is ConstructionDepartment) _dbContext.Entry(originalEntity).Collection("DepartmentEquipments").Load();

            if (originalEntity == null) return;

            MainWindowViewModel.Instance?.SwitchContent(typeof(DataEditViewModel));

            if (MainWindowViewModel.Instance?.ContentSwitcher is DataEditViewModel editVm)
            {
                editVm.SelectedTable = SelectedTable;
                editVm.OriginalEntity = originalEntity;
                editVm.DataManipulationMode = DataEditViewModel.ManipulationMode.Edit;
                editVm.Initialize();
                SelectedTable = null;
                FilterText = null;
            }
        }
        [RelayCommand]
        private void Add()
        {
            if (SelectedTable == null) return;

            var entityType = GetEntityType(SelectedTable);
            if (entityType == null) return;

            var newEntity = Activator.CreateInstance(entityType);
            if (newEntity == null) return;

            MainWindowViewModel.Instance?.SwitchContent(typeof(DataEditViewModel));

            if (MainWindowViewModel.Instance?.ContentSwitcher is DataEditViewModel editVm)
            {
                editVm.SelectedTable = SelectedTable;
                editVm.OriginalEntity = newEntity;
                editVm.DataManipulationMode = DataEditViewModel.ManipulationMode.Add;
                editVm.Initialize();
                SelectedTable = null;
                FilterText = null;
            }
        }
        [RelayCommand]
        private async Task Delete(object selectedItem)
        {
            if (_dbContext == null) return;
            if (selectedItem == null || string.IsNullOrEmpty(SelectedTable))
            {
                _currentNotification = CreateNotification("Ошибка", "Не выбран объект для удаления", NotificationManager, _currentNotification);
                return;
            }

            try
            {
                var entityType = GetEntityType(SelectedTable);
                if (entityType == null)
                {
                    _currentNotification = CreateNotification("Ошибка", $"Не найдена таблица {SelectedTable}", NotificationManager, _currentNotification);
                    return;
                }
                var originalEntity = FindOriginalEntity(selectedItem, entityType);
                if (originalEntity == null)
                {
                    _currentNotification = CreateNotification("Ошибка", "Не удалось найти объект в базе данных", NotificationManager, _currentNotification);
                    return;
                }

                var restrictErrors = await CheckRestrictConstraintsAsync(originalEntity, entityType);
                if (restrictErrors.Count != 0)
                {
                    _currentNotification = CreateNotification("Ошибка", $"Невозможно удалить объект:\n{string.Join("\n", restrictErrors)}", NotificationManager, _currentNotification);
                    return;
                }

                if (!await ShowConfirmationDialog("Подтверждение удаления",
                    $"Вы уверены, что хотите удалить\nэтот объект из таблицы {SelectedTable}?")) return;

                _dbContext.Remove(originalEntity);
                await _dbContext.SaveChangesAsync();

                RefreshData();
                _currentNotification = CreateNotification("Успех", "Объект успешно удален", NotificationManager, _currentNotification);
            }
            catch (Exception ex)
            {
                _currentNotification = CreateNotification("Ошибка", $"Ошибка при удалении: {ex.Message}", NotificationManager, _currentNotification);
            }
        }
        [RelayCommand]
        private void CopyElement(object selectedItem)
        {
            if (selectedItem == null) return;

            var text = string.Join("\n", selectedItem.GetType()
                                                     .GetProperties()
                                                     .Where(p => !p.Name.Contains("Id", StringComparison.OrdinalIgnoreCase))
                                                     .Select(p => $"{p.Name}: {p.GetValue(selectedItem)}"));

            if (DataManipulationView.Instance?.GetVisualRoot() is TopLevel topLevel)
            {
                topLevel?.Clipboard?.SetTextAsync(text);
                _currentNotification = CreateNotification("Успех", "Объект успешно скопирован в буфер обмена.", NotificationManager, _currentNotification);
            }
            else 
            {
                _currentNotification = CreateNotification("Ошибка", $"Не удалось скопировать объект в буфер обмена.", NotificationManager, _currentNotification);
            }
        }
        private static Type? GetEntityType(string? tableName)
        {
            if (_dbContext == null) return null;
            return _dbContext.Model.GetEntityTypes().FirstOrDefault(e => e.ClrType.Name == tableName ||
                                                                        e.ClrType.Name + "s" == tableName ||
                                                                        e.ClrType.Name[..^1] + "ies" == tableName ||
                                                                        e.GetTableName() == tableName)?.ClrType;
        }
        private static object? FindOriginalEntity(object gridItem, Type? entityType)
        {
            if (_dbContext == null) return null;
            var entity = Activator.CreateInstance(entityType!);
            if (entity == null) return null;
            var primaryKey = _dbContext.Model.FindEntityType(entityType!)?.FindPrimaryKey();
            if (primaryKey == null) return null;

            foreach (var property in primaryKey.Properties)
            {
                var gridItemProp = gridItem.GetType().GetProperty(property.Name);
                if (gridItemProp == null) continue;

                var value = gridItemProp.GetValue(gridItem);
                entity.GetType().GetProperty(property.Name)?.SetValue(entity, value);
            }

            return _dbContext.Find(entityType!, [.. primaryKey.Properties.Select(p => entity.GetType().GetProperty(p.Name)?.GetValue(entity))]);
        }
        private static async Task<List<string>> CheckRestrictConstraintsAsync(object entity, Type entityType)
        {
            if (_dbContext == null) return [];

            var errors = new List<string>();
            var entityEntry = _dbContext.Entry(entity);

            var entityMetadata = _dbContext.Model.FindEntityType(entityType)!;

            foreach (var navigation in entityMetadata.GetNavigations())
            {
                if (navigation.ForeignKey.DeleteBehavior == DeleteBehavior.SetNull ||
                    navigation.ForeignKey.DeleteBehavior == DeleteBehavior.Cascade)
                    continue;

                if (!entityEntry.Navigation(navigation.Name).IsLoaded)
                {
                    await entityEntry.Navigation(navigation.Name).LoadAsync();
                }

                var relatedValue = entityEntry.Navigation(navigation.Name).CurrentValue;

                if (relatedValue != null && relatedValue is IEnumerable<object> collection && collection.Any())
                {
                    errors.Add($"• Используется в {navigation.Name} ({collection.Count()} записей)");
                }
            }
            return errors;
        }
    }
}
