using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreatedJobsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobForm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobForm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    EmploymentType = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    JobFormId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobDetails_JobForm_JobFormId",
                        column: x => x.JobFormId,
                        principalTable: "JobForm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobApplication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    EmailAddress = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    StreetAddress = table.Column<string>(type: "text", nullable: true),
                    Apartment = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    ZipCode = table.Column<string>(type: "text", nullable: true),
                    HasCleaningExperience = table.Column<bool>(type: "boolean", nullable: true),
                    HasBankAccountForDirectDeposit = table.Column<bool>(type: "boolean", nullable: true),
                    IsLegallyAuthorizedToWorkInUS = table.Column<bool>(type: "boolean", nullable: true),
                    AllowsConsentToFreeBackgroundCheck = table.Column<bool>(type: "boolean", nullable: true),
                    CleaningExperienceDescription = table.Column<string>(type: "text", nullable: true),
                    Media = table.Column<string>(type: "text", nullable: false),
                    JobDetailsId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplication_JobDetails_JobDetailsId",
                        column: x => x.JobDetailsId,
                        principalTable: "JobDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobApplication_JobDetailsId",
                table: "JobApplication",
                column: "JobDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_JobDetails_JobFormId",
                table: "JobDetails",
                column: "JobFormId");

            migrationBuilder.Sql(@"
                ALTER TABLE ""JobForm"" DISABLE ROW LEVEL SECURITY;
                ALTER TABLE ""JobDetails"" DISABLE ROW LEVEL SECURITY;
                ALTER TABLE ""JobApplication"" DISABLE ROW LEVEL SECURITY;
    
                GRANT SELECT, INSERT, UPDATE, DELETE ON ""JobForm"" TO app_user;
                GRANT SELECT, INSERT, UPDATE, DELETE ON ""JobDetails"" TO app_user;
                GRANT SELECT, INSERT, UPDATE, DELETE ON ""JobApplication"" TO app_user;
    
                GRANT USAGE, SELECT ON SEQUENCE ""JobForm_Id_seq"" TO app_user;
                GRANT USAGE, SELECT ON SEQUENCE ""JobDetails_Id_seq"" TO app_user;
                GRANT USAGE, SELECT ON SEQUENCE ""JobApplication_Id_seq"" TO app_user;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobApplication");

            migrationBuilder.DropTable(
                name: "JobDetails");

            migrationBuilder.DropTable(
                name: "JobForm");
        }
    }
}
