using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MareSynchronosServer.Migrations
{
    /// <inheritdoc />
    public partial class AddManifestsAndPex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "manifests",
                columns: table => new
                {
                    file_hash = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    magnet = table.Column<string>(type: "text", nullable: false),
                    piece_hashes_json = table.Column<string>(type: "text", nullable: false),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_manifests", x => x.file_hash);
                    table.ForeignKey(
                        name: "fk_manifests_files_file_hash",
                        column: x => x.file_hash,
                        principalTable: "file_caches",
                        principalColumn: "hash",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "p2p_peers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_hash = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    user_uid = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    area = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    port = table.Column<int>(type: "integer", nullable: false),
                    last_seen_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_p2p_peers", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_manifests_created_utc",
                table: "manifests",
                column: "created_utc");

            migrationBuilder.CreateIndex(
                name: "ix_p2p_peers_area",
                table: "p2p_peers",
                column: "area");

            migrationBuilder.CreateIndex(
                name: "ix_p2p_peers_file_hash",
                table: "p2p_peers",
                column: "file_hash");

            migrationBuilder.CreateIndex(
                name: "ix_p2p_peers_last_seen_utc",
                table: "p2p_peers",
                column: "last_seen_utc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "manifests");

            migrationBuilder.DropTable(
                name: "p2p_peers");
        }
    }
}
