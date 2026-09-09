using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSecretaryAndRefactorRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Doctors_DoctorMedicalId",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_DoctorMedicalId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "DoctorMedicalId",
                table: "RefreshTokens");

            migrationBuilder.AddColumn<string>(
                name: "UserIdentifier",
                table: "RefreshTokens",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Secretaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secretaries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserIdentifier",
                table: "RefreshTokens",
                column: "UserIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Secretaries_UserName",
                table: "Secretaries",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Secretaries");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_UserIdentifier",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UserIdentifier",
                table: "RefreshTokens");

            migrationBuilder.AddColumn<string>(
                name: "DoctorMedicalId",
                table: "RefreshTokens",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_DoctorMedicalId",
                table: "RefreshTokens",
                column: "DoctorMedicalId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Doctors_DoctorMedicalId",
                table: "RefreshTokens",
                column: "DoctorMedicalId",
                principalTable: "Doctors",
                principalColumn: "MedicalId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
