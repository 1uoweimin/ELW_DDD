using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileService.Infrastructure.Pg_Migrations
{
    /// <inheritdoc />
    public partial class FileServiceInital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_FS_UploadedItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FileName = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileSHA256Hash = table.Column<string>(type: "character varying(64)", unicode: false, maxLength: 64, nullable: false),
                    BackupUrl = table.Column<string>(type: "text", nullable: false),
                    RemoveUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_FS_UploadedItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_FS_UploadedItem_FileSizeInBytes_FileSHA256Hash",
                table: "T_FS_UploadedItem",
                columns: new[] { "FileSizeInBytes", "FileSHA256Hash" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_FS_UploadedItem");
        }
    }
}
