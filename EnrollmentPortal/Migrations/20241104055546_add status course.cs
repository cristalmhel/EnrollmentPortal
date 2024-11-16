using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnrollmentPortal.Migrations
{
    /// <inheritdoc />
    public partial class addstatuscourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleInitial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentFiles",
                columns: table => new
                {
                    StudId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    STFSTUDLNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    STFSTUDFNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    STFSTUDMNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    STFSTUDYEAR = table.Column<int>(type: "int", nullable: false),
                    STFSTUDREMARKS = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    STFSTUDSTATUS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentFiles", x => x.StudId);
                    table.ForeignKey(
                        name: "FK_StudentFiles_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SFSUBJCODE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SFSUBJDESC = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SFSUBJUNITS = table.Column<int>(type: "int", nullable: false),
                    SFSUBJREGOFRNG = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SFSUBJSCHLYR = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SFSUBJCATEGORY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SFSUBJSTATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SFSUBJCURRCODE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectFiles_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EnrollmentHeaderFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ENRHFSTUDDATEENROLL = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ENRHFSTUDSCHLYR = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ENRHFSTUDSEM = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ENRHFSTUDENCODER = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ENRHFSTUDTOTALUNITS = table.Column<double>(type: "float", nullable: false),
                    ENRHFSTUDSTATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StudentFileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentHeaderFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnrollmentHeaderFiles_StudentFiles_StudentFileId",
                        column: x => x.StudentFileId,
                        principalTable: "StudentFiles",
                        principalColumn: "StudId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectPreqFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectFileId = table.Column<int>(type: "int", nullable: false),
                    PrerequisiteSubjectId = table.Column<int>(type: "int", nullable: false),
                    SUBJCATEGORY = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SubjectFileId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectPreqFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectPreqFiles_SubjectFiles_PrerequisiteSubjectId",
                        column: x => x.PrerequisiteSubjectId,
                        principalTable: "SubjectFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectPreqFiles_SubjectFiles_SubjectFileId",
                        column: x => x.SubjectFileId,
                        principalTable: "SubjectFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectPreqFiles_SubjectFiles_SubjectFileId1",
                        column: x => x.SubjectFileId1,
                        principalTable: "SubjectFiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubjectSchedFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SSFEDPCODE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SubjectFileId = table.Column<int>(type: "int", nullable: false),
                    SSFSTARTTIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SSFENDTIME = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SSFDAYS = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SSFROOM = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SSFMAXSIZE = table.Column<int>(type: "int", nullable: false),
                    SSFCLASSSIZE = table.Column<int>(type: "int", nullable: false),
                    SSFSTATUS = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SSFXM = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    SSFSECTION = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SSFSCHOOLYEAR = table.Column<int>(type: "int", nullable: false),
                    SSFSSEM = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectSchedFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectSchedFiles_SubjectFiles_SubjectFileId",
                        column: x => x.SubjectFileId,
                        principalTable: "SubjectFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnrollmentDetailFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentHeaderFileId = table.Column<int>(type: "int", nullable: false),
                    SubjectFileId = table.Column<int>(type: "int", nullable: false),
                    SubjectSchedFileId = table.Column<int>(type: "int", nullable: false),
                    ENRDFSTUDSTATUS = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentDetailFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnrollmentDetailFiles_EnrollmentHeaderFiles_EnrollmentHeaderFileId",
                        column: x => x.EnrollmentHeaderFileId,
                        principalTable: "EnrollmentHeaderFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnrollmentDetailFiles_SubjectFiles_SubjectFileId",
                        column: x => x.SubjectFileId,
                        principalTable: "SubjectFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EnrollmentDetailFiles_SubjectSchedFiles_SubjectSchedFileId",
                        column: x => x.SubjectSchedFileId,
                        principalTable: "SubjectSchedFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentDetailFiles_EnrollmentHeaderFileId",
                table: "EnrollmentDetailFiles",
                column: "EnrollmentHeaderFileId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentDetailFiles_SubjectFileId",
                table: "EnrollmentDetailFiles",
                column: "SubjectFileId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentDetailFiles_SubjectSchedFileId",
                table: "EnrollmentDetailFiles",
                column: "SubjectSchedFileId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentHeaderFiles_StudentFileId",
                table: "EnrollmentHeaderFiles",
                column: "StudentFileId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFiles_CourseId",
                table: "StudentFiles",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectFiles_CourseId",
                table: "SubjectFiles",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectPreqFiles_PrerequisiteSubjectId",
                table: "SubjectPreqFiles",
                column: "PrerequisiteSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectPreqFiles_SubjectFileId",
                table: "SubjectPreqFiles",
                column: "SubjectFileId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectPreqFiles_SubjectFileId1",
                table: "SubjectPreqFiles",
                column: "SubjectFileId1");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectSchedFiles_SubjectFileId",
                table: "SubjectSchedFiles",
                column: "SubjectFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnrollmentDetailFiles");

            migrationBuilder.DropTable(
                name: "SubjectPreqFiles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "EnrollmentHeaderFiles");

            migrationBuilder.DropTable(
                name: "SubjectSchedFiles");

            migrationBuilder.DropTable(
                name: "StudentFiles");

            migrationBuilder.DropTable(
                name: "SubjectFiles");

            migrationBuilder.DropTable(
                name: "Courses");
        }
    }
}
