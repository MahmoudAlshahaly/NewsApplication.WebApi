using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NewsApplication.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addroles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7b5302c9-304f-462a-828a-645d818176d1", null, "ContentAdmin", null },
                    { "97da9636-9b67-4378-8a82-190ea1ed5030", null, "Admin", null },
                    { "cd30616a-b07c-4fbd-80b7-46b53431ae75", null, "Normal", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7b5302c9-304f-462a-828a-645d818176d1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97da9636-9b67-4378-8a82-190ea1ed5030");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd30616a-b07c-4fbd-80b7-46b53431ae75");
        }
    }
}
