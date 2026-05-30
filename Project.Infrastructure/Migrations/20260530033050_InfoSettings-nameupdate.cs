using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InfoSettingsnameupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SchoolSettings",
                table: "SchoolSettings");

            migrationBuilder.RenameTable(
                name: "SchoolSettings",
                newName: "InfoSettings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InfoSettings",
                table: "InfoSettings",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InfoSettings",
                table: "InfoSettings");

            migrationBuilder.RenameTable(
                name: "InfoSettings",
                newName: "SchoolSettings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SchoolSettings",
                table: "SchoolSettings",
                column: "Id");
        }
    }
}
