using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace constructionOrgManagement.ViewModels
{
    public partial class DataEditViewModel : ViewModelBase
    {
        [ObservableProperty] private string? _selectedTable;
        [ObservableProperty] private object? _originalEntity;

        [ObservableProperty] private List<Control> _editControls = [];

        private class CollectionEditState(
            object originalCollection,
            IList currentItems,
            IList availableItems,
            Action applyChanges,
            Action revertChanges)
        {
            public object OriginalCollection { get; } = originalCollection;
            public IList CurrentItems { get; } = currentItems;
            public IList AvailableItems { get; } = availableItems;
            public Action ApplyChanges { get; } = applyChanges;
            public Action RevertChanges { get; } = revertChanges;
        }

        private readonly List<CollectionEditState> _currentCollectionEditState = [];
        public ManipulationMode DataManipulationMode { get; set; }
        public enum ManipulationMode
        {
            Edit,
            Add
        }

        private readonly Thickness _defaultMargin = new(8);

        [RelayCommand]
        private async Task Save()
        {
            if (OriginalEntity == null) return;

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                if (DataManipulationMode == ManipulationMode.Add)
                {
                    dbContext.Add(OriginalEntity);
                }
                _currentCollectionEditState.ForEach(es => es?.ApplyChanges?.Invoke());

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _currentCollectionEditState?.Clear();

                SwitchToDataManipulationView();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                CreateNotification("Ошибка", ex.InnerException?.Message ?? ex.Message, NotificationManager, _currentNotification);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            _currentCollectionEditState.ForEach(es => es?.RevertChanges?.Invoke());

            if (OriginalEntity != null) dbContext.Entry(OriginalEntity).State = EntityState.Detached;

            _currentCollectionEditState?.Clear();

            SwitchToDataManipulationView();
        }
        private void SwitchToDataManipulationView()
        {
            MainWindowViewModel.Instance?.SwitchContent(typeof(DataManipulationViewModel));
            if (MainWindowViewModel.Instance?.ContentSwitcher is DataManipulationViewModel vm) vm.SelectedTable = SelectedTable;
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
        private void AddEstimateInObjectControls(Models.Object obj)
        {
            var allPossibleMaterial = dbContext.BuildingMaterials.ToList();

            var currentEstimate = dbContext.Estimates
                .Where(e => e.EstimateObjectId == obj.ObjectId)
                .Include(e => e.Material)
                .ToList();

            var columns = new Dictionary<string, string>
            {
                { "Материал", nameof(EstimateDTO.MaterialName) },
                { "Стоимость за единицу", nameof(EstimateDTO.UnitPrice) },
                { "Планируемое количество", nameof(EstimateDTO.PlannedMaterialQuantity) },
                { "Фактическое количество", nameof(EstimateDTO.ActualMaterialQuantity) }
            };

            var columnControls = new Dictionary<string, Func<EstimateDTO, Control>>
            {
                { nameof(EstimateDTO.MaterialName), dto => new TextBlock { Text = dto.MaterialName, VerticalAlignment = VerticalAlignment.Center } },
                { nameof(EstimateDTO.UnitPrice), dto => new NumericUpDown
                    {
                        FormatString = "F2",
                        Maximum = decimal.MaxValue,
                        Minimum = 0
                    }
                },
                { nameof(EstimateDTO.PlannedMaterialQuantity), dto => new NumericUpDown
                    {
                        FormatString = "F2",
                        Maximum = decimal.MaxValue,
                        Minimum = 0
                    }
                },
                { nameof(EstimateDTO.ActualMaterialQuantity), dto => new NumericUpDown
                    {
                        FormatString = "F2",
                        Maximum = decimal.MaxValue,
                        Minimum = 0
                    }
                },
            };

            SetupCollectionEditor<Estimate, EstimateDTO>(
                entityCollection: new ObservableCollection<Estimate>(currentEstimate),
                allPossibleItems: allPossibleMaterial
                    .Where(bm => !currentEstimate.Any(e => e.MaterialId == bm.BuildingMaterialId))
                    .Select(bm => new Estimate
                    {
                        MaterialId = bm.BuildingMaterialId,
                        EstimateObjectId = obj.ObjectId,
                        PlannedMaterialQuantity = 0,
                        ActualMaterialQuantity = null,
                        UnitPrice = 0,
                        Material = bm
                    }),
                entityToViewModel: e => new EstimateDTO
                {
                    MaterialId = e.MaterialId,
                    EstimateObjectId = e.EstimateObjectId,
                    MaterialName = e.Material.MaterialName,
                    UnitPrice = e.UnitPrice,
                    PlannedMaterialQuantity = e.PlannedMaterialQuantity,
                    ActualMaterialQuantity = e.ActualMaterialQuantity
                },
                viewModelToEntity: dto =>
                {
                    var existingEntity = dbContext.Estimates.Local.FirstOrDefault(e =>
                        e.MaterialId == dto.MaterialId && e.EstimateObjectId == dto.EstimateObjectId);

                    if (existingEntity != null)
                    {
                        existingEntity.PlannedMaterialQuantity = dto.PlannedMaterialQuantity;
                        existingEntity.ActualMaterialQuantity = dto.ActualMaterialQuantity;
                        existingEntity.UnitPrice = dto.UnitPrice;
                        return existingEntity;
                    }
                    else
                    {
                        return new Estimate
                        {
                            MaterialId = dto.MaterialId,
                            EstimateObjectId = obj.ObjectId,
                            PlannedMaterialQuantity = dto.PlannedMaterialQuantity,
                            ActualMaterialQuantity = dto.ActualMaterialQuantity,
                            UnitPrice = dto.UnitPrice,
                            Material = allPossibleMaterial.First(bm => bm.BuildingMaterialId == dto.MaterialId)
                        };
                    }
                },
                columns: columns,
                columnControls: columnControls,
                displayMember: nameof(EstimateDTO.MaterialName),
                sortSelector: dto => dto.MaterialName,
                label: "Изменение сметы объекта:",
                addButtonText: "Добавить материал",
                placeholderText: "Выберите материал",
                onAdd: entity => dbContext.Estimates.Add(entity),
                onRemove: entity => dbContext.Estimates.Remove(entity)
            );
        }
        private void SetupCollectionEditor<TEntity, TViewModel>(
                        ICollection<TEntity> entityCollection,
                        IEnumerable<TEntity> allPossibleItems,
                        Func<TEntity, TViewModel> entityToViewModel,
                        Func<TViewModel, TEntity> viewModelToEntity,
                        Dictionary<string, string> columns,
                        Dictionary<string, Func<TViewModel, Control>> columnControls,
                        string displayMember,
                        Func<TViewModel, IComparable> sortSelector,
                        string label,
                        string addButtonText = "Добавить",
                        string placeholderText = "Выберите элемент",
                        Action<TEntity> onAdd = null!,
                        Action<TEntity> onRemove = null!)
        {
            var originalCopy = entityCollection.ToList();

            var currentItems = new ObservableCollection<TViewModel>(
                entityCollection.Select(entityToViewModel).OrderBy(x => sortSelector(x)));

            var availableItems = new ObservableCollection<TViewModel>(
                allPossibleItems.Except(entityCollection)
                               .Select(entityToViewModel)
                               .OrderBy(x => sortSelector(x)));

            _currentCollectionEditState.Add(new CollectionEditState(
                                          originalCollection: originalCopy,
                                          currentItems: currentItems,
                                          availableItems: availableItems,
                                          applyChanges: () =>
                                          {
                                              entityCollection.Clear();
                                              foreach (var vm in currentItems)
                                              {
                                                  entityCollection.Add(viewModelToEntity(vm));
                                              }
                                          },
                                          revertChanges: () =>
                                          {
                                              entityCollection.Clear();
                                              foreach (TEntity entity in originalCopy)
                                              {
                                                  entityCollection.Add(entity);
                                              }
                                          }));

            var dataGrid = CreateEditableDataGrid(currentItems, columns, columnControls);

            dataGrid.Columns.Add(new DataGridTemplateColumn
            {
                Header = "Действия",
                CellTemplate = new FuncDataTemplate<TViewModel>((item, _) =>
                {
                    var button = new Button { Content = "Удалить", Width = 110, HorizontalContentAlignment = HorizontalAlignment.Center};
                    button.Click += (s, e) =>
                    {
                        var entity = viewModelToEntity(item);
                        currentItems.Remove(item);
                        availableItems.Add(item);
                        entityCollection.Remove(entity);
                        onRemove?.Invoke(entity);

                        var sorted = availableItems.OrderBy(x => sortSelector(x)).ToList();
                        availableItems.Clear();
                        foreach (var sortedItem in sorted)
                        {
                            availableItems.Add(sortedItem);
                        }
                    };
                    return button;
                })
            });

            var comboBox = new ComboBox
            {
                ItemsSource = availableItems,
                PlaceholderText = placeholderText,
                DisplayMemberBinding = new Binding(displayMember),
                Margin = _defaultMargin,
                Width = 250
            };

            var addButton = new Button { Content = addButtonText };
            addButton.Click += (s, e) =>
            {
                if (comboBox.SelectedItem is TViewModel selectedItem)
                {
                    var entity = viewModelToEntity(selectedItem);
                    currentItems.Add(selectedItem);
                    availableItems.Remove(selectedItem);
                    entityCollection.Add(entity);
                    onAdd?.Invoke(entity);
                    comboBox.SelectedItem = null;
                }
            };

            var choosePanel = new StackPanel
            {
                Spacing = 5,
                Orientation = Orientation.Horizontal,
                Children = { new TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center, FontWeight = Avalonia.Media.FontWeight.Bold }, comboBox, addButton }
            };

            var panel = new StackPanel();
            panel.Children.Add(choosePanel);
            panel.Children.Add(dataGrid);

            AddControlToEditControls(panel);
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

            var availableEquipment = new ObservableCollection<DepartmentEquipment>([.. dbContext.DepartmentEquipments
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
            //var dataGrid = CreateEditableDataGrid(currentEquipment, columns, columnControls, availableEquipment
                           /*item =>
                           {
                               if (item is ObjectEquipment oe)
                               {
                                   return dbContext.DepartmentEquipments
                                       .Include(de => de.OrgEquipment)
                                       .FirstOrDefault(de => de.DepartmentEquipmentId == oe.EquipmentId)!;
                               }
                               return null!;
                           });*/

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
                        //EquipmentId = selectedDeptEquipment.DepartmentEquipmentId,
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
            //panel.Children.Add(dataGrid);

            AddControlToEditControls(panel);
        }

        private void AddTextBoxControl(string label, string propertyName, int maxLenght = 1000)
        {
            var textBox = new TextBox
            {
                [!TextBox.TextProperty] = new Binding($"OriginalEntity.{propertyName}"),
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
                [!CheckBox.IsCheckedProperty] = new Binding($"OriginalEntity.{propertyName}"),
                Margin = _defaultMargin,
                Width = 300
            };
            var panel = CreateLabeledControlPanel(label, checkBox);
            AddControlToEditControls(panel);
        }
        private void AddNumericUpDownControl(string label, string propertyName, decimal initialValue, int increment, string formatString = "F0")
        {
            var numericUpDown = new NumericUpDown
            {
                Value = initialValue,
                Minimum = 0,
                Maximum = decimal.MaxValue,
                FormatString = formatString,
                Increment = increment,
                [!NumericUpDown.ValueProperty] = new Binding($"OriginalEntity.{propertyName}") { Mode = BindingMode.TwoWay },
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
                [!DatePicker.SelectedDateProperty] = new Binding($"OriginalEntity.{propertyName}")
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
                if (OriginalEntity != null && comboBox.SelectedItem != null)
                {
                    UpdateSelectedItemProperty(comboBox.SelectedItem, propertyName,
                                             isPrimitive, displayMember, valueMember);
                }
            };
        }

        private void UpdateSelectedItemProperty(object selectedComboItem, string propertyName,
                                              bool isPrimitive, string displayMember, string valueMember)
        {
            var prop = OriginalEntity!.GetType().GetProperty(propertyName);
            if (prop == null) return;

            object valueToSet = isPrimitive
                ? selectedComboItem
                : GetValueFromSelectedItem(selectedComboItem, displayMember, valueMember);

            if (valueToSet != null)
            {
                prop.SetValue(OriginalEntity, valueToSet);
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
            control.HorizontalAlignment = HorizontalAlignment.Center;
            EditControls.Add(control);
        }
        private DataGrid CreateEditableDataGrid<T>(
                                ObservableCollection<T> itemsSource,
                                Dictionary<string, string> columns,
                                Dictionary<string, Func<T, Control>> columnControls)
        {
            var dataGrid = new DataGrid
            {
                ItemsSource = itemsSource,
                AutoGenerateColumns = false,
                CanUserReorderColumns = true,
                CanUserSortColumns = true,
                CanUserResizeColumns = true,
                Margin = _defaultMargin,

                HorizontalAlignment = HorizontalAlignment.Center,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                MaxWidth = 1200
            };

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
            return dataGrid;
        }
        private static void SetupControlBinding(Control control, string propertyName)
        {
            switch (control)
            {
                case TextBox textBox:
                    textBox[!TextBox.TextProperty] = new Binding(propertyName, BindingMode.TwoWay);
                    break;
                case NumericUpDown numericUpDown:
                    numericUpDown[!NumericUpDown.ValueProperty] = new Binding(propertyName);
                    numericUpDown.AddHandler(InputElement.TextInputEvent, static (sender, e) => {
                        if (!char.IsDigit(e.Text![0])) e.Handled = true;
                    }, RoutingStrategies.Tunnel);
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
    public class ItemIdAndValueDTO
    {
        public int ItemId { get; set; }
        public required string ValueString { get; set; }
    }
    public class SpecificItemCharacteristicValueDTO
    {
        public int CharacteristicId { get; set; }
        public int SpecificItemId { get; set; }
        public required string CharacteristicValue { get; set; }
        public required string CharacteristicName { get; set; }
    }
    public class EstimateDTO
    {
        public int MaterialId { get; set; }
        public int EstimateObjectId { get; set; }
        public required string MaterialName { get; set; }
        public decimal PlannedMaterialQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? ActualMaterialQuantity { get; set; }
    }
}

