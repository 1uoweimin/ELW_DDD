using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaEncoderService.Infrastructure.Pg_Migrations
{
    /// <inheritdoc />
    public partial class MediaEncoderServiceInital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_ME_EncodingItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: true),
                    FileSHA256Hash = table.Column<string>(type: "character varying(64)", unicode: false, maxLength: 64, nullable: true),
                    SourceSystem = table.Column<string>(type: "text", nullable: false),
                    SourceUrl = table.Column<string>(type: "text", nullable: false),
                    OutputUrl = table.Column<string>(type: "text", nullable: true),
                    OutputFormat = table.Column<string>(type: "character varying(10)", unicode: false, maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    LogText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_ME_EncodingItems", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_ME_EncodingItems");
        }
    }
}
