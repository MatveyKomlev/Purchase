using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Purchase.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProposalId",
                table: "ProposalCatalogs");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProposalCatalogs");

            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "Proposals",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                table: "Proposals",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Proposals",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CatalogId",
                table: "ProposalMaterials",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedPrice",
                table: "ProposalMaterials",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerName",
                table: "ProposalMaterials",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerPartNumber",
                table: "ProposalMaterials",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitOfMeasure",
                table: "ProposalMaterials",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerName",
                table: "ProposalCatalogs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerPartNumber",
                table: "ProposalCatalogs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitOfMeasure",
                table: "ProposalCatalogs",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_Number",
                table: "Proposals",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProposalMaterials_CatalogId",
                table: "ProposalMaterials",
                column: "CatalogId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalMaterials_ProposalCatalogs_CatalogId",
                table: "ProposalMaterials",
                column: "CatalogId",
                principalTable: "ProposalCatalogs",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProposalMaterials_ProposalCatalogs_CatalogId",
                table: "ProposalMaterials");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_Number",
                table: "Proposals");

            migrationBuilder.DropIndex(
                name: "IX_ProposalMaterials_CatalogId",
                table: "ProposalMaterials");

            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Explanation",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "CatalogId",
                table: "ProposalMaterials");

            migrationBuilder.DropColumn(
                name: "EstimatedPrice",
                table: "ProposalMaterials");

            migrationBuilder.DropColumn(
                name: "ManufacturerName",
                table: "ProposalMaterials");

            migrationBuilder.DropColumn(
                name: "ManufacturerPartNumber",
                table: "ProposalMaterials");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasure",
                table: "ProposalMaterials");

            migrationBuilder.DropColumn(
                name: "ManufacturerName",
                table: "ProposalCatalogs");

            migrationBuilder.DropColumn(
                name: "ManufacturerPartNumber",
                table: "ProposalCatalogs");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasure",
                table: "ProposalCatalogs");

            migrationBuilder.AddColumn<int>(
                name: "ProposalId",
                table: "ProposalCatalogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "ProposalCatalogs",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
