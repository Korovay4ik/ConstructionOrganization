using constructionOrgManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace constructionOrgManagement.ViewModels
{
    public partial class DataEditViewModel
    {
        private void CreateObjectEquipmentControls()
        {
            if (SelectedItem is not ObjectEquipment oe) return;

            AddTextBlockControl("Объект", dbContext.Objects.FirstOrDefault(o => o.ObjectId == oe.EquipmentForObjectId)?.ObjectName);
            AddTextBlockControl("Название техники", dbContext.DepartmentEquipments
                                                             .Include(de => de.OrgEquipment)
                                                             .FirstOrDefault(de => de.DepartmentEquipmentId == oe.EquipmentId)?.OrgEquipment?.EquipmentName);
            AddNumericUpDownControl("Количество техники", nameof(ObjectEquipment.EquipObjectQuantity), oe.EquipObjectQuantity, 1);
            AddDatePickerControl("Дата выдачи", nameof(ObjectEquipment.AssignmentDate));
            AddDatePickerControl("Дата возврата", nameof(ObjectEquipment.ReturnDate));
        }

        private void CreateWorkTypeForCategoryControls()
        {
            if (SelectedItem is not WorkTypeForCategory wtfc) return;

            AddTextBlockControl("Категория объекта", dbContext.ObjectCategories.FirstOrDefault(oc => oc.ObjectCategoryId == wtfc.SpecificCategoryId)?.ObjCategoryName);
            AddTextBlockControl("Тип работ", dbContext.WorkTypes.FirstOrDefault(wt => wt.WorkTypeId == wtfc.WtfcWorkTypeId)?.WorkTypeName);
            AddCheckBoxControl("Является обязательным", nameof(WorkTypeForCategory.IsMandatory), wtfc.IsMandatory ?? false);
        }

        private void CreateWorkTypeControls()
        {
            if (SelectedItem is not WorkType) return;

            AddTextBoxControl("Название работ", nameof(WorkType.WorkTypeName), 100);
            AddTextBoxControl("Описание", nameof(WorkType.WorkTypeDescription));
        }

        private void CreateWorkScheduleControl()
        {
            if (SelectedItem is not WorkSchedule ws) return;

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
            if (SelectedItem is not SpecificObjectCharacteristic soc) return;

            AddTextBlockControl("Объект", dbContext.Objects.FirstOrDefault(o => o.ObjectId == soc.SpecificObjectId)?.ObjectName);
            AddTextBlockControl("Характеристика", dbContext.ObjectCharacteristics.FirstOrDefault(oc => oc.ObjectCharacteristicId == soc.CharacteristicId)?.ObjCharacteristicName);
            AddTextBoxControl("Значение", nameof(SpecificObjectCharacteristic.CharacteristicValue), 255);
        }

        private void CreateSpecificEmployeeAttributeControls()
        {
            if (SelectedItem is not SpecificEmployeeAttribute sea) return;

            var employee = dbContext.Employees.FirstOrDefault(e => e.EmployeeId == sea.SpecificEmployeeId);
            AddTextBlockControl("Сотрудник", DataManipulationViewModel.GetFullName(employee));
            AddTextBlockControl("Название атрибута", dbContext.EmployeeAttributes.FirstOrDefault(ea => ea.EmployeeAttributeId == sea.AttributeId)?.AttributeName);
            AddTextBoxControl("Значение", nameof(SpecificEmployeeAttribute.AttributeValue), 255);
        }

        private void CreateSiteControls()
        {
            if (SelectedItem is not Site site) return;

            AddTextBoxControl("Название участка", nameof(Site.SiteName), 100);
            var siteSupervisors = dbContext.Employees.Include(e => e.EmplCategory)
                                                .Where(e => e.EmplCategory.CategoryType == "Инженерно-технический персонал")
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
            if (SelectedItem is not OrganizationEquipment oe) return;

            AddTextBoxControl("Название техники", nameof(OrganizationEquipment.EquipmentName), 100);
            AddNumericUpDownControl("Количество техники", nameof(OrganizationEquipment.OrgEquipmentQuantity), oe.OrgEquipmentQuantity, 1);
        }

        private void CreateObjectCharacteristicControls()
        {
            if (SelectedItem is not ObjectCharacteristic) return;

            AddTextBoxControl("Название характеристики", nameof(ObjectCharacteristic.ObjCharacteristicName), 50);
        }

        private void CreateObjectCategoryControls()
        {
            if (SelectedItem is not ObjectCategory) return;

            AddTextBoxControl("Название категории", nameof(ObjectCategory.ObjCategoryName), 50);
        }

        private void CreateEstimateControls()
        {
            if (SelectedItem is not Estimate estimate) return;

            AddTextBlockControl("Материал", dbContext.BuildingMaterials.FirstOrDefault(bm => bm.BuildingMaterialId == estimate.MaterialId)?.MaterialName);
            AddTextBlockControl("Объект", dbContext.Objects.FirstOrDefault(o => o.ObjectId == estimate.EstimateObjectId)?.ObjectName);
            //AddComboBoxControl("Материал", nameof(Estimate.MaterialId), dbContext.BuildingMaterials.ToList().OrderBy(bm=>bm.MaterialName),
            //    "MaterialName", estimate.MaterialId, "BuildingMaterialId");
            //AddComboBoxControl("Объект", nameof(Estimate.EstimateObjectId), dbContext.Objects.ToList().OrderBy(o=>o.ObjectName),
            //    "ObjectName", estimate.EstimateObjectId, "ObjectId");
            AddNumericUpDownControl("Плановое количество", nameof(Estimate.PlannedMaterialQuantity), estimate.PlannedMaterialQuantity, 100, "F2");
            AddNumericUpDownControl("Цена за единицу", nameof(Estimate.UnitPrice), estimate.UnitPrice, 1, "F2");
            //AddNumericUpDownControl("Фактическое количество", nameof(Estimate.ActualMaterialQuantity), estimate.ActualMaterialQuantity, 100, "F2");
        }

        private void CreateEmployeeCategoryControls()
        {
            if (SelectedItem is not EmployeeCategory ec) return;

            AddTextBoxControl("Название должности", nameof(EmployeeCategory.EmplCategoryName), 50);
            AddComboBoxControl("Тип категории", nameof(EmployeeCategory.CategoryType), ["Рабочие", "Инженерно-технический персонал"],
                "", ec.CategoryType);
        }

        private void CreateEmployeeAttributeControls()
        {
            if (SelectedItem is not EmployeeAttribute) return;

            AddTextBoxControl("Название атрибута", nameof(EmployeeAttribute.AttributeName), 100);
        }

        private void CreateDepartmentEquipmentControls()
        {
            if (SelectedItem is not DepartmentEquipment de) return;

            AddComboBoxControl("Название техники", nameof(DepartmentEquipment.OrgEquipmentId),
                dbContext.OrganizationEquipments.ToList().OrderBy(oe => oe.EquipmentName), "EquipmentName", de.OrgEquipmentId, "OrganizationEquipmentId");
            AddComboBoxControl("Строительное управление", nameof(DepartmentEquipment.DepartmentId),
                dbContext.ConstructionDepartments.ToList().OrderBy(cd => cd.DepartmentName), "DepartmentName", de.DepartmentId, "ConstructionDepartmentId");
            AddNumericUpDownControl("Количество техники", nameof(DepartmentEquipment.DepartEquipmentQuantity), de.DepartEquipmentQuantity, 1);
        }

        private void CreateCustomerControls()
        {
            if (SelectedItem is not Customer) return;

            AddTextBoxControl("Клиент", nameof(Customer.CustomerName), 100);
        }

        private void CreateContractControls()
        {
            if (SelectedItem is not Contract contract) return;

            AddTextBoxControl("Название контракта", nameof(Contract.ContractName));
            AddComboBoxControl("Клиент", nameof(Contract.ContractCustomerId),
                dbContext.Customers.ToList().OrderBy(c => c.CustomerName), "CustomerName", contract.ContractCustomerId, "CustomerId");
            AddNumericUpDownControl("Стоимость", nameof(Contract.TotalAmount), contract.TotalAmount, 1000, "F2");
            AddComboBoxControl("Статус контракта", nameof(Contract.ContractStatus), ["active", "completed", "terminated"], "", contract.ContractStatus);
        }

        private void CreateConstructionDepartmentControls()
        {
            if (SelectedItem is not ConstructionDepartment cd) return;

            AddTextBoxControl("Название управления", nameof(ConstructionDepartment.DepartmentName), 100);
            AddTextBoxControl("Адрес управления", nameof(ConstructionDepartment.DepartmentLocation), 150);

            var departSupervisors = dbContext.Employees.Include(e => e.EmplCategory)
                                                .Where(e => e.EmplCategory.CategoryType == "Инженерно-технический персонал")
                                                .Select(e => new
                                                {
                                                    e.EmployeeId,
                                                    FullName = $"{e.Surname} {e.Name} {e.Patronymic ?? string.Empty}".Trim()
                                                }).ToList().OrderBy(e => e.FullName);
            AddComboBoxControl("Начальник управления", nameof(ConstructionDepartment.DepartmentSupervisorId),
                departSupervisors, "FullName", cd.DepartmentSupervisorId!, "EmployeeId");
        }

        private void CreateBuildingMaterialControls()
        {
            if (SelectedItem is not BuildingMaterial) return;

            AddTextBoxControl("Название материала", nameof(BuildingMaterial.MaterialName), 100);
            AddTextBoxControl("Единица измерения", nameof(BuildingMaterial.UnitOfMeasure), 20);
        }

        private void CreateEmployeeControls()
        {
            if (SelectedItem is not Employee employee) return;

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
        }

        private void CreateBrigadeControls()
        {
            if (SelectedItem is not Brigade brigade) return;

            AddTextBoxControl("Название бригады", nameof(Brigade.BrigadeName), 100);

            var brigadiers = dbContext.Employees.Include(e => e.EmplCategory)
                                                .Where(e => e.EmplCategory.CategoryType == "Рабочие")
                                                .Select(e => new
                                                {
                                                    e.EmployeeId,
                                                    FullName = $"{e.Surname} {e.Name} {e.Patronymic ?? string.Empty}".Trim()
                                                }).ToList().OrderBy(e => e.FullName);
            AddComboBoxControl("Бригадир", nameof(Brigade.BrigadierId), brigadiers, "FullName", brigade.BrigadierId!, "EmployeeId");

            AddComboBoxControl("Статус бригады", nameof(Brigade.BrigadeStatus),
                ["active", "terminated"], "", brigade.BrigadeStatus);
        }
    }
}
