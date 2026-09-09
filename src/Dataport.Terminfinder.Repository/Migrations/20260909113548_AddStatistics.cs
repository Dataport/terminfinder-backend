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
            var sqlInhalt = "update public.appconfig set configvalue='1.4.0' WHERE configkey='version';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

            sqlInhalt = "update public.appconfig set configvalue='2026-09-09' WHERE configkey='builddate';";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);

            migrationBuilder.CreateTable(
                name: "appointmentStatistic",
                schema: "public",
                columns: table => new
                {
                    appointmentStatisticId = table.Column<Guid>(type: "uuid", nullable: false),
                    customerId = table.Column<Guid>(type: "uuid", nullable: false),
                    yearMonth = table.Column<DateOnly>(type: "date", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointmentStatistic", x => x.appointmentStatisticId);
                });

            migrationBuilder.CreateTable(
                name: "participantStatistic",
                schema: "public",
                columns: table => new
                {
                    participantStatisticId = table.Column<Guid>(type: "uuid", nullable: false),
                    customerId = table.Column<Guid>(type: "uuid", nullable: false),
                    yearMonth = table.Column<DateOnly>(type: "date", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_participantStatistic", x => x.participantStatisticId);
                });

            migrationBuilder.CreateTable(
                name: "votingStatistic",
                schema: "public",
                columns: table => new
                {
                    votingStatisticId = table.Column<Guid>(type: "uuid", nullable: false),
                    customerId = table.Column<Guid>(type: "uuid", nullable: false),
                    yearMonth = table.Column<DateOnly>(type: "date", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_votingStatistic", x => x.votingStatisticId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointmentStatistic_customerId_yearMonth",
                schema: "public",
                table: "appointmentStatistic",
                columns: new[] { "customerId", "yearMonth" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_participantStatistic_customerId_yearMonth",
                schema: "public",
                table: "participantStatistic",
                columns: new[] { "customerId", "yearMonth" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_votingStatistic_customerId_yearMonth",
                schema: "public",
                table: "votingStatistic",
                columns: new[] { "customerId", "yearMonth" },
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
                name: "appointmentStatistic",
                schema: "public");

            migrationBuilder.DropTable(
                name: "participantStatistic",
                schema: "public");

            migrationBuilder.DropTable(
                name: "votingStatistic",
                schema: "public");
        }
    }
}
