using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.DependencyInjection;
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
            if (OriginalEntity is not Models.Object obj) return;

            var foremans = dbContext.Employees.Include(e => e.EmplCategory)
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
                dbContext.Sites.ToList().OrderBy(s => s.SiteName), "SiteName", obj.ObjectSiteId, "SiteId");

            AddComboBoxControl("По контракту", nameof(Models.Object.ObjectContractId),
                dbContext.Contracts.ToList().OrderBy(c => c.ContractName), "ContractName", obj.ObjectContractId, "ContractId");

            AddComboBoxControl("Категория объекта", nameof(Models.Object.CategoryId),
                dbContext.ObjectCategories.ToList().OrderBy(oc => oc.ObjCategoryName),
                "ObjCategoryName", obj.CategoryId, "ObjectCategoryId");


            AddComboBoxControl("Статус объекта", nameof(Models.Object.ObjectStatus),
                ["in_planning", "in_progress", "completed", "terminated"], "", obj.ObjectStatus);

            AddObjectEquipmentInObjectControls(obj);
            AddEstimateInObjectControls(obj);
            AddSpecificObjectCharacteristicInObjectControls(obj);
            AddMasterOnObjectControl(obj);
        }
        private void CreateObjectEquipmentControls()
        {
            if (OriginalEntity is not ObjectEquipment oe) return;

            if (DataManipulationMode == ManipulationMode.Add)
            {
                AddComboBoxControl("Объект", nameof(ObjectEquipment.EquipmentForObjectId), dbContext.Objects.ToList().OrderBy(o => o.ObjectName),
                    "ObjectName", oe.EquipmentForObjectId, "ObjectId");
                AddComboBoxControl("Название техники", nameof(ObjectEquipment.EquipmentId), dbContext.DepartmentEquipments.Include(de=>de.OrgEquipment).ToList().OrderBy(o=>o.OrgEquipment.EquipmentName),
                    "OrgEquipment.EquipmentName", oe.EquipmentId, "DepartmentEquipmentId");
                oe.AssignmentDate = DateOnly.FromDateTime(DateTime.Now);
            }
            else
            {
                AddTextBlockControl("Объект", dbContext.Objects.FirstOrDefault(o => o.ObjectId == oe.EquipmentForObjectId)?.ObjectName);
                AddTextBlockControl("Название техники", dbContext.DepartmentEquipments
                                                             .Include(de => de.OrgEquipment)
                                                             .FirstOrDefault(de => de.DepartmentEquipmentId == oe.EquipmentId)?.OrgEquipment?.EquipmentName);
            }
            AddNumericUpDownControl("Количество техники", nameof(ObjectEquipment.EquipObjectQuantity), oe.EquipObjectQuantity, 1);
            AddDatePickerControl("Дата выдачи", nameof(ObjectEquipment.AssignmentDate));
            AddDatePickerControl("Дата возврата", nameof(ObjectEquipment.ReturnDate));
        }

        private void CreateWorkTypeForCategoryControls()
        {
            if (OriginalEntity is not WorkTypeForCategory wtfc) return;

            if (DataManipulationMode == ManipulationMode.Add)
            {
                AddComboBoxControl("Категория объекта", nameof(WorkTypeForCategory.SpecificCategoryId), dbContext.ObjectCategories.ToList().OrderBy(oc=>oc.ObjCategoryName),
                    "ObjCategoryName", wtfc.SpecificCategoryId, "ObjectCategoryId");
                AddComboBoxControl("Тип работ", nameof(WorkTypeForCategory.WtfcWorkTypeId), dbContext.WorkTypes.ToList().OrderBy(wt=>wt.WorkTypeName),
                    "WorkTypeName", wtfc.WtfcWorkTypeId, "WorkTypeId");
                wtfc.IsMandatory = false;
            }
            else
            {
                AddTextBlockControl("Категория объекта", dbContext.ObjectCategories.FirstOrDefault(oc => oc.ObjectCategoryId == wtfc.SpecificCategoryId)?.ObjCategoryName);
                AddTextBlockControl("Тип работ", dbContext.WorkTypes.FirstOrDefault(wt => wt.WorkTypeId == wtfc.WtfcWorkTypeId)?.WorkTypeName);
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
            if (OriginalEntity is not WorkSchedule ws) return;

            var selectedObject = dbContext.Objects.FirstOrDefault(o => o.ObjectId == ws.ScheduleObjectId);

            var workTypesForCategories = selectedObject != null
                ? dbContext.WorkTypeForCategories
                    .Where(wtfc => wtfc.SpecificCategoryId == selectedObject.CategoryId)
                    .Select(wtfc => wtfc.WtfcWorkType)
                    .OrderBy(wt => wt.WorkTypeName)
                    .ToList()
                : [.. dbContext.WorkTypes.OrderBy(wt => wt.WorkTypeName)];

            AddComboBoxControl("Объект", nameof(WorkSchedule.ScheduleObjectId), dbContext.Objects.ToList().OrderBy(o => o.ObjectName),
                "ObjectName", ws.ScheduleObjectId, "ObjectId");
            AddComboBoxControl("Бригада", nameof(WorkSchedule.ScheduleBrigadeId), dbContext.Brigades.ToList().Where(b => b.BrigadeStatus != "terminated").OrderBy(b => b.BrigadeName),
                "BrigadeName", ws.ScheduleBrigadeId, "BrigadeId");
            AddComboBoxControl("Тип работы", nameof(WorkSchedule.ScheduleWorkTypeId), workTypesForCategories, "WorkTypeName", ws.ScheduleWorkTypeId, "WorkTypeId");
            AddDatePickerControl("Плановая дата начала", nameof(WorkSchedule.PlannedStartDate));
            AddDatePickerControl("Плановая дата конца", nameof(WorkSchedule.PlannedEndDate));
            AddDatePickerControl("Фактическая дата начала", nameof(WorkSchedule.ActualStartDate));
            AddDatePickerControl("Фактическая дата конца", nameof(WorkSchedule.ActualEndDate));
        }

        private void CreateSpecificObjectCharacteristicControls()
        {
            if (OriginalEntity is not SpecificObjectCharacteristic soc) return;

            if (DataManipulationMode == ManipulationMode.Add)
            {
                AddComboBoxControl("Объект", nameof(SpecificObjectCharacteristic.SpecificObjectId), dbContext.Objects.ToList().OrderBy(o=>o.ObjectName),
                    "ObjectName", soc.SpecificObjectId, "ObjectId");
                AddComboBoxControl("Характеристика", nameof(SpecificObjectCharacteristic.CharacteristicId), dbContext.ObjectCharacteristics.ToList().OrderBy(oc=>oc.ObjCharacteristicName),
                    "ObjCharacteristicName", soc.CharacteristicId, "ObjectCharacteristicId");
            }
            else
            { 
                AddTextBlockControl("Объект", dbContext.Objects.FirstOrDefault(o => o.ObjectId == soc.SpecificObjectId)?.ObjectName);
                AddTextBlockControl("Характеристика", dbContext.ObjectCharacteristics.FirstOrDefault(oc => oc.ObjectCharacteristicId == soc.CharacteristicId)?.ObjCharacteristicName);
            }
            
            AddTextBoxControl("Значение", nameof(SpecificObjectCharacteristic.CharacteristicValue), 255);
        }

        private void CreateSpecificEmployeeAttributeControls()
        {
            if (OriginalEntity is not SpecificEmployeeAttribute sea) return;

            if (DataManipulationMode == ManipulationMode.Add)
            {
                var employeeList = dbContext.Employees.Select(e => new
                                                      {
                                                          e.EmployeeId,
                                                          FullName = $"{e.Surname} {e.Name} {e.Patronymic ?? string.Empty}".Trim()
                                                      }).ToList().OrderBy(e => e.FullName);
                AddComboBoxControl("Сотрудник", nameof(SpecificEmployeeAttribute.SpecificEmployeeId),
                    employeeList, "FullName", sea.SpecificEmployeeId, "EmployeeId");
                AddComboBoxControl("Название атрибута", nameof(SpecificEmployeeAttribute.AttributeId), dbContext.EmployeeAttributes.ToList().OrderBy(ea=>ea.AttributeName),
                    "AttributeName", sea.AttributeId, "EmployeeAttributeId");
            }
            else
            {
                var employee = dbContext.Employees.FirstOrDefault(e => e.EmployeeId == sea.SpecificEmployeeId);
                AddTextBlockControl("Сотрудник", DataManipulationViewModel.GetFullName(employee));
                AddTextBlockControl("Название атрибута", dbContext.EmployeeAttributes.FirstOrDefault(ea => ea.EmployeeAttributeId == sea.AttributeId)?.AttributeName);
            }
            AddTextBoxControl("Значение", nameof(SpecificEmployeeAttribute.AttributeValue), 255);
        }

        private void CreateSiteControls()
        {
            if (OriginalEntity is not Site site) return;

            AddTextBoxControl("Название участка", nameof(Site.SiteName), 100);
            var siteSupervisors = dbContext.Employees.Include(e => e.EmplCategory)
                                                .Where(e => e.EmplCategory.CategoryType == "Инженерно-технический персонал" && e.FireDate == null)
                                                .Select(e => new
                                                {
                                                    e.EmployeeId,
                                                    FullName = $"{e.Surname} {e.Name} {e.Patronymic ?? string.Empty}".Trim()
                                                }).ToList().OrderBy(e => e.FullName);
            AddComboBoxControl("Начальник участка", nameof(Site.SiteSupervisorId),
                siteSupervisors, "FullName", site.SiteSupervisorId!, "EmployeeId");
            AddTextBoxControl("Адрес", nameof(Site.SiteLocation), 150);
            AddComboBoxControl("Принадлежит управлению", nameof(Site.SiteDepartmentId), dbContext.ConstructionDepartments.ToList().OrderBy(cd => cd.DepartmentName),
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

            AddWorkTypeForCategoryInObjectCategoryControls(oc);
        }

        private void CreateEstimateControls()
        {
            if (OriginalEntity is not Estimate estimate) return;

            if (DataManipulationMode == ManipulationMode.Add)
            {
                AddComboBoxControl("Материал", nameof(Estimate.MaterialId), dbContext.BuildingMaterials.ToList().OrderBy(bm => bm.MaterialName),
                    "MaterialName", estimate.MaterialId, "BuildingMaterialId");
                AddComboBoxControl("Объект", nameof(Estimate.EstimateObjectId), dbContext.Objects.ToList().OrderBy(o => o.ObjectName),
                    "ObjectName", estimate.EstimateObjectId, "ObjectId");
            }
            else
            {
                AddTextBlockControl("Материал", dbContext.BuildingMaterials.FirstOrDefault(bm => bm.BuildingMaterialId == estimate.MaterialId)?.MaterialName);
                AddTextBlockControl("Объект", dbContext.Objects.FirstOrDefault(o => o.ObjectId == estimate.EstimateObjectId)?.ObjectName);
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
            if (OriginalEntity is not DepartmentEquipment de) return;

            AddComboBoxControl("Название техники", nameof(DepartmentEquipment.OrgEquipmentId),
                dbContext.OrganizationEquipments.ToList().OrderBy(oe => oe.EquipmentName), "EquipmentName", de.OrgEquipmentId, "OrganizationEquipmentId");
            AddComboBoxControl("Строительное управление", nameof(DepartmentEquipment.DepartmentId),
                dbContext.ConstructionDepartments.ToList().OrderBy(cd => cd.DepartmentName), "DepartmentName", de.DepartmentId, "ConstructionDepartmentId");
            AddNumericUpDownControl("Количество техники", nameof(DepartmentEquipment.DepartEquipmentQuantity), de.DepartEquipmentQuantity, 1);
        }

        private void CreateCustomerControls()
        {
            if (OriginalEntity is not Customer) return;

            AddTextBoxControl("Клиент", nameof(Customer.CustomerName), 100);
        }

        private void CreateContractControls()
        {
            if (OriginalEntity is not Contract contract) return;

            AddTextBoxControl("Название контракта", nameof(Contract.ContractName));
            AddComboBoxControl("Клиент", nameof(Contract.ContractCustomerId),
                dbContext.Customers.ToList().OrderBy(c => c.CustomerName), "CustomerName", contract.ContractCustomerId, "CustomerId");
            AddNumericUpDownControl("Стоимость", nameof(Contract.TotalAmount), contract.TotalAmount, 1000, "F2");
            AddComboBoxControl("Статус контракта", nameof(Contract.ContractStatus), ["active", "completed", "terminated"], "", contract.ContractStatus);
        }

        private void CreateConstructionDepartmentControls()
        {
            if (OriginalEntity is not ConstructionDepartment cd) return;

            AddTextBoxControl("Название управления", nameof(ConstructionDepartment.DepartmentName), 100);
            AddTextBoxControl("Адрес управления", nameof(ConstructionDepartment.DepartmentLocation), 150);

            var departSupervisors = dbContext.Employees.Include(e => e.EmplCategory)
                                                .Where(e => e.EmplCategory.CategoryType == "Инженерно-технический персонал" && e.FireDate == null)
                                                .Select(e => new
                                                {
                                                    e.EmployeeId,
                                                    FullName = $"{e.Surname} {e.Name} {e.Patronymic ?? string.Empty}".Trim()
                                                }).ToList().OrderBy(e => e.FullName);
            AddComboBoxControl("Начальник управления", nameof(ConstructionDepartment.DepartmentSupervisorId),
                departSupervisors, "FullName", cd.DepartmentSupervisorId!, "EmployeeId");

            AddDepartmentEquipmentInDepartmentControls(cd);
        }

        private void CreateBuildingMaterialControls()
        {
            if (OriginalEntity is not BuildingMaterial) return;

            AddTextBoxControl("Название материала", nameof(BuildingMaterial.MaterialName), 100);
            AddTextBoxControl("Единица измерения", nameof(BuildingMaterial.UnitOfMeasure), 20);
        }

        private void CreateEmployeeControls()
        {
            if (OriginalEntity is not Employee employee) return;

            AddTextBoxControl("Имя", nameof(Employee.Name), 50);
            AddTextBoxControl("Фамилия", nameof(Employee.Surname), 50);
            AddTextBoxControl("Отчество", nameof(Employee.Patronymic), 50);

            AddComboBoxControl(label: "Должность",
                               propertyName: nameof(Employee.EmplCategoryId),
                               items: dbContext.EmployeeCategories.ToList().OrderBy(ec => ec.EmplCategoryName),
                               displayMember: "EmplCategoryName",
                               selectedValue: employee.EmplCategoryId,
                               valueMember: "EmployeeCategoryId");
            AddComboBoxControl("Образование", nameof(Employee.Education),
                ["Высшее", "Среднее специальное"], "", employee.Education);

            AddTextBoxControl("Телефон", nameof(Employee.ContactNumber), 20);

            AddDatePickerControl("Дата приема", nameof(Employee.HireDate));
            AddDatePickerControl("Дата увольнения", nameof(Employee.FireDate));

            AddSpecificEmployeeAttributeInEmployeeControls(employee);
        }

        private void CreateBrigadeControls()
        {
            if (OriginalEntity  is not Brigade brigade) return;

            AddTextBoxControl("Название бригады", nameof(Brigade.BrigadeName), 100);

            var brigadiers = dbContext.Employees.Include(e => e.EmplCategory)
                                                .Where(e => e.EmplCategory.CategoryType == "Рабочие" && e.FireDate == null)
                                                .Select(e => new
                                                {
                                                    e.EmployeeId,
                                                    FullName = $"{e.Surname} {e.Name} {e.Patronymic ?? string.Empty}".Trim()
                                                }).ToList().OrderBy(e => e.FullName);
            AddComboBoxControl("Бригадир", nameof(Brigade.BrigadierId), brigadiers, "FullName", brigade.BrigadierId!, "EmployeeId");

            AddComboBoxControl("Статус бригады", nameof(Brigade.BrigadeStatus),
                ["active", "terminated"], "", brigade.BrigadeStatus);

            AddBrigadeCompositionControl(brigade);
        }
        private void AddSpecificEmployeeAttributeInEmployeeControls(Employee employee)
        {
            var allPossibleAttributes = dbContext.EmployeeAttributes.ToList();

            var currentAttributes = dbContext.SpecificEmployeeAttributes
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
                    var existingEntity = dbContext.SpecificEmployeeAttributes.Local.FirstOrDefault(sea =>
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
                onAdd: entity => dbContext.SpecificEmployeeAttributes.Add(entity),
                onRemove: entity => dbContext.SpecificEmployeeAttributes.Remove(entity)
            );
        }
        private void AddMasterOnObjectControl(Models.Object obj)
        {
            var allMasters = dbContext.Employees
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
                viewModelToEntity: vm => dbContext.Employees.First(e => e.EmployeeId == vm.ItemId),
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
            var availableWorkers = dbContext.Employees
                    .Include(e => e.EmplCategory)
                    .Where(e => e.EmplCategory.CategoryType == "Рабочие" && e.FireDate == null &&
                               !dbContext.Brigades.Any(b => b.BrigadeStatus == "active" && (b.Workers.Contains(e) || b.BrigadierId == e.EmployeeId)))
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
                viewModelToEntity: vm => dbContext.Employees.First(e => e.EmployeeId == vm.ItemId),
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
            var allPossibleCharacteristic = dbContext.ObjectCharacteristics.ToList();

            var currentCharacteristic = dbContext.SpecificObjectCharacteristics
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
                    var existingEntity = dbContext.SpecificObjectCharacteristics.Local.FirstOrDefault(soc =>
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
                onAdd: entity => dbContext.SpecificObjectCharacteristics.Add(entity),
                onRemove: entity => dbContext.SpecificObjectCharacteristics.Remove(entity)
            );
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
        private void AddWorkTypeForCategoryInObjectCategoryControls(ObjectCategory oc)
        {
            var allPossibleWorkType = dbContext.WorkTypes.ToList();

            var currentWorkType = dbContext.WorkTypeForCategories
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
                    var existingEntity = dbContext.WorkTypeForCategories.Local.FirstOrDefault(wtfc =>
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
                onAdd: entity => dbContext.WorkTypeForCategories.Add(entity),
                onRemove: entity => dbContext.WorkTypeForCategories.Remove(entity)
            );
        }
    }
}
