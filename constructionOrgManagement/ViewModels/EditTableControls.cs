using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using constructionOrgManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace constructionOrgManagement.ViewModels
{
    public partial class DataEditViewModel
    {
        private void CreateObjectControls()
        {
            if (OriginalEntity is not Models.Object obj || _dbContext == null) return;

            var foremans = _dbContext.Employees.Include(e => e.EmplCategory)
                                              .Where(e => e.EmplCategory.CategoryType == "Рабочие" && e.FireDate == null)
                                              .Select(e => new
                                              {
                                                  e.EmployeeId,
                                                  FullName = $"{e.Surname} {e.Name} {e.Patronymic ?? string.Empty}".Trim()
                                              }).ToList().OrderBy(e => e.FullName);

            AddTextBoxControl("Название объекта", nameof(Models.Object.ObjectName), 100);
            AddComboBoxControl("Прораб", nameof(Models.Object.ForemanId), foremans, "FullName", obj.ForemanId!, "EmployeeId");

            AddTextBoxControl("Адрес объекта", nameof(Models.Object.ObjectLocation), 150);

            AddComboBoxControl("Принадлежит участку", nameof(Models.Object.ObjectSiteId),
                _dbContext.Sites.ToList().OrderBy(s => s.SiteName), "SiteName", obj.ObjectSiteId, "SiteId");

            AddComboBoxControl("По контракту", nameof(Models.Object.ObjectContractId),
                _dbContext.Contracts.ToList().OrderBy(c => c.ContractName), "ContractName", obj.ObjectContractId, "ContractId");

            AddComboBoxControl("Категория объекта", nameof(Models.Object.CategoryId),
                _dbContext.ObjectCategories.ToList().OrderBy(oc => oc.ObjCategoryName),
                "ObjCategoryName", obj.CategoryId, "ObjectCategoryId");


            AddComboBoxControl("Статус объекта", nameof(Models.Object.ObjectStatus),
                ["in_planning", "in_progress", "completed", "terminated"], "", obj.ObjectStatus);

            if (DataManipulationMode == ManipulationMode.Edit)
            {
                AddObjectEquipmentInObjectControls(obj);
                AddEstimateInObjectControls(obj);
                AddSpecificObjectCharacteristicInObjectControls(obj);
                AddMasterOnObjectControl(obj);
            }
        }
        private void CreateObjectEquipmentControls()
        {
            if (OriginalEntity is not ObjectEquipment oe || _dbContext == null) return;

            if (DataManipulationMode == ManipulationMode.Add)
            {
                AddComboBoxControl("Объект", nameof(ObjectEquipment.EquipmentForObjectId), _dbContext.Objects.ToList().OrderBy(o => o.ObjectName),
                    "ObjectName", oe.EquipmentForObjectId, "ObjectId");
                AddComboBoxControl("Название техники", nameof(ObjectEquipment.EquipmentId), _dbContext.DepartmentEquipments.Include(de=>de.OrgEquipment).ToList().OrderBy(o=>o.OrgEquipment.EquipmentName),
                    "OrgEquipment.EquipmentName", oe.EquipmentId, "DepartmentEquipmentId");
                oe.AssignmentDate = DateOnly.FromDateTime(DateTime.Now);
            }
            else
            {
                AddTextBlockControl("Объект", _dbContext.Objects.FirstOrDefault(o => o.ObjectId == oe.EquipmentForObjectId)?.ObjectName);
                AddTextBlockControl("Название техники", _dbContext.DepartmentEquipments
                                                             .Include(de => de.OrgEquipment)
                                                             .FirstOrDefault(de => de.DepartmentEquipmentId == oe.EquipmentId)?.OrgEquipment?.EquipmentName);
            }
            AddNumericUpDownControl("Количество техники", nameof(ObjectEquipment.EquipObjectQuantity), oe.EquipObjectQuantity, 1);
            AddDatePickerControl("Дата выдачи", nameof(ObjectEquipment.AssignmentDate));
            AddDatePickerControl("Дата возврата", nameof(ObjectEquipment.ReturnDate));
        }

        private void CreateWorkTypeForCategoryControls()
        {
            if (OriginalEntity is not WorkTypeForCategory wtfc || _dbContext == null) return;

            if (DataManipulationMode == ManipulationMode.Add)
            {
                AddComboBoxControl("Категория объекта", nameof(WorkTypeForCategory.SpecificCategoryId), _dbContext.ObjectCategories.ToList().OrderBy(oc=>oc.ObjCategoryName),
                    "ObjCategoryName", wtfc.SpecificCategoryId, "ObjectCategoryId");
                AddComboBoxControl("Тип работ", nameof(WorkTypeForCategory.WtfcWorkTypeId), _dbContext.WorkTypes.ToList().OrderBy(wt=>wt.WorkTypeName),
                    "WorkTypeName", wtfc.WtfcWorkTypeId, "WorkTypeId");
                wtfc.IsMandatory = false;
            }
            else
            {
                AddTextBlockControl("Категория объекта", _dbContext.ObjectCategories.FirstOrDefault(oc => oc.ObjectCategoryId == wtfc.SpecificCategoryId)?.ObjCategoryName);
                AddTextBlockControl("Тип работ", _dbContext.WorkTypes.FirstOrDefault(wt => wt.WorkTypeId == wtfc.WtfcWorkTypeId)?.WorkTypeName);
            }
            AddCheckBoxControl("Является обязательным", nameof(WorkTypeForCategory.IsMandatory), wtfc.IsMandatory ?? false);
        }

        private void CreateWorkTypeControls()
        {
            if (OriginalEntity is not WorkType) return;

            AddTextBoxControl("Название работ", nameof(WorkType.WorkTypeName), 100);
            AddTextBoxControl("Описание", nameof(WorkType.WorkTypeDescription));
        }

        private void CreateWorkScheduleControl()
        {
            if (OriginalEntity is not WorkSchedule ws || _dbContext == null) return;

            var selectedObject = _dbContext.Objects.FirstOrDefault(o => o.ObjectId == ws.ScheduleObjectId);

            var workTypesForCategories = selectedObject != null
                ? _dbContext.WorkTypeForCategories
                    .Where(wtfc => wtfc.SpecificCategoryId == selectedObject.CategoryId)
                    .Select(wtfc => wtfc.WtfcWorkType)
                    .OrderBy(wt => wt.WorkTypeName)
                    .ToList()
                : [.. _dbContext.WorkTypes.OrderBy(wt => wt.WorkTypeName)];

            AddComboBoxControl("Объект", nameof(WorkSchedule.ScheduleObjectId), _dbContext.Objects.ToList().OrderBy(o => o.ObjectName),
                "ObjectName", ws.ScheduleObjectId, "ObjectId");
            AddComboBoxControl("Бригада", nameof(WorkSchedule.ScheduleBrigadeId), _dbContext.Brigades.ToList().Where(b => b.BrigadeStatus != "terminated").OrderBy(b => b.BrigadeName),
                "BrigadeName", ws.ScheduleBrigadeId, "BrigadeId");
            AddComboBoxControl("Тип работы", nameof(WorkSchedule.ScheduleWorkTypeId), workTypesForCategories, "WorkTypeName", ws.ScheduleWorkTypeId, "WorkTypeId");
            AddDatePickerControl("Плановая дата начала", nameof(WorkSchedule.PlannedStartDate));
            AddDatePickerControl("Плановая дата конца", nameof(WorkSchedule.PlannedEndDate));
            AddDatePickerControl("Фактическая дата начала", nameof(WorkSchedule.ActualStartDate));
            AddDatePickerControl("Фактическая дата конца", nameof(WorkSchedule.ActualEndDate));
        }

        private void CreateSpecificObjectCharacteristicControls()
        {
            if (OriginalEntity is not SpecificObjectCharacteristic soc || _dbContext == null) return;

            if (DataManipulationMode == ManipulationMode.Add)
            {
                AddComboBoxControl("Объект", nameof(SpecificObjectCharacteristic.SpecificObjectId), _dbContext.Objects.ToList().OrderBy(o=>o.ObjectName),
                    "ObjectName", soc.SpecificObjectId, "ObjectId");
                AddComboBoxControl("Характеристика", nameof(SpecificObjectCharacteristic.CharacteristicId), _dbContext.ObjectCharacteristics.ToList().OrderBy(oc=>oc.ObjCharacteristicName),
                    "ObjCharacteristicName", soc.CharacteristicId, "ObjectCharacteristicId");
            }
            else
            { 
                AddTextBlockControl("Объект", _dbContext.Objects.FirstOrDefault(o => o.ObjectId == soc.SpecificObjectId)?.ObjectName);
                AddTextBlockControl("Характеристика", _dbContext.ObjectCharacteristics.FirstOrDefault(oc => oc.ObjectCharacteristicId == soc.CharacteristicId)?.ObjCharacteristicName);
            }
            
            AddTextBoxControl("Значение", nameof(SpecificObjectCharacteristic.CharacteristicValue), 255);
        }

        private void CreateSpecificEmployeeAttributeControls()
        {
            if (OriginalEntity is not SpecificEmployeeAttribute sea || _dbContext == null) return;

            if (DataManipulationMode == ManipulationMode.Add)
            {
                var employeeList = _dbContext.Employees.Select(e => new
                                                      {
                                                          e.EmployeeId,
                                                          FullName = $"{e.Surname} {e.Name} {e.Patronymic ?? string.Empty}".Trim()
                                                      }).ToList().OrderBy(e => e.FullName);
                AddComboBoxControl("Сотрудник", nameof(SpecificEmployeeAttribute.SpecificEmployeeId),
                    employeeList, "FullName", sea.SpecificEmployeeId, "EmployeeId");
                AddComboBoxControl("Название атрибута", nameof(SpecificEmployeeAttribute.AttributeId), _dbContext.EmployeeAttributes.ToList().OrderBy(ea=>ea.AttributeName),
                    "AttributeName", sea.AttributeId, "EmployeeAttributeId");
            }
            else
            {
                var employee = _dbContext.Employees.FirstOrDefault(e => e.EmployeeId == sea.SpecificEmployeeId);
                AddTextBlockControl("Сотрудник", DataManipulationViewModel.GetFullName(employee));
                AddTextBlockControl("Название атрибута", _dbContext.EmployeeAttributes.FirstOrDefault(ea => ea.EmployeeAttributeId == sea.AttributeId)?.AttributeName);
            }
            AddTextBoxControl("Значение", nameof(SpecificEmployeeAttribute.AttributeValue), 255);
        }

        private void CreateSiteControls()
        {
            if (OriginalEntity is not Site site || _dbContext == null) return;

            AddTextBoxControl("Название участка", nameof(Site.SiteName), 100);
            var siteSupervisors = _dbContext.Employees.Include(e => e.EmplCategory)
                                                .Where(e => e.EmplCategory.CategoryType == "Инженерно-технический персонал" && e.FireDate == null)
                                                .Select(e => new
                                                {
                                                    e.EmployeeId,
                                                    FullName = $"{e.Surname} {e.Name} {e.Patronymic ?? string.Empty}".Trim()
                                                }).ToList().OrderBy(e => e.FullName);
            AddComboBoxControl("Начальник участка", nameof(Site.SiteSupervisorId),
                siteSupervisors, "FullName", site.SiteSupervisorId!, "EmployeeId");
            AddTextBoxControl("Адрес", nameof(Site.SiteLocation), 150);
            AddComboBoxControl("Принадлежит управлению", nameof(Site.SiteDepartmentId), _dbContext.ConstructionDepartments.ToList().OrderBy(cd => cd.DepartmentName),
                "DepartmentName", site.SiteDepartmentId, "ConstructionDepartmentId");
        }

        private void CreateOrganizationEquipmentControls()
        {
            if (OriginalEntity is not OrganizationEquipment oe) return;

            AddTextBoxControl("Название техники", nameof(OrganizationEquipment.EquipmentName), 100);
            AddNumericUpDownControl("Количество техники", nameof(OrganizationEquipment.OrgEquipmentQuantity), oe.OrgEquipmentQuantity, 1);
        }

        private void CreateObjectCharacteristicControls()
        {
            if (OriginalEntity is not ObjectCharacteristic) return;

            AddTextBoxControl("Название характеристики", nameof(ObjectCharacteristic.ObjCharacteristicName), 50);
        }

        private void CreateObjectCategoryControls()
        {
            if (OriginalEntity is not ObjectCategory oc) return;

            AddTextBoxControl("Название категории", nameof(ObjectCategory.ObjCategoryName), 50);

            if (DataManipulationMode == ManipulationMode.Edit)
            { 
                AddWorkTypeForCategoryInObjectCategoryControls(oc); 
            }
        }

        private void CreateEstimateControls()
        {
            if (OriginalEntity is not Estimate estimate || _dbContext == null) return;

            if (DataManipulationMode == ManipulationMode.Add)
            {
                AddComboBoxControl("Материал", nameof(Estimate.MaterialId), _dbContext.BuildingMaterials.ToList().OrderBy(bm => bm.MaterialName),
                    "MaterialName", estimate.MaterialId, "BuildingMaterialId");
                AddComboBoxControl("Объект", nameof(Estimate.EstimateObjectId), _dbContext.Objects.ToList().OrderBy(o => o.ObjectName),
                    "ObjectName", estimate.EstimateObjectId, "ObjectId");
            }
            else
            {
                AddTextBlockControl("Материал", _dbContext.BuildingMaterials.FirstOrDefault(bm => bm.BuildingMaterialId == estimate.MaterialId)?.MaterialName);
                AddTextBlockControl("Объект", _dbContext.Objects.FirstOrDefault(o => o.ObjectId == estimate.EstimateObjectId)?.ObjectName);
            }
            AddNumericUpDownControl("Плановое количество", nameof(Estimate.PlannedMaterialQuantity), estimate.PlannedMaterialQuantity, 100, "F2");
            AddNumericUpDownControl("Цена за единицу", nameof(Estimate.UnitPrice), estimate.UnitPrice, 1, "F2");
            AddNumericUpDownControl("Фактическое количество", nameof(Estimate.ActualMaterialQuantity), estimate.ActualMaterialQuantity ?? 0, 100, "F2");
        }

        private void CreateEmployeeCategoryControls()
        {
            if (OriginalEntity is not EmployeeCategory ec) return;

            AddTextBoxControl("Название должности", nameof(EmployeeCategory.EmplCategoryName), 50);
            AddComboBoxControl("Тип категории", nameof(EmployeeCategory.CategoryType), ["Рабочие", "Инженерно-технический персонал"],
                "", ec.CategoryType);
        }

        private void CreateEmployeeAttributeControls()
        {
            if (OriginalEntity is not EmployeeAttribute) return;

            AddTextBoxControl("Название атрибута", nameof(EmployeeAttribute.AttributeName), 100);
        }

        private void CreateDepartmentEquipmentControls()
        {
            if (OriginalEntity is not DepartmentEquipment de || _dbContext == null) return;

            AddComboBoxControl("Название техники", nameof(DepartmentEquipment.OrgEquipmentId),
                _dbContext.OrganizationEquipments.ToList().OrderBy(oe => oe.EquipmentName), "EquipmentName", de.OrgEquipmentId, "OrganizationEquipmentId");
            AddComboBoxControl("Строительное управление", nameof(DepartmentEquipment.DepartmentId),
                _dbContext.ConstructionDepartments.ToList().OrderBy(cd => cd.DepartmentName), "DepartmentName", de.DepartmentId, "ConstructionDepartmentId");
            AddNumericUpDownControl("Количество техники", nameof(DepartmentEquipment.DepartEquipmentQuantity), de.DepartEquipmentQuantity, 1);
        }

        private void CreateCustomerControls()
        {
            if (OriginalEntity is not Customer) return;

            AddTextBoxControl("Клиент", nameof(Customer.CustomerName), 100);
        }

        private void CreateContractControls()
        {
            if (OriginalEntity is not Contract contract || _dbContext == null) return;

            AddTextBoxControl("Название контракта", nameof(Contract.ContractName));
            AddComboBoxControl("Клиент", nameof(Contract.ContractCustomerId),
                _dbContext.Customers.ToList().OrderBy(c => c.CustomerName), "CustomerName", contract.ContractCustomerId, "CustomerId");
            AddNumericUpDownControl("Стоимость", nameof(Contract.TotalAmount), contract.TotalAmount, 1000, "F2");
            AddComboBoxControl("Статус контракта", nameof(Contract.ContractStatus), ["active", "completed", "terminated"], "", contract.ContractStatus);
        }

        private void CreateConstructionDepartmentControls()
        {
            if (OriginalEntity is not ConstructionDepartment cd || _dbContext == null) return;

            AddTextBoxControl("Название управления", nameof(ConstructionDepartment.DepartmentName), 100);
            AddTextBoxControl("Адрес управления", nameof(ConstructionDepartment.DepartmentLocation), 150);

            var departSupervisors = _dbContext.Employees.Include(e => e.EmplCategory)
                                                .Where(e => e.EmplCategory.CategoryType == "Инженерно-технический персонал" && e.FireDate == null)
                                                .Select(e => new
                                                {
                                                    e.EmployeeId,
                                                    FullName = $"{e.Surname} {e.Name} {e.Patronymic ?? string.Empty}".Trim()
                                                }).ToList().OrderBy(e => e.FullName);
            AddComboBoxControl("Начальник управления", nameof(ConstructionDepartment.DepartmentSupervisorId),
                departSupervisors, "FullName", cd.DepartmentSupervisorId!, "EmployeeId");


            if (DataManipulationMode == ManipulationMode.Edit)
            {
                AddDepartmentEquipmentInDepartmentControls(cd);
            }
        }

        private void CreateBuildingMaterialControls()
        {
            if (OriginalEntity is not BuildingMaterial) return;

            AddTextBoxControl("Название материала", nameof(BuildingMaterial.MaterialName), 100);
            AddTextBoxControl("Единица измерения", nameof(BuildingMaterial.UnitOfMeasure), 20);
        }

        private void CreateEmployeeControls()
        {
            if (OriginalEntity is not Employee employee || _dbContext == null) return;

            AddTextBoxControl("Имя", nameof(Employee.Name), 50);
            AddTextBoxControl("Фамилия", nameof(Employee.Surname), 50);
            AddTextBoxControl("Отчество", nameof(Employee.Patronymic), 50);

            AddComboBoxControl(label: "Должность",
                               propertyName: nameof(Employee.EmplCategoryId),
                               items: _dbContext.EmployeeCategories.ToList().OrderBy(ec => ec.EmplCategoryName),
                               displayMember: "EmplCategoryName",
                               selectedValue: employee.EmplCategoryId,
                               valueMember: "EmployeeCategoryId");
            AddComboBoxControl("Образование", nameof(Employee.Education),
                ["Высшее", "Среднее специальное"], "", employee.Education);

            AddTextBoxControl("Телефон", nameof(Employee.ContactNumber), 20);

            AddDatePickerControl("Дата приема", nameof(Employee.HireDate));
            AddDatePickerControl("Дата увольнения", nameof(Employee.FireDate));

            if (DataManipulationMode == ManipulationMode.Edit)
            { 
                AddSpecificEmployeeAttributeInEmployeeControls(employee); 
            }
        }

        private void CreateBrigadeControls()
        {
            if (OriginalEntity is not Brigade brigade || _dbContext == null) return;

            AddTextBoxControl("Название бригады", nameof(Brigade.BrigadeName), 100);

            var brigadiers = _dbContext.Employees.Include(e => e.EmplCategory)
                                                .Where(e => e.EmplCategory.CategoryType == "Рабочие" && e.FireDate == null)
                                                .Select(e => new
                                                {
                                                    e.EmployeeId,
                                                    FullName = $"{e.Surname} {e.Name} {e.Patronymic ?? string.Empty}".Trim()
                                                }).ToList().OrderBy(e => e.FullName);
            AddComboBoxControl("Бригадир", nameof(Brigade.BrigadierId), brigadiers, "FullName", brigade.BrigadierId!, "EmployeeId");

            AddComboBoxControl("Статус бригады", nameof(Brigade.BrigadeStatus),
                ["active", "terminated"], "", brigade.BrigadeStatus);

            if (DataManipulationMode == ManipulationMode.Edit)
            {
                AddBrigadeCompositionControl(brigade); 
            }
        }
        private void AddSpecificEmployeeAttributeInEmployeeControls(Employee employee)
        {
            if (_dbContext == null) return;
            var allPossibleAttributes = _dbContext.EmployeeAttributes.ToList();

            var currentAttributes = _dbContext.SpecificEmployeeAttributes
                .Where(sea => sea.SpecificEmployeeId == employee.EmployeeId)
                .Include(sea => sea.Attribute)
                .ToList();

            var columns = new Dictionary<string, string>
            {
                { "Атрибут", nameof(SpecificItemCharacteristicValueDTO.CharacteristicName) },
                { "Значение", nameof(SpecificItemCharacteristicValueDTO.CharacteristicValue) }
            };

            var columnControls = new Dictionary<string, Func<SpecificItemCharacteristicValueDTO, Control>>
            {
                { nameof(SpecificItemCharacteristicValueDTO.CharacteristicName), vm => new TextBlock { Text = vm.CharacteristicName } },
                { nameof(SpecificItemCharacteristicValueDTO.CharacteristicValue), vm => new TextBox
                    {
                        MaxLength = 255,
                        [!TextBox.TextProperty] = new Binding(nameof(SpecificItemCharacteristicValueDTO.CharacteristicValue), BindingMode.TwoWay)
                    }
                }
            };

            SetupCollectionEditor<SpecificEmployeeAttribute, SpecificItemCharacteristicValueDTO>(
                entityCollection: new ObservableCollection<SpecificEmployeeAttribute>(currentAttributes),
                allPossibleItems: allPossibleAttributes
                    .Where(a => !currentAttributes.Any(ca => ca.AttributeId == a.EmployeeAttributeId))
                    .Select(a => new SpecificEmployeeAttribute
                    {
                        AttributeId = a.EmployeeAttributeId,
                        SpecificEmployeeId = employee.EmployeeId,
                        Attribute = a,
                        AttributeValue = string.Empty
                    }),
                entityToViewModel: sea => new SpecificItemCharacteristicValueDTO
                {
                    CharacteristicId = sea.AttributeId,
                    SpecificItemId = sea.SpecificEmployeeId,
                    CharacteristicValue = sea.AttributeValue,
                    CharacteristicName = sea.Attribute.AttributeName
                },
                viewModelToEntity: vm =>
                {
                    var existingEntity = _dbContext.SpecificEmployeeAttributes.Local.FirstOrDefault(sea =>
                                     sea.AttributeId == vm.CharacteristicId && sea.SpecificEmployeeId == vm.SpecificItemId);

                    if (existingEntity != null)
                    {
                        existingEntity.AttributeValue = vm.CharacteristicValue;
                        return existingEntity;
                    }
                    else
                    {
                        return new SpecificEmployeeAttribute
                        {
                            AttributeId = vm.CharacteristicId,
                            SpecificEmployeeId = vm.SpecificItemId,
                            AttributeValue = vm.CharacteristicValue,
                            Attribute = allPossibleAttributes.First(a => a.EmployeeAttributeId == vm.CharacteristicId)
                        };
                    }
                },
                columns: columns,
                columnControls: columnControls,
                displayMember: nameof(SpecificItemCharacteristicValueDTO.CharacteristicName),
                sortSelector: vm => vm.CharacteristicName,
                label: "Изменение атрибутов сотрудника:",
                addButtonText: "Добавить атрибут",
                placeholderText: "Выберите атрибут",
                onAdd: entity => _dbContext.SpecificEmployeeAttributes.Add(entity),
                onRemove: entity => _dbContext.SpecificEmployeeAttributes.Remove(entity)
            );
        }
        private void AddMasterOnObjectControl(Models.Object obj)
        {
            if (_dbContext == null) return;
            var allMasters = _dbContext.Employees
                .Include(e => e.EmplCategory)
                .Where(e => e.EmplCategory.CategoryType == "Инженерно-технический персонал" && e.FireDate == null)
                .ToList();

            var columns = new Dictionary<string, string>
            {
                { "Имя мастера", nameof(ItemIdAndValueDTO.ItemId) }
            };

            var columnControls = new Dictionary<string, Func<ItemIdAndValueDTO, Control>>
            {
                { nameof(ItemIdAndValueDTO.ItemId), emv => new TextBlock { Text = emv.ValueString ?? string.Empty, VerticalAlignment = VerticalAlignment.Center } }
            };

            SetupCollectionEditor<Employee, ItemIdAndValueDTO>(
                entityCollection: obj.MasterEmployees,
                allPossibleItems: allMasters,
                entityToViewModel: e => new ItemIdAndValueDTO
                {
                    ItemId = e.EmployeeId,
                    ValueString = $"{e.Surname} {e.Name} {e.Patronymic ?? string.Empty}".Trim()
                },
                viewModelToEntity: vm => _dbContext.Employees.First(e => e.EmployeeId == vm.ItemId),
                columns: columns,
                columnControls: columnControls,
                displayMember: nameof(ItemIdAndValueDTO.ValueString),
                sortSelector: vm => vm.ValueString,
                label: "Добавление мастеров на объект:",
                addButtonText: "Добавить мастера",
                placeholderText: "Выберите сотрудника");
        }
        private void AddBrigadeCompositionControl(Brigade brigade)
        {
            if (_dbContext == null) return;
            var availableWorkers = _dbContext.Employees
                    .Include(e => e.EmplCategory)
                    .Where(e => e.EmplCategory.CategoryType == "Рабочие" && e.FireDate == null &&
                               !_dbContext.Brigades.Any(b => b.BrigadeStatus == "active" && (b.Workers.Contains(e) || b.BrigadierId == e.EmployeeId)))
                    .ToList();
            var columns = new Dictionary<string, string>
            {
                { "Имя рабочего", nameof(ItemIdAndValueDTO.ItemId) }
            };
            var columnControls = new Dictionary<string, Func<ItemIdAndValueDTO, Control>>
            {
                { nameof(ItemIdAndValueDTO.ItemId), emv => new TextBlock { Text = emv.ValueString ?? string.Empty, VerticalAlignment = VerticalAlignment.Center } }
            };
            SetupCollectionEditor<Employee, ItemIdAndValueDTO>(
                entityCollection: brigade.Workers,
                allPossibleItems: availableWorkers,
                entityToViewModel: e => new ItemIdAndValueDTO
                {
                    ItemId = e.EmployeeId,
                    ValueString = $"{e.Surname} {e.Name} {e.Patronymic ?? string.Empty}".Trim()
                },
                viewModelToEntity: vm => _dbContext.Employees.First(e => e.EmployeeId == vm.ItemId),
                columns: columns,
                columnControls: columnControls,
                displayMember: nameof(ItemIdAndValueDTO.ValueString),
                sortSelector: vm => vm.ValueString,
                label: "Изменение состава бригады:",
                addButtonText: "Добавить рабочего",
                placeholderText: "Выберите рабочего");
        }
        private void AddSpecificObjectCharacteristicInObjectControls(Models.Object obj)
        {
            if (_dbContext == null) return;
            var allPossibleCharacteristic = _dbContext.ObjectCharacteristics.ToList();

            var currentCharacteristic = _dbContext.SpecificObjectCharacteristics
                .Where(soc => soc.SpecificObjectId == obj.ObjectId)
                .Include(soc => soc.Characteristic)
                .ToList();

            var columns = new Dictionary<string, string>
            {
                { "Характеристика", nameof(SpecificItemCharacteristicValueDTO.CharacteristicName) },
                { "Значение", nameof(SpecificItemCharacteristicValueDTO.CharacteristicValue) }
            };

            var columnControls = new Dictionary<string, Func<SpecificItemCharacteristicValueDTO, Control>>
            {
                { nameof(SpecificItemCharacteristicValueDTO.CharacteristicName), dto => new TextBlock { Text = dto.CharacteristicName, VerticalAlignment = VerticalAlignment.Center } },
                { nameof(SpecificItemCharacteristicValueDTO.CharacteristicValue), dto => new TextBox{MaxLength = 255}
                }
            };

            SetupCollectionEditor<SpecificObjectCharacteristic, SpecificItemCharacteristicValueDTO>(
                entityCollection: new ObservableCollection<SpecificObjectCharacteristic>(currentCharacteristic),
                allPossibleItems: allPossibleCharacteristic
                    .Where(oc => !currentCharacteristic.Any(cc => cc.CharacteristicId == oc.ObjectCharacteristicId))
                    .Select(oc => new SpecificObjectCharacteristic
                    {
                        CharacteristicId = oc.ObjectCharacteristicId,
                        SpecificObjectId = obj.ObjectId,
                        Characteristic = oc,
                        CharacteristicValue = string.Empty
                    }),
                entityToViewModel: soc => new SpecificItemCharacteristicValueDTO
                {
                    CharacteristicId = soc.CharacteristicId,
                    SpecificItemId = soc.SpecificObjectId,
                    CharacteristicValue = soc.CharacteristicValue,
                    CharacteristicName = soc.Characteristic.ObjCharacteristicName
                },
                viewModelToEntity: vm =>
                {
                    var existingEntity = _dbContext.SpecificObjectCharacteristics.Local.FirstOrDefault(soc =>
                        soc.CharacteristicId == vm.CharacteristicId && soc.SpecificObjectId == vm.SpecificItemId);

                    if (existingEntity != null)
                    {
                        existingEntity.CharacteristicValue = vm.CharacteristicValue;
                        return existingEntity;
                    }
                    else
                    {
                        return new SpecificObjectCharacteristic
                        {
                            CharacteristicId = vm.CharacteristicId,
                            SpecificObjectId = vm.SpecificItemId,
                            CharacteristicValue = vm.CharacteristicValue,
                            Characteristic = allPossibleCharacteristic.First(oc => oc.ObjectCharacteristicId == vm.CharacteristicId)
                        };
                    }
                },
                columns: columns,
                columnControls: columnControls,
                displayMember: nameof(SpecificItemCharacteristicValueDTO.CharacteristicName),
                sortSelector: vm => vm.CharacteristicName,
                label: "Изменение характеристик объекта:",
                addButtonText: "Добавить характеристику",
                placeholderText: "Выберите характеристику",
                onAdd: entity => _dbContext.SpecificObjectCharacteristics.Add(entity),
                onRemove: entity => _dbContext.SpecificObjectCharacteristics.Remove(entity)
            );
        }
        private void AddEstimateInObjectControls(Models.Object obj)
        {
            if (_dbContext == null) return;
            var allPossibleMaterial = _dbContext.BuildingMaterials.ToList();

            var currentEstimate = _dbContext.Estimates
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
                    var existingEntity = _dbContext.Estimates.Local.FirstOrDefault(e =>
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
                onAdd: entity => _dbContext.Estimates.Add(entity),
                onRemove: entity => _dbContext.Estimates.Remove(entity)
            );
        }
        private void AddWorkTypeForCategoryInObjectCategoryControls(ObjectCategory oc)
        {
            if (_dbContext == null) return;
            var allPossibleWorkType = _dbContext.WorkTypes.ToList();

            var currentWorkType = _dbContext.WorkTypeForCategories
                .Where(wtfc => wtfc.SpecificCategoryId == oc.ObjectCategoryId)
                .Include(wtfc => wtfc.WtfcWorkType).ToList();

            var columns = new Dictionary<string, string>
            {
                { "Вид работ", nameof(CategoryWorkTypeDTO.WorkTypeName) },
                { "Выполняется обязательно", nameof(CategoryWorkTypeDTO.IsWorkTypeMandatory) }
            };

            var columnControls = new Dictionary<string, Func<CategoryWorkTypeDTO, Control>>
            {
                { nameof(CategoryWorkTypeDTO.WorkTypeName), dto => new TextBlock { Text = dto.WorkTypeName, VerticalAlignment = VerticalAlignment.Center } },
                { nameof(CategoryWorkTypeDTO.IsWorkTypeMandatory), dto => new CheckBox { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center} },
            };

            SetupCollectionEditor<WorkTypeForCategory, CategoryWorkTypeDTO>(
                entityCollection: new ObservableCollection<WorkTypeForCategory>(currentWorkType),
                allPossibleItems: allPossibleWorkType
                    .Where(wt => !currentWorkType.Any(wtfc => wtfc.WtfcWorkTypeId == wt.WorkTypeId))
                    .Select(wt => new WorkTypeForCategory
                    {
                        SpecificCategoryId = oc.ObjectCategoryId,
                        WtfcWorkTypeId = wt.WorkTypeId,
                        IsMandatory = false,
                        WtfcWorkType = wt
                    }),
                entityToViewModel: wtfc => new CategoryWorkTypeDTO
                {
                    SpecificCategoryId = wtfc.SpecificCategoryId,
                    WorkTypeID = wtfc.WtfcWorkTypeId,
                    IsWorkTypeMandatory = wtfc.IsMandatory ?? false,
                    WorkTypeName = wtfc.WtfcWorkType.WorkTypeName
                },
                viewModelToEntity: dto =>
                {
                    var existingEntity = _dbContext.WorkTypeForCategories.Local.FirstOrDefault(wtfc =>
                        wtfc.WtfcWorkTypeId == dto.WorkTypeID && wtfc.SpecificCategoryId == dto.SpecificCategoryId);

                    if (existingEntity != null)
                    {
                        existingEntity.IsMandatory = dto.IsWorkTypeMandatory;
                        return existingEntity;
                    }
                    else
                    {
                        return new WorkTypeForCategory
                        {
                            SpecificCategoryId = dto.SpecificCategoryId,
                            WtfcWorkTypeId = dto.WorkTypeID,
                            IsMandatory = dto.IsWorkTypeMandatory,
                            WtfcWorkType = allPossibleWorkType.First(wt => wt.WorkTypeId == dto.WorkTypeID)
                        };
                    }
                },
                columns: columns,
                columnControls: columnControls,
                displayMember: nameof(CategoryWorkTypeDTO.WorkTypeName),
                sortSelector: dto => dto.WorkTypeName,
                label: "Изменение видов работ для категории:",
                addButtonText: "Добавить вид работ",
                placeholderText: "Выберите вид работ",
                onAdd: entity => _dbContext.WorkTypeForCategories.Add(entity),
                onRemove: entity => _dbContext.WorkTypeForCategories.Remove(entity)
            );
        }
        private void AddDepartmentEquipmentInDepartmentControls(ConstructionDepartment department)
        {
            if (_dbContext == null) return;
            var allPossibleEquipment = _dbContext.OrganizationEquipments.ToList();

            var currentEquipment = _dbContext.DepartmentEquipments
                .Where(de => de.DepartmentId == department.ConstructionDepartmentId)
                .Include(de => de.OrgEquipment).ToList();

            var columns = new Dictionary<string, string>
            {
                { "Оборудование", nameof(DepartmentEquipmentDTO.EquipmentName) },
                { "Количество", nameof(DepartmentEquipmentDTO.DepartEquipmentQuantity) }
            };

            var columnControls = new Dictionary<string, Func<DepartmentEquipmentDTO, Control>>
            {
                { nameof(DepartmentEquipmentDTO.EquipmentName), dto => new TextBlock { Text = dto.EquipmentName, VerticalAlignment = VerticalAlignment.Center } },
                { nameof(DepartmentEquipmentDTO.DepartEquipmentQuantity), dto => new NumericUpDown
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Minimum = 0,
                    Maximum = int.MaxValue,
                    FormatString = "F0"
                } },
            };

            SetupCollectionEditor<DepartmentEquipment, DepartmentEquipmentDTO>(
                entityCollection: new ObservableCollection<DepartmentEquipment>(currentEquipment),
                allPossibleItems: allPossibleEquipment
                    .Where(oe => !currentEquipment.Any(de => de.OrgEquipmentId == oe.OrganizationEquipmentId))
                    .Select(oe => new DepartmentEquipment
                    {
                        OrgEquipmentId = oe.OrganizationEquipmentId,
                        DepartmentId = department.ConstructionDepartmentId,
                        DepartEquipmentQuantity = 0,
                        OrgEquipment = oe
                    }),
                entityToViewModel: de => new DepartmentEquipmentDTO
                {
                    DepartmentEquipmentId = de.DepartmentEquipmentId,
                    OrgEquipmentId = de.OrgEquipmentId,
                    DepartmentId = de.DepartmentId,
                    DepartEquipmentQuantity = de.DepartEquipmentQuantity,
                    EquipmentName = de.OrgEquipment.EquipmentName
                },
                viewModelToEntity: dto =>
                {
                    var existingEntity = _dbContext.DepartmentEquipments.Local.FirstOrDefault(de =>
                        de.DepartmentEquipmentId == dto.DepartmentEquipmentId);

                    if (existingEntity != null)
                    {
                        existingEntity.DepartEquipmentQuantity = dto.DepartEquipmentQuantity;
                        existingEntity.OrgEquipmentId = dto.OrgEquipmentId;
                        existingEntity.DepartmentId = dto.DepartmentId;
                        return existingEntity;
                    }
                    else
                    {
                        return new DepartmentEquipment
                        {
                            DepartmentEquipmentId = dto.DepartmentEquipmentId,
                            OrgEquipmentId = dto.OrgEquipmentId,
                            DepartmentId = dto.DepartmentId,
                            DepartEquipmentQuantity = dto.DepartEquipmentQuantity,
                            OrgEquipment = allPossibleEquipment.First(oe => oe.OrganizationEquipmentId == dto.OrgEquipmentId)
                        };
                    }
                },
                columns: columns,
                columnControls: columnControls,
                displayMember: nameof(DepartmentEquipmentDTO.EquipmentName),
                sortSelector: dto => dto.EquipmentName,
                label: "Изменение оборудования для строительного управления:",
                addButtonText: "Добавить оборудование",
                placeholderText: "Выберите оборудование",
                onAdd: entity => _dbContext.DepartmentEquipments.Add(entity),
                onRemove: entity => _dbContext.DepartmentEquipments.Remove(entity)
            );
        }
        private void AddObjectEquipmentInObjectControls(Models.Object obj)
        {
            if (_dbContext == null) return;
            var departmentId = _dbContext.Sites
                .Where(s => s.SiteId == obj.ObjectSiteId)
                .Select(s => s.SiteDepartmentId).FirstOrDefault();
            var allPossibleEquipment = _dbContext.OrganizationEquipments
                                       .Where(oe => oe.DepartmentEquipments.Any(de => de.DepartmentId == departmentId))
                                       .Include(oe => oe.DepartmentEquipments.Where(de => de.DepartmentId == departmentId))
                                       .ToList();

            var currentEquipment = _dbContext.ObjectEquipments
                .Where(oe => oe.EquipmentForObjectId == obj.ObjectId)
                .Include(oe => oe.Equipment).ThenInclude(de => de.OrgEquipment).ToList();

            var columns = new Dictionary<string, string>
            {
                { "Оборудование", nameof(ObjectEquipmentDTO.EquipmentName) },
                {"Дата выдачи", nameof(ObjectEquipmentDTO.AssignmentDate) },
                { "Количество", nameof(ObjectEquipmentDTO.EquipmentQuantity) },
                {"Дата возврата", nameof(ObjectEquipmentDTO.ReturnDate) }
            };

            var columnControls = new Dictionary<string, Func<ObjectEquipmentDTO, Control>>
            {
                { nameof(ObjectEquipmentDTO.EquipmentName), dto => new TextBlock { Text = dto.EquipmentName, VerticalAlignment = VerticalAlignment.Center } },
                { nameof(ObjectEquipmentDTO.EquipmentQuantity), dto => new NumericUpDown
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Minimum = 0,
                    Maximum = int.MaxValue,
                    FormatString = "F0"
                } },
                { nameof(ObjectEquipmentDTO.AssignmentDate), dto =>
                {
                    var datePicker = new DatePicker();
                    var clearButton = new Button
                    {
                        Content = "×",
                        Classes = { "ClearButton" }
                    };
                    clearButton.Click += (s, e) => datePicker.SelectedDate = null;

                    var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
                    stackPanel.Children.AddRange([datePicker, clearButton]);
                    return stackPanel;
                } },
                { nameof(ObjectEquipmentDTO.ReturnDate), dto =>
                {
                    var datePicker = new DatePicker();
                    var clearButton = new Button
                    {
                        Content = "×",
                        Classes = { "ClearButton" }
                    };
                    clearButton.Click += (s, e) => datePicker.SelectedDate = null;

                    var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
                    stackPanel.Children.AddRange([datePicker, clearButton]);
                    return stackPanel;
                } }
            };

            SetupCollectionEditor<ObjectEquipment, ObjectEquipmentDTO>(
                entityCollection: new ObservableCollection<ObjectEquipment>(currentEquipment),
                allPossibleItems: allPossibleEquipment
                    .SelectMany(orgEq => orgEq.DepartmentEquipments)
                    .Where(de => !currentEquipment.Any(oe => oe.EquipmentId == de.DepartmentEquipmentId))
                    .Select(de => new ObjectEquipment
                    {
                        EquipmentId = de.DepartmentEquipmentId,
                        EquipmentForObjectId = obj.ObjectId,
                        Equipment = de,
                        AssignmentDate = DateOnly.FromDateTime(DateTime.Now),
                        ReturnDate = null,
                        EquipObjectQuantity = 0
                    }),
                entityToViewModel: oe => new ObjectEquipmentDTO
                {
                    EquipmentId = oe.EquipmentId,
                    ObjectId = oe.EquipmentForObjectId,
                    EquipmentQuantity = oe.EquipObjectQuantity,
                    AssignmentDate = oe.AssignmentDate ?? DateOnly.FromDateTime(DateTime.Now),
                    ReturnDate = oe.ReturnDate,
                    EquipmentName = oe.Equipment.OrgEquipment.EquipmentName
                },
                viewModelToEntity: dto =>
                {
                    var existingEntity = _dbContext.ObjectEquipments.Local.FirstOrDefault(oe =>
                        oe.EquipmentId == dto.EquipmentId && oe.EquipmentForObjectId == dto.ObjectId);

                    if (existingEntity != null)
                    {
                        existingEntity.AssignmentDate = dto.AssignmentDate;
                        existingEntity.ReturnDate = dto.ReturnDate;
                        existingEntity.EquipObjectQuantity = dto.EquipmentQuantity;
                        return existingEntity;
                    }
                    else
                    {
                        return new ObjectEquipment
                        {
                            EquipmentId = dto.EquipmentId,
                            EquipmentForObjectId = dto.ObjectId,
                            EquipObjectQuantity = dto.EquipmentQuantity,
                            AssignmentDate = dto.AssignmentDate,
                            ReturnDate = dto.ReturnDate,
                            Equipment = allPossibleEquipment.FirstOrDefault(oe => oe.DepartmentEquipments
                                                            .Any(de => de.DepartmentEquipmentId == dto.EquipmentId))!.DepartmentEquipments
                                                            .First(de => de.DepartmentEquipmentId == dto.EquipmentId)
                        };
                    }
                },
                columns: columns,
                columnControls: columnControls,
                displayMember: nameof(ObjectEquipmentDTO.EquipmentName),
                sortSelector: dto => dto.EquipmentName,
                label: "Изменение оборудования для объекта:",
                addButtonText: "Добавить оборудование",
                placeholderText: "Выберите оборудование",
                onAdd: entity => _dbContext.ObjectEquipments.Add(entity),
                onRemove: entity => _dbContext.ObjectEquipments.Remove(entity)
            );
        }
    }
}
