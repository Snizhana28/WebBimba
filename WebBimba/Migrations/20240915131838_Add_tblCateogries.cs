using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebBimba.Migrations
{
    /// <inheritdoc />
    public partial class Add_tblCateogries : Migration //створюємо нову міграцію
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) //метод для створення таблиці
        {
            migrationBuilder.CreateTable(
                name: "tbl_categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Image = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_categories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) //метод для видалення таблиці
        {
            migrationBuilder.DropTable(
                name: "tbl_categories");
        }
    }
}
