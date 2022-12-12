using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dataport.Terminfinder.Repository.Migrations
{
    /// <summary>
    /// Migrations InitialCreate
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class InitialCreate : Migration
    {
        /// <summary>
        /// Up
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "appconfig",
                schema: "public",
                columns: table => new
                {
                    configkey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    configvalue = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("appconfig_pkey", x => x.configkey);
                });

            migrationBuilder.CreateTable(
                name: "customer",
                schema: "public",
                columns: table => new
                {
                    customerid = table.Column<Guid>(type: "uuid", nullable: false),
                    customername = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    creationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("customer_pkey", x => x.customerid);
                });

            migrationBuilder.CreateTable(
                name: "appointment",
                schema: "public",
                columns: table => new
                {
                    appointmentid = table.Column<Guid>(type: "uuid", nullable: false),
                    customerid = table.Column<Guid>(type: "uuid", nullable: false),
                    adminid = table.Column<Guid>(type: "uuid", nullable: false),
                    creatorname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    creationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    subject = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    description = table.Column<string>(type: "character varying(1500)", maxLength: 1500, nullable: true),
                    place = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    password = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("appointment_pkey", x => x.appointmentid);
                    table.ForeignKey(
                        name: "appointment_customerid_fkey",
                        column: x => x.customerid,
                        principalSchema: "public",
                        principalTable: "customer",
                        principalColumn: "customerid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "participant",
                schema: "public",
                columns: table => new
                {
                    participantid = table.Column<Guid>(type: "uuid", nullable: false),
                    appointmentid = table.Column<Guid>(type: "uuid", nullable: false),
                    customerid = table.Column<Guid>(type: "uuid", nullable: false),
                    creationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("participant_pkey", x => x.participantid);
                    table.ForeignKey(
                        name: "participant_appointmentid_fkey",
                        column: x => x.appointmentid,
                        principalSchema: "public",
                        principalTable: "appointment",
                        principalColumn: "appointmentid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "participant_customerid_fkey",
                        column: x => x.customerid,
                        principalSchema: "public",
                        principalTable: "customer",
                        principalColumn: "customerid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "suggesteddate",
                schema: "public",
                columns: table => new
                {
                    suggesteddateid = table.Column<Guid>(type: "uuid", nullable: false),
                    appointmentid = table.Column<Guid>(type: "uuid", nullable: false),
                    customerid = table.Column<Guid>(type: "uuid", nullable: false),
                    creationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    startdate = table.Column<DateTime>(type: "date", nullable: false),
                    starttime = table.Column<DateTimeOffset>(type: "time with time zone", nullable: true),
                    enddate = table.Column<DateTime>(type: "date", nullable: true),
                    endtime = table.Column<DateTimeOffset>(type: "time with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("suggesteddate_pkey", x => x.suggesteddateid);
                    table.ForeignKey(
                        name: "suggesteddate_appointmentid_fkey",
                        column: x => x.appointmentid,
                        principalSchema: "public",
                        principalTable: "appointment",
                        principalColumn: "appointmentid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "suggesteddate_customerid_fkey",
                        column: x => x.customerid,
                        principalSchema: "public",
                        principalTable: "customer",
                        principalColumn: "customerid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "voting",
                schema: "public",
                columns: table => new
                {
                    votingid = table.Column<Guid>(type: "uuid", nullable: false),
                    participantid = table.Column<Guid>(type: "uuid", nullable: false),
                    appointmentid = table.Column<Guid>(type: "uuid", nullable: false),
                    customerid = table.Column<Guid>(type: "uuid", nullable: false),
                    suggesteddateid = table.Column<Guid>(type: "uuid", nullable: false),
                    creationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("voting_pkey", x => x.votingid);
                    table.ForeignKey(
                        name: "voting_appointmentid_fkey",
                        column: x => x.appointmentid,
                        principalSchema: "public",
                        principalTable: "appointment",
                        principalColumn: "appointmentid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "voting_customerid_fkey",
                        column: x => x.customerid,
                        principalSchema: "public",
                        principalTable: "customer",
                        principalColumn: "customerid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "voting_participantid_fkey",
                        column: x => x.participantid,
                        principalSchema: "public",
                        principalTable: "participant",
                        principalColumn: "participantid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "voting_suggesteddateid_fkey",
                        column: x => x.suggesteddateid,
                        principalSchema: "public",
                        principalTable: "suggesteddate",
                        principalColumn: "suggesteddateid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "appointment_adminid_ix",
                schema: "public",
                table: "appointment",
                column: "adminid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "appointment_customerid_ix",
                schema: "public",
                table: "appointment",
                column: "customerid");

            migrationBuilder.CreateIndex(
                name: "participant_appointmentid_ix",
                schema: "public",
                table: "participant",
                column: "appointmentid");

            migrationBuilder.CreateIndex(
                name: "participant_customerid_ix",
                schema: "public",
                table: "participant",
                column: "customerid");

            migrationBuilder.CreateIndex(
                name: "suggesteddate_appointmentid_ix",
                schema: "public",
                table: "suggesteddate",
                column: "appointmentid");

            migrationBuilder.CreateIndex(
                name: "suggestedDate_customerid_ix",
                schema: "public",
                table: "suggesteddate",
                column: "customerid");

            migrationBuilder.CreateIndex(
                name: "suggesteddate_enddate_ix",
                schema: "public",
                table: "suggesteddate",
                column: "enddate");

            migrationBuilder.CreateIndex(
                name: "suggesteddate_startdate_ix",
                schema: "public",
                table: "suggesteddate",
                column: "startdate");

            migrationBuilder.CreateIndex(
                name: "voting_appointmentid_ix",
                schema: "public",
                table: "voting",
                column: "appointmentid");

            migrationBuilder.CreateIndex(
                name: "voting_customerid_ix",
                schema: "public",
                table: "voting",
                column: "customerid");

            migrationBuilder.CreateIndex(
                name: "voting_participantid_ix",
                schema: "public",
                table: "voting",
                column: "participantid");

            migrationBuilder.CreateIndex(
                name: "voting_suggesteddateid_ix",
                schema: "public",
                table: "voting",
                column: "suggesteddateid");
        }

        /// <summary>
        /// Down
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appconfig",
                schema: "public");

            migrationBuilder.DropTable(
                name: "voting",
                schema: "public");

            migrationBuilder.DropTable(
                name: "participant",
                schema: "public");

            migrationBuilder.DropTable(
                name: "suggesteddate",
                schema: "public");

            migrationBuilder.DropTable(
                name: "appointment",
                schema: "public");

            migrationBuilder.DropTable(
                name: "customer",
                schema: "public");
        }
    }
}
