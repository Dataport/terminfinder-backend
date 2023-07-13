using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dataport.Terminfinder.Repository.Migrations
{
    /// <summary>
    /// DB Migration
    /// </summary>
    public partial class Create_Extention_for_uuid_v4 : Migration
    {
        /// <summary>
        /// BuildTargetModel
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.IsNpgsql())
            {
                var sqlInhalt = "CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";";
                migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);
            }
        }
    }
}
