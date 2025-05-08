using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace constructionOrgManagement.Models;

public partial class ConstructionOrganizationContext : DbContext
{
    public ConstructionOrganizationContext()
    {
    }

    public ConstructionOrganizationContext(DbContextOptions<ConstructionOrganizationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brigade> Brigades { get; set; }

    public virtual DbSet<BuildingMaterial> BuildingMaterials { get; set; }

    public virtual DbSet<ConstructionDepartment> ConstructionDepartments { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<DepartmentEquipment> DepartmentEquipments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeAttribute> EmployeeAttributes { get; set; }

    public virtual DbSet<EmployeeCategory> EmployeeCategories { get; set; }

    public virtual DbSet<Estimate> Estimates { get; set; }

    public virtual DbSet<Object> Objects { get; set; }

    public virtual DbSet<ObjectCategory> ObjectCategories { get; set; }

    public virtual DbSet<ObjectCharacteristic> ObjectCharacteristics { get; set; }

    public virtual DbSet<ObjectEquipment> ObjectEquipments { get; set; }

    public virtual DbSet<OrganizationEquipment> OrganizationEquipments { get; set; }

    public virtual DbSet<Site> Sites { get; set; }

    public virtual DbSet<SpecificEmployeeAttribute> SpecificEmployeeAttributes { get; set; }

    public virtual DbSet<SpecificObjectCharacteristic> SpecificObjectCharacteristics { get; set; }

    public virtual DbSet<ViewConstructionWorkAnalysis> ViewConstructionWorkAnalyses { get; set; }

    public virtual DbSet<ViewDepartmentAndSitesObjectWithSchedule> ViewDepartmentAndSitesObjectWithSchedules { get; set; }

    public virtual DbSet<ViewDepartmentAndSitesWithLeader> ViewDepartmentAndSitesWithLeaders { get; set; }

    public virtual DbSet<ViewDepartmentEquipmentList> ViewDepartmentEquipmentLists { get; set; }

    public virtual DbSet<ViewEngineeringStaffByDepartment> ViewEngineeringStaffByDepartments { get; set; }

    public virtual DbSet<ViewMaterialReport> ViewMaterialReports { get; set; }

    public virtual DbSet<ViewObjectBrigade> ViewObjectBrigades { get; set; }

    public virtual DbSet<ViewObjectConstructionReport> ViewObjectConstructionReports { get; set; }

    public virtual DbSet<ViewObjectEquipment> ViewObjectEquipments { get; set; }

    public virtual DbSet<ViewObjectScheduleAndEstimate> ViewObjectScheduleAndEstimates { get; set; }

    public virtual DbSet<WorkSchedule> WorkSchedules { get; set; }

    public virtual DbSet<WorkType> WorkTypes { get; set; }

    public virtual DbSet<WorkTypeForCategory> WorkTypeForCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Brigade>(entity =>
        {
            entity.HasKey(e => e.BrigadeId).HasName("PRIMARY");

            entity.ToTable("brigade");

            entity.HasIndex(e => e.BrigadierId, "brigade_ibfk_1");

            entity.HasIndex(e => e.BrigadeName, "index_brigade_name");

            entity.Property(e => e.BrigadeId).HasColumnName("brigade_id");
            entity.Property(e => e.BrigadeName)
                .HasMaxLength(100)
                .HasColumnName("brigade_name");
            entity.Property(e => e.BrigadeStatus)
                .HasDefaultValueSql("'active'")
                .HasColumnType("enum('active','terminated')")
                .HasColumnName("brigade_status");
            entity.Property(e => e.BrigadierId).HasColumnName("brigadier_id");

            entity.HasOne(d => d.Brigadier).WithMany(p => p.Brigades)
                .HasForeignKey(d => d.BrigadierId)
                .HasConstraintName("brigade_ibfk_1");
        });

        modelBuilder.Entity<BuildingMaterial>(entity =>
        {
            entity.HasKey(e => e.BuildingMaterialId).HasName("PRIMARY");

            entity.ToTable("building_material");

            entity.Property(e => e.BuildingMaterialId).HasColumnName("building_material_id");
            entity.Property(e => e.MaterialName)
                .HasMaxLength(100)
                .HasColumnName("material_name");
            entity.Property(e => e.UnitOfMeasure)
                .HasMaxLength(20)
                .HasColumnName("unit_of_measure");
        });

        modelBuilder.Entity<ConstructionDepartment>(entity =>
        {
            entity.HasKey(e => e.ConstructionDepartmentId).HasName("PRIMARY");

            entity.ToTable("construction_department");

            entity.HasIndex(e => e.DepartmentSupervisorId, "construction_department_ibfk_1");

            entity.HasIndex(e => e.DepartmentName, "index_department_name");

            entity.Property(e => e.ConstructionDepartmentId).HasColumnName("construction_department_id");
            entity.Property(e => e.DepartmentLocation)
                .HasMaxLength(150)
                .HasColumnName("department_location");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .HasColumnName("department_name");
            entity.Property(e => e.DepartmentSupervisorId).HasColumnName("department_supervisor_id");

            entity.HasOne(d => d.DepartmentSupervisor).WithMany(p => p.ConstructionDepartments)
                .HasForeignKey(d => d.DepartmentSupervisorId)
                .HasConstraintName("construction_department_ibfk_1");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PRIMARY");

            entity.ToTable("contract");

            entity.HasIndex(e => e.ContractCustomerId, "contract_customer_id");

            entity.Property(e => e.ContractId).HasColumnName("contract_id");
            entity.Property(e => e.ContractCustomerId).HasColumnName("contract_customer_id");
            entity.Property(e => e.ContractName)
                .HasColumnType("text")
                .HasColumnName("contract_name");
            entity.Property(e => e.ContractStatus)
                .HasDefaultValueSql("'active'")
                .HasColumnType("enum('active','completed','terminated')")
                .HasColumnName("contract_status");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(15, 2)
                .HasColumnName("total_amount");

            entity.HasOne(d => d.ContractCustomer).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.ContractCustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contract_ibfk_1");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PRIMARY");

            entity.ToTable("customer");

            entity.HasIndex(e => e.CustomerName, "index_customer_name");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(100)
                .HasColumnName("customer_name");
        });

        modelBuilder.Entity<DepartmentEquipment>(entity =>
        {
            entity.HasKey(e => e.DepartmentEquipmentId).HasName("PRIMARY");

            entity.ToTable("department_equipment");

            entity.HasIndex(e => e.DepartmentId, "department_id");

            entity.HasIndex(e => e.OrgEquipmentId, "org_equipment_id");

            entity.Property(e => e.DepartmentEquipmentId).HasColumnName("department_equipment_id");
            entity.Property(e => e.DepartEquipmentQuantity).HasColumnName("depart_equipment_quantity");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.OrgEquipmentId).HasColumnName("org_equipment_id");

            entity.HasOne(d => d.Department).WithMany(p => p.DepartmentEquipments)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("department_equipment_ibfk_2");

            entity.HasOne(d => d.OrgEquipment).WithMany(p => p.DepartmentEquipments)
                .HasForeignKey(d => d.OrgEquipmentId)
                .HasConstraintName("department_equipment_ibfk_1");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PRIMARY");

            entity.ToTable("employee");

            entity.HasIndex(e => e.EmplCategoryId, "empl_category_id");

            entity.HasIndex(e => new { e.Surname, e.Name, e.Patronymic }, "index_full_name");

            entity.HasIndex(e => new { e.Surname, e.Name }, "index_surname_name");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(20)
                .HasColumnName("contact_number");
            entity.Property(e => e.Education)
                .HasDefaultValueSql("'Среднее специальное'")
                .HasColumnType("enum('Среднее специальное','Высшее')")
                .HasColumnName("education");
            entity.Property(e => e.EmplCategoryId).HasColumnName("empl_category_id");
            entity.Property(e => e.FireDate).HasColumnName("fire_date");
            entity.Property(e => e.HireDate).HasColumnName("hire_date");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .HasColumnName("patronymic");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .HasColumnName("surname");

            entity.HasOne(d => d.EmplCategory).WithMany(p => p.Employees)
                .HasForeignKey(d => d.EmplCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_ibfk_1");

            entity.HasMany(d => d.BcBrigades).WithMany(p => p.Workers)
                .UsingEntity<Dictionary<string, object>>(
                    "BrigadeComposition",
                    r => r.HasOne<Brigade>().WithMany()
                        .HasForeignKey("BcBrigadeId")
                        .HasConstraintName("brigade_composition_ibfk_2"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("WorkerId")
                        .HasConstraintName("brigade_composition_ibfk_1"),
                    j =>
                    {
                        j.HasKey("WorkerId", "BcBrigadeId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("brigade_composition");
                        j.HasIndex(new[] { "BcBrigadeId" }, "bc_brigade_id");
                        j.IndexerProperty<int>("WorkerId").HasColumnName("worker_id");
                        j.IndexerProperty<int>("BcBrigadeId").HasColumnName("bc_brigade_id");
                    });
        });

        modelBuilder.Entity<EmployeeAttribute>(entity =>
        {
            entity.HasKey(e => e.EmployeeAttributeId).HasName("PRIMARY");

            entity.ToTable("employee_attribute");

            entity.Property(e => e.EmployeeAttributeId).HasColumnName("employee_attribute_id");
            entity.Property(e => e.AttributeName)
                .HasMaxLength(100)
                .HasColumnName("attribute_name");
        });

        modelBuilder.Entity<EmployeeCategory>(entity =>
        {
            entity.HasKey(e => e.EmployeeCategoryId).HasName("PRIMARY");

            entity.ToTable("employee_category");

            entity.Property(e => e.EmployeeCategoryId).HasColumnName("employee_category_id");
            entity.Property(e => e.CategoryType)
                .HasDefaultValueSql("'Рабочие'")
                .HasColumnType("enum('Рабочие','Инженерно-технический персонал')")
                .HasColumnName("category_type");
            entity.Property(e => e.EmplCategoryName)
                .HasMaxLength(50)
                .HasColumnName("empl_category_name");
        });

        modelBuilder.Entity<Estimate>(entity =>
        {
            entity.HasKey(e => new { e.MaterialId, e.EstimateObjectId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("estimate");

            entity.HasIndex(e => e.EstimateObjectId, "estimate_object_id");

            entity.Property(e => e.MaterialId).HasColumnName("material_id");
            entity.Property(e => e.EstimateObjectId).HasColumnName("estimate_object_id");
            entity.Property(e => e.ActualMaterialQuantity)
                .HasPrecision(10, 2)
                .HasColumnName("actual_material_quantity");
            entity.Property(e => e.PlannedMaterialQuantity)
                .HasPrecision(10, 2)
                .HasColumnName("planned_material_quantity");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.EstimateObject).WithMany(p => p.Estimates)
                .HasForeignKey(d => d.EstimateObjectId)
                .HasConstraintName("estimate_ibfk_1");

            entity.HasOne(d => d.Material).WithMany(p => p.Estimates)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("estimate_ibfk_2");
        });

        modelBuilder.Entity<Object>(entity =>
        {
            entity.HasKey(e => e.ObjectId).HasName("PRIMARY");

            entity.ToTable("object");

            entity.HasIndex(e => e.CategoryId, "category_id");

            entity.HasIndex(e => e.ForemanId, "foreman_id");

            entity.HasIndex(e => e.ObjectName, "index_object_name");

            entity.HasIndex(e => e.ObjectContractId, "object_contract_id");

            entity.HasIndex(e => e.ObjectSiteId, "object_site_id");

            entity.Property(e => e.ObjectId).HasColumnName("object_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.ForemanId).HasColumnName("foreman_id");
            entity.Property(e => e.ObjectContractId).HasColumnName("object_contract_id");
            entity.Property(e => e.ObjectLocation)
                .HasMaxLength(150)
                .HasColumnName("object_location");
            entity.Property(e => e.ObjectName)
                .HasMaxLength(100)
                .HasColumnName("object_name");
            entity.Property(e => e.ObjectSiteId).HasColumnName("object_site_id");
            entity.Property(e => e.ObjectStatus)
                .HasDefaultValueSql("'in_planning'")
                .HasColumnType("enum('in_planning','in_progress','completed','terminated')")
                .HasColumnName("object_status");

            entity.HasOne(d => d.Category).WithMany(p => p.Objects)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("object_ibfk_2");

            entity.HasOne(d => d.Foreman).WithMany(p => p.Objects)
                .HasForeignKey(d => d.ForemanId)
                .HasConstraintName("object_ibfk_1");

            entity.HasOne(d => d.ObjectContract).WithMany(p => p.Objects)
                .HasForeignKey(d => d.ObjectContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("object_ibfk_3");

            entity.HasOne(d => d.ObjectSite).WithMany(p => p.Objects)
                .HasForeignKey(d => d.ObjectSiteId)
                .HasConstraintName("object_ibfk_4");

            entity.HasMany(d => d.MasterEmployees).WithMany(p => p.MasterObjects)
                .UsingEntity<Dictionary<string, object>>(
                    "ObjectMaster",
                    r => r.HasOne<Employee>().WithMany()
                        .HasForeignKey("MasterEmployeeId")
                        .HasConstraintName("object_master_ibfk_2"),
                    l => l.HasOne<Object>().WithMany()
                        .HasForeignKey("MasterObjectId")
                        .HasConstraintName("object_master_ibfk_1"),
                    j =>
                    {
                        j.HasKey("MasterObjectId", "MasterEmployeeId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("object_master");
                        j.HasIndex(new[] { "MasterEmployeeId" }, "master_employee_id");
                        j.IndexerProperty<int>("MasterObjectId").HasColumnName("master_object_id");
                        j.IndexerProperty<int>("MasterEmployeeId").HasColumnName("master_employee_id");
                    });
        });

        modelBuilder.Entity<ObjectCategory>(entity =>
        {
            entity.HasKey(e => e.ObjectCategoryId).HasName("PRIMARY");

            entity.ToTable("object_category");

            entity.Property(e => e.ObjectCategoryId).HasColumnName("object_category_id");
            entity.Property(e => e.ObjCategoryName)
                .HasMaxLength(50)
                .HasColumnName("obj_category_name");
        });

        modelBuilder.Entity<ObjectCharacteristic>(entity =>
        {
            entity.HasKey(e => e.ObjectCharacteristicId).HasName("PRIMARY");

            entity.ToTable("object_characteristic");

            entity.Property(e => e.ObjectCharacteristicId).HasColumnName("object_characteristic_id");
            entity.Property(e => e.ObjCharacteristicName)
                .HasMaxLength(50)
                .HasColumnName("obj_characteristic_name");
        });

        modelBuilder.Entity<ObjectEquipment>(entity =>
        {
            entity.HasKey(e => new { e.EquipmentId, e.EquipmentForObjectId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("object_equipment");

            entity.HasIndex(e => e.EquipmentForObjectId, "equipment_for_object_id");

            entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
            entity.Property(e => e.EquipmentForObjectId).HasColumnName("equipment_for_object_id");
            entity.Property(e => e.AssignmentDate).HasColumnName("assignment_date");
            entity.Property(e => e.EquipObjectQuantity).HasColumnName("equip_object_quantity");
            entity.Property(e => e.ReturnDate).HasColumnName("return_date");

            entity.HasOne(d => d.EquipmentForObject).WithMany(p => p.ObjectEquipments)
                .HasForeignKey(d => d.EquipmentForObjectId)
                .HasConstraintName("object_equipment_ibfk_1");

            entity.HasOne(d => d.Equipment).WithMany(p => p.ObjectEquipments)
                .HasForeignKey(d => d.EquipmentId)
                .HasConstraintName("object_equipment_ibfk_2");
        });

        modelBuilder.Entity<OrganizationEquipment>(entity =>
        {
            entity.HasKey(e => e.OrganizationEquipmentId).HasName("PRIMARY");

            entity.ToTable("organization_equipment");

            entity.Property(e => e.OrganizationEquipmentId).HasColumnName("organization_equipment_id");
            entity.Property(e => e.EquipmentName)
                .HasMaxLength(100)
                .HasColumnName("equipment_name");
            entity.Property(e => e.OrgEquipmentQuantity).HasColumnName("org_equipment_quantity");
        });

        modelBuilder.Entity<Site>(entity =>
        {
            entity.HasKey(e => e.SiteId).HasName("PRIMARY");

            entity.ToTable("site");

            entity.HasIndex(e => e.SiteDepartmentId, "department_id");

            entity.HasIndex(e => e.SiteName, "index_site_name");

            entity.HasIndex(e => e.SiteSupervisorId, "site_supervisor_id");

            entity.Property(e => e.SiteId).HasColumnName("site_id");
            entity.Property(e => e.SiteDepartmentId).HasColumnName("site_department_id");
            entity.Property(e => e.SiteLocation)
                .HasMaxLength(150)
                .HasColumnName("site_location");
            entity.Property(e => e.SiteName)
                .HasMaxLength(100)
                .HasColumnName("site_name");
            entity.Property(e => e.SiteSupervisorId).HasColumnName("site_supervisor_id");

            entity.HasOne(d => d.SiteDepartment).WithMany(p => p.Sites)
                .HasForeignKey(d => d.SiteDepartmentId)
                .HasConstraintName("site_ibfk_2");

            entity.HasOne(d => d.SiteSupervisor).WithMany(p => p.Sites)
                .HasForeignKey(d => d.SiteSupervisorId)
                .HasConstraintName("site_ibfk_1");
        });

        modelBuilder.Entity<SpecificEmployeeAttribute>(entity =>
        {
            entity.HasKey(e => new { e.AttributeId, e.SpecificEmployeeId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("specific_employee_attribute");

            entity.HasIndex(e => e.SpecificEmployeeId, "specific_employee_id");

            entity.Property(e => e.AttributeId).HasColumnName("attribute_id");
            entity.Property(e => e.SpecificEmployeeId).HasColumnName("specific_employee_id");
            entity.Property(e => e.AttributeValue)
                .HasMaxLength(255)
                .HasColumnName("attribute_value");

            entity.HasOne(d => d.Attribute).WithMany(p => p.SpecificEmployeeAttributes)
                .HasForeignKey(d => d.AttributeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("specific_employee_attribute_ibfk_2");

            entity.HasOne(d => d.SpecificEmployee).WithMany(p => p.SpecificEmployeeAttributes)
                .HasForeignKey(d => d.SpecificEmployeeId)
                .HasConstraintName("specific_employee_attribute_ibfk_1");
        });

        modelBuilder.Entity<SpecificObjectCharacteristic>(entity =>
        {
            entity.HasKey(e => new { e.SpecificObjectId, e.CharacteristicId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("specific_object_characteristic");

            entity.HasIndex(e => e.CharacteristicId, "characteristic_id");

            entity.Property(e => e.SpecificObjectId).HasColumnName("specific_object_id");
            entity.Property(e => e.CharacteristicId).HasColumnName("characteristic_id");
            entity.Property(e => e.CharacteristicValue)
                .HasMaxLength(255)
                .HasColumnName("characteristic_value");

            entity.HasOne(d => d.Characteristic).WithMany(p => p.SpecificObjectCharacteristics)
                .HasForeignKey(d => d.CharacteristicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("specific_object_characteristic_ibfk_2");

            entity.HasOne(d => d.SpecificObject).WithMany(p => p.SpecificObjectCharacteristics)
                .HasForeignKey(d => d.SpecificObjectId)
                .HasConstraintName("specific_object_characteristic_ibfk_1");
        });

        modelBuilder.Entity<ViewConstructionWorkAnalysis>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_construction_work_analysis");

            entity.Property(e => e.ActualEndDate).HasColumnName("actual_end_date");
            entity.Property(e => e.ActualStartDate).HasColumnName("actual_start_date");
            entity.Property(e => e.BrigadeId)
                .HasDefaultValueSql("'0'")
                .HasColumnName("brigade_id");
            entity.Property(e => e.BrigadeName)
                .HasMaxLength(100)
                .HasColumnName("brigade_name");
            entity.Property(e => e.CurrentDelayDays).HasColumnName("current_delay_days");
            entity.Property(e => e.DeadlineStatus)
                .HasMaxLength(17)
                .HasDefaultValueSql("''")
                .HasColumnName("deadline_status")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.DelayDays).HasColumnName("delay_days");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .HasColumnName("department_name");
            entity.Property(e => e.ObjectCategory)
                .HasMaxLength(50)
                .HasColumnName("object_category");
            entity.Property(e => e.ObjectId).HasColumnName("object_id");
            entity.Property(e => e.ObjectName)
                .HasMaxLength(100)
                .HasColumnName("object_name");
            entity.Property(e => e.PlannedEndDate).HasColumnName("planned_end_date");
            entity.Property(e => e.PlannedStartDate).HasColumnName("planned_start_date");
            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.SiteId).HasColumnName("site_id");
            entity.Property(e => e.SiteName)
                .HasMaxLength(100)
                .HasColumnName("site_name");
            entity.Property(e => e.WorkStatus)
                .HasMaxLength(11)
                .HasDefaultValueSql("''")
                .HasColumnName("work_status")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.WorkTypeId).HasColumnName("work_type_id");
            entity.Property(e => e.WorkTypeName)
                .HasMaxLength(100)
                .HasColumnName("work_type_name");
        });

        modelBuilder.Entity<ViewDepartmentAndSitesObjectWithSchedule>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_department_and_sites_object_with_schedule");

            entity.Property(e => e.ActualEndDate).HasColumnName("actual_end_date");
            entity.Property(e => e.ActualStartDate).HasColumnName("actual_start_date");
            entity.Property(e => e.AssignedBrigade)
                .HasMaxLength(100)
                .HasColumnName("assigned_brigade");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .HasColumnName("department_name");
            entity.Property(e => e.ObjectCategory)
                .HasMaxLength(50)
                .HasColumnName("object_category");
            entity.Property(e => e.ObjectId).HasColumnName("object_id");
            entity.Property(e => e.ObjectLocation)
                .HasMaxLength(150)
                .HasColumnName("object_location");
            entity.Property(e => e.ObjectName)
                .HasMaxLength(100)
                .HasColumnName("object_name");
            entity.Property(e => e.ObjectStatus)
                .HasDefaultValueSql("'in_planning'")
                .HasColumnType("enum('in_planning','in_progress','completed','terminated')")
                .HasColumnName("object_status");
            entity.Property(e => e.PlannedEndDate).HasColumnName("planned_end_date");
            entity.Property(e => e.PlannedStartDate).HasColumnName("planned_start_date");
            entity.Property(e => e.ScheduleId)
                .HasDefaultValueSql("'0'")
                .HasColumnName("schedule_id");
            entity.Property(e => e.SiteId).HasColumnName("site_id");
            entity.Property(e => e.SiteName)
                .HasMaxLength(100)
                .HasColumnName("site_name");
            entity.Property(e => e.WorkStatus)
                .HasMaxLength(11)
                .HasDefaultValueSql("''")
                .HasColumnName("work_status")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.WorkTypeName)
                .HasMaxLength(100)
                .HasColumnName("work_type_name");
        });

        modelBuilder.Entity<ViewDepartmentAndSitesWithLeader>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_department_and_sites_with_leaders");

            entity.Property(e => e.EntityType)
                .HasMaxLength(10)
                .HasDefaultValueSql("''")
                .HasColumnName("entity_type")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LeaderName)
                .HasMaxLength(152)
                .HasColumnName("leader_name");
            entity.Property(e => e.LeaderPosition)
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasColumnName("leader_position");
            entity.Property(e => e.Location)
                .HasMaxLength(150)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasDefaultValueSql("''")
                .HasColumnName("name");
            entity.Property(e => e.ParentDepartmentId).HasColumnName("parent_department_id");
            entity.Property(e => e.ParentDepartmentName)
                .HasMaxLength(100)
                .HasColumnName("parent_department_name");
        });

        modelBuilder.Entity<ViewDepartmentEquipmentList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_department_equipment_list");

            entity.Property(e => e.AssignedQuantity).HasColumnName("assigned_quantity");
            entity.Property(e => e.AvailableDepartmentQuantity).HasColumnName("available_department_quantity");
            entity.Property(e => e.DepartmentEquipmentId).HasColumnName("department_equipment_id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .HasColumnName("department_name");
            entity.Property(e => e.EquipmentName)
                .HasMaxLength(100)
                .HasColumnName("equipment_name");
            entity.Property(e => e.TotalInOrganization).HasColumnName("total_in_organization");
        });

        modelBuilder.Entity<ViewEngineeringStaffByDepartment>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_engineering_staff_by_department");

            entity.Property(e => e.ContactNumber)
                .HasMaxLength(20)
                .HasColumnName("contact_number");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .HasDefaultValueSql("''")
                .HasColumnName("department");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.FireDate).HasColumnName("fire_date");
            entity.Property(e => e.FullName)
                .HasMaxLength(152)
                .HasColumnName("full_name");
            entity.Property(e => e.Position)
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasColumnName("position");
            entity.Property(e => e.Site)
                .HasMaxLength(100)
                .HasColumnName("site");
        });

        modelBuilder.Entity<ViewMaterialReport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_material_report");

            entity.Property(e => e.ActualCost)
                .HasPrecision(19, 2)
                .HasColumnName("actual_cost");
            entity.Property(e => e.ActualMaterialQuantity)
                .HasPrecision(10, 2)
                .HasColumnName("actual_material_quantity");
            entity.Property(e => e.BudgetStatus)
                .HasMaxLength(10)
                .HasDefaultValueSql("''")
                .HasColumnName("budget_status")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .HasColumnName("department_name");
            entity.Property(e => e.MaterialId).HasColumnName("material_id");
            entity.Property(e => e.MaterialName)
                .HasMaxLength(100)
                .HasColumnName("material_name");
            entity.Property(e => e.ObjectCategory)
                .HasMaxLength(50)
                .HasColumnName("object_category");
            entity.Property(e => e.ObjectId).HasColumnName("object_id");
            entity.Property(e => e.ObjectName)
                .HasMaxLength(100)
                .HasColumnName("object_name");
            entity.Property(e => e.OverbudgetPercent)
                .HasPrecision(17, 2)
                .HasColumnName("overbudget_percent");
            entity.Property(e => e.PlannedCost)
                .HasPrecision(19, 2)
                .HasColumnName("planned_cost");
            entity.Property(e => e.PlannedMaterialQuantity)
                .HasPrecision(10, 2)
                .HasColumnName("planned_material_quantity");
            entity.Property(e => e.SiteId).HasColumnName("site_id");
            entity.Property(e => e.SiteName)
                .HasMaxLength(100)
                .HasColumnName("site_name");
            entity.Property(e => e.UnitOfMeasure)
                .HasMaxLength(20)
                .HasColumnName("unit_of_measure");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");
        });

        modelBuilder.Entity<ViewObjectBrigade>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_object_brigade");

            entity.Property(e => e.ActualEndDate).HasColumnName("actual_end_date");
            entity.Property(e => e.ActualStartDate).HasColumnName("actual_start_date");
            entity.Property(e => e.BrigadeId).HasColumnName("brigade_id");
            entity.Property(e => e.BrigadeName)
                .HasMaxLength(100)
                .HasColumnName("brigade_name");
            entity.Property(e => e.BrigadeStatus)
                .HasDefaultValueSql("'active'")
                .HasColumnType("enum('active','terminated')")
                .HasColumnName("brigade_status");
            entity.Property(e => e.BrigadierName)
                .HasMaxLength(152)
                .HasColumnName("brigadier_name");
            entity.Property(e => e.CurrentWorkType)
                .HasMaxLength(100)
                .HasColumnName("current_work_type");
            entity.Property(e => e.ObjectId).HasColumnName("object_id");
            entity.Property(e => e.ObjectName)
                .HasMaxLength(100)
                .HasColumnName("object_name");
            entity.Property(e => e.PlannedEndDate).HasColumnName("planned_end_date");
            entity.Property(e => e.PlannedStartDate).HasColumnName("planned_start_date");
        });

        modelBuilder.Entity<ViewObjectConstructionReport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_object_construction_report");

            entity.Property(e => e.ActualCost)
                .HasPrecision(20, 4)
                .HasColumnName("actual_cost");
            entity.Property(e => e.ActualEndDate).HasColumnName("actual_end_date");
            entity.Property(e => e.ActualQuantity)
                .HasPrecision(10, 2)
                .HasColumnName("actual_quantity");
            entity.Property(e => e.ActualStartDate).HasColumnName("actual_start_date");
            entity.Property(e => e.BrigadeName)
                .HasMaxLength(100)
                .HasColumnName("brigade_name");
            entity.Property(e => e.ConstructionDepartment)
                .HasMaxLength(100)
                .HasDefaultValueSql("''")
                .HasColumnName("construction_department");
            entity.Property(e => e.ConstructionSite)
                .HasMaxLength(100)
                .HasDefaultValueSql("''")
                .HasColumnName("construction_site");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ItemName)
                .HasMaxLength(100)
                .HasDefaultValueSql("''")
                .HasColumnName("item_name");
            entity.Property(e => e.ObjectId).HasColumnName("object_id");
            entity.Property(e => e.ObjectName)
                .HasMaxLength(100)
                .HasDefaultValueSql("''")
                .HasColumnName("object_name");
            entity.Property(e => e.ObjectType)
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasColumnName("object_type");
            entity.Property(e => e.PlannedEndDate).HasColumnName("planned_end_date");
            entity.Property(e => e.PlannedQuantity)
                .HasPrecision(10, 2)
                .HasColumnName("planned_quantity");
            entity.Property(e => e.PlannedStartDate).HasColumnName("planned_start_date");
            entity.Property(e => e.RecordType)
                .HasMaxLength(8)
                .HasDefaultValueSql("''")
                .HasColumnName("record_type")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.UnitOfMeasure)
                .HasMaxLength(20)
                .HasColumnName("unit_of_measure");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");
        });

        modelBuilder.Entity<ViewObjectEquipment>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_object_equipment");

            entity.Property(e => e.AssignedQuantity).HasColumnName("assigned_quantity");
            entity.Property(e => e.AssignmentDate).HasColumnName("assignment_date");
            entity.Property(e => e.AssignmentStatus)
                .HasMaxLength(18)
                .HasDefaultValueSql("''")
                .HasColumnName("assignment_status")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.DepartmentEquipmentId).HasColumnName("department_equipment_id");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .HasColumnName("department_name");
            entity.Property(e => e.EquipmentName)
                .HasMaxLength(100)
                .HasColumnName("equipment_name");
            entity.Property(e => e.ObjectCategory)
                .HasMaxLength(50)
                .HasColumnName("object_category");
            entity.Property(e => e.ObjectId).HasColumnName("object_id");
            entity.Property(e => e.ObjectName)
                .HasMaxLength(100)
                .HasColumnName("object_name");
            entity.Property(e => e.ReturnDate).HasColumnName("return_date");
            entity.Property(e => e.SiteName)
                .HasMaxLength(100)
                .HasColumnName("site_name");
        });

        modelBuilder.Entity<ViewObjectScheduleAndEstimate>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_object_schedule_and_estimate");

            entity.Property(e => e.ActualEndDate).HasColumnName("actual_end_date");
            entity.Property(e => e.ActualQuantity)
                .HasPrecision(10, 2)
                .HasColumnName("actual_quantity");
            entity.Property(e => e.ActualStartDate).HasColumnName("actual_start_date");
            entity.Property(e => e.AssignedTeam)
                .HasMaxLength(100)
                .HasColumnName("assigned_team");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.MaterialName)
                .HasMaxLength(100)
                .HasColumnName("material_name");
            entity.Property(e => e.ObjectId).HasColumnName("object_id");
            entity.Property(e => e.ObjectName)
                .HasMaxLength(100)
                .HasDefaultValueSql("''")
                .HasColumnName("object_name");
            entity.Property(e => e.PlannedEndDate).HasColumnName("planned_end_date");
            entity.Property(e => e.PlannedQuantity)
                .HasPrecision(10, 2)
                .HasColumnName("planned_quantity");
            entity.Property(e => e.PlannedStartDate).HasColumnName("planned_start_date");
            entity.Property(e => e.RecordType)
                .HasMaxLength(13)
                .HasDefaultValueSql("''")
                .HasColumnName("record_type")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.ScheduleOrMaterialId).HasColumnName("schedule_or_material_id");
            entity.Property(e => e.Status)
                .HasMaxLength(11)
                .HasDefaultValueSql("''")
                .HasColumnName("status")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.TotalCost)
                .HasPrecision(20, 4)
                .HasColumnName("total_cost");
            entity.Property(e => e.UnitOfMeasure)
                .HasMaxLength(20)
                .HasColumnName("unit_of_measure");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");
        });

        modelBuilder.Entity<WorkSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PRIMARY");

            entity.ToTable("work_schedule");

            entity.HasIndex(e => e.ScheduleBrigadeId, "schedule_brigade_id");

            entity.HasIndex(e => e.ScheduleObjectId, "schedule_object_id");

            entity.HasIndex(e => e.ScheduleWorkTypeId, "schedule_work_type_id");

            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.ActualEndDate).HasColumnName("actual_end_date");
            entity.Property(e => e.ActualStartDate).HasColumnName("actual_start_date");
            entity.Property(e => e.PlannedEndDate).HasColumnName("planned_end_date");
            entity.Property(e => e.PlannedStartDate).HasColumnName("planned_start_date");
            entity.Property(e => e.ScheduleBrigadeId).HasColumnName("schedule_brigade_id");
            entity.Property(e => e.ScheduleObjectId).HasColumnName("schedule_object_id");
            entity.Property(e => e.ScheduleWorkTypeId).HasColumnName("schedule_work_type_id");

            entity.HasOne(d => d.ScheduleBrigade).WithMany(p => p.WorkSchedules)
                .HasForeignKey(d => d.ScheduleBrigadeId)
                .HasConstraintName("work_schedule_ibfk_3");

            entity.HasOne(d => d.ScheduleObject).WithMany(p => p.WorkSchedules)
                .HasForeignKey(d => d.ScheduleObjectId)
                .HasConstraintName("work_schedule_ibfk_1");

            entity.HasOne(d => d.ScheduleWorkType).WithMany(p => p.WorkSchedules)
                .HasForeignKey(d => d.ScheduleWorkTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("work_schedule_ibfk_2");
        });

        modelBuilder.Entity<WorkType>(entity =>
        {
            entity.HasKey(e => e.WorkTypeId).HasName("PRIMARY");

            entity.ToTable("work_type");

            entity.Property(e => e.WorkTypeId).HasColumnName("work_type_id");
            entity.Property(e => e.WorkTypeDescription)
                .HasColumnType("text")
                .HasColumnName("work_type_description");
            entity.Property(e => e.WorkTypeName)
                .HasMaxLength(100)
                .HasColumnName("work_type_name");
        });

        modelBuilder.Entity<WorkTypeForCategory>(entity =>
        {
            entity.HasKey(e => new { e.SpecificCategoryId, e.WtfcWorkTypeId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("work_type_for_category");

            entity.HasIndex(e => e.WtfcWorkTypeId, "wtfc_work_type_id");

            entity.Property(e => e.SpecificCategoryId).HasColumnName("specific_category_id");
            entity.Property(e => e.WtfcWorkTypeId).HasColumnName("wtfc_work_type_id");
            entity.Property(e => e.IsMandatory)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_mandatory");

            entity.HasOne(d => d.SpecificCategory).WithMany(p => p.WorkTypeForCategories)
                .HasForeignKey(d => d.SpecificCategoryId)
                .HasConstraintName("work_type_for_category_ibfk_1");

            entity.HasOne(d => d.WtfcWorkType).WithMany(p => p.WorkTypeForCategories)
                .HasForeignKey(d => d.WtfcWorkTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("work_type_for_category_ibfk_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
