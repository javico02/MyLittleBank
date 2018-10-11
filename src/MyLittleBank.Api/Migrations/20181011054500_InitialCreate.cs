using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyLittleBank.Api.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(nullable: true),
                    Balance = table.Column<decimal>(nullable: false),
                    IsLocked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BankAccounts",
                columns: new[] { "Id", "Balance", "IsLocked", "Number" },
                values: new object[] { 1, 102.45m, false, "PBO1221323" });

            migrationBuilder.InsertData(
                table: "BankAccounts",
                columns: new[] { "Id", "Balance", "IsLocked", "Number" },
                values: new object[] { 2, 10002.98m, false, "PIC9984567" });

            migrationBuilder.InsertData(
                table: "BankAccounts",
                columns: new[] { "Id", "Balance", "IsLocked", "Number" },
                values: new object[] { 3, 677.71m, true, "PAC0784412" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankAccounts");
        }
    }
}
