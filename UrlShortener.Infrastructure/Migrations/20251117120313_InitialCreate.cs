using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UrlShortener.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.CreateTable(
            //     name: "UrlMappings",
            //     columns: table => new
            //     {
            //         Id = table.Column<long>(type: "bigint", nullable: false)
            //             .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //         OriginalUrl = table.Column<string>(type: "text", nullable: false),
            //         ShortenedUrl = table.Column<string>(type: "text", nullable: false),
            //         ClickCount = table.Column<int>(type: "integer", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_UrlMappings", x => x.Id);
            //     });
            //
            // migrationBuilder.InsertData(
            //     table: "UrlMappings",
            //     columns: new[] { "Id", "ClickCount", "OriginalUrl", "ShortenedUrl" },
            //     values: new object[,]
            //     {
            //         { 1L, 0, "https://www.postgresql.org/download/windows/", "abcdef" },
            //         { 2L, 0, "https://www.postgresql.org/download/", "123456" },
            //         { 3L, 0, "https://www.postgresql.org/", "xyz123" }
            //     });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UrlMappings");
        }
    }
}
