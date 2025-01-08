using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenPaymentMock.Server.Migrations;

/// <inheritdoc />
public partial class PaymentTimeoutCalculation : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Callbacks_PaymentSituationId",
            table: "Callbacks");

        migrationBuilder.RenameColumn(
            name: "Timeout",
            table: "PaymentSituations",
            newName: "TimeoutAt");

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "TimeoutAt",
            table: "PaymentAttempts",
            type: "TEXT",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

        migrationBuilder.CreateIndex(
            name: "IX_Callbacks_PaymentSituationId",
            table: "Callbacks",
            column: "PaymentSituationId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Callbacks_PaymentSituationId",
            table: "Callbacks");

        migrationBuilder.DropColumn(
            name: "TimeoutAt",
            table: "PaymentAttempts");

        migrationBuilder.RenameColumn(
            name: "TimeoutAt",
            table: "PaymentSituations",
            newName: "Timeout");

        migrationBuilder.CreateIndex(
            name: "IX_Callbacks_PaymentSituationId",
            table: "Callbacks",
            column: "PaymentSituationId",
            unique: true);
    }
}
