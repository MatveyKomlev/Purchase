using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Purchase.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProposalIdFromCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProposalCatalogs_Proposals_ProposalId",
                table: "ProposalCatalogs");

            migrationBuilder.RenameColumn(
                name: "ProposalId",
                table: "ProposalCatalogs",
                newName: "ProposalID");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalCatalogs_ProposalId",
                table: "ProposalCatalogs",
                newName: "IX_ProposalCatalogs_ProposalID");

            migrationBuilder.AlterColumn<int>(
                name: "ProposalID",
                table: "ProposalCatalogs",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Material",
                table: "ProposalCatalogs",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "ProposalCatalogs",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalCatalogs_Proposals_ProposalID",
                table: "ProposalCatalogs",
                column: "ProposalID",
                principalTable: "Proposals",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProposalCatalogs_Proposals_ProposalID",
                table: "ProposalCatalogs");

            migrationBuilder.RenameColumn(
                name: "ProposalID",
                table: "ProposalCatalogs",
                newName: "ProposalId");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalCatalogs_ProposalID",
                table: "ProposalCatalogs",
                newName: "IX_ProposalCatalogs_ProposalId");

            migrationBuilder.AlterColumn<int>(
                name: "ProposalId",
                table: "ProposalCatalogs",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Material",
                table: "ProposalCatalogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "ProposalCatalogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalCatalogs_Proposals_ProposalId",
                table: "ProposalCatalogs",
                column: "ProposalId",
                principalTable: "Proposals",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
