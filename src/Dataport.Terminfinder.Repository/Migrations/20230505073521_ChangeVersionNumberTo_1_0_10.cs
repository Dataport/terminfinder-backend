using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Dataport.Terminfinder.Repository.Migrations
{
    /// <summary>
    /// DB Migration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class ChangeVersionNumberTo_1_0_10 : Migration
    {
        /// <summary>
        /// BuildTargetModel
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlInhalt = "insert into public.appconfig(configkey, configvalue) select 'version','' WHERE NOT EXISTS(select 1 from public.appconfig WHERE configkey='version');";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

            sqlInhalt = "insert into public.appconfig(configkey, configvalue) select 'builddate','' WHERE NOT EXISTS(select 1 from public.appconfig WHERE configkey='builddate');";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

            sqlInhalt = "update public.appconfig set configvalue='1.0.10' WHERE configkey='version';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

            sqlInhalt = "update public.appconfig set configvalue='2023-05-05' WHERE configkey='builddate';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);
        }
    }
}
