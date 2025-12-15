using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedCalendlyCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CalendlyAccessToken",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalendlyClientId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalendlyClientSecret",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalendlyCode",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalendlyRedirectUri",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalendlyAccessToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CalendlyClientId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CalendlyClientSecret",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CalendlyCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CalendlyRedirectUri",
                table: "AspNetUsers");
        }
    }
}
