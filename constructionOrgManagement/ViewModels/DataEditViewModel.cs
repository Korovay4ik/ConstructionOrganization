using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using constructionOrgManagement.Converters;
using constructionOrgManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace constructionOrgManagement.ViewModels
{
    public partial class DataEditViewModel : ViewModelBase
    {
        [ObservableProperty] private string? _selectedTable;
        [ObservableProperty] private object? _selectedItem;
        [ObservableProperty] private object? _originalEntity;

        [ObservableProperty] private List<Control> _editControls = [];

        private readonly Thickness _defaultMargin = new(8);

        [RelayCommand]
        private async Task Save()
        {
            if (OriginalEntity == null || SelectedItem == null) return;
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                //var entry = dbContext.Entry(OriginalEntity);
                //entry.CurrentValues.SetValues(SelectedItem);

                //if (SelectedItem is Models.Object obj)
                //{
                //    SaveObjectEquipment(obj);
                //}
                //await dbContext.SaveChangesAsync();
                //MainWindowViewModel.Instance?.SwitchContent(typeof(DataManipulationViewModel));
                var entry = dbContext.Entry(OriginalEntity);
                entry.CurrentValues.SetValues(SelectedItem);

                if (SelectedItem is Models.Object obj)
                {
                    SaveObjectEquipment(obj);
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                MainWindowViewModel.Instance?.SwitchContent(typeof(DataManipulationViewModel));
            }
            catch (Exception ex)
            {
                //dbContext.Entry(OriginalEntity).State = EntityState.Unchanged;
                await transaction.RollbackAsync();
                CreateNotification("Ошибка", ex.InnerException?.Message ?? ex.Message, NotificationManager, _currentNotification);
            }
        }

        //private static async Task<object> ReloadObjectWithNavigationsAsync(object item)
        //{
        //    if (item is Models.Object obj)
        //    {
        //        return await dbContext.Objects
        //        .Include(o => o.ObjectEquipments)
        //            .ThenInclude(oe => oe.Equipment)
        //        .Include(o => o.ObjectSite)
        //        .Include(o => o.ObjectContract)
        //        .FirstOrDefaultAsync(o => o.ObjectId == obj.ObjectId)
        //        ?? throw new Exception("Объект не найден в базе данных");
        //    }
        //    return null!;
        //}

        private void SaveObjectEquipment(Models.Object obj)
        {
            //if (OriginalEntity is not Models.Object originalObject) return;
            //originalObject.ObjectEquipments = obj.ObjectEquipments;
            if (OriginalEntity is not Models.Object originalObject) return;

            // Получаем текущие записи из БД
            var existingEquipment = dbContext.ObjectEquipments
                .Where(oe => oe.EquipmentForObjectId == obj.ObjectId)
                .ToList();

            // Обрабатываем изменения в коллекции
            foreach (var uiItem in obj.ObjectEquipments)
            {
                // Для новых элементов (без ID или с временным ID)
                if (uiItem.EquipmentId == 0)
                {
                    // Убедимся, что Equipment загружен из контекста
                    var equipmentInContext = dbContext.DepartmentEquipments
                        .Include(de => de.OrgEquipment)
                        .FirstOrDefault(de => de.DepartmentEquipmentId == uiItem.EquipmentId);

                    if (equipmentInContext != null)
                    {
                        var newEquipment = new ObjectEquipment
                        {
                            EquipmentForObjectId = obj.ObjectId,
                            EquipmentId = equipmentInContext.DepartmentEquipmentId,
                            Equipment = equipmentInContext,
                            EquipObjectQuantity = uiItem.EquipObjectQuantity,
                            AssignmentDate = uiItem.AssignmentDate,
                            ReturnDate = uiItem.ReturnDate
                        };
                        dbContext.ObjectEquipments.Add(newEquipment);
                    }
                }
                else // Для существующих элементов
                {
                    var dbItem = existingEquipment.FirstOrDefault(e => e.EquipmentId == uiItem.EquipmentId);
                    if (dbItem != null)
                    {
                        dbItem.EquipObjectQuantity = uiItem.EquipObjectQuantity;
                        dbItem.AssignmentDate = uiItem.AssignmentDate;
                        dbItem.ReturnDate = uiItem.ReturnDate;
                    }
                }
            }

            // Удаляем элементы, которые есть в БД, но отсутствуют в UI
            foreach (var dbItem in existingEquipment)
            {
                if (!obj.ObjectEquipments.Any(uiItem => uiItem.EquipmentId == dbItem.EquipmentId))
                {
                    dbContext.ObjectEquipments.Remove(dbItem);
                }
            }
        }

        [RelayCommand]
        private static void Cancel()
        {
            MainWindowViewModel.Instance?.SwitchContent(typeof(DataManipulationViewModel));
        }

        public void Initialize()
        {
            CreateEditControls();
        }

        private void CreateEditControls()
        {
            EditControls.Clear();

            switch (SelectedTable)
            {
                case "Employee":
                    CreateEmployeeControls();
                    break;
                case "Brigade":
                    CreateBrigadeControls();
                    break;
                case "Object":
                    CreateObjectControls();
                    break;
                case "BuildingMaterial":
                    CreateBuildingMaterialControls();
                    break;
                case "ConstructionDepartment":
                    CreateConstructionDepartmentControls();
                    break;
                case "Contract":
                    CreateContractControls();
                    break;
                case "Customer":
                    CreateCustomerControls();
                    break;
                case "DepartmentEquipment":
                    CreateDepartmentEquipmentControls();
                    break;
                case "EmployeeAttribute":
                    CreateEmployeeAttributeControls();
                    break;
                case "EmployeeCategory":
                    CreateEmployeeCategoryControls();
                    break;
                case "Estimate":
                    CreateEstimateControls();
                    break;
                case "ObjectCategory":
                    CreateObjectCategoryControls();
                    break;
                case "ObjectCharacteristic":
                    CreateObjectCharacteristicControls();
                    break;
                case "ObjectEquipment":
                    CreateObjectEquipmentControls();
                    break;
                case "OrganizationEquipment":
                    CreateOrganizationEquipmentControls();
                    break;
                case "Site":
                    CreateSiteControls();
                    break;
                case "SpecificEmployeeAttribute":
                    CreateSpecificEmployeeAttributeControls();
                    break;
                case "SpecificObjectCharacteristic":
                    CreateSpecificObjectCharacteristicControls();
                    break;
                case "WorkSchedule":
                    CreateWorkScheduleControl();
                    break;
                case "WorkType":
                    CreateWorkTypeControls();
                    break;
                case "WorkTypeForCategory":
                    CreateWorkTypeForCategoryControls();
                    break;
                default:
                    break;
            }
        }
        private void CreateObjectControls()
        {
            if (SelectedItem is not Models.Object obj) return;

            var foremans = dbContext.Employees.Include(e => e.EmplCategory)
                                              .Where(e => e.EmplCategory.CategoryType == "Рабочие")
                                              .Select(e => new
                                              {
                                                  e.EmployeeId,
                                                  FullName = $"{e.Surname} {e.Name} {e.Patronymic ?? string.Empty}".Trim()
                                              }).ToList().OrderBy(e => e.FullName);

            AddTextBoxControl("Название объекта", nameof(Models.Object.ObjectName), 100);
            AddComboBoxControl("Прораб", nameof(Models.Object.ForemanId), foremans, "FullName", obj.ForemanId!, "EmployeeId");

            AddTextBoxControl("Адрес объекта", nameof(Models.Object.ObjectLocation), 150);

            AddComboBoxControl("Принадлежит участку", nameof(Models.Object.ObjectSiteId),
                dbContext.Sites.ToList().OrderBy(s=>s.SiteName), "SiteName", obj.ObjectSiteId, "SiteId");

            AddComboBoxControl("По контракту", nameof(Models.Object.ObjectContractId),
                dbContext.Contracts.ToList().OrderBy(c => c.ContractName), "ContractName", obj.ObjectContractId, "ContractId");

            AddComboBoxControl("Категория объекта", nameof(Models.Object.CategoryId),
                dbContext.ObjectCategories.ToList().OrderBy(oc=>oc.ObjCategoryName),
                "ObjCategoryName", obj.CategoryId, "ObjectCategoryId");


            AddComboBoxControl("Статус объекта", nameof(Models.Object.ObjectStatus),
                ["in_planning", "in_progress", "completed", "terminated"], "", obj.ObjectStatus);

            AddObjectEquipmentInObjectControl(obj);
        }

        private void AddObjectEquipmentInObjectControl(Models.Object obj)
        {
            var currentEquipment = new ObservableCollection<ObjectEquipment>([.. dbContext.ObjectEquipments
                      .Where(oe => oe.EquipmentForObjectId == obj.ObjectId)
                      .Include(oe => oe.Equipment)
                      .ThenInclude(de => de.OrgEquipment)]);

            var currentEquipmentIds = dbContext.ObjectEquipments
                                  .Where(oe => oe.EquipmentForObjectId == obj.ObjectId)
                                  .Select(oe => oe.EquipmentId)
                                  .ToList();

            var departmentId = dbContext.Objects
                .Where(o => o.ObjectId == obj.ObjectId)
                .Select(o => o.ObjectSite.SiteDepartmentId)
                .FirstOrDefault();

            var availableEquipment = new ObservableCollection<DepartmentEquipment>( [.. dbContext.DepartmentEquipments
                                     .Where(de => de.DepartmentId == departmentId && !currentEquipmentIds.Contains(de.DepartmentEquipmentId))
                                     .Include(de => de.OrgEquipment)]);

            var columns = new Dictionary<string, string>
            {
                { "Техника", "Equipment.OrgEquipment.EquipmentName" },
                { "Количество", "EquipObjectQuantity" },
                { "Дата выдачи", "AssignmentDate" },
                { "Дата возврата", "ReturnDate" }
            };
            var columnControls = new Dictionary<string, Func<ObjectEquipment, Control>>
            {
                //{ "Equipment.OrgEquipment.EquipmentName", oe => new TextBlock
                //{
                //    Text = oe.Equipment?.OrgEquipment?.EquipmentName ?? string.Empty
                //}},
                { "EquipObjectQuantity", oe => new NumericUpDown { Minimum = 1, Maximum = 10000, Width = 120, FormatString = "F0"}},
                { "AssignmentDate", oe => new DatePicker { Width = 300 }},
                { "ReturnDate", oe => new DatePicker { Width = 300 }}
            };
            var dataGrid = CreateEditableDataGrid(currentEquipment,columns,columnControls,availableEquipment
                           /*item =>
                           {
                               if (item is ObjectEquipment oe)
                               {
                                   return dbContext.DepartmentEquipments
                                       .Include(de => de.OrgEquipment)
                                       .FirstOrDefault(de => de.DepartmentEquipmentId == oe.EquipmentId)!;
                               }
                               return null!;
                           }*/);

            var comboBox = new ComboBox
            {
                ItemsSource = availableEquipment,
                DisplayMemberBinding = new Binding("OrgEquipment.EquipmentName"),
                Margin = _defaultMargin,
                Width = 250
            };

            var addButton = new Button { Content = "Добавить технику" };
            addButton.Click += (s, e) =>
            {
                if (comboBox.SelectedItem is DepartmentEquipment selectedDeptEquipment)
                {
                    // Явно загружаем Equipment из БД
                    var equipmentInContext = dbContext.DepartmentEquipments
                        .Include(de => de.OrgEquipment)
                        .FirstOrDefault(de => de.DepartmentEquipmentId == selectedDeptEquipment.DepartmentEquipmentId);

                    if (equipmentInContext == null) return;

                    var newObjectEquipment = new ObjectEquipment
                    {
                        EquipmentForObjectId = obj.ObjectId,
                        //EquipmentId = equipmentInContext.DepartmentEquipmentId,
                        Equipment = equipmentInContext, // Важно: используем объект из контекста
                        EquipObjectQuantity = 1,
                        AssignmentDate = DateOnly.FromDateTime(DateTime.Now)
                    };

                    currentEquipment.Add(newObjectEquipment);
                    obj.ObjectEquipments.Add(newObjectEquipment);
                    availableEquipment.Remove(selectedDeptEquipment);
                    comboBox.SelectedItem = null;
                }
            };
            var choosePanel = new StackPanel { Spacing = 5, Orientation = Orientation.Horizontal };
            choosePanel.Children.Add(comboBox);
            choosePanel.Children.Add(addButton);

            var panel = new StackPanel();
            panel.Children.Add(choosePanel);
            panel.Children.Add(dataGrid);

            AddControlToEditControls(panel);
        }

        private void AddTextBoxControl(string label, string propertyName, int maxLenght = 1000)
        {
            var textBox = new TextBox
            {
                [!TextBox.TextProperty] = new Binding($"SelectedItem.{propertyName}"),
                Margin = _defaultMargin,
                Width = 300,
                MaxLength = maxLenght
            };

            var panel = CreateLabeledControlPanel(label, textBox);
            AddControlToEditControls(panel);
        }
        private void AddTextBlockControl(string label, object? initialValue)
        {
            var textBlock = new TextBlock
            {
                Text = initialValue?.ToString(),
                Margin = _defaultMargin,
                Width = 300
            };

            var panel = CreateLabeledControlPanel(label, textBlock);
            AddControlToEditControls(panel);
        }
        private void AddCheckBoxControl(string label, string propertyName, bool initialValue)
        {
            var checkBox = new CheckBox
            {
                IsChecked = initialValue,
                [!CheckBox.IsCheckedProperty] = new Binding($"SelectedItem.{propertyName}"),
                Margin = _defaultMargin,
                Width = 300
            };
            var panel = CreateLabeledControlPanel(label, checkBox);
            AddControlToEditControls(panel);
        }
        private void AddNumericUpDownControl(string label, string propertyName, decimal initialValue, int increment, string formatString = "")
        {
            var numericUpDown = new NumericUpDown
            {
                Value = initialValue,
                Minimum = 0,
                Maximum = decimal.MaxValue,
                FormatString = formatString,
                Increment = increment,
                [!NumericUpDown.ValueProperty] = new Binding($"SelectedItem.{propertyName}") { Mode = BindingMode.TwoWay },
                Margin = _defaultMargin,
                Width = 300
            };
            numericUpDown.AddHandler(InputElement.TextInputEvent, static (sender, e) => {
                if (!char.IsDigit(e.Text![0])) e.Handled = true;
            }, RoutingStrategies.Tunnel);

            var panel = CreateLabeledControlPanel(label, numericUpDown);
            AddControlToEditControls(panel);
        }
        private void AddDatePickerControl(string label, string propertyName)
        {
            var datePicker = CreateDatePicker(propertyName, 300);

            var panel = CreateLabeledControlPanel(label, datePicker);
            AddControlToEditControls(panel);
        }
        private DatePicker CreateDatePicker(string propertyName, int width)
        {
            var datePicker = new DatePicker
            {
                [!DatePicker.SelectedDateProperty] = new Binding($"SelectedItem.{propertyName}")
                {
                    Converter = new Converters.DateConverter()
                },
                Margin = _defaultMargin,
                Width = width
            };
            return datePicker;
        }
        private void AddComboBoxControl<T>(string label, string propertyName,
                                         IEnumerable<T> items, string displayMember,
                                         object selectedValue, string valueMember = "")
        {
            var comboBox = SetupComboBox(propertyName, items, displayMember, selectedValue, valueMember);

            var panel = CreateLabeledControlPanel(label, comboBox);
            AddControlToEditControls(panel);
        }
        private ComboBox SetupComboBox<T>(string propertyName, IEnumerable<T> items, string displayMember, object selectedValue, string valueMember = "")
        {
            var (itemsList, isPrimitive) = PrepareComboBoxItems(items);
            var selectedItem = FindSelectedItem(itemsList, selectedValue, isPrimitive, displayMember, valueMember);
            var comboBox = CreateComboBox(itemsList, selectedItem, displayMember, isPrimitive);

            SetupComboBoxSelectionHandler(comboBox, propertyName, isPrimitive, displayMember, valueMember);
            return comboBox;
        }
        private static (List<T> itemsList, bool isPrimitive) PrepareComboBoxItems<T>(IEnumerable<T> items)
        {
            var itemsList = items.ToList();
            var isPrimitive = typeof(T).IsPrimitive || typeof(T) == typeof(string);
            return (itemsList, isPrimitive);
        }

        private static object FindSelectedItem<T>(List<T> itemsList, object selectedValue,
                                         bool isPrimitive, string displayMember, string valueMember)
        {
            if (isPrimitive)
            {
                return itemsList.FirstOrDefault(item => object.Equals(item, selectedValue))!;
            }

            return itemsList.FirstOrDefault(item =>
            {
                if (item == null) return false;
                var prop = string.IsNullOrEmpty(valueMember)
                    ? item.GetType().GetProperty(displayMember)
                    : item.GetType().GetProperty(valueMember);
                if (prop == null) return false;
                return object.Equals(prop.GetValue(item), selectedValue);
            })!;
        }

        private ComboBox CreateComboBox<T>(List<T> items, object selectedItem,
                                         string displayMember, bool isPrimitive)
        {
            var comboBox = new ComboBox
            {
                ItemsSource = items,
                SelectedItem = selectedItem,
                Margin = _defaultMargin,
                Width = 300
            };

            if (!isPrimitive && !string.IsNullOrEmpty(displayMember))
            {
                comboBox.DisplayMemberBinding = new Binding(displayMember);
            }

            return comboBox;
        }

        private void SetupComboBoxSelectionHandler(ComboBox comboBox, string propertyName,
                                                bool isPrimitive, string displayMember, string valueMember)
        {
            comboBox.SelectionChanged += (sender, e) =>
            {
                if (SelectedItem != null && comboBox.SelectedItem != null)
                {
                    UpdateSelectedItemProperty(comboBox.SelectedItem, propertyName,
                                             isPrimitive, displayMember, valueMember);
                }
            };
        }

        private void UpdateSelectedItemProperty(object selectedComboItem, string propertyName,
                                              bool isPrimitive, string displayMember, string valueMember)
        {
            var prop = SelectedItem!.GetType().GetProperty(propertyName);
            if (prop == null) return;

            object valueToSet = isPrimitive
                ? selectedComboItem
                : GetValueFromSelectedItem(selectedComboItem, displayMember, valueMember);

            if (valueToSet != null)
            {
                prop.SetValue(SelectedItem, valueToSet);
            }
        }

        private static object GetValueFromSelectedItem(object selectedItem,
                                              string displayMember, string valueMember)
        {
            var valueProp = string.IsNullOrEmpty(valueMember)
                ? selectedItem.GetType().GetProperty(displayMember)
                : selectedItem.GetType().GetProperty(valueMember);
            return valueProp?.GetValue(selectedItem)!;
        }
        private StackPanel CreateLabeledControlPanel(string label, Control control)
        {
            var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 5 };
            panel.Children.Add(new TextBlock
            {
                Text = label,
                Width = 180,
                Margin = _defaultMargin
            });
            panel.Children.Add(control);
            return panel;
        }
        private void AddControlToEditControls(Control control)
        {
            EditControls.Add(control);
        }
        private DataGrid CreateEditableDataGrid<T, TSource>(
    ObservableCollection<T> itemsSource,
    Dictionary<string, string> columns,
    Dictionary<string, Func<T, Control>> columnControls,
    ObservableCollection<TSource> sourceCollection = null!,
    Func<T, TSource> itemToSourceConverter = null!) where TSource : class
        {
            var dataGrid = new DataGrid
            {
                ItemsSource = itemsSource,
                AutoGenerateColumns = false,
                CanUserReorderColumns = true,
                CanUserSortColumns = true,
                CanUserResizeColumns = true,
                Margin = _defaultMargin,
                Width = 1000,
            };

            // Добавляем колонки из словаря columns
            foreach (var column in columns)
            {
                var propertyName = column.Value;
                var header = column.Key;

                if (columnControls.TryGetValue(propertyName, out var controlFactory))
                {
                    dataGrid.Columns.Add(new DataGridTemplateColumn
                    {
                        Header = header,
                        CellTemplate = new FuncDataTemplate<T>((item, _) =>
                        {
                            var control = controlFactory(item);
                            SetupControlBinding(control, propertyName);
                            return control;
                        })
                    });
                }
                else
                {
                    dataGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = header,
                        Binding = new Binding(propertyName)
                    });
                }
            }

            // Добавляем колонку с кнопкой удаления
            dataGrid.Columns.Add(new DataGridTemplateColumn
            {
                Header = "Действия",
                CellTemplate = new FuncDataTemplate<T>((item, _) =>
                {
                    var button = new Button { Content = "Удалить" };
                    button.Click += (s, e) =>
                    {
                        if (item is ObjectEquipment oe)
                        {
                            // Возвращаем оборудование в список доступных
                            if (sourceCollection != null)
                            {
                                var deptEquipment = dbContext.DepartmentEquipments
                                    .FirstOrDefault(de => de.DepartmentEquipmentId == oe.EquipmentId);
                                if (deptEquipment != null && deptEquipment is TSource sourceItem)
                                {
                                    sourceCollection.Add(sourceItem);
                                }
                            }

                            // Удаляем из текущей коллекции
                            itemsSource.Remove(item);

                            // Если редактируемый объект - Models.Object, удаляем из его коллекции
                            if (SelectedItem is Models.Object obj)
                            {
                                obj.ObjectEquipments.Remove(oe);
                            }
                        }
                        else
                        {
                            // Общая логика для других типов
                            itemsSource.Remove(item);
                            if (sourceCollection != null && itemToSourceConverter != null)
                            {
                                var sourceItem = itemToSourceConverter(item);
                                if (sourceItem != null)
                                {
                                    sourceCollection.Add(sourceItem);
                                }
                            }
                        }
                    };
                    return button;
                })
            });

            return dataGrid;
        }
        private static void SetupControlBinding(Control control, string propertyName)
        {
            switch (control)
            {
                case TextBox textBox:
                    textBox[!TextBox.TextProperty] = new Binding(propertyName);
                    break;
                case NumericUpDown numericUpDown:
                    numericUpDown[!NumericUpDown.ValueProperty] = new Binding(propertyName);
                    break;
                case DatePicker datePicker:
                    datePicker[!DatePicker.SelectedDateProperty] = new Binding(propertyName)
                    {
                        Converter = new DateConverter()
                    };
                    break;
                case Calendar calendar:
                    calendar[!Calendar.SelectedDateProperty] = new Binding(propertyName)
                    {
                        Converter = new DateConverter()
                    };
                    break;
                default:
                    break;
                
                //case ComboBox:
                //    // Для ComboBox нужно дополнительно настроить ItemsSource и SelectedValue
                //    break;
            }
        }
    }
}
