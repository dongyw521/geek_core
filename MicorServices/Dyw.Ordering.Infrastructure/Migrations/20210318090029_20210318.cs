using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dyw.Ordering.Infrastructure.Migrations
{
    public partial class _20210318 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(maxLength: 100, nullable: true),
                    UserName = table.Column<string>(maxLength: 200, nullable: true),
                    ItemCount = table.Column<long>(nullable: false),
                    Address_Street = table.Column<string>(maxLength: 300, nullable: true),
                    Address_City = table.Column<string>(maxLength: 20, nullable: true),
                    Address_ZipCode = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order");
        }
    }
}
