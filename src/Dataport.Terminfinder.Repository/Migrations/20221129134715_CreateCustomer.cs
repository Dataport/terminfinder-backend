using Dataport.Terminfinder.BusinessObject;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Dataport.Terminfinder.Repository.Migrations
{
    /// <summary>
    /// DB Migration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class CreateCustomer : Migration
    {
        /// <summary>
        /// BuildTargetModel
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlInhalt = "insert into public.customer(customerid, customername, status) select '80248A42-8FE2-4D4A-89DA-02E683511F76', 'General Customer', 'Started' WHERE NOT EXISTS (select 1 from public.customer);";
            migrationBuilder.Sql(sqlInhalt, suppressTransaction: true);
        }
    }
}
