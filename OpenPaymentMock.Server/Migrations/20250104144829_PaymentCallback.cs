using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenPaymentMock.Server.Migrations;

/// <inheritdoc />
public partial class PaymentCallback : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Secret",
            table: "PaymentSituations",
            type: "TEXT",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "Callbacks",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                CallbackUrl = table.Column<string>(type: "TEXT", nullable: false),
                Status = table.Column<int>(type: "INTEGER", nullable: false),
                PaymentSituationId = table.Column<Guid>(type: "TEXT", nullable: false),
                LatestResponse = table.Column<string>(type: "TEXT", nullable: true),
                LatestResponseAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                RetryCount = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Callbacks", x => x.Id);
                table.ForeignKey(
                    name: "FK_Callbacks_PaymentSituations_PaymentSituationId",
                    column: x => x.PaymentSituationId,
                    principalTable: "PaymentSituations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Callbacks_PaymentSituationId",
            table: "Callbacks",
            column: "PaymentSituationId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Callbacks");

        migrationBuilder.DropColumn(
            name: "Secret",
            table: "PaymentSituations");
    }
}
