using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dataport.Terminfinder.Repository.Migrations
{
    public partial class CreateApp : Migration
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

            sqlInhalt = "update public.appconfig set configvalue='1.0.9' WHERE configkey='version';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

            sqlInhalt = "update public.appconfig set configvalue='2022-12-01' WHERE configkey='builddate';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);
        }
    }
}
