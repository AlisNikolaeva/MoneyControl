using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyControl.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Account_AccountId",
                schema: "dbo",
                table: "Transaction");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                schema: "dbo",
                table: "Transaction",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "dbo",
                table: "Account",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Account_Name",
                schema: "dbo",
                table: "Account",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Account_AccountId",
                schema: "dbo",
                table: "Transaction",
                column: "AccountId",
                principalSchema: "dbo",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Account_AccountId",
                schema: "dbo",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Account_Name",
                schema: "dbo",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "dbo",
                table: "Account");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                schema: "dbo",
                table: "Transaction",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Account_AccountId",
                schema: "dbo",
                table: "Transaction",
                column: "AccountId",
                principalSchema: "dbo",
                principalTable: "Account",
                principalColumn: "Id");
        }
    }
}
