using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PasswordWallet.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    passwordhash = table.Column<string>(name: "password_hash", type: "character varying(512)", maxLength: 512, nullable: false),
                    salt = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ispasswordkeptashash = table.Column<bool>(name: "is_password_kept_as_hash", type: "boolean", nullable: false),
                    subsequentbadlogins = table.Column<int>(name: "subsequent_bad_logins", type: "integer", nullable: false, defaultValue: 0),
                    lockouttime = table.Column<DateTime>(name: "lockout_time", type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "login_ip_address",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ipaddress = table.Column<IPAddress>(name: "ip_address", type: "inet", nullable: false),
                    amountofgoodlogins = table.Column<long>(name: "amount_of_good_logins", type: "bigint", nullable: false, defaultValue: 0L),
                    amountofbadlogins = table.Column<long>(name: "amount_of_bad_logins", type: "bigint", nullable: false, defaultValue: 0L),
                    subsequentbadlogins = table.Column<int>(name: "subsequent_bad_logins", type: "integer", nullable: false, defaultValue: 0),
                    temporarylock = table.Column<DateTime>(name: "temporary_lock", type: "timestamp with time zone", nullable: true),
                    permanentlock = table.Column<bool>(name: "permanent_lock", type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_login_ip_address", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "credential",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    password = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    login = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    webaddress = table.Column<string>(name: "web_address", type: "character varying(256)", maxLength: 256, nullable: true),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_credential", x => x.id);
                    table.ForeignKey(
                        name: "fk_credential_account_account_id",
                        column: x => x.accountid,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_login",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    correct = table.Column<bool>(type: "boolean", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    loginipaddressid = table.Column<long>(name: "login_ip_address_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_account_login", x => x.id);
                    table.ForeignKey(
                        name: "fk_account_login_account_account_id",
                        column: x => x.accountid,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_account_login_login_ip_address_login_ip_address_id",
                        column: x => x.loginipaddressid,
                        principalTable: "login_ip_address",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_account_login",
                table: "account",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_account_login_account_id",
                table: "account_login",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_account_login_login_ip_address_id",
                table: "account_login",
                column: "login_ip_address_id");

            migrationBuilder.CreateIndex(
                name: "ix_credential_account_id",
                table: "credential",
                column: "account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_login");

            migrationBuilder.DropTable(
                name: "credential");

            migrationBuilder.DropTable(
                name: "login_ip_address");

            migrationBuilder.DropTable(
                name: "account");
        }
    }
}
