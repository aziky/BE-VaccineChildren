using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Infrastructure;

public partial class VaccineSystemDbContext : DbContext
{

    public VaccineSystemDbContext()
    {
    }

    public VaccineSystemDbContext(DbContextOptions<VaccineSystemDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Batch> Batches { get; set; }

    public virtual DbSet<Child> Children { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Holiday> Holidays { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Package> Packages { get; set; }
    public virtual DbSet<PackageVaccine> PackageVaccines { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Template> Templates { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCart> UserCarts { get; set; }

    public virtual DbSet<Vaccine> Vaccines { get; set; }

    public virtual DbSet<VaccineManufacture> VaccineManufactures { get; set; }

    public virtual DbSet<VaccineReaction> VaccineReactions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),    // Chuyển sang UTC trước khi lưu vào DB
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        modelBuilder.Entity<Batch>(entity =>
        {
            entity.HasKey(e => e.BatchId).HasName("batch_pkey");

            entity.ToTable("batch");

            entity.Property(e => e.BatchId)
                .ValueGeneratedNever()
                .HasColumnName("batch_id");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expiration_date");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.ProductionDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("production_date");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");

            entity.HasOne(d => d.Vaccine).WithMany(p => p.Batches)
                .HasPrincipalKey(p => p.VaccineId)
                .HasForeignKey(d => d.VaccineId)
                .HasConstraintName("batch_vaccine_id_fkey");
        });

        modelBuilder.Entity<Child>(entity =>
        {
            entity.HasKey(e => e.ChildId).HasName("children_pkey");

            entity.ToTable("children");

            entity.Property(e => e.ChildId)
                .ValueGeneratedNever()
                .HasColumnName("child_id");
            entity.Property(e => e.AllergiesNotes).HasColumnName("allergies_notes");
            entity.Property(e => e.BloodType)
                .HasMaxLength(50)
                .HasColumnName("blood_type");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Children)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("children_user_id_fkey");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("feedback_pkey");

            entity.ToTable("feedback");

            entity.Property(e => e.FeedbackId)
                .ValueGeneratedNever()
                .HasColumnName("feedback_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Order).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("feedback_order_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("feedback_user_id_fkey");
        });

        modelBuilder.Entity<Holiday>(entity =>
        {
            entity.HasKey(e => e.HolidayId).HasName("holidays_pkey");

            entity.ToTable("holidays");

            entity.Property(e => e.HolidayId)
                .ValueGeneratedNever()
                .HasColumnName("holiday_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.EndDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_date");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.StartDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_date");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.Holidays)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("holidays_modified_by_fkey");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId).HasName("manufacturers_pkey");

            entity.ToTable("manufacturers");

            entity.Property(e => e.ManufacturerId)
                .ValueGeneratedNever()
                .HasColumnName("manufacturer_id");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(10)
                .HasColumnName("country_code");
            entity.Property(e => e.CountryName)
                .HasMaxLength(50)
                .HasColumnName("country_name");
            entity.Property(e => e.CreatedAt)
                .HasConversion(dateTimeConverter)
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.ShortName)
                .HasMaxLength(50)
                .HasColumnName("short_name");
            entity.Property(e => e.UpdatedAt)
                .HasConversion(dateTimeConverter)
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("updated_by");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("notifications_pkey");

            entity.ToTable("notifications");

            entity.Property(e => e.NotificationId)
                .ValueGeneratedNever()
                .HasColumnName("notification_id");
            entity.Property(e => e.ChildId).HasColumnName("child_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.TemplateId).HasColumnName("template_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Child).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("notifications_child_id_fkey");

            entity.HasOne(d => d.Template).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.TemplateId)
                .HasConstraintName("notifications_template_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("notifications_user_id_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("order_id");
            entity.Property(e => e.ApprovedStaff).HasColumnName("approved_staff");
            entity.Property(e => e.ChildId).HasColumnName("child_id");
            entity.Property(e => e.ConfirmedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("confirmed_at");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.IsConfirmed).HasColumnName("is_confirmed");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OrderDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("order_date");
            entity.Property(e => e.PackageModified).HasColumnName("package_modified");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.ApprovedStaffNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ApprovedStaff)
                .HasConstraintName("orders_approved_staff_fkey");

            entity.HasOne(d => d.Child).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("orders_child_id_fkey");

            entity.HasMany(d => d.Packages).WithMany(p => p.Orders)
                .UsingEntity<Dictionary<string, object>>(
                    "OrderPackage",
                    r => r.HasOne<Package>().WithMany()
                        .HasForeignKey("PackageId")
                        .HasConstraintName("order_package_package_id_fkey"),
                    l => l.HasOne<Order>().WithMany()
                        .HasForeignKey("OrderId")
                        .HasConstraintName("order_package_order_id_fkey"),
                    j =>
                    {
                        j.HasKey("OrderId", "PackageId").HasName("order_package_pkey");
                        j.ToTable("order_package");
                        j.IndexerProperty<Guid>("OrderId").HasColumnName("order_id");
                        j.IndexerProperty<Guid>("PackageId").HasColumnName("package_id");
                    });

            entity.HasMany(d => d.Vaccines).WithMany(p => p.Orders)
                .UsingEntity<Dictionary<string, object>>(
                    "OrderVaccine",
                    r => r.HasOne<VaccineManufacture>().WithMany()
                        .HasPrincipalKey("VaccineId")
                        .HasForeignKey("VaccineId")
                        .HasConstraintName("order_vaccine_vaccine_id_fkey"),
                    l => l.HasOne<Order>().WithMany()
                        .HasForeignKey("OrderId")
                        .HasConstraintName("order_vaccine_order_id_fkey"),
                    j =>
                    {
                        j.HasKey("OrderId", "VaccineId").HasName("order_vaccine_pkey");
                        j.ToTable("order_vaccine");
                        j.IndexerProperty<Guid>("OrderId").HasColumnName("order_id");
                        j.IndexerProperty<Guid>("VaccineId").HasColumnName("vaccine_id");
                    });
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("packages_pkey");

            entity.ToTable("packages");

            entity.Property(e => e.PackageId)
                .ValueGeneratedNever()
                .HasColumnName("package_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Discount)
                .HasPrecision(10, 2)
                .HasColumnName("discount");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.PackageName)
                .HasMaxLength(100)
                .HasColumnName("package_name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("updated_by");
            entity.Property(e => e.MaxAge).HasColumnName("max_age");
            entity.Property(e => e.MinAge).HasColumnName("min_age");
            entity.Property(e => e.Unit).HasColumnName("unit")
                .HasMaxLength(20);

            entity.HasMany(d => d.Vaccines)
                .WithMany(p => p.Packages)
                .UsingEntity<PackageVaccine>(
                    j => j.HasOne(pv => pv.Vaccine)
                            .WithMany()
                            .HasForeignKey(pv => pv.VaccineId)
                            .HasConstraintName("fk_package_vaccine_vaccine"),
                    j => j.HasOne(pv => pv.Package)
                            .WithMany()
                            .HasForeignKey(pv => pv.PackageId)
                            .HasConstraintName("fk_package_vaccine_package"),
                    j =>
                    {
                        j.ToTable("package_vaccine");
                        j.HasKey(pv => new { pv.PackageId, pv.VaccineId });
                        j.Property(pv => pv.PackageId).HasColumnName("package_id");
                        j.Property(pv => pv.VaccineId).HasColumnName("vaccine_id");
                    });
        });

                    modelBuilder.Entity<PackageVaccine>()
                        .HasKey(pv => new { pv.PackageId, pv.VaccineId });

                    modelBuilder.Entity<PackageVaccine>()
                        .HasOne(pv => pv.Package)
                        .WithMany(p => p.PackageVaccines)
                        .HasForeignKey(pv => pv.PackageId);

                    modelBuilder.Entity<PackageVaccine>()
                        .HasOne(pv => pv.Vaccine)
                        .WithMany(v => v.PackageVaccines)
                        .HasForeignKey(pv => pv.VaccineId);



        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payments_pkey");

            entity.ToTable("payments");

            entity.HasIndex(e => e.OrderId, "payments_order_id_key").IsUnique();

            entity.Property(e => e.PaymentId)
                .ValueGeneratedNever()
                .HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(100)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .HasColumnName("payment_status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Order).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.OrderId)
                .HasConstraintName("payments_order_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("payments_user_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.RoleId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("role_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("updated_by");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("schedule_pkey");

            entity.ToTable("schedule");

            entity.Property(e => e.ScheduleId)
                .ValueGeneratedNever()
                .HasColumnName("schedule_id");
            entity.Property(e => e.ActualDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("actual_date");
            entity.Property(e => e.AdministeredBy).HasColumnName("administered_by");
            entity.Property(e => e.ChildId).HasColumnName("child_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.IsVaccinated).HasColumnName("is_vaccinated");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ScheduleDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("schedule_date");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("updated_by");
            entity.Property(e => e.VaccineType)
                .HasMaxLength(255)
                .HasColumnName("vaccine_type");

            entity.HasOne(d => d.AdministeredByNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.AdministeredBy)
                .HasConstraintName("schedule_administered_by_fkey");

            entity.HasOne(d => d.Child).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("schedule_child_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("schedule_order_id_fkey");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("staff_pkey");

            entity.ToTable("staff");

            entity.Property(e => e.StaffId)
                .ValueGeneratedNever()
                .HasColumnName("staff_id");
            entity.Property(e => e.BloodType)
                .HasMaxLength(50)
                .HasColumnName("blood_type");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .HasColumnName("gender");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Role).WithMany(p => p.Staff)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("staff_role_id_fkey");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status)
                .HasColumnType("text")
                .HasColumnName("status");
        });

        modelBuilder.Entity<Template>(entity =>
        {
            entity.HasKey(e => e.TemplateId).HasName("template_pkey");

            entity.ToTable("template");

            entity.Property(e => e.TemplateId).HasColumnName("template_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Subject)
                .HasMaxLength(255)
                .HasColumnName("subject");
            entity.Property(e => e.Temaplate).HasColumnName("temaplate");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("updated_by");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.Address)
                .HasMaxLength(256)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("updated_by");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("user_name");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("users_role_id_fkey");

        });

        modelBuilder.Entity<UserCart>(entity =>
        {
            entity.HasKey(e => new { e.ChildId, e.VaccineId, e.PackageId }).HasName("user_cart_pkey");

            entity.ToTable("user_cart");

            entity.Property(e => e.ChildId).HasColumnName("child_id");
            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");
            entity.Property(e => e.PackageId).HasColumnName("package_id");

            entity.HasOne(d => d.Child).WithMany(p => p.UserCarts)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("user_cart_child_id_fkey");

            entity.HasOne(d => d.Package).WithMany(p => p.UserCarts)
                .HasForeignKey(d => d.PackageId)
                .HasConstraintName("user_cart_package_id_fkey");

            entity.HasOne(d => d.Vaccine).WithMany(p => p.UserCarts)
                .HasForeignKey(d => d.VaccineId)
                .HasConstraintName("user_cart_vaccine_id_fkey");
        });

        modelBuilder.Entity<Vaccine>(entity =>
        {
            entity.HasKey(e => e.VaccineId).HasName("vaccine_pkey");

            entity.ToTable("vaccine");

            entity.Property(e => e.VaccineId)
                .ValueGeneratedNever()
                .HasColumnName("vaccine_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.MaxAge).HasColumnName("max_age");
            entity.Property(e => e.MinAge).HasColumnName("min_age");
            entity.Property(e => e.NumberDose).HasColumnName("number_dose");
            entity.Property(e => e.Unit)
                .HasMaxLength(20)
                .HasColumnName("unit");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("updated_by");
            entity.Property(e => e.VaccineName)
                .HasMaxLength(100)
                .HasColumnName("vaccine_name");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
        });

        modelBuilder.Entity<VaccineManufacture>(entity =>
        {
            entity.HasKey(e => new { e.ManufacturerId, e.VaccineId }).HasName("vaccine_manufactures_pkey");

            entity.ToTable("vaccine_manufactures");

            entity.HasIndex(e => e.VaccineId, "vaccine_manufactures_vaccine_id_key").IsUnique();

            entity.Property(e => e.ManufacturerId).HasColumnName("manufacturer_id");
            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.VaccineManufactures)
                .HasForeignKey(d => d.ManufacturerId)
                .HasConstraintName("vaccine_manufactures_manufacturer_id_fkey");

            entity.HasOne(d => d.Vaccine).WithOne(p => p.VaccineManufacture)
                .HasForeignKey<VaccineManufacture>(d => d.VaccineId)
                .HasConstraintName("vaccine_manufactures_vaccine_id_fkey");
        });

        modelBuilder.Entity<VaccineReaction>(entity =>
        {
            entity.HasKey(e => e.ReactionId).HasName("vaccine_reactions_pkey");

            entity.ToTable("vaccine_reactions");

            entity.Property(e => e.ReactionId)
                .ValueGeneratedNever()
                .HasColumnName("reaction_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .HasColumnName("created_by");
            entity.Property(e => e.OnsetTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("onset_time");
            entity.Property(e => e.ReactionDescription).HasColumnName("reaction_description");
            entity.Property(e => e.ResolvedTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("resolved_time");
            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.Severity)
                .HasMaxLength(50)
                .HasColumnName("severity");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(255)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Schedule).WithMany(p => p.VaccineReactions)
                .HasForeignKey(d => d.ScheduleId)
                .HasConstraintName("vaccine_reactions_schedule_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
