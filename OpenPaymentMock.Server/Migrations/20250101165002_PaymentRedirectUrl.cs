using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenPaymentMock.Server.Migrations;

/// <inheritdoc />
public partial class PaymentRedirectUrl : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "RedirectUrl",
            table: "PaymentSituations",
            type: "TEXT",
            nullable: false,
            defaultValue: "https://google.hu");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RedirectUrl",
            table: "PaymentSituations");
    }
}
