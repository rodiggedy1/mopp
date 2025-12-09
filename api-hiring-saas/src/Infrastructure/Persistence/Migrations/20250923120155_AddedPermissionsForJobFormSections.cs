using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedPermissionsForJobFormSections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""JobFormSection"" DISABLE ROW LEVEL SECURITY;
    
                GRANT SELECT, INSERT, UPDATE, DELETE ON ""JobFormSection"" TO app_user;
    
                GRANT USAGE, SELECT ON SEQUENCE ""JobFormSection_Id_seq"" TO app_user;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
