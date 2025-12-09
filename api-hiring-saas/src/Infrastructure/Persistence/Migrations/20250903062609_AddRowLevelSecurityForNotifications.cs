using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRowLevelSecurityForNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Notification"" ENABLE ROW LEVEL SECURITY;
                ALTER TABLE ""Notification"" FORCE ROW LEVEL SECURITY;

                DROP POLICY IF EXISTS notification_owner_select ON ""Notification"";

                CREATE POLICY notification_owner_select
                ON ""Notification""
                FOR SELECT
                USING (
                    current_setting('app.is_admin', true)::boolean = true
                    OR ""UserId"" = current_setting('app.current_user_id')::int
                );

                DROP POLICY IF EXISTS notification_owner_update ON ""Notification"";

                CREATE POLICY notification_owner_update
                ON ""Notification""
                FOR UPDATE
                USING (
                    current_setting('app.is_admin', true)::boolean = true
                    OR ""UserId"" = current_setting('app.current_user_id')::int
                )
                WITH CHECK (
                    current_setting('app.is_admin', true)::boolean = true
                    OR ""UserId"" = current_setting('app.current_user_id')::int
                );

                CREATE ROLE app_user LOGIN PASSWORD 'StrongPassword123';

                GRANT CONNECT ON DATABASE saas TO app_user;
                GRANT USAGE ON SCHEMA public TO app_user;

                GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO app_user;

                ALTER ROLE app_user NOBYPASSRLS;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
