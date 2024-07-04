using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace Infrastructure.Extensions;

public static class MigrationBuilderExtensions
{
    public static OperationBuilder<SqlOperation> AddDeferredUniqueConstraint(this MigrationBuilder migrationBuilder,
        string tableName, string[] columns) =>
        migrationBuilder.ActiveProvider switch
        {
            "Npgsql.EntityFrameworkCore.PostgreSQL" => migrationBuilder.Sql(
                $"ALTER TABLE {tableName} ADD CONSTRAINT uq_{tableName}_{string.Join("_", columns)} UNIQUE ({string.Join(", ", columns)}) DEFERRABLE INITIALLY DEFERRED"),
            _ => throw new Exception("Unexpected provider.")
        };
}