using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Purchase.Migrations
{
    /// <inheritdoc />
    public partial class AddComponentIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLogin",
                table: "Users",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Deadline",
                table: "Proposals",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreation",
                table: "Proposals",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateTable(
                name: "ComponentStandards",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentStandards", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ElectronicComponents",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ManufacturerPartNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Manufacturer = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DatasheetUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ComplianceStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LocalizationPercent = table.Column<int>(type: "integer", nullable: false),
                    CatalogId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastVerifiedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectronicComponents", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ElectronicComponents_ProposalCatalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "ProposalCatalogs",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ComponentStandardRelations",
                columns: table => new
                {
                    ElectronicComponentID = table.Column<int>(type: "integer", nullable: false),
                    StandardsID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentStandardRelations", x => new { x.ElectronicComponentID, x.StandardsID });
                    table.ForeignKey(
                        name: "FK_ComponentStandardRelations_ComponentStandards_StandardsID",
                        column: x => x.StandardsID,
                        principalTable: "ComponentStandards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComponentStandardRelations_ElectronicComponents_ElectronicC~",
                        column: x => x.ElectronicComponentID,
                        principalTable: "ElectronicComponents",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComponentStandardRelations_StandardsID",
                table: "ComponentStandardRelations",
                column: "StandardsID");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentStandards_Code",
                table: "ComponentStandards",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicComponents_CatalogId",
                table: "ElectronicComponents",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectronicComponents_ManufacturerPartNumber",
                table: "ElectronicComponents",
                column: "ManufacturerPartNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComponentStandardRelations");

            migrationBuilder.DropTable(
                name: "ComponentStandards");

            migrationBuilder.DropTable(
                name: "ElectronicComponents");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLogin",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Deadline",
                table: "Proposals",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreation",
                table: "Proposals",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }
    }
}
