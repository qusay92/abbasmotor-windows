using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class mig1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lookups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CashType = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BuyType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Culture = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Users_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Users_ModificationUserId",
                        column: x => x.ModificationUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LookupValues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LookupId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LookupValues_Lookups_LookupId",
                        column: x => x.LookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Containers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArrivalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepartureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeparturePortId = table.Column<long>(type: "bigint", nullable: true),
                    DestinationId = table.Column<long>(type: "bigint", nullable: true),
                    IsArchive = table.Column<byte>(type: "tinyint", nullable: true),
                    ShippingCompanyId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Containers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Containers_LookupValues_DeparturePortId",
                        column: x => x.DeparturePortId,
                        principalTable: "LookupValues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Containers_LookupValues_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "LookupValues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Containers_LookupValues_ShippingCompanyId",
                        column: x => x.ShippingCompanyId,
                        principalTable: "LookupValues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Containers_Users_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Containers_Users_ModificationUserId",
                        column: x => x.ModificationUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Autos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrandId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VinNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Engine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarType = table.Column<int>(type: "int", nullable: false),
                    CarName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuyingAccountId = table.Column<long>(type: "bigint", nullable: true),
                    RemainingPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BuyerId = table.Column<long>(type: "bigint", nullable: true),
                    LoadPortId = table.Column<long>(type: "bigint", nullable: false),
                    AuctionId = table.Column<long>(type: "bigint", nullable: false),
                    CityId = table.Column<long>(type: "bigint", nullable: false),
                    DestinationId = table.Column<long>(type: "bigint", nullable: false),
                    ContainerId = table.Column<long>(type: "bigint", nullable: true),
                    BuyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CarStatus = table.Column<int>(type: "int", nullable: false),
                    PaperStatus = table.Column<int>(type: "int", nullable: true),
                    DisplayStatus = table.Column<int>(type: "int", nullable: false),
                    IsArchive = table.Column<byte>(type: "tinyint", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Autos_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Containers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Autos_LookupValues_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "LookupValues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Autos_LookupValues_BrandId",
                        column: x => x.BrandId,
                        principalTable: "LookupValues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Autos_LookupValues_BuyingAccountId",
                        column: x => x.BuyingAccountId,
                        principalTable: "LookupValues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Autos_LookupValues_CityId",
                        column: x => x.CityId,
                        principalTable: "LookupValues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Autos_LookupValues_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "LookupValues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Autos_LookupValues_LoadPortId",
                        column: x => x.LoadPortId,
                        principalTable: "LookupValues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Autos_Users_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Autos_Users_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Autos_Users_ModificationUserId",
                        column: x => x.ModificationUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContainerImages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PreviewImageSrc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThumbnailImageSrc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Alt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extintion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContainerId = table.Column<long>(type: "bigint", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainerImages_Containers_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Containers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContainerImages_Users_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContainerImages_Users_ModificationUserId",
                        column: x => x.ModificationUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AutoImages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AutoId = table.Column<long>(type: "bigint", nullable: false),
                    Alt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extintion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviewImageSrc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThumbnailImageSrc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutoImages_Autos_AutoId",
                        column: x => x.AutoId,
                        principalTable: "Autos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AutoImages_Users_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AutoImages_Users_ModificationUserId",
                        column: x => x.ModificationUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AutoId = table.Column<long>(type: "bigint", nullable: false),
                    CreationUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationUserId = table.Column<long>(type: "bigint", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Autos_AutoId",
                        column: x => x.AutoId,
                        principalTable: "Autos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_Users_CreationUserId",
                        column: x => x.CreationUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Payments_Users_ModificationUserId",
                        column: x => x.ModificationUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutoImages_AutoId",
                table: "AutoImages",
                column: "AutoId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoImages_CreationUserId",
                table: "AutoImages",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoImages_ModificationUserId",
                table: "AutoImages",
                column: "ModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_AuctionId",
                table: "Autos",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_BrandId",
                table: "Autos",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_BuyerId",
                table: "Autos",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_BuyingAccountId",
                table: "Autos",
                column: "BuyingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_CityId",
                table: "Autos",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_ContainerId",
                table: "Autos",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_CreationUserId",
                table: "Autos",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_DestinationId",
                table: "Autos",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_LoadPortId",
                table: "Autos",
                column: "LoadPortId");

            migrationBuilder.CreateIndex(
                name: "IX_Autos_ModificationUserId",
                table: "Autos",
                column: "ModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerImages_ContainerId",
                table: "ContainerImages",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerImages_CreationUserId",
                table: "ContainerImages",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerImages_ModificationUserId",
                table: "ContainerImages",
                column: "ModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_CreationUserId",
                table: "Containers",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_DeparturePortId",
                table: "Containers",
                column: "DeparturePortId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_DestinationId",
                table: "Containers",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_ModificationUserId",
                table: "Containers",
                column: "ModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_ShippingCompanyId",
                table: "Containers",
                column: "ShippingCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LookupValues_LookupId",
                table: "LookupValues",
                column: "LookupId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_AutoId",
                table: "Payments",
                column: "AutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CreationUserId",
                table: "Payments",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ModificationUserId",
                table: "Payments",
                column: "ModificationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreationUserId",
                table: "Users",
                column: "CreationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ModificationUserId",
                table: "Users",
                column: "ModificationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoImages");

            migrationBuilder.DropTable(
                name: "ContainerImages");

            migrationBuilder.DropTable(
                name: "PaymentDetails");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "Autos");

            migrationBuilder.DropTable(
                name: "Containers");

            migrationBuilder.DropTable(
                name: "LookupValues");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Lookups");
        }
    }
}
