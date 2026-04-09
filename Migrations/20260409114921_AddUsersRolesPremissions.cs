using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaxAccount.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersRolesPremissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 9, 11, 49, 19, 653, DateTimeKind.Utc).AddTicks(9864), "View products", "products.view" },
                    { 2, new DateTime(2026, 4, 9, 11, 49, 19, 653, DateTimeKind.Utc).AddTicks(9869), "Create products", "products.create" },
                    { 3, new DateTime(2026, 4, 9, 11, 49, 19, 653, DateTimeKind.Utc).AddTicks(9873), "Edit products", "products.edit" },
                    { 4, new DateTime(2026, 4, 9, 11, 49, 19, 653, DateTimeKind.Utc).AddTicks(9877), "Delete products", "products.delete" },
                    { 5, new DateTime(2026, 4, 9, 11, 49, 19, 653, DateTimeKind.Utc).AddTicks(9881), "View invoices", "invoices.view" },
                    { 6, new DateTime(2026, 4, 9, 11, 49, 19, 653, DateTimeKind.Utc).AddTicks(9885), "Create invoices", "invoices.create" },
                    { 7, new DateTime(2026, 4, 9, 11, 49, 19, 653, DateTimeKind.Utc).AddTicks(9889), "Approve invoices", "invoices.approve" },
                    { 8, new DateTime(2026, 4, 9, 11, 49, 19, 653, DateTimeKind.Utc).AddTicks(9892), "View reports", "reports.view" },
                    { 9, new DateTime(2026, 4, 9, 11, 49, 19, 653, DateTimeKind.Utc).AddTicks(9896), "Manage users", "users.manage" },
                    { 10, new DateTime(2026, 4, 9, 11, 49, 19, 653, DateTimeKind.Utc).AddTicks(9900), "Manage accounts", "accounts.manage" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 9, 11, 49, 19, 653, DateTimeKind.Utc).AddTicks(9291), "Full access", "Admin" },
                    { 2, new DateTime(2026, 4, 9, 11, 49, 19, 653, DateTimeKind.Utc).AddTicks(9298), "Manage operations", "Manager" },
                    { 3, new DateTime(2026, 4, 9, 11, 49, 19, 653, DateTimeKind.Utc).AddTicks(9303), "Day to day operations", "Staff" },
                    { 4, new DateTime(2026, 4, 9, 11, 49, 19, 653, DateTimeKind.Utc).AddTicks(9308), "View only access", "Customer" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 },
                    { 7, 1 },
                    { 8, 1 },
                    { 9, 1 },
                    { 10, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 2 },
                    { 5, 2 },
                    { 6, 2 },
                    { 7, 2 },
                    { 8, 2 },
                    { 1, 3 },
                    { 2, 3 },
                    { 5, 3 },
                    { 6, 3 },
                    { 1, 4 },
                    { 5, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
