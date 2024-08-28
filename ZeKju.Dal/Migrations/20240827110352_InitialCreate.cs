using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeKju.Dal.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    RouteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OriginCityId = table.Column<int>(type: "int", nullable: false),
                    DestinationCityId = table.Column<int>(type: "int", nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.RouteId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    AgencyId = table.Column<int>(type: "int", nullable: false),
                    OriginCityId = table.Column<int>(type: "int", nullable: false),
                    DestinationCityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => new { x.AgencyId, x.OriginCityId, x.DestinationCityId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    FlightId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AirlineId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.FlightId);
                    table.ForeignKey(
                        name: "FK_Flights_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "RouteId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_RouteId",
                table: "Flights",
                column: "RouteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Routes");
        }
    }
}
