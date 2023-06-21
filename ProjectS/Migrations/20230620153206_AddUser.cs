using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    public partial class AddUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            for(int i=1; i<100;i++)
            {
                migrationBuilder.InsertData(
                    "Users",
                    columns: new[] {
                    "Id",
                    "UserName",
                    "Email",
                    "SecurityStamp",
                    "EmailConfirmed",
                    "PhoneNumberConfirmed",
                    "TwoFactorEnabled",
                    "LockoutEnabled",
                    "AccessFailedCount",
                    },
                    values: new object[] {
                    Guid.NewGuid().ToString(),
                    "User-"+i.ToString("D3"),
                    $"email{i.ToString("D3")}@exaple.com",
					Guid.NewGuid().ToString(),
                    true,
                    false,
                    false,
                    false,
                    0



					}
					);
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
