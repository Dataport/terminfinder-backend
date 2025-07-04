using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Dataport.Terminfinder.Repository.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class ChangeVersionNumberTo_1_2_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlInhalt = "insert into public.appconfig(configkey, configvalue) select 'version','' WHERE NOT EXISTS(select 1 from public.appconfig WHERE configkey='version');";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

            sqlInhalt = "insert into public.appconfig(configkey, configvalue) select 'builddate','' WHERE NOT EXISTS(select 1 from public.appconfig WHERE configkey='builddate');";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

            sqlInhalt = "update public.appconfig set configvalue='1.2.2' WHERE configkey='version';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

            sqlInhalt = "update public.appconfig set configvalue='2025-07-04' WHERE configkey='builddate';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);
        }
    }
}
