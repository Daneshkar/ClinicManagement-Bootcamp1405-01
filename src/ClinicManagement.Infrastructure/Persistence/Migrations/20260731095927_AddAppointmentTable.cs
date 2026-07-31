using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorMedicalId = table.Column<string>(type: "varchar(20)", nullable: false),
                    PatientNationalCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Prescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Doctors_DoctorMedicalId",
                        column: x => x.DoctorMedicalId,
                        principalTable: "Doctors",
                        principalColumn: "MedicalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Patients_PatientNationalCode",
                        column: x => x.PatientNationalCode,
                        principalTable: "Patients",
                        principalColumn: "NationalCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorMedicalId_VisitDate",
                table: "Appointments",
                columns: new[] { "DoctorMedicalId", "VisitDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientNationalCode",
                table: "Appointments",
                column: "PatientNationalCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");
        }
    }
}
