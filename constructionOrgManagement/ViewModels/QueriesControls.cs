using CommunityToolkit.Mvvm.Input;
using constructionOrgManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace constructionOrgManagement.ViewModels
{
    public partial class MainWindowViewModel
    {
        [RelayCommand]
        private void ShowDepartmentSupervisor()
        {
            if (_dbContext == null) return;

            string title = "Строительные управления и участки с руководителями";
            string description = "Запрос №1 Получение перечня строительных управлений или участков и их руководителей";
            var parameters = new List<QueryParameterInfo>
            {
                new ()
                {
                    Name = "Тип структуры",
                    ParameterType = typeof(string),
                    DefaultValue = "Строительные управления",
                    AvailableValues = ["Строительные управления", "Участки"]
                }
            };
            var columnSelector = new Func<Dictionary<string, object>, List<DataGridColumnInfo>>(
                parameter =>
                {
                    var dataType = (string)parameter["Тип структуры"];
                    return dataType switch
                    {
                        "Строительные управления" =>
                        [
                            new() { PropertyName = nameof(ViewDepartmentAndSitesWithLeader.Name), Header = "Название управления" },
                        new() { PropertyName = nameof(ViewDepartmentAndSitesWithLeader.Location), Header = "Адрес" },
                        new() { PropertyName = nameof(ViewDepartmentAndSitesWithLeader.LeaderName), Header = "Руководитель" },
                    ],
                        "Участки" =>
                        [
                            new() { PropertyName = nameof(ViewDepartmentAndSitesWithLeader.Name), Header = "Название участка" },
                        new() { PropertyName = nameof(ViewDepartmentAndSitesWithLeader.Location), Header = "Адрес" },
                        new() { PropertyName = nameof(ViewDepartmentAndSitesWithLeader.LeaderName), Header = "Руководитель" },
                        new() { PropertyName = nameof(ViewDepartmentAndSitesWithLeader.ParentDepartmentName), Header = "Принадлежит управлению" },
                    ],
                        _ => throw new ArgumentException("Неизвестный тип данных")
                    };
                });
            static IQueryable<ViewDepartmentAndSitesWithLeader> queryExecutor(ConstructionOrganizationContext db, Dictionary<string, object> parameters)
            {
                var dataType = (string)parameters["Тип структуры"];

                return dataType switch
                {
                    "Строительные управления" => db.Set<ViewDepartmentAndSitesWithLeader>()
                                .Where(x => EF.Functions.Collate(x.EntityType, "utf8mb4_unicode_ci") == "department"),
                    "Участки" => db.Set<ViewDepartmentAndSitesWithLeader>()
                               .Where(x => EF.Functions.Collate(x.EntityType, "utf8mb4_unicode_ci") == "site"),
                    _ => throw new ArgumentException("Неизвестный тип данных")
                };
            }

            ShowQueryWindow(title, description, parameters, columnSelector, queryExecutor);
        }
        [RelayCommand]
        private void ShowEngineeringAndTechnicalStaffQuery()
        {
            if (_dbContext == null) return;

            string title = "Инженерно-технический состав";
            string description = "Запрос №2 Получение списка специалистов инженерно-технического состава участка или строительного управления";
            var parameters = new List<QueryParameterInfo>
            {
                new ()
                {
                    Name = "Тип структуры",
                    ParameterType = typeof(string),
                    DefaultValue = "Строительное управление",
                    AvailableValues = ["Строительное управление", "Участок"]
                },
                new ()
                {
                    Name = "Управление",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.ConstructionDepartments.OrderBy(cd=>cd.DepartmentName).Select(cd=>cd.DepartmentName)]
                },
                new ()
                {
                    Name = "Участок",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.Sites.OrderBy(s=>s.SiteName).Select(s=>s.SiteName)]
                }
            };
            var columnSelector = new Func<Dictionary<string, object>, List<DataGridColumnInfo>>(
                parameter =>
                {
                    var dataType = (string)parameter["Тип структуры"];
                    return dataType switch
                    {
                        "Строительное управление" =>
                        [
                            new() { PropertyName = nameof(ViewEngineeringStaffByDepartment.Department), Header = "Название управления" },
                            new() { PropertyName = nameof(ViewEngineeringStaffByDepartment.FullName), Header = "Специалист" },
                            new() { PropertyName = nameof(ViewEngineeringStaffByDepartment.Position), Header = "Должность" },
                            new() { PropertyName = nameof(ViewEngineeringStaffByDepartment.ContactNumber), Header = "Номер телефона" },
                        ],
                        "Участок" =>
                        [
                            new() { PropertyName = nameof(ViewEngineeringStaffByDepartment.Site), Header = "Название участка" },
                            new() { PropertyName = nameof(ViewEngineeringStaffByDepartment.Department), Header = "Принадлежит управлению" },
                            new() { PropertyName = nameof(ViewEngineeringStaffByDepartment.FullName), Header = "Специалист" },
                            new() { PropertyName = nameof(ViewEngineeringStaffByDepartment.Position), Header = "Должность" },
                            new() { PropertyName = nameof(ViewEngineeringStaffByDepartment.ContactNumber), Header = "Номер телефона" },
                        ],
                        _ => throw new ArgumentException("Неизвестный тип данных")
                    };
                });
            static IQueryable<ViewEngineeringStaffByDepartment> queryExecutor(ConstructionOrganizationContext db, Dictionary<string, object> parameters)
            {
                var dataType = (string)parameters["Тип структуры"];
                var department = (string)parameters["Управление"];
                var site = (string)parameters["Участок"];

                return dataType switch
                {
                    "Строительное управление" => db.Set<ViewEngineeringStaffByDepartment>()
                                .Where(x => x.Department == department),
                    "Участок" => db.Set<ViewEngineeringStaffByDepartment>()
                               .Where(x => x.Site == site),
                    _ => throw new ArgumentException("Неизвестный тип данных")
                };
            }

            ShowQueryWindow(title, description, parameters, columnSelector, queryExecutor);
        }
        [RelayCommand]
        private void ShowObjectWithScheduleQuery()
        {
            if (_dbContext == null) return;

            string title = "Объекты и графики их возведения";
            string description = "Запрос №3 Получение перечня объектов строительного управления или участка и графики их возведения";
            var parameters = new List<QueryParameterInfo>
            {
                new ()
                {
                    Name = "Объекты, возводимые",
                    ParameterType = typeof(string),
                    DefaultValue = "Строительным управлением",
                    AvailableValues = ["Строительным управлением", "Участком"]
                },
                new ()
                {
                    Name = "Управление",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.ConstructionDepartments.OrderBy(cd=>cd.DepartmentName).Select(cd=>cd.DepartmentName)]
                },
                new ()
                {
                    Name = "Участок",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.Sites.OrderBy(s=>s.SiteName).Select(s=>s.SiteName)]
                }
            };
            var columnSelector = new Func<Dictionary<string, object>, List<DataGridColumnInfo>>(
                parameter =>
                {
                    var dataType = (string)parameter["Объекты, возводимые"];
                    return dataType switch
                    {
                        "Строительным управлением" =>
                        [
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.ObjectName), Header = "Название объекта" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.DepartmentName), Header = "Управление" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.WorkTypeName), Header = "Вид работ" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.PlannedStartDate), Header = "Плановая дата начала", FormatString = "dd.MM.yyyy" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.PlannedEndDate), Header = "Плановая дата конца", FormatString = "dd.MM.yyyy" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.ActualStartDate), Header = "Фактическая дата начала", FormatString = "dd.MM.yyyy" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.ActualEndDate), Header = "Фактическая дата конца", FormatString = "dd.MM.yyyy" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.WorkStatus), Header = "Статус работы" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.AssignedBrigade), Header = "Бригада" },
                        ],
                        "Участком" =>
                        [
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.ObjectName), Header = "Название Объекта" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.SiteName), Header = "Участок" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.WorkTypeName), Header = "Вид работ" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.PlannedStartDate), Header = "Плановая дата начала", FormatString = "dd.MM.yyyy" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.PlannedEndDate), Header = "Плановая дата конца", FormatString = "dd.MM.yyyy" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.ActualStartDate), Header = "Фактическая дата начала", FormatString = "dd.MM.yyyy" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.ActualEndDate), Header = "Фактическая дата конца", FormatString = "dd.MM.yyyy" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.WorkStatus), Header = "Статус работы" },
                            new() { PropertyName = nameof(ViewDepartmentAndSitesObjectWithSchedule.AssignedBrigade), Header = "Бригада" },
                        ],
                        _ => throw new ArgumentException("Неизвестный тип данных")
                    };
                });
            static IQueryable<ViewDepartmentAndSitesObjectWithSchedule> queryExecutor(ConstructionOrganizationContext db, Dictionary<string, object> parameters)
            {
                var dataType = (string)parameters["Объекты, возводимые"];
                var department = (string)parameters["Управление"];
                var site = (string)parameters["Участок"];

                return dataType switch
                {
                    "Строительным управлением" => db.Set<ViewDepartmentAndSitesObjectWithSchedule>()
                                .Where(x => x.DepartmentName == department),
                    "Участком" => db.Set<ViewDepartmentAndSitesObjectWithSchedule>()
                               .Where(x => x.SiteName == site),
                    _ => throw new ArgumentException("Неизвестный тип данных")
                };
            }

            ShowQueryWindow(title, description, parameters, columnSelector, queryExecutor);
        }
        [RelayCommand]
        private void ShowBrigadeOnObjectQuery()
        {
            if (_dbContext == null) return;

            string title = "Бригады на объекте";
            string description = "Запрос №4 Получение бригад, работавших над строительством объекта";
            var parameters = new List<QueryParameterInfo>
            {
                new ()
                {
                    Name = "Объект",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.Objects.OrderBy(o=>o.ObjectName).Select(o=>o.ObjectName)]
                },
            };
            var columnSelector = new Func<Dictionary<string, object>, List<DataGridColumnInfo>>(
                parameter =>
                {
                    return
                    [
                        new() { PropertyName = nameof(ViewObjectBrigade.ObjectName), Header = "Объект" },
                        new() { PropertyName = nameof(ViewObjectBrigade.BrigadeName), Header = "Бригада" },
                        new() { PropertyName = nameof(ViewObjectBrigade.BrigadierName), Header = "Бригадир" },
                        new() { PropertyName = nameof(ViewObjectBrigade.ActualStartDate), Header = "Фактическая дата начала", FormatString = "dd.MM.yyyy" },
                        new() { PropertyName = nameof(ViewObjectBrigade.ActualEndDate), Header = "Фактическая дата конца", FormatString = "dd.MM.yyyy" },
                    ];
                });
            static IQueryable<ViewObjectBrigade> queryExecutor(ConstructionOrganizationContext db, Dictionary<string, object> parameters)
            {
                var objectName = (string)parameters["Объект"];

                return db.Set<ViewObjectBrigade>()
                      .Where(x => x.ObjectName == objectName &&
                             x.ActualStartDate < DateOnly.FromDateTime(DateTime.Now));
            }

            ShowQueryWindow(title, description, parameters, columnSelector, queryExecutor);
        }
        [RelayCommand]
        private void ShowDepartmentEquipmentQuery()
        {
            if (_dbContext == null) return;

            string title = "Техника управления";
            string description = "Запрос №5 Получения перечня строительной техники, выделенной строительному управлению";
            var parameters = new List<QueryParameterInfo>
            {
                new ()
                {
                    Name = "Управление",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.ConstructionDepartments.OrderBy(cd=>cd.DepartmentName).Select(cd=>cd.DepartmentName)]
                },
            };
            var columnSelector = new Func<Dictionary<string, object>, List<DataGridColumnInfo>>(
                parameter =>
                {
                    return
                    [
                        new() { PropertyName = nameof(ViewDepartmentEquipmentList.DepartmentName), Header = "Управление" },
                        new() { PropertyName = nameof(ViewDepartmentEquipmentList.EquipmentName), Header = "Техника" },
                        new() { PropertyName = nameof(ViewDepartmentEquipmentList.AssignedQuantity), Header = "Выделеное кол-во" },
                        new() { PropertyName = nameof(ViewDepartmentEquipmentList.AvailableDepartmentQuantity), Header = "Доступное кол-во" },
                        new() { PropertyName = nameof(ViewDepartmentEquipmentList.TotalInOrganization), Header = "Всего в организации" },
                    ];
                });
            static IQueryable<ViewDepartmentEquipmentList> queryExecutor(ConstructionOrganizationContext db, Dictionary<string, object> parameters)
            {
                var departName = (string)parameters["Управление"];

                return db.Set<ViewDepartmentEquipmentList>()
                      .Where(x => x.DepartmentName == departName);
            }

            ShowQueryWindow(title, description, parameters, columnSelector, queryExecutor);
        }
        [RelayCommand]
        private void ShowObjectEquipmentQuery()
        {
            if (_dbContext == null) return;

            string title = "Техника объекта";
            string description = "Запрос №6 Получения перечня строительной техники, используемой на объекте";
            var parameters = new List<QueryParameterInfo>
            {
                new ()
                {
                    Name = "Объект",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.Objects.OrderBy(o=>o.ObjectName).Select(o=>o.ObjectName)]
                },
                new ()
                {
                    Name = "Вся техника объекта",
                    ParameterType = typeof(bool),
                    DefaultValue = false,
                },
                new ()
                {
                    Name = "Начало периода",
                    ParameterType = typeof(DateTimeOffset),
                    DefaultValue = DateTimeOffset.Now,
                },
                new ()
                {
                    Name = "Конец периода",
                    ParameterType = typeof(DateTimeOffset),
                    DefaultValue = DateTimeOffset.Now,
                }
            };
            var columnSelector = new Func<Dictionary<string, object>, List<DataGridColumnInfo>>(
                parameter =>
                {
                    return
                    [
                        new() { PropertyName = nameof(ViewObjectEquipment.ObjectName), Header = "Объект" },
                        new() { PropertyName = nameof(ViewObjectEquipment.EquipmentName), Header = "Техника" },
                        new() { PropertyName = nameof(ViewObjectEquipment.AssignedQuantity), Header = "Выделеное кол-во" },
                        new() { PropertyName = nameof(ViewObjectEquipment.AssignmentDate), Header = "Дата выдачи", FormatString = "dd.MM.yyyy" },
                        new() { PropertyName = nameof(ViewObjectEquipment.ReturnDate), Header = "Дата конца", FormatString = "dd.MM.yyyy" },
                        new() { PropertyName = nameof(ViewObjectEquipment.AssignmentStatus), Header = "Статус техники" },
                    ];
                });
            static IQueryable<ViewObjectEquipment> queryExecutor(ConstructionOrganizationContext db, Dictionary<string, object> parameters)
            {
                var objectName = (string)parameters["Объект"];
                var isNotPeriod = (bool)parameters["Вся техника объекта"];
                var startPeriod = DateOnly.FromDateTime(((DateTimeOffset)parameters["Начало периода"]).DateTime);
                var endPeriod = DateOnly.FromDateTime(((DateTimeOffset)parameters["Конец периода"]).DateTime);

                return db.Set<ViewObjectEquipment>()
                         .Where(x => x.ObjectName == objectName)
                         .Where(x => isNotPeriod ||
                               (x.AssignmentDate != null &&
                                x.AssignmentDate >= startPeriod &&
                                x.AssignmentDate <= endPeriod));
            }

            ShowQueryWindow(title, description, parameters, columnSelector, queryExecutor);
        }
        [RelayCommand]
        private void ShowObjectScheduleQuery()
        {
            if (_dbContext == null) return;

            string title = "График и смета на строительство объекта";
            string description = "Запрос №7 Получение графика или сметы для указанного объекта";
            var parameters = new List<QueryParameterInfo>
            {
                new ()
                {
                    Name = "Название объекта",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.Objects.OrderBy(o=>o.ObjectName).Select(o=>o.ObjectName)]
                },
                new()
                {
                    Name = "Тип данных",
                    ParameterType = typeof(string),
                    DefaultValue = "График",
                    AvailableValues = ["График", "Смета"]
                }
            };
            var columnSelector = new Func<Dictionary<string, object>, List<DataGridColumnInfo>>(
                parameter =>
                {
                    var dataType = (string)parameter["Тип данных"];
                    return dataType switch
                    {
                        "График" =>
                        [
                            new() { PropertyName = nameof(ViewObjectScheduleAndEstimate.ObjectName), Header = "Объект" },
                            new() { PropertyName = nameof(ViewObjectScheduleAndEstimate.Description), Header = "Вид работ" },
                            new() { PropertyName = nameof(ViewObjectScheduleAndEstimate.PlannedStartDate), Header = "Плановая дата начала", FormatString = "dd.MM.yyyy" },
                            new() { PropertyName = nameof(ViewObjectScheduleAndEstimate.PlannedEndDate), Header = "Плановая дата конца", FormatString = "dd.MM.yyyy" },
                            new() { PropertyName = nameof(ViewObjectScheduleAndEstimate.ActualStartDate), Header = "Фактическая дата начала", FormatString = "dd.MM.yyyy" },
                            new() { PropertyName = nameof(ViewObjectScheduleAndEstimate.ActualEndDate), Header = "Фактическая дата конца", FormatString = "dd.MM.yyyy" },
                            new() { PropertyName = nameof(ViewObjectScheduleAndEstimate.AssignedTeam), Header = "Назначенная бригада" },
                            new() { PropertyName = nameof(ViewObjectScheduleAndEstimate.Status), Header = "Статус работ" }
                        ],
                        "Смета" =>
                        [
                            new() { PropertyName = nameof(ViewObjectScheduleAndEstimate.ObjectName), Header = "Объект" },
                            new() { PropertyName = nameof(ViewObjectScheduleAndEstimate.MaterialName), Header = "Материал" },
                            new() { PropertyName = nameof(ViewObjectScheduleAndEstimate.UnitOfMeasure), Header = "Ед. измерения" },
                            new() { PropertyName = nameof(ViewObjectScheduleAndEstimate.PlannedQuantity), Header = "Плановое кол-во", FormatString = "N2" },
                            new() { PropertyName = nameof(ViewObjectScheduleAndEstimate.ActualQuantity), Header = "Фактическое кол-во", FormatString = "N2" },
                            new() { PropertyName = nameof(ViewObjectScheduleAndEstimate.UnitPrice), Header = "Цена за ед.", FormatString = "C2" },
                            new() { PropertyName = nameof(ViewObjectScheduleAndEstimate.TotalCost), Header = "Общая цена", FormatString = "C2" }
                        ],
                        _ => throw new ArgumentException("Неизвестный тип данных")
                    };
                });
            static IQueryable<ViewObjectScheduleAndEstimate> queryExecutor(ConstructionOrganizationContext db, Dictionary<string, object> parameters)
            {
                var objectName = (string)parameters["Название объекта"];
                var dataType = (string)parameters["Тип данных"];

                return dataType switch
                {
                    "График" => db.Set<ViewObjectScheduleAndEstimate>()
                                .Where(x => x.ObjectName.Contains(objectName) &&
                                EF.Functions.Collate(x.RecordType, "utf8mb4_unicode_ci").Contains("work_schedule")),
                    "Смета" => db.Set<ViewObjectScheduleAndEstimate>()
                               .Where(x => x.ObjectName.Contains(objectName) &&
                               EF.Functions.Collate(x.RecordType, "utf8mb4_unicode_ci").Contains("estimate")),
                    _ => throw new ArgumentException("Неизвестный тип данных")
                };
            }

            ShowQueryWindow(title, description, parameters, columnSelector, queryExecutor);
        }
        [RelayCommand]
        private void ShowObjectReportQuery()
        {
            if (_dbContext == null) return;

            string title = "Отчет о строительстве объекта";
            string description = "Запрос №8 Получение отчета о сооружении указанного объекта";
            var parameters = new List<QueryParameterInfo>
            {
                new ()
                {
                    Name = "Название объекта",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.Objects.Where(o => o.ObjectStatus == "completed" || o.ObjectStatus == "terminated")
                                                           .OrderBy(o=>o.ObjectName).Select(o=>o.ObjectName)]
                },
                new()
                {
                    Name = "Тип данных",
                    ParameterType = typeof(string),
                    DefaultValue = "Сроки выполнения работ",
                    AvailableValues = ["Сроки выполнения работ", "Расход материалов"]
                }
            };
            var columnSelector = new Func<Dictionary<string, object>, List<DataGridColumnInfo>>(
                parameter =>
                {
                    var dataType = (string)parameter["Тип данных"];
                    return dataType switch
                    {
                        "Сроки выполнения работ" =>
                        [
                            new() { PropertyName = nameof(ViewObjectConstructionReport.ObjectName), Header = "Объект" },
                            new() { PropertyName = nameof(ViewObjectConstructionReport.ItemName), Header = "Вид работ" },
                            new() { PropertyName = nameof(ViewObjectConstructionReport.ActualStartDate), Header = "Дата начала", FormatString = "dd.MM.yyyy" },
                            new() { PropertyName = nameof(ViewObjectConstructionReport.ActualEndDate), Header = "Дата конца", FormatString = "dd.MM.yyyy" },
                            new() { PropertyName = nameof(ViewObjectConstructionReport.BrigadeName), Header = "Бригада" }
                        ],
                        "Расход материалов" =>
                        [
                            new() { PropertyName = nameof(ViewObjectConstructionReport.ObjectName), Header = "Объект" },
                            new() { PropertyName = nameof(ViewObjectConstructionReport.ItemName), Header = "Материал" },
                            new() { PropertyName = nameof(ViewObjectConstructionReport.UnitOfMeasure), Header = "Ед. измерения" },
                            new() { PropertyName = nameof(ViewObjectConstructionReport.ActualQuantity), Header = "Кол-во", FormatString = "N2" },
                            new() { PropertyName = nameof(ViewObjectConstructionReport.UnitPrice), Header = "Цена за ед.", FormatString = "C2" },
                            new() { PropertyName = nameof(ViewObjectConstructionReport.ActualCost), Header = "Общая цена", FormatString = "C2" }
                        ],
                        _ => throw new ArgumentException("Неизвестный тип данных")
                    };
                });
            static IQueryable<ViewObjectConstructionReport> queryExecutor(ConstructionOrganizationContext db, Dictionary<string, object> parameters)
            {
                var objectName = (string)parameters["Название объекта"];
                var dataType = (string)parameters["Тип данных"];

                return dataType switch
                {
                    "Сроки выполнения работ" => db.Set<ViewObjectConstructionReport>()
                                .Where(x => x.ObjectName.Contains(objectName) &&
                                EF.Functions.Collate(x.RecordType, "utf8mb4_unicode_ci").Contains("work")),
                    "Расход материалов" => db.Set<ViewObjectConstructionReport>()
                               .Where(x => x.ObjectName.Contains(objectName) &&
                               EF.Functions.Collate(x.RecordType, "utf8mb4_unicode_ci").Contains("material")),
                    _ => throw new ArgumentException("Неизвестный тип данных")
                };
            }

            ShowQueryWindow(title, description, parameters, columnSelector, queryExecutor);
        }
        [RelayCommand]
        private void ShowObjectsWithWorkTypesQuery()
        {
            if (_dbContext == null) return;

            string title = "Объекты, на которых проводились указанные работы";
            string description = "Запрос №9 Получить перечень объектов строительного управления или в целом по организации, на которых проводился указанный вид работ";
            var parameters = new List<QueryParameterInfo>
            {
                new ()
                {
                    Name = "Тип работ",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.WorkTypes.OrderBy(wt=>wt.WorkTypeName).Select(wt=>wt.WorkTypeName)]
                },
                new ()
                {
                    Name = "Начало периода",
                    ParameterType = typeof(DateTimeOffset),
                    DefaultValue = DateTimeOffset.Now,
                },
                new ()
                {
                    Name = "Конец периода",
                    ParameterType = typeof(DateTimeOffset),
                    DefaultValue = DateTimeOffset.Now,
                },
                new ()
                {
                    Name = "Искать по всей организации",
                    ParameterType = typeof(bool),
                    DefaultValue = false
                },
                new()
                {
                    Name = "Управление",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.ConstructionDepartments.OrderBy(cd=>cd.DepartmentName).Select(cd=>cd.DepartmentName)]
                }
            };
            var columnSelector = new Func<Dictionary<string, object>, List<DataGridColumnInfo>>(
                parameter =>
                {
                    var isAllOrg = (bool)parameter["Искать по всей организации"];
                    var columnList = new List<DataGridColumnInfo>
                    {
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.WorkTypeName), Header = "Вид работ" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.ObjectName), Header = "Объект" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.ActualStartDate), Header = "Дата начала", FormatString = "dd.MM.yyyy" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.ActualEndDate), Header = "Дата конца", FormatString = "dd.MM.yyyy" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.BrigadeName), Header = "Бригада" }
                    };
                    if (!isAllOrg)
                    {
                        columnList.Insert(1, new() { PropertyName = nameof(ViewConstructionWorkAnalysis.DepartmentName), Header = "Управление" });
                    }
                    return columnList;
                });
            static IQueryable<ViewConstructionWorkAnalysis> queryExecutor(ConstructionOrganizationContext db, Dictionary<string, object> parameters)
            {
                var isAllOrg = (bool)parameters["Искать по всей организации"];
                var workTypeName = (string)parameters["Тип работ"];
                var startPeriod = DateOnly.FromDateTime(((DateTimeOffset)parameters["Начало периода"]).DateTime);
                var endPeriod = DateOnly.FromDateTime(((DateTimeOffset)parameters["Конец периода"]).DateTime);
                var departmentName = (string)parameters["Управление"];

                return db.Set<ViewConstructionWorkAnalysis>()
                                .Where(x => x.WorkTypeName == workTypeName &&
                                       x.ActualStartDate >= startPeriod &&
                                       x.ActualStartDate <= endPeriod)
                                .Where(x => isAllOrg ||
                                       x.DepartmentName == departmentName);
            }

            ShowQueryWindow(title, description, parameters, columnSelector, queryExecutor);
        }
        [RelayCommand]
        private void ShowWorkTypeWithDelayQuery()
        {
            if (_dbContext == null) return;

            string title = "Виды работ, по которым имело место превышение сроков";
            string description = "Запрос №10 Получить перечень видов работ по котором были превышены сроки на указанном участке, управлении или в целом по организации";
            var parameters = new List<QueryParameterInfo>
            {
                new ()
                {
                    Name = "Тип данных",
                    ParameterType = typeof(string),
                    DefaultValue = "Вся организация",
                    AvailableValues = ["Строительное управление", "Участок", "Вся организация"]
                },
                new()
                {
                    Name = "Управление",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.ConstructionDepartments.OrderBy(cd=>cd.DepartmentName).Select(cd=>cd.DepartmentName)]
                },
                new()
                {
                    Name = "Участок",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.Sites.OrderBy(s=>s.SiteName).Select(s=>s.SiteName)]
                }
            };
            var columnSelector = new Func<Dictionary<string, object>, List<DataGridColumnInfo>>(
                parameter =>
                {
                    var dataType = (string)parameter["Тип данных"];
                    var columnList = new List<DataGridColumnInfo>
                    {
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.WorkTypeName), Header = "Вид работ" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.PlannedStartDate), Header = "Плановая дата начала", FormatString = "dd.MM.yyyy" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.PlannedEndDate), Header = "Плановая дата конца", FormatString = "dd.MM.yyyy" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.DelayDays), Header = "Задержка (дни)" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.WorkStatus), Header = "Статус работ" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.ObjectName), Header = "Объект" }
                    };
                    switch (dataType)
                    {
                        case "Строительное управление":
                            columnList.Insert(0, new() { PropertyName = nameof(ViewConstructionWorkAnalysis.DepartmentName), Header = "Управление" });
                            break;
                        case "Участок":
                            columnList.Insert(0, new() { PropertyName = nameof(ViewConstructionWorkAnalysis.SiteName), Header = "Участок" });
                            break;
                    }
                    return columnList;
                });
            static IQueryable<ViewConstructionWorkAnalysis> queryExecutor(ConstructionOrganizationContext db, Dictionary<string, object> parameters)
            {
                var dataType = (string)parameters["Тип данных"];
                var siteName = (string)parameters["Участок"];
                var departmentName = (string)parameters["Управление"];

                return db.Set<ViewConstructionWorkAnalysis>()
                            .Where(x => x.DelayDays > 0)
                            .Where(x => dataType == "Вся организация" ||
                                       (dataType == "Строительное управление" && x.DepartmentName == departmentName) ||
                                       (dataType == "Участок" && x.SiteName == siteName));
            }

            ShowQueryWindow(title, description, parameters, columnSelector, queryExecutor);
        }
        [RelayCommand]
        private void ShowMaterialWithOverEstimateQuery()
        {
            if (_dbContext == null) return;

            string title = "Материалы, по которым было превышение сметы";
            string description = "Запрос №11 Получить перечень материалов по котором было превышение по сметке на указанном участке, управлении или в целом по организации";
            var parameters = new List<QueryParameterInfo>
            {
                new ()
                {
                    Name = "Тип данных",
                    ParameterType = typeof(string),
                    DefaultValue = "Вся организация",
                    AvailableValues = ["Строительное управление", "Участок", "Вся организация"]
                },
                new()
                {
                    Name = "Управление",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.ConstructionDepartments.OrderBy(cd=>cd.DepartmentName).Select(cd=>cd.DepartmentName)]
                },
                new()
                {
                    Name = "Участок",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.Sites.OrderBy(s=>s.SiteName).Select(s=>s.SiteName)]
                }
            };
            var columnSelector = new Func<Dictionary<string, object>, List<DataGridColumnInfo>>(
                parameter =>
                {
                    var dataType = (string)parameter["Тип данных"];
                    var columnList = new List<DataGridColumnInfo>
                    {
                        new() { PropertyName = nameof(ViewMaterialReport.MaterialName), Header = "Материал" },
                        new() { PropertyName = nameof(ViewMaterialReport.UnitOfMeasure), Header = "Ед. измерения" },
                        new() { PropertyName = nameof(ViewMaterialReport.PlannedMaterialQuantity), Header = "Плановое кол-во" },
                        new() { PropertyName = nameof(ViewMaterialReport.ActualMaterialQuantity), Header = "Фактическое кол-во" },
                        new() { PropertyName = nameof(ViewMaterialReport.OverbudgetPercent), Header = "Превышение (%)" },
                        new() { PropertyName = nameof(ViewMaterialReport.ObjectName), Header = "Объект" }
                    };
                    switch (dataType)
                    {
                        case "Строительное управление":
                            columnList.Insert(0, new() { PropertyName = nameof(ViewMaterialReport.DepartmentName), Header = "Управление" });
                            break;
                        case "Участок":
                            columnList.Insert(0, new() { PropertyName = nameof(ViewMaterialReport.SiteName), Header = "Участок" });
                            break;
                    }
                    return columnList;
                });
            static IQueryable<ViewMaterialReport> queryExecutor(ConstructionOrganizationContext db, Dictionary<string, object> parameters)
            {
                var dataType = (string)parameters["Тип данных"];
                var siteName = (string)parameters["Участок"];
                var departmentName = (string)parameters["Управление"];

                return db.Set<ViewMaterialReport>()
                            .Where(x => x.OverbudgetPercent > 0)
                            .Where(x => dataType == "Вся организация" ||
                                       (dataType == "Строительное управление" && x.DepartmentName == departmentName) ||
                                       (dataType == "Участок" && x.SiteName == siteName));
            }

            ShowQueryWindow(title, description, parameters, columnSelector, queryExecutor);
        }
        [RelayCommand]
        private void ShowWorkTypeBrigadeQuery()
        {
            if (_dbContext == null) return;

            string title = "Виды работ, выполненных указанной бригадой";
            string description = "Запрос №12 Получить перечень видов работ, выполненных указанной бригадой в установленные сроки";
            var parameters = new List<QueryParameterInfo>
            {
                new()
                {
                    Name = "Бригада",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.Brigades.OrderBy(b=>b.BrigadeName).Select(b=>b.BrigadeName)]
                },
                new ()
                {
                    Name = "Начало периода",
                    ParameterType = typeof(DateTimeOffset),
                    DefaultValue = DateTimeOffset.Now,
                },
                new ()
                {
                    Name = "Конец периода",
                    ParameterType = typeof(DateTimeOffset),
                    DefaultValue = DateTimeOffset.Now,
                }
            };
            var columnSelector = new Func<Dictionary<string, object>, List<DataGridColumnInfo>>(
                parameter =>
                {
                    return
                    [
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.BrigadeName), Header = "Бригада" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.WorkTypeName), Header = "Вид работ" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.ActualStartDate), Header = "Дата начала", FormatString = "dd.MM.yyyy" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.ActualEndDate), Header = "Дата конца", FormatString = "dd.MM.yyyy" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.WorkStatus), Header = "Статус работ" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.ObjectName), Header = "Объект" },
                    ];
                });
            static IQueryable<ViewConstructionWorkAnalysis> queryExecutor(ConstructionOrganizationContext db, Dictionary<string, object> parameters)
            {
                var brigadeName = (string)parameters["Бригада"];
                var startPeriod = DateOnly.FromDateTime(((DateTimeOffset)parameters["Начало периода"]).DateTime);
                var endPeriod = DateOnly.FromDateTime(((DateTimeOffset)parameters["Конец периода"]).DateTime);

                return db.Set<ViewConstructionWorkAnalysis>()
                            .Where(x => x.BrigadeName == brigadeName &&
                                   x.ActualStartDate >= startPeriod &&
                                   x.ActualStartDate <= endPeriod);
            }

            ShowQueryWindow(title, description, parameters, columnSelector, queryExecutor);
        }
        [RelayCommand]
        private void ShowBrigadeByWorkTypeQuery()
        {
            if (_dbContext == null) return;

            string title = "Бригады, выполнявшие указанный вид работ";
            string description = "Запрос №13 Получить перечень бригад, выполнявших указанный вид работ в установленные сроки";
            var parameters = new List<QueryParameterInfo>
            {
                new()
                {
                    Name = "Вид работ",
                    ParameterType = typeof(string),
                    DefaultValue = "",
                    AvailableValues = [.._dbContext.WorkTypes.OrderBy(wt=>wt.WorkTypeName).Select(wt=>wt.WorkTypeName)]
                },
                new ()
                {
                    Name = "Начало периода",
                    ParameterType = typeof(DateTimeOffset),
                    DefaultValue = DateTimeOffset.Now,
                },
                new ()
                {
                    Name = "Конец периода",
                    ParameterType = typeof(DateTimeOffset),
                    DefaultValue = DateTimeOffset.Now,
                }
            };
            var columnSelector = new Func<Dictionary<string, object>, List<DataGridColumnInfo>>(
                parameter =>
                {
                    return
                    [
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.WorkTypeName), Header = "Вид работ" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.BrigadeName), Header = "Бригада" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.ActualStartDate), Header = "Дата начала", FormatString = "dd.MM.yyyy" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.ActualEndDate), Header = "Дата конца", FormatString = "dd.MM.yyyy" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.WorkStatus), Header = "Статус работ" },
                        new() { PropertyName = nameof(ViewConstructionWorkAnalysis.ObjectName), Header = "Объект" },
                    ];
                });
            static IQueryable<ViewConstructionWorkAnalysis> queryExecutor(ConstructionOrganizationContext db, Dictionary<string, object> parameters)
            {
                var workTypeName = (string)parameters["Вид работ"];
                var startPeriod = DateOnly.FromDateTime(((DateTimeOffset)parameters["Начало периода"]).DateTime);
                var endPeriod = DateOnly.FromDateTime(((DateTimeOffset)parameters["Конец периода"]).DateTime);

                return db.Set<ViewConstructionWorkAnalysis>()
                            .Where(x => x.WorkTypeName == workTypeName &&
                                   x.ActualStartDate >= startPeriod &&
                                   x.ActualStartDate <= endPeriod);
            }

            ShowQueryWindow(title, description, parameters, columnSelector, queryExecutor);
        }
    }
}
