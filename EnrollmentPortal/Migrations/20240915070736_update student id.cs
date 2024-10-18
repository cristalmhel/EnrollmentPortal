using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnrollmentPortal.Migrations
{
    /// <inheritdoc />
    public partial class updatestudentid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "StudentFiles",
                newName: "StudId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudId",
                table: "StudentFiles",
                newName: "Id");
        }
    }
}
