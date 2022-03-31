using System.IO;

using Microsoft.EntityFrameworkCore.Migrations;

namespace QuartzContext.Migrations
{
    public partial class Initial_Structure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(File.ReadAllText(@"sql/initial_structure.sql"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
