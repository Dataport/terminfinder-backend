using Dataport.Terminfinder.BusinessObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

#nullable disable

namespace Dataport.Terminfinder.Repository.Migrations
{
    /// <summary>
    /// DB Migration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class AlterTables_uuid_generate : Migration
    {
        /// <summary>
        /// BuildTargetModel
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.IsNpgsql())
            {
                var sqlInhalt = "ALTER TABLE public.customer ALTER COLUMN customerid SET DEFAULT uuid_generate_v4();";
                migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

                sqlInhalt = "ALTER TABLE public.appointment ALTER COLUMN appointmentid SET DEFAULT uuid_generate_v4();";
                migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

                sqlInhalt = "ALTER TABLE public.appointment ALTER COLUMN adminid SET DEFAULT uuid_generate_v4();";
                migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

                sqlInhalt =
                    "ALTER TABLE public.suggesteddate ALTER COLUMN suggesteddateid SET DEFAULT uuid_generate_v4();";
                migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

                sqlInhalt = "ALTER TABLE public.participant ALTER COLUMN participantid SET DEFAULT uuid_generate_v4();";
                migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

                sqlInhalt = "ALTER TABLE public.voting ALTER COLUMN votingid SET DEFAULT uuid_generate_v4();";
                migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);
            }
        }
    }
}
