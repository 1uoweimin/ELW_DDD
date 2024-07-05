using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListeningService.Infrastructure.Pg_Migrations
{
    /// <inheritdoc />
    public partial class ListeningServiceInital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Listening_Album",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false),
                    Name_Chinese = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name_English = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsVisible = table.Column<bool>(type: "boolean", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Listening_Album", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_Listening_Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false),
                    Name_Chinese = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name_English = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CoverUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Listening_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_Listening_Episode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false),
                    Name_Chinese = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name_English = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsVisible = table.Column<bool>(type: "boolean", nullable: false),
                    AlbumId = table.Column<Guid>(type: "uuid", nullable: false),
                    AudioUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DurationInSecond = table.Column<double>(type: "double precision", nullable: false),
                    Subtitle_Content = table.Column<string>(type: "text", maxLength: 2147483647, nullable: false),
                    Subtitle_Type = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Listening_Episode", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_Listening_Album_CategoryId_IsDeleted",
                table: "T_Listening_Album",
                columns: new[] { "CategoryId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_T_Listening_Episode_AlbumId_IsDeleted",
                table: "T_Listening_Episode",
                columns: new[] { "AlbumId", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Listening_Album");

            migrationBuilder.DropTable(
                name: "T_Listening_Categories");

            migrationBuilder.DropTable(
                name: "T_Listening_Episode");
        }
    }
}
