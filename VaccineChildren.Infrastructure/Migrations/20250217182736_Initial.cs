using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VaccineChildren.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "manufacturers",
                columns: table => new
                {
                    manufacturer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    short_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    country_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    country_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("manufacturers_pkey", x => x.manufacturer_id);
                });

            migrationBuilder.CreateTable(
                name: "packages",
                columns: table => new
                {
                    package_id = table.Column<Guid>(type: "uuid", nullable: false),
                    package_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    discount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    MinAge = table.Column<int>(type: "integer", nullable: true),
                    MaxAge = table.Column<int>(type: "integer", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("packages_pkey", x => x.package_id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    role_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("roles_pkey", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "template",
                columns: table => new
                {
                    template_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    temaplate = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("template_pkey", x => x.template_id);
                });

            migrationBuilder.CreateTable(
                name: "vaccine",
                columns: table => new
                {
                    vaccine_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vaccine_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    min_age = table.Column<int>(type: "integer", nullable: true),
                    max_age = table.Column<int>(type: "integer", nullable: true),
                    number_dose = table.Column<int>(type: "integer", nullable: true),
                    image = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    duration = table.Column<int>(type: "integer", nullable: true),
                    unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("vaccine_pkey", x => x.vaccine_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: true),
                    user_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.user_id);
                    table.ForeignKey(
                        name: "users_role_id_fkey",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "package_vaccine",
                columns: table => new
                {
                    package_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vaccine_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("package_vaccine_pkey", x => new { x.package_id, x.vaccine_id });
                    table.ForeignKey(
                        name: "package_vaccine_package_id_fkey",
                        column: x => x.package_id,
                        principalTable: "packages",
                        principalColumn: "package_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "package_vaccine_service_id_fkey",
                        column: x => x.vaccine_id,
                        principalTable: "vaccine",
                        principalColumn: "vaccine_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vaccine_manufactures",
                columns: table => new
                {
                    manufacturer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vaccine_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("vaccine_manufactures_pkey", x => new { x.manufacturer_id, x.vaccine_id });
                    table.UniqueConstraint("AK_vaccine_manufactures_vaccine_id", x => x.vaccine_id);
                    table.ForeignKey(
                        name: "vaccine_manufactures_manufacturer_id_fkey",
                        column: x => x.manufacturer_id,
                        principalTable: "manufacturers",
                        principalColumn: "manufacturer_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "vaccine_manufactures_vaccine_id_fkey",
                        column: x => x.vaccine_id,
                        principalTable: "vaccine",
                        principalColumn: "vaccine_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "children",
                columns: table => new
                {
                    child_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    dob = table.Column<DateOnly>(type: "date", nullable: true),
                    gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    blood_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    allergies_notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("children_pkey", x => x.child_id);
                    table.ForeignKey(
                        name: "children_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "staff",
                columns: table => new
                {
                    staff_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dob = table.Column<DateOnly>(type: "date", nullable: true),
                    gender = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    blood_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("staff_pkey", x => x.staff_id);
                    table.ForeignKey(
                        name: "FK_staff_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "role_id");
                    table.ForeignKey(
                        name: "FK_staff_users_staff_id",
                        column: x => x.staff_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "batch",
                columns: table => new
                {
                    batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vaccine_id = table.Column<Guid>(type: "uuid", nullable: true),
                    production_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TestColumn = table.Column<string>(type: "text", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: true),
                    expiration_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("batch_pkey", x => x.batch_id);
                    table.ForeignKey(
                        name: "batch_vaccine_id_fkey",
                        column: x => x.vaccine_id,
                        principalTable: "vaccine_manufactures",
                        principalColumn: "vaccine_id");
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    template_id = table.Column<int>(type: "integer", nullable: true),
                    child_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_read = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("notifications_pkey", x => x.notification_id);
                    table.ForeignKey(
                        name: "notifications_child_id_fkey",
                        column: x => x.child_id,
                        principalTable: "children",
                        principalColumn: "child_id");
                    table.ForeignKey(
                        name: "notifications_template_id_fkey",
                        column: x => x.template_id,
                        principalTable: "template",
                        principalColumn: "template_id");
                    table.ForeignKey(
                        name: "notifications_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "user_cart",
                columns: table => new
                {
                    child_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vaccine_id = table.Column<Guid>(type: "uuid", nullable: false),
                    package_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_cart_pkey", x => new { x.child_id, x.vaccine_id, x.package_id });
                    table.ForeignKey(
                        name: "user_cart_child_id_fkey",
                        column: x => x.child_id,
                        principalTable: "children",
                        principalColumn: "child_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "user_cart_package_id_fkey",
                        column: x => x.package_id,
                        principalTable: "packages",
                        principalColumn: "package_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "user_cart_vaccine_id_fkey",
                        column: x => x.vaccine_id,
                        principalTable: "vaccine",
                        principalColumn: "vaccine_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "holidays",
                columns: table => new
                {
                    holiday_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("holidays_pkey", x => x.holiday_id);
                    table.ForeignKey(
                        name: "holidays_modified_by_fkey",
                        column: x => x.modified_by,
                        principalTable: "staff",
                        principalColumn: "staff_id");
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    child_id = table.Column<Guid>(type: "uuid", nullable: true),
                    approved_staff = table.Column<Guid>(type: "uuid", nullable: true),
                    order_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    package_modified = table.Column<bool>(type: "boolean", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    is_confirmed = table.Column<bool>(type: "boolean", nullable: true),
                    confirmed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("orders_pkey", x => x.order_id);
                    table.ForeignKey(
                        name: "orders_approved_staff_fkey",
                        column: x => x.approved_staff,
                        principalTable: "staff",
                        principalColumn: "staff_id");
                    table.ForeignKey(
                        name: "orders_child_id_fkey",
                        column: x => x.child_id,
                        principalTable: "children",
                        principalColumn: "child_id");
                });

            migrationBuilder.CreateTable(
                name: "feedback",
                columns: table => new
                {
                    feedback_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rating = table.Column<int>(type: "integer", nullable: true),
                    comment = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feedback_pkey", x => x.feedback_id);
                    table.ForeignKey(
                        name: "feedback_order_id_fkey",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "feedback_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "order_package",
                columns: table => new
                {
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    package_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("order_package_pkey", x => new { x.order_id, x.package_id });
                    table.ForeignKey(
                        name: "order_package_order_id_fkey",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "order_package_package_id_fkey",
                        column: x => x.package_id,
                        principalTable: "packages",
                        principalColumn: "package_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_vaccine",
                columns: table => new
                {
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vaccine_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("order_vaccine_pkey", x => new { x.order_id, x.vaccine_id });
                    table.ForeignKey(
                        name: "order_vaccine_order_id_fkey",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "order_vaccine_vaccine_id_fkey",
                        column: x => x.vaccine_id,
                        principalTable: "vaccine_manufactures",
                        principalColumn: "vaccine_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    payment_method = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    payment_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    payment_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("payments_pkey", x => x.payment_id);
                    table.ForeignKey(
                        name: "payments_order_id_fkey",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "payments_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "schedule",
                columns: table => new
                {
                    schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    child_id = table.Column<Guid>(type: "uuid", nullable: true),
                    vaccine_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    administered_by = table.Column<Guid>(type: "uuid", nullable: true),
                    schedule_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    is_vaccinated = table.Column<bool>(type: "boolean", nullable: true),
                    actual_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("schedule_pkey", x => x.schedule_id);
                    table.ForeignKey(
                        name: "schedule_administered_by_fkey",
                        column: x => x.administered_by,
                        principalTable: "staff",
                        principalColumn: "staff_id");
                    table.ForeignKey(
                        name: "schedule_child_id_fkey",
                        column: x => x.child_id,
                        principalTable: "children",
                        principalColumn: "child_id");
                    table.ForeignKey(
                        name: "schedule_order_id_fkey",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id");
                });

            migrationBuilder.CreateTable(
                name: "vaccine_reactions",
                columns: table => new
                {
                    reaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    schedule_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reaction_description = table.Column<string>(type: "text", nullable: true),
                    severity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    onset_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    resolved_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("vaccine_reactions_pkey", x => x.reaction_id);
                    table.ForeignKey(
                        name: "vaccine_reactions_schedule_id_fkey",
                        column: x => x.schedule_id,
                        principalTable: "schedule",
                        principalColumn: "schedule_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_batch_vaccine_id",
                table: "batch",
                column: "vaccine_id");

            migrationBuilder.CreateIndex(
                name: "IX_children_user_id",
                table: "children",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_order_id",
                table: "feedback",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_user_id",
                table: "feedback",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_holidays_modified_by",
                table: "holidays",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_child_id",
                table: "notifications",
                column: "child_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_template_id",
                table: "notifications",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_user_id",
                table: "notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_package_package_id",
                table: "order_package",
                column: "package_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_vaccine_vaccine_id",
                table: "order_vaccine",
                column: "vaccine_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_approved_staff",
                table: "orders",
                column: "approved_staff");

            migrationBuilder.CreateIndex(
                name: "IX_orders_child_id",
                table: "orders",
                column: "child_id");

            migrationBuilder.CreateIndex(
                name: "IX_package_vaccine_vaccine_id",
                table: "package_vaccine",
                column: "vaccine_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_user_id",
                table: "payments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "payments_order_id_key",
                table: "payments",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_schedule_administered_by",
                table: "schedule",
                column: "administered_by");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_child_id",
                table: "schedule",
                column: "child_id");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_order_id",
                table: "schedule",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_RoleId",
                table: "staff",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_user_cart_package_id",
                table: "user_cart",
                column: "package_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_cart_vaccine_id",
                table: "user_cart",
                column: "vaccine_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "vaccine_manufactures_vaccine_id_key",
                table: "vaccine_manufactures",
                column: "vaccine_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vaccine_reactions_schedule_id",
                table: "vaccine_reactions",
                column: "schedule_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "batch");

            migrationBuilder.DropTable(
                name: "feedback");

            migrationBuilder.DropTable(
                name: "holidays");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "order_package");

            migrationBuilder.DropTable(
                name: "order_vaccine");

            migrationBuilder.DropTable(
                name: "package_vaccine");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "user_cart");

            migrationBuilder.DropTable(
                name: "vaccine_reactions");

            migrationBuilder.DropTable(
                name: "template");

            migrationBuilder.DropTable(
                name: "vaccine_manufactures");

            migrationBuilder.DropTable(
                name: "packages");

            migrationBuilder.DropTable(
                name: "schedule");

            migrationBuilder.DropTable(
                name: "manufacturers");

            migrationBuilder.DropTable(
                name: "vaccine");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "staff");

            migrationBuilder.DropTable(
                name: "children");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
