using Microsoft.EntityFrameworkCore.Migrations;
using Purchase.Data;
#nullable disable

namespace Purchase.Migrations
{
    /// <inheritdoc />
    public partial class FixEnumConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProposalMaterials_ProposalCatalogs_CatalogId",
                table: "ProposalMaterials");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_Number",
                table: "Proposals");


            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Proposals",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalMaterials_ProposalCatalogs_CatalogId",
                table: "ProposalMaterials",
                column: "CatalogId",
                principalTable: "ProposalCatalogs",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProposalMaterials_ProposalCatalogs_CatalogId",
                table: "ProposalMaterials");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Proposals",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "Proposals",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Number",
                table: "Proposals",
                type: "integer",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_Number",
                table: "Proposals",
                column: "Number",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalMaterials_ProposalCatalogs_CatalogId",
                table: "ProposalMaterials",
                column: "CatalogId",
                principalTable: "ProposalCatalogs",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
