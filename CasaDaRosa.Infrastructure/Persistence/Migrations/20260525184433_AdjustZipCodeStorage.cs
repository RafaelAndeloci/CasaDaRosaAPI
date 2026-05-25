using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasaDaRosa.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdjustZipCodeStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ZipCodeRawValue",
                table: "Addresses",
                type: "int",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "ZipCodeRawValue",
                table: "Addresses",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
