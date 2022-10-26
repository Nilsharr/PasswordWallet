using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PasswordWallet.Server.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    salt = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    is_password_kept_as_hash = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "credentials",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    password = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    login = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    web_address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    account_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_credentials", x => x.id);
                    table.ForeignKey(
                        name: "fk_credentials_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_account_login",
                table: "account",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_credentials_account_id",
                table: "credentials",
                column: "account_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "credentials");

            migrationBuilder.DropTable(
                name: "account");
        }
    }
}
