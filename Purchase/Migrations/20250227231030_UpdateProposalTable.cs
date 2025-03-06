using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Purchase.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProposalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "Proposals");

            migrationBuilder.RenameColumn(
                name: "Department",
                table: "Proposals",
                newName: "Division");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "Proposals",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "ApplicationNum",
                table: "Proposals",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationNum",
                table: "Proposals");

            migrationBuilder.RenameColumn(
                name: "Division",
                table: "Proposals",
                newName: "Department");

            migrationBuilder.AlterColumn<string>(
                name: "ID",
                table: "Proposals",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "Proposals",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
