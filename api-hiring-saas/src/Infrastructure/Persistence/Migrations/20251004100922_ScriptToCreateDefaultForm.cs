using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ScriptToCreateDefaultForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.Sql(@"
                WITH new_forms AS (
                    INSERT INTO ""JobForm""
                        (""Title"", ""Description"", ""Created"", ""CreatedBy"", ""LastModified"", ""LastModifiedBy"", ""UniqueHash"")
                    SELECT
                        'Cleaning Services',
                        'Ready-made form for residential and commercial cleaning positions',
                        now(),
                        u.""Id"",
                        now(),
                        u.""Id"",
		                (SELECT string_agg(substr('ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789', (random()*35+1)::int, 1), '')
		                 FROM generate_series(1,6))
 	                FROM ""AspNetUsers"" u
                    JOIN ""AspNetUserRoles"" ur ON ur.""UserId"" = u.""Id""
                    WHERE ur.""RoleId"" = 1
                      AND NOT EXISTS (
                          SELECT 1
                          FROM ""JobForm"" jf
                          WHERE jf.""CreatedBy"" = u.""Id""
                      )
                    RETURNING ""Id"" AS ""JobFormId"", ""CreatedBy"", ""UniqueHash""
                )
                INSERT INTO ""JobDetails""
                    (""Title"", ""Description"", ""Location"", ""EmploymentType"", ""Price"",
                     ""JobFormId"", ""Created"", ""CreatedBy"", ""LastModified"", ""LastModifiedBy"", ""UniqueHash"")
                SELECT
                    'Cleaning Professional Application',
                    'Join our team of cleaning professionals',
                    'Mumbai',
                    'Full Time',
                    10000,
                    nf.""JobFormId"",
                    now(),
                    nf.""CreatedBy"",
                    now(),
                    nf.""CreatedBy"",
                    nf.""UniqueHash""
                FROM new_forms nf;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
