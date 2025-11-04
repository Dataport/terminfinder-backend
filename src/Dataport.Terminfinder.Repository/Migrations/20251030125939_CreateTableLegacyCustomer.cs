using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Dataport.Terminfinder.Repository.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class CreateTableLegacyCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "legacycustomer",
                schema: "public",
                columns: table => new
                {
                    customerid = table.Column<Guid>(type: "uuid", nullable: false),
                    hostaddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    creationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("legacycustomer_pkey", x => x.customerid);
                });
            
            var sqlInhalt = "update public.appconfig set configvalue='1.3.0' WHERE configkey='version';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

            sqlInhalt = "update public.appconfig set configvalue='2025-10-30' WHERE configkey='builddate';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "legacycustomer",
                schema: "public");
            
            var sqlInhalt = "update public.appconfig set configvalue='1.2.2' WHERE configkey='version';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

            sqlInhalt = "update public.appconfig set configvalue='2025-07-04' WHERE configkey='builddate';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);
        }
    }
}
