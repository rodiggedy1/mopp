using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedJobIdNotNullableInJobApplicationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplication_JobDetails_JobDetailsId",
                table: "JobApplication");

            migrationBuilder.AlterColumn<int>(
                name: "JobDetailsId",
                table: "JobApplication",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplication_JobDetails_JobDetailsId",
                table: "JobApplication",
                column: "JobDetailsId",
                principalTable: "JobDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplication_JobDetails_JobDetailsId",
                table: "JobApplication");

            migrationBuilder.AlterColumn<int>(
                name: "JobDetailsId",
                table: "JobApplication",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplication_JobDetails_JobDetailsId",
                table: "JobApplication",
                column: "JobDetailsId",
                principalTable: "JobDetails",
                principalColumn: "Id");
        }
    }
}
