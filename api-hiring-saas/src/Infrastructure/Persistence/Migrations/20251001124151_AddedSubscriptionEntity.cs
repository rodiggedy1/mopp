using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubscriptionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransaction_AspNetUsers_UserId",
                table: "PaymentTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentTransaction",
                table: "PaymentTransaction");

            migrationBuilder.DropColumn(
                name: "HasActivePaidSubscription",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "PaymentTransaction",
                newName: "PaymentTransactions");

            migrationBuilder.RenameColumn(
                name: "NextPaymentDueDate",
                table: "AspNetUsers",
                newName: "TrialStartedAt");

            migrationBuilder.RenameColumn(
                name: "LastPaymentDate",
                table: "AspNetUsers",
                newName: "TrialEndsAt");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentTransaction_UserId",
                table: "PaymentTransactions",
                newName: "IX_PaymentTransactions_UserId");

            migrationBuilder.AddColumn<string>(
                name: "StripeSubscriptionId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionEndsAt",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionStartedAt",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionStatus",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StripePaymentIntentId",
                table: "PaymentTransactions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "StripeInvoiceId",
                table: "PaymentTransactions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StripeCustomerId",
                table: "PaymentTransactions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FailureReason",
                table: "PaymentTransactions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "PaymentTransactions",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "usd",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "PaymentTransactions",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentTransactions",
                table: "PaymentTransactions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_CreatedBy",
                table: "PaymentTransactions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_LastModifiedBy",
                table: "PaymentTransactions",
                column: "LastModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_StripePaymentIntentId",
                table: "PaymentTransactions",
                column: "StripePaymentIntentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_AspNetUsers_CreatedBy",
                table: "PaymentTransactions",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_AspNetUsers_LastModifiedBy",
                table: "PaymentTransactions",
                column: "LastModifiedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_AspNetUsers_UserId",
                table: "PaymentTransactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"
                ALTER TABLE ""PaymentTransactions"" DISABLE ROW LEVEL SECURITY;
    
                GRANT SELECT, INSERT, UPDATE, DELETE ON ""PaymentTransactions"" TO app_user;
            ");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_AspNetUsers_CreatedBy",
                table: "PaymentTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_AspNetUsers_LastModifiedBy",
                table: "PaymentTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_AspNetUsers_UserId",
                table: "PaymentTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentTransactions",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_CreatedBy",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_LastModifiedBy",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_StripePaymentIntentId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "StripeSubscriptionId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubscriptionEndsAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubscriptionStartedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubscriptionStatus",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "PaymentTransactions",
                newName: "PaymentTransaction");

            migrationBuilder.RenameColumn(
                name: "TrialStartedAt",
                table: "AspNetUsers",
                newName: "NextPaymentDueDate");

            migrationBuilder.RenameColumn(
                name: "TrialEndsAt",
                table: "AspNetUsers",
                newName: "LastPaymentDate");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentTransactions_UserId",
                table: "PaymentTransaction",
                newName: "IX_PaymentTransaction_UserId");

            migrationBuilder.AddColumn<bool>(
                name: "HasActivePaidSubscription",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "StripePaymentIntentId",
                table: "PaymentTransaction",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "StripeInvoiceId",
                table: "PaymentTransaction",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StripeCustomerId",
                table: "PaymentTransaction",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FailureReason",
                table: "PaymentTransaction",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "PaymentTransaction",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "usd");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "PaymentTransaction",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentTransaction",
                table: "PaymentTransaction",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransaction_AspNetUsers_UserId",
                table: "PaymentTransaction",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
