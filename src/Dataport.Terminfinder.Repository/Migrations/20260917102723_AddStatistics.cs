using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Dataport.Terminfinder.Repository.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class AddStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlInhalt = "update public.appconfig set configvalue='1.4.1' WHERE configkey='version';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

            sqlInhalt = "update public.appconfig set configvalue='2026-09-17' WHERE configkey='builddate';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

            migrationBuilder.CreateTable(
                name: "appointmentstatistic",
                schema: "public",
                columns: table => new
                {
                    appointmentstatisticid = table.Column<Guid>(type: "uuid", nullable: false),
                    customerid = table.Column<Guid>(type: "uuid", nullable: false),
                    yearmonth = table.Column<DateOnly>(type: "date", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointmentstatistic", x => x.appointmentstatisticid);
                });

            migrationBuilder.CreateTable(
                name: "participantstatistic",
                schema: "public",
                columns: table => new
                {
                    participantstatisticid = table.Column<Guid>(type: "uuid", nullable: false),
                    customerid = table.Column<Guid>(type: "uuid", nullable: false),
                    yearmonth = table.Column<DateOnly>(type: "date", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_participantstatistic", x => x.participantstatisticid);
                });

            migrationBuilder.CreateTable(
                name: "votingstatistic",
                schema: "public",
                columns: table => new
                {
                    votingstatisticid = table.Column<Guid>(type: "uuid", nullable: false),
                    customerid = table.Column<Guid>(type: "uuid", nullable: false),
                    yearmonth = table.Column<DateOnly>(type: "date", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_votingstatistic", x => x.votingstatisticid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointmentstatistic_customerid_yearmonth",
                schema: "public",
                table: "appointmentstatistic",
                columns: new[] { "customerid", "yearmonth" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_participantstatistic_customerid_yearmonth",
                schema: "public",
                table: "participantstatistic",
                columns: new[] { "customerid", "yearmonth" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_votingstatistic_customerid_yearmonth",
                schema: "public",
                table: "votingstatistic",
                columns: new[] { "customerid", "yearmonth" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sqlInhalt = "update public.appconfig set configvalue='1.3.0' WHERE configkey='version';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

            sqlInhalt = "update public.appconfig set configvalue='2025-11-06' WHERE configkey='builddate';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

            migrationBuilder.DropTable(
                name: "appointmentstatistic",
                schema: "public");

            migrationBuilder.DropTable(
                name: "participantstatistic",
                schema: "public");

            migrationBuilder.DropTable(
                name: "votingstatistic",
                schema: "public");
        }
    }
}
