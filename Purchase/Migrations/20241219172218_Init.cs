using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Purchase.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proposals",
                columns: table => new
                {
                    ID = table.Column<string>(type: "text", nullable: false),
                    Number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "timestamp without time zone", maxLength: 60, nullable: false),
                    FullNumber = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Status = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    TextStatus = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Department = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Author = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposals", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Proposals");
        }
    }
}
