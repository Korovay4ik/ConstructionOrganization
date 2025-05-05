using constructionOrgManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace constructionOrgManagement.ViewModels
{
    public partial class DataManipulationViewModel
    {
        private static List<object> GetEmployeeData(List<object> items)
        {
            var enhancedItems = new List<object>();

            foreach (var item in items.Cast<Employee>())
            {
                enhancedItems.Add(new
                {
                    item.EmployeeId,
                    Имя = item.Name,
                    Фамилия = item.Surname,
                    Отчество = item.Patronymic,
                    Должность = dbContext.EmployeeCategories.FirstOrDefault(c => c.EmployeeCategoryId == item.EmplCategoryId)?.EmplCategoryName,
                    Образование = item.Education,
                    Телефон = item.ContactNumber,
                    Дата_приема = item.HireDate,
                    Дата_увольнения = item.FireDate
                });
            }
            return enhancedItems;
        }

        private static List<object> GetBrigadeData(List<object> items)
        {
            var enhancedItems = new List<object>();

            foreach (var item in items.Cast<Brigade>())
            {
                var brigadier = dbContext.Employees.FirstOrDefault(e => e.EmployeeId == item.BrigadierId);
                var brigadierName = brigadier != null
                    ? $"{brigadier.Surname} {brigadier.Name} {brigadier.Patronymic}"
                    : null;

                enhancedItems.Add(new
                {
                    item.BrigadeId,
                    Название = item.BrigadeName,
                    Бригадир = brigadierName,
                    Статус = item.BrigadeStatus
                });
            }
            return enhancedItems;
        }
        private static List<object> GetContractData(List<object> items)
        {
            var enhancedItems = new List<object>();

            foreach (var item in items.Cast<Contract>())
            {
                enhancedItems.Add(new
                {
                    item.ContractId,
                    Название = item.ContractName,
                    Клиент = dbContext.Customers.FirstOrDefault(c => c.CustomerId == item.ContractCustomerId)?.CustomerName,
                    Сумма_сделки = item.TotalAmount,
                    Статус_контракта = item.ContractStatus
                });
            }
            return enhancedItems;
        }

        private static List<object> GetObjectData(List<object> items)
        {
            var enhancedItems = new List<object>();

            foreach (var item in items.Cast<constructionOrgManagement.Models.Object>())
            {
                var foreman = dbContext.Employees.FirstOrDefault(e => e.EmployeeId == item.ForemanId);
                enhancedItems.Add(new
                {
                    item.ObjectId,
                    Название = item.ObjectName,
                    Прораб = GetFullName(foreman),
                    Адрес = item.ObjectLocation,
                    Участок = dbContext.Sites.FirstOrDefault(s => s.SiteId == item.ObjectSiteId)?.SiteName,
                    По_контракту = dbContext.Contracts.FirstOrDefault(c => c.ContractId == item.ObjectContractId)?.ContractName,
                    Категория = dbContext.ObjectCategories.FirstOrDefault(oc => oc.ObjectCategoryId == item.CategoryId)?.ObjCategoryName,
                    Статус = item.ObjectStatus
                });
            }
            return enhancedItems;
        }
        private static List<object> GetSiteData(List<object> items)
        {
            var enhancedItems = new List<object>();

            foreach (var item in items.Cast<Site>())
            {
                var siteSupervisor = dbContext.Employees.FirstOrDefault(e => e.EmployeeId == item.SiteSupervisorId);
                enhancedItems.Add(new
                {
                    item.SiteId,
                    Название = item.SiteName,
                    Начальник_участка = GetFullName(siteSupervisor),
                    Адрес = item.SiteLocation,
                    Строительное_управление = dbContext.ConstructionDepartments.FirstOrDefault(cd => cd.ConstructionDepartmentId == item.SiteDepartmentId)?.DepartmentName
                });
            }
            return enhancedItems;
        }
        private static List<object> GetConstructionDepartmentData(List<object> items)
        {
            var enhancedItems = new List<object>();

            foreach (var item in items.Cast<ConstructionDepartment>())
            {
                var departSupervisor = dbContext.Employees.FirstOrDefault(e => e.EmployeeId == item.DepartmentSupervisorId);
                enhancedItems.Add(new
                {
                    item.ConstructionDepartmentId,
                    Название = item.DepartmentName,
                    Адрес = item.DepartmentLocation,
                    Начальник_управления = GetFullName(departSupervisor)
                });
            }
            return enhancedItems;
        }
        private static List<object> GetOrganizationEquipmentData(List<object> items)
        {
            var enhancedItems = new List<object>();

            foreach (var item in items.Cast<OrganizationEquipment>())
            {
                enhancedItems.Add(new
                {
                    item.OrganizationEquipmentId,
                    Название = item.EquipmentName,
                    Количествово_в_организации = item.OrgEquipmentQuantity
                });
            }
            return enhancedItems;
        }
        private static List<object> GetBuildingMaterialData(List<object> items)
        {
            var enhancedItems = new List<object>();

            foreach (var item in items.Cast<BuildingMaterial>())
            {
                enhancedItems.Add(new
                {
                    item.BuildingMaterialId,
                    Название = item.MaterialName,
                    Единицы_измерения = item.UnitOfMeasure
                });
            }
            return enhancedItems;
        }
        private static List<object> GetCustomerData(List<object> items)
        {
            var enhancedItems = new List<object>();

            foreach (var item in items.Cast<Customer>())
            {
                enhancedItems.Add(new
                {
                    item.CustomerId,
                    Клиент = item.CustomerName
                });
            }
            return enhancedItems;
        }
        private static List<object> GetDepartmentEquipmentData(List<object> items)
        {
            var enhancedItems = new List<object>();

            foreach (var item in items.Cast<DepartmentEquipment>())
            {
                enhancedItems.Add(new
                {
                    item.DepartmentEquipmentId,
                    Название_техники = dbContext.OrganizationEquipments.FirstOrDefault(oe => oe.OrganizationEquipmentId == item.OrgEquipmentId)?.EquipmentName,
                    Выделено_управлению = dbContext.ConstructionDepartments.FirstOrDefault(cd => cd.ConstructionDepartmentId == item.DepartmentId)?.DepartmentName,
                    Выделенное_количество = item.DepartEquipmentQuantity
                });
            }
            return enhancedItems;
        }
        private static List<object> GetEmployeeAttributeData(List<object> items)
        {
            var enhancedItems = new List<object>();

            foreach (var item in items.Cast<EmployeeAttribute>())
            {
                enhancedItems.Add(new
                {
                    item.EmployeeAttributeId,
                    Атрибут = item.AttributeName
                });
            }
            return enhancedItems;
        }
        private static List<object> GetEmployeeCategoryData(List<object> items)
        {
            var enhancedItems = new List<object>();

            foreach (var item in items.Cast<EmployeeCategory>())
            {
                enhancedItems.Add(new
                {
                    item.EmployeeCategoryId,
                    Должность = item.EmplCategoryName,
                    Тип_категории = item.CategoryType
                });
            }
            return enhancedItems;
        }
        private static List<object> GetEstimateData(List<object> items)
        {
            var enhancedItems = new List<object>();
            foreach (var item in items.Cast<Estimate>())
            {
                enhancedItems.Add(new
                {
                    item.MaterialId,
                    item.EstimateObjectId,
                    Материал = dbContext.BuildingMaterials.FirstOrDefault(bm => bm.BuildingMaterialId == item.MaterialId)?.MaterialName,
                    Закуплено_на_объект = dbContext.Objects.FirstOrDefault(o => o.ObjectId == item.EstimateObjectId)?.ObjectName,
                    Планируемое_количество = item.PlannedMaterialQuantity,
                    Цена_за_единицу = item.UnitPrice,
                    Фактическое_количество = item.ActualMaterialQuantity
                });
            }
            return enhancedItems;
        }
        private static List<object> GetObjectCategoryData(List<object> items)
        {
            var enhancedItems = new List<object>();
            foreach (var item in items.Cast<ObjectCategory>())
            {
                enhancedItems.Add(new
                {
                    item.ObjectCategoryId,
                    Название = item.ObjCategoryName
                });
            }
            return enhancedItems;
        }
        private static List<object> GetObjectCharacteristicData(List<object> items)
        {
            var enhancedItems = new List<object>();
            foreach (var item in items.Cast<ObjectCharacteristic>())
            {
                enhancedItems.Add(new
                {
                    item.ObjectCharacteristicId,
                    Название = item.ObjCharacteristicName
                });
            }
            return enhancedItems;
        }
        private static List<object> GetObjectEquipmentData(List<object> items)
        {
            var enhancedItems = new List<object>();

            foreach (var item in items.Cast<ObjectEquipment>())
            {
                enhancedItems.Add(new
                {
                    item.EquipmentId,
                    item.EquipmentForObjectId,
                    Объект = dbContext.Objects.FirstOrDefault(o => o.ObjectId == item.EquipmentForObjectId)?.ObjectName,
                    Название = dbContext.DepartmentEquipments
                                       .Include(de => de.OrgEquipment)
                                       .FirstOrDefault(de => de.DepartmentEquipmentId == item.EquipmentId)?.OrgEquipment?
                                       .EquipmentName,
                    Количество = item.EquipObjectQuantity,
                    Дата_выдачи = item.AssignmentDate,
                    Дата_возврата = item.ReturnDate
                });
            }
            return enhancedItems;
        }
        private static List<object> GetSpecificEmployeeAttributeData(List<object> items)
        {
            var enhancedItems = new List<object>();
            foreach (var item in items.Cast<SpecificEmployeeAttribute>())
            {
                var employee = dbContext.Employees.FirstOrDefault(e => e.EmployeeId == item.SpecificEmployeeId);
                enhancedItems.Add(new
                {
                    item.AttributeId,
                    item.SpecificEmployeeId,
                    Сотрудник = GetFullName(employee),
                    Атрибут = dbContext.EmployeeAttributes.FirstOrDefault(ea => ea.EmployeeAttributeId == item.AttributeId)?.AttributeName,
                    Значение = item.AttributeValue
                });
            }
            return enhancedItems;
        }
        private static List<object> GetSpecificObjectCharacteristicData(List<object> items)
        {
            var enhancedItems = new List<object>();
            foreach (var item in items.Cast<SpecificObjectCharacteristic>())
            {
                enhancedItems.Add(new
                {
                    item.SpecificObjectId,
                    item.CharacteristicId,
                    Объект = dbContext.Objects.FirstOrDefault(o => o.ObjectId == item.SpecificObjectId)?.ObjectName,
                    Характеристика = dbContext.ObjectCharacteristics.FirstOrDefault(oc => oc.ObjectCharacteristicId == item.CharacteristicId)?.ObjCharacteristicName,
                    Значение = item.CharacteristicValue
                });
            }
            return enhancedItems;
        }
        private static List<object> GetWorkScheduleData(List<object> items)
        {
            var enhancedItems = new List<object>();
            foreach (var item in items.Cast<WorkSchedule>())
            {
                enhancedItems.Add(new
                {
                    item.ScheduleId,
                    Объект = dbContext.Objects.FirstOrDefault(o => o.ObjectId == item.ScheduleObjectId)?.ObjectName,
                    Бригада = dbContext.Brigades.FirstOrDefault(b => b.BrigadeId == item.ScheduleBrigadeId)?.BrigadeName,
                    Тип_работ = dbContext.WorkTypes.FirstOrDefault(wt => wt.WorkTypeId == item.ScheduleWorkTypeId)?.WorkTypeName,
                    Плановая_дата_начала = item.PlannedStartDate,
                    Плановая_дата_конца = item.PlannedEndDate,
                    Фактическая_дата_начала = item.ActualStartDate,
                    Фактическая_дата_конца = item.ActualEndDate
                });
            }
            return enhancedItems;
        }
        private static List<object> GetWorkTypeData(List<object> items)
        {
            var enhancedItems = new List<object>();
            foreach (var item in items.Cast<WorkType>())
            {
                enhancedItems.Add(new
                {
                    item.WorkTypeId,
                    Название = item.WorkTypeName,
                    Описание = item.WorkTypeDescription
                });
            }
            return enhancedItems;
        }
        private static List<object> GetWorkTypeForCategoryData(List<object> items)
        {
            var enhancedItems = new List<object>();
            foreach (var item in items.Cast<WorkTypeForCategory>())
            {
                enhancedItems.Add(new
                {
                    item.WtfcWorkTypeId,
                    item.SpecificCategoryId,
                    Категория_объекта = dbContext.ObjectCategories.FirstOrDefault(oc => oc.ObjectCategoryId == item.SpecificCategoryId)?.ObjCategoryName,
                    Тип_работы = dbContext.WorkTypes.FirstOrDefault(wt => wt.WorkTypeId == item.WtfcWorkTypeId)?.WorkTypeName,
                    Обязательно = item.IsMandatory
                });
            }
            return enhancedItems;
        }
        private static List<object> GetDefaultData(List<object> items)
        {
            var firstItem = items.First();
            var properties = firstItem.GetType().GetProperties()
                .Where(p => p.PropertyType.IsPrimitive ||
                           p.PropertyType == typeof(string) ||
                           p.PropertyType == typeof(DateOnly) ||
                           p.PropertyType == typeof(decimal))
                .ToList();

            var columnNames = properties.Select(p => p.Name).ToList();
            return items;
        }
    }
}
