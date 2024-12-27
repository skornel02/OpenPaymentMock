using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenPaymentMock.Server.Migrations;

/// <inheritdoc />
public partial class PartnerAccessKeys : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PaymentOptions",
            table: "PaymentSituations",
            type: "TEXT",
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateTable(
            name: "PartnerAccessKeys",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                Key = table.Column<string>(type: "TEXT", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                LastUsed = table.Column<DateTime>(type: "TEXT", nullable: true),
                Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                UsageCount = table.Column<long>(type: "INTEGER", nullable: false),
                PartnerId = table.Column<Guid>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PartnerAccessKeys", x => x.Id);
                table.ForeignKey(
                    name: "FK_PartnerAccessKeys_Partners_PartnerId",
                    column: x => x.PartnerId,
                    principalTable: "Partners",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PartnerAccessKeys_Key",
            table: "PartnerAccessKeys",
            column: "Key",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PartnerAccessKeys_PartnerId",
            table: "PartnerAccessKeys",
            column: "PartnerId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "PartnerAccessKeys");

        migrationBuilder.DropColumn(
            name: "PaymentOptions",
            table: "PaymentSituations");
    }
}
