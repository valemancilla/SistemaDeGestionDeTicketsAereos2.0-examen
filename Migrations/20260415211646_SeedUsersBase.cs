using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsersBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BaggageType",
                columns: table => new
                {
                    IdBaggageType = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TypeName = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaggageType", x => x.IdBaggageType);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CheckInChannel",
                columns: table => new
                {
                    IdChannel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ChannelName = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckInChannel", x => x.IdChannel);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    IdCountry = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ISOCode = table.Column<string>(type: "char(2)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.IdCountry);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Crew",
                columns: table => new
                {
                    IdCrew = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GroupName = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crew", x => x.IdCrew);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DocumentType",
                columns: table => new
                {
                    IdDocumentType = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentType", x => x.IdDocumentType);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmployeeRole",
                columns: table => new
                {
                    IdRole = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleName = table.Column<string>(type: "varchar(80)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeRole", x => x.IdRole);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Gender",
                columns: table => new
                {
                    IdGender = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gender", x => x.IdGender);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Manufacturer",
                columns: table => new
                {
                    IdManufacturer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturer", x => x.IdManufacturer);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    IdPaymentMethod = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MethodName = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.IdPaymentMethod);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    IdUserRole = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleName = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.IdUserRole);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SeatClass",
                columns: table => new
                {
                    IdClase = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClassName = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatClass", x => x.IdClase);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SystemStatus",
                columns: table => new
                {
                    IdStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StatusName = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityType = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemStatus", x => x.IdStatus);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TimeZone",
                columns: table => new
                {
                    IdTimeZone = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UTCOffset = table.Column<string>(type: "varchar(10)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZone", x => x.IdTimeZone);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Airline",
                columns: table => new
                {
                    IdAirline = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(150)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IATACode = table.Column<string>(type: "char(2)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdCountry = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airline", x => x.IdAirline);
                    table.ForeignKey(
                        name: "FK_Airline_Country_IdCountry",
                        column: x => x.IdCountry,
                        principalTable: "Country",
                        principalColumn: "IdCountry",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    IdCity = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdCountry = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.IdCity);
                    table.ForeignKey(
                        name: "FK_City_Country_IdCountry",
                        column: x => x.IdCountry,
                        principalTable: "Country",
                        principalColumn: "IdCountry",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AircraftModel",
                columns: table => new
                {
                    IdModel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdManufacturer = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AircraftModel", x => x.IdModel);
                    table.ForeignKey(
                        name: "FK_AircraftModel_Manufacturer_IdManufacturer",
                        column: x => x.IdManufacturer,
                        principalTable: "Manufacturer",
                        principalColumn: "IdManufacturer",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Fare",
                columns: table => new
                {
                    IdFare = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FareName = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BasePrice = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    IdAirline = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidTo = table.Column<DateOnly>(type: "date", nullable: false),
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    ExpirationDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fare", x => x.IdFare);
                    table.ForeignKey(
                        name: "FK_Fare_Airline_IdAirline",
                        column: x => x.IdAirline,
                        principalTable: "Airline",
                        principalColumn: "IdAirline",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Airport",
                columns: table => new
                {
                    IdAirport = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(150)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IATACode = table.Column<string>(type: "char(3)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdCity = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airport", x => x.IdAirport);
                    table.ForeignKey(
                        name: "FK_Airport_City_IdCity",
                        column: x => x.IdCity,
                        principalTable: "City",
                        principalColumn: "IdCity",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Aircraft",
                columns: table => new
                {
                    IdAircraft = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdAirline = table.Column<int>(type: "int", nullable: false),
                    IdModel = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aircraft", x => x.IdAircraft);
                    table.ForeignKey(
                        name: "FK_Aircraft_AircraftModel_IdModel",
                        column: x => x.IdModel,
                        principalTable: "AircraftModel",
                        principalColumn: "IdModel",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Aircraft_Airline_IdAirline",
                        column: x => x.IdAirline,
                        principalTable: "Airline",
                        principalColumn: "IdAirline",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AirportTimeZone",
                columns: table => new
                {
                    IdAirport = table.Column<int>(type: "int", nullable: false),
                    IdTimeZone = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirportTimeZone", x => new { x.IdAirport, x.IdTimeZone });
                    table.ForeignKey(
                        name: "FK_AirportTimeZone_Airport_IdAirport",
                        column: x => x.IdAirport,
                        principalTable: "Airport",
                        principalColumn: "IdAirport",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AirportTimeZone_TimeZone_IdTimeZone",
                        column: x => x.IdTimeZone,
                        principalTable: "TimeZone",
                        principalColumn: "IdTimeZone",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Route",
                columns: table => new
                {
                    IdRoute = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OriginAirport = table.Column<int>(type: "int", nullable: false),
                    DestinationAirport = table.Column<int>(type: "int", nullable: false),
                    DistanceKm = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    EstDuration = table.Column<TimeOnly>(type: "time(6)", nullable: false),
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Route", x => x.IdRoute);
                    table.ForeignKey(
                        name: "FK_Route_Airport_DestinationAirport",
                        column: x => x.DestinationAirport,
                        principalTable: "Airport",
                        principalColumn: "IdAirport",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Route_Airport_OriginAirport",
                        column: x => x.OriginAirport,
                        principalTable: "Airport",
                        principalColumn: "IdAirport",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Seat",
                columns: table => new
                {
                    IdSeat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdAircraft = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<string>(type: "varchar(5)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdClase = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seat", x => x.IdSeat);
                    table.ForeignKey(
                        name: "FK_Seat_Aircraft_IdAircraft",
                        column: x => x.IdAircraft,
                        principalTable: "Aircraft",
                        principalColumn: "IdAircraft",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seat_SeatClass_IdClase",
                        column: x => x.IdClase,
                        principalTable: "SeatClass",
                        principalColumn: "IdClase",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Flight",
                columns: table => new
                {
                    IdFlight = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdRoute = table.Column<int>(type: "int", nullable: false),
                    IdAircraft = table.Column<int>(type: "int", nullable: false),
                    FlightNumber = table.Column<string>(type: "varchar(10)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    DepartureTime = table.Column<TimeOnly>(type: "time(6)", nullable: false),
                    ArrivalTime = table.Column<TimeOnly>(type: "time(6)", nullable: false),
                    TotalCapacity = table.Column<int>(type: "int", nullable: false),
                    AvailableSeats = table.Column<int>(type: "int", nullable: false),
                    IdStatus = table.Column<int>(type: "int", nullable: false),
                    IdCrew = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flight", x => x.IdFlight);
                    table.ForeignKey(
                        name: "FK_Flight_Aircraft_IdAircraft",
                        column: x => x.IdAircraft,
                        principalTable: "Aircraft",
                        principalColumn: "IdAircraft",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flight_Crew_IdCrew",
                        column: x => x.IdCrew,
                        principalTable: "Crew",
                        principalColumn: "IdCrew",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flight_Route_IdRoute",
                        column: x => x.IdRoute,
                        principalTable: "Route",
                        principalColumn: "IdRoute",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flight_SystemStatus_IdStatus",
                        column: x => x.IdStatus,
                        principalTable: "SystemStatus",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    IdBooking = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BookingCode = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FlightDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IdStatus = table.Column<int>(type: "int", nullable: false),
                    IdFlight = table.Column<int>(type: "int", nullable: false),
                    SeatCount = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Observations = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.IdBooking);
                    table.ForeignKey(
                        name: "FK_Booking_Flight_IdFlight",
                        column: x => x.IdFlight,
                        principalTable: "Flight",
                        principalColumn: "IdFlight",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Booking_SystemStatus_IdStatus",
                        column: x => x.IdStatus,
                        principalTable: "SystemStatus",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SeatFlight",
                columns: table => new
                {
                    IdSeatFlight = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdSeat = table.Column<int>(type: "int", nullable: false),
                    IdFlight = table.Column<int>(type: "int", nullable: false),
                    Available = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatFlight", x => x.IdSeatFlight);
                    table.ForeignKey(
                        name: "FK_SeatFlight_Flight_IdFlight",
                        column: x => x.IdFlight,
                        principalTable: "Flight",
                        principalColumn: "IdFlight",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeatFlight_Seat_IdSeat",
                        column: x => x.IdSeat,
                        principalTable: "Seat",
                        principalColumn: "IdSeat",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    IdPayment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdBooking = table.Column<int>(type: "int", nullable: false),
                    IdPaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IdStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.IdPayment);
                    table.ForeignKey(
                        name: "FK_Payment_Booking_IdBooking",
                        column: x => x.IdBooking,
                        principalTable: "Booking",
                        principalColumn: "IdBooking",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payment_PaymentMethod_IdPaymentMethod",
                        column: x => x.IdPaymentMethod,
                        principalTable: "PaymentMethod",
                        principalColumn: "IdPaymentMethod",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payment_SystemStatus_IdStatus",
                        column: x => x.IdStatus,
                        principalTable: "SystemStatus",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    IdTicket = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TicketCode = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdBooking = table.Column<int>(type: "int", nullable: false),
                    IdFare = table.Column<int>(type: "int", nullable: false),
                    IdStatus = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.IdTicket);
                    table.ForeignKey(
                        name: "FK_Ticket_Booking_IdBooking",
                        column: x => x.IdBooking,
                        principalTable: "Booking",
                        principalColumn: "IdBooking",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_Fare_IdFare",
                        column: x => x.IdFare,
                        principalTable: "Fare",
                        principalColumn: "IdFare",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_SystemStatus_IdStatus",
                        column: x => x.IdStatus,
                        principalTable: "SystemStatus",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Baggage",
                columns: table => new
                {
                    IdBaggage = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdTicket = table.Column<int>(type: "int", nullable: false),
                    IdBaggageType = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(6,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baggage", x => x.IdBaggage);
                    table.ForeignKey(
                        name: "FK_Baggage_BaggageType_IdBaggageType",
                        column: x => x.IdBaggageType,
                        principalTable: "BaggageType",
                        principalColumn: "IdBaggageType",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Baggage_Ticket_IdTicket",
                        column: x => x.IdTicket,
                        principalTable: "Ticket",
                        principalColumn: "IdTicket",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BookingCancellation",
                columns: table => new
                {
                    IdCancellation = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdBooking = table.Column<int>(type: "int", nullable: false),
                    CancellationReason = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PenaltyAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0.00m),
                    CancellationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IdUser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingCancellation", x => x.IdCancellation);
                    table.ForeignKey(
                        name: "FK_BookingCancellation_Booking_IdBooking",
                        column: x => x.IdBooking,
                        principalTable: "Booking",
                        principalColumn: "IdBooking",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BookingCustomer",
                columns: table => new
                {
                    IdBookingCustomer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdBooking = table.Column<int>(type: "int", nullable: false),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    IdPerson = table.Column<int>(type: "int", nullable: false),
                    IdSeat = table.Column<int>(type: "int", nullable: false),
                    IsPrimary = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    AssociationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingCustomer", x => x.IdBookingCustomer);
                    table.ForeignKey(
                        name: "FK_BookingCustomer_Booking_IdBooking",
                        column: x => x.IdBooking,
                        principalTable: "Booking",
                        principalColumn: "IdBooking",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingCustomer_Seat_IdSeat",
                        column: x => x.IdSeat,
                        principalTable: "Seat",
                        principalColumn: "IdSeat",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BookingStatusHistory",
                columns: table => new
                {
                    IdHistory = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdBooking = table.Column<int>(type: "int", nullable: false),
                    IdStatus = table.Column<int>(type: "int", nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    Observation = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingStatusHistory", x => x.IdHistory);
                    table.ForeignKey(
                        name: "FK_BookingStatusHistory_Booking_IdBooking",
                        column: x => x.IdBooking,
                        principalTable: "Booking",
                        principalColumn: "IdBooking",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingStatusHistory_SystemStatus_IdStatus",
                        column: x => x.IdStatus,
                        principalTable: "SystemStatus",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CheckIn",
                columns: table => new
                {
                    IdCheckIn = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdTicket = table.Column<int>(type: "int", nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IdChannel = table.Column<int>(type: "int", nullable: false),
                    IdSeat = table.Column<int>(type: "int", nullable: false),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    IdStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckIn", x => x.IdCheckIn);
                    table.ForeignKey(
                        name: "FK_CheckIn_CheckInChannel_IdChannel",
                        column: x => x.IdChannel,
                        principalTable: "CheckInChannel",
                        principalColumn: "IdChannel",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CheckIn_Seat_IdSeat",
                        column: x => x.IdSeat,
                        principalTable: "Seat",
                        principalColumn: "IdSeat",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CheckIn_SystemStatus_IdStatus",
                        column: x => x.IdStatus,
                        principalTable: "SystemStatus",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CheckIn_Ticket_IdTicket",
                        column: x => x.IdTicket,
                        principalTable: "Ticket",
                        principalColumn: "IdTicket",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CrewMember",
                columns: table => new
                {
                    IdCrewMember = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdCrew = table.Column<int>(type: "int", nullable: false),
                    IdEmployee = table.Column<int>(type: "int", nullable: false),
                    IdRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrewMember", x => x.IdCrewMember);
                    table.ForeignKey(
                        name: "FK_CrewMember_Crew_IdCrew",
                        column: x => x.IdCrew,
                        principalTable: "Crew",
                        principalColumn: "IdCrew",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CrewMember_EmployeeRole_IdRole",
                        column: x => x.IdRole,
                        principalTable: "EmployeeRole",
                        principalColumn: "IdRole",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    IdCustomer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdPerson = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    RegistrationDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.IdCustomer);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CustomerEmail",
                columns: table => new
                {
                    IdEmail = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdPerson = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "varchar(150)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerEmail", x => x.IdEmail);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CustomerPhone",
                columns: table => new
                {
                    IdPhone = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdPerson = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPhone", x => x.IdPhone);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    IdEmployee = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdPerson = table.Column<int>(type: "int", nullable: false),
                    IdAirline = table.Column<int>(type: "int", nullable: false),
                    IdRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.IdEmployee);
                    table.ForeignKey(
                        name: "FK_Employee_Airline_IdAirline",
                        column: x => x.IdAirline,
                        principalTable: "Airline",
                        principalColumn: "IdAirline",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_EmployeeRole_IdRole",
                        column: x => x.IdRole,
                        principalTable: "EmployeeRole",
                        principalColumn: "IdRole",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FlightStatusHistory",
                columns: table => new
                {
                    IdHistory = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdFlight = table.Column<int>(type: "int", nullable: false),
                    IdStatus = table.Column<int>(type: "int", nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    Observation = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightStatusHistory", x => x.IdHistory);
                    table.ForeignKey(
                        name: "FK_FlightStatusHistory_Flight_IdFlight",
                        column: x => x.IdFlight,
                        principalTable: "Flight",
                        principalColumn: "IdFlight",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlightStatusHistory_SystemStatus_IdStatus",
                        column: x => x.IdStatus,
                        principalTable: "SystemStatus",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    IdPerson = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdDocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentNumber = table.Column<string>(type: "varchar(30)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstName = table.Column<string>(type: "varchar(80)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "varchar(80)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IdGender = table.Column<int>(type: "int", nullable: false),
                    IdCountry = table.Column<int>(type: "int", nullable: false),
                    IdAddress = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.IdPerson);
                    table.ForeignKey(
                        name: "FK_Person_Country_IdCountry",
                        column: x => x.IdCountry,
                        principalTable: "Country",
                        principalColumn: "IdCountry",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Person_DocumentType_IdDocumentType",
                        column: x => x.IdDocumentType,
                        principalTable: "DocumentType",
                        principalColumn: "IdDocumentType",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Person_Gender_IdGender",
                        column: x => x.IdGender,
                        principalTable: "Gender",
                        principalColumn: "IdGender",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PersonAddress",
                columns: table => new
                {
                    IdAddress = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdPerson = table.Column<int>(type: "int", nullable: false),
                    Street = table.Column<string>(type: "varchar(150)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Number = table.Column<string>(type: "varchar(20)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdCity = table.Column<int>(type: "int", nullable: false),
                    ZipCode = table.Column<string>(type: "varchar(20)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonAddress", x => x.IdAddress);
                    table.ForeignKey(
                        name: "FK_PersonAddress_City_IdCity",
                        column: x => x.IdCity,
                        principalTable: "City",
                        principalColumn: "IdCity",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonAddress_Person_IdPerson",
                        column: x => x.IdPerson,
                        principalTable: "Person",
                        principalColumn: "IdPerson",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(60)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdUserRole = table.Column<int>(type: "int", nullable: false),
                    IdPerson = table.Column<int>(type: "int", nullable: true),
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.IdUser);
                    table.ForeignKey(
                        name: "FK_User_Person_IdPerson",
                        column: x => x.IdPerson,
                        principalTable: "Person",
                        principalColumn: "IdPerson",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_Role_IdUserRole",
                        column: x => x.IdUserRole,
                        principalTable: "Role",
                        principalColumn: "IdUserRole",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TicketStatusHistory",
                columns: table => new
                {
                    IdHistory = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdTicket = table.Column<int>(type: "int", nullable: false),
                    IdStatus = table.Column<int>(type: "int", nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    Observation = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketStatusHistory", x => x.IdHistory);
                    table.ForeignKey(
                        name: "FK_TicketStatusHistory_SystemStatus_IdStatus",
                        column: x => x.IdStatus,
                        principalTable: "SystemStatus",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketStatusHistory_Ticket_IdTicket",
                        column: x => x.IdTicket,
                        principalTable: "Ticket",
                        principalColumn: "IdTicket",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketStatusHistory_User_IdUser",
                        column: x => x.IdUser,
                        principalTable: "User",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "IdUserRole", "RoleName" },
                values: new object[,]
                {
                    { 1, "Administrador" },
                    { 2, "Cliente" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "IdUser", "Active", "IdPerson", "IdUserRole", "Password", "Username" },
                values: new object[,]
                {
                    { 1, true, null, 1, "12345678", "admin" },
                    { 2, true, null, 2, "12345678", "customer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aircraft_IdAirline",
                table: "Aircraft",
                column: "IdAirline");

            migrationBuilder.CreateIndex(
                name: "IX_Aircraft_IdModel",
                table: "Aircraft",
                column: "IdModel");

            migrationBuilder.CreateIndex(
                name: "IX_AircraftModel_IdManufacturer",
                table: "AircraftModel",
                column: "IdManufacturer");

            migrationBuilder.CreateIndex(
                name: "IX_Airline_IdCountry",
                table: "Airline",
                column: "IdCountry");

            migrationBuilder.CreateIndex(
                name: "UQ_Airline_IATACode",
                table: "Airline",
                column: "IATACode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Airport_IdCity",
                table: "Airport",
                column: "IdCity");

            migrationBuilder.CreateIndex(
                name: "UQ_Airport_IATACode",
                table: "Airport",
                column: "IATACode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AirportTimeZone_IdTimeZone",
                table: "AirportTimeZone",
                column: "IdTimeZone");

            migrationBuilder.CreateIndex(
                name: "IX_Baggage_IdBaggageType",
                table: "Baggage",
                column: "IdBaggageType");

            migrationBuilder.CreateIndex(
                name: "IX_Baggage_IdTicket",
                table: "Baggage",
                column: "IdTicket");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_IdFlight",
                table: "Booking",
                column: "IdFlight");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_IdStatus",
                table: "Booking",
                column: "IdStatus");

            migrationBuilder.CreateIndex(
                name: "UQ_Booking_Code",
                table: "Booking",
                column: "BookingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingCancellation_IdBooking",
                table: "BookingCancellation",
                column: "IdBooking");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCancellation_IdUser",
                table: "BookingCancellation",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCustomer_IdBooking",
                table: "BookingCustomer",
                column: "IdBooking");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCustomer_IdPerson",
                table: "BookingCustomer",
                column: "IdPerson");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCustomer_IdSeat",
                table: "BookingCustomer",
                column: "IdSeat");

            migrationBuilder.CreateIndex(
                name: "IX_BookingCustomer_IdUser",
                table: "BookingCustomer",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_BookingStatusHistory_IdBooking",
                table: "BookingStatusHistory",
                column: "IdBooking");

            migrationBuilder.CreateIndex(
                name: "IX_BookingStatusHistory_IdStatus",
                table: "BookingStatusHistory",
                column: "IdStatus");

            migrationBuilder.CreateIndex(
                name: "IX_BookingStatusHistory_IdUser",
                table: "BookingStatusHistory",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIn_IdChannel",
                table: "CheckIn",
                column: "IdChannel");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIn_IdSeat",
                table: "CheckIn",
                column: "IdSeat");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIn_IdStatus",
                table: "CheckIn",
                column: "IdStatus");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIn_IdTicket",
                table: "CheckIn",
                column: "IdTicket");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIn_IdUser",
                table: "CheckIn",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_City_IdCountry",
                table: "City",
                column: "IdCountry");

            migrationBuilder.CreateIndex(
                name: "UQ_Country_ISOCode",
                table: "Country",
                column: "ISOCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CrewMember_IdCrew",
                table: "CrewMember",
                column: "IdCrew");

            migrationBuilder.CreateIndex(
                name: "IX_CrewMember_IdEmployee",
                table: "CrewMember",
                column: "IdEmployee");

            migrationBuilder.CreateIndex(
                name: "IX_CrewMember_IdRole",
                table: "CrewMember",
                column: "IdRole");

            migrationBuilder.CreateIndex(
                name: "UQ_Customer_Person",
                table: "Customer",
                column: "IdPerson",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerEmail_IdPerson",
                table: "CustomerEmail",
                column: "IdPerson");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPhone_IdPerson",
                table: "CustomerPhone",
                column: "IdPerson");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_IdAirline",
                table: "Employee",
                column: "IdAirline");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_IdPerson",
                table: "Employee",
                column: "IdPerson");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_IdRole",
                table: "Employee",
                column: "IdRole");

            migrationBuilder.CreateIndex(
                name: "IX_Fare_IdAirline",
                table: "Fare",
                column: "IdAirline");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_IdAircraft",
                table: "Flight",
                column: "IdAircraft");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_IdCrew",
                table: "Flight",
                column: "IdCrew");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_IdRoute",
                table: "Flight",
                column: "IdRoute");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_IdStatus",
                table: "Flight",
                column: "IdStatus");

            migrationBuilder.CreateIndex(
                name: "IX_FlightStatusHistory_IdFlight",
                table: "FlightStatusHistory",
                column: "IdFlight");

            migrationBuilder.CreateIndex(
                name: "IX_FlightStatusHistory_IdStatus",
                table: "FlightStatusHistory",
                column: "IdStatus");

            migrationBuilder.CreateIndex(
                name: "IX_FlightStatusHistory_IdUser",
                table: "FlightStatusHistory",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_IdBooking",
                table: "Payment",
                column: "IdBooking");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_IdPaymentMethod",
                table: "Payment",
                column: "IdPaymentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_IdStatus",
                table: "Payment",
                column: "IdStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Person_IdAddress",
                table: "Person",
                column: "IdAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Person_IdCountry",
                table: "Person",
                column: "IdCountry");

            migrationBuilder.CreateIndex(
                name: "IX_Person_IdDocumentType",
                table: "Person",
                column: "IdDocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_Person_IdGender",
                table: "Person",
                column: "IdGender");

            migrationBuilder.CreateIndex(
                name: "IX_PersonAddress_IdCity",
                table: "PersonAddress",
                column: "IdCity");

            migrationBuilder.CreateIndex(
                name: "IX_PersonAddress_IdPerson",
                table: "PersonAddress",
                column: "IdPerson");

            migrationBuilder.CreateIndex(
                name: "IX_Route_DestinationAirport",
                table: "Route",
                column: "DestinationAirport");

            migrationBuilder.CreateIndex(
                name: "IX_Route_OriginAirport",
                table: "Route",
                column: "OriginAirport");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_IdAircraft",
                table: "Seat",
                column: "IdAircraft");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_IdClase",
                table: "Seat",
                column: "IdClase");

            migrationBuilder.CreateIndex(
                name: "IX_SeatFlight_IdFlight",
                table: "SeatFlight",
                column: "IdFlight");

            migrationBuilder.CreateIndex(
                name: "UQ_SeatFlight",
                table: "SeatFlight",
                columns: new[] { "IdSeat", "IdFlight" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_IdBooking",
                table: "Ticket",
                column: "IdBooking");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_IdFare",
                table: "Ticket",
                column: "IdFare");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_IdStatus",
                table: "Ticket",
                column: "IdStatus");

            migrationBuilder.CreateIndex(
                name: "UQ_Ticket_Code",
                table: "Ticket",
                column: "TicketCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusHistory_IdStatus",
                table: "TicketStatusHistory",
                column: "IdStatus");

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusHistory_IdTicket",
                table: "TicketStatusHistory",
                column: "IdTicket");

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusHistory_IdUser",
                table: "TicketStatusHistory",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_User_IdPerson",
                table: "User",
                column: "IdPerson");

            migrationBuilder.CreateIndex(
                name: "IX_User_IdUserRole",
                table: "User",
                column: "IdUserRole");

            migrationBuilder.CreateIndex(
                name: "UQ_User_Username",
                table: "User",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingCancellation_User_IdUser",
                table: "BookingCancellation",
                column: "IdUser",
                principalTable: "User",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingCustomer_Person_IdPerson",
                table: "BookingCustomer",
                column: "IdPerson",
                principalTable: "Person",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingCustomer_User_IdUser",
                table: "BookingCustomer",
                column: "IdUser",
                principalTable: "User",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingStatusHistory_User_IdUser",
                table: "BookingStatusHistory",
                column: "IdUser",
                principalTable: "User",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CheckIn_User_IdUser",
                table: "CheckIn",
                column: "IdUser",
                principalTable: "User",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CrewMember_Employee_IdEmployee",
                table: "CrewMember",
                column: "IdEmployee",
                principalTable: "Employee",
                principalColumn: "IdEmployee",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Person_IdPerson",
                table: "Customer",
                column: "IdPerson",
                principalTable: "Person",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerEmail_Person_IdPerson",
                table: "CustomerEmail",
                column: "IdPerson",
                principalTable: "Person",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPhone_Person_IdPerson",
                table: "CustomerPhone",
                column: "IdPerson",
                principalTable: "Person",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Person_IdPerson",
                table: "Employee",
                column: "IdPerson",
                principalTable: "Person",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FlightStatusHistory_User_IdUser",
                table: "FlightStatusHistory",
                column: "IdUser",
                principalTable: "User",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Person_PersonAddress_IdAddress",
                table: "Person",
                column: "IdAddress",
                principalTable: "PersonAddress",
                principalColumn: "IdAddress",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_City_Country_IdCountry",
                table: "City");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_Country_IdCountry",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonAddress_City_IdCity",
                table: "PersonAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonAddress_Person_IdPerson",
                table: "PersonAddress");

            migrationBuilder.DropTable(
                name: "AirportTimeZone");

            migrationBuilder.DropTable(
                name: "Baggage");

            migrationBuilder.DropTable(
                name: "BookingCancellation");

            migrationBuilder.DropTable(
                name: "BookingCustomer");

            migrationBuilder.DropTable(
                name: "BookingStatusHistory");

            migrationBuilder.DropTable(
                name: "CheckIn");

            migrationBuilder.DropTable(
                name: "CrewMember");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "CustomerEmail");

            migrationBuilder.DropTable(
                name: "CustomerPhone");

            migrationBuilder.DropTable(
                name: "FlightStatusHistory");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "SeatFlight");

            migrationBuilder.DropTable(
                name: "TicketStatusHistory");

            migrationBuilder.DropTable(
                name: "TimeZone");

            migrationBuilder.DropTable(
                name: "BaggageType");

            migrationBuilder.DropTable(
                name: "CheckInChannel");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "PaymentMethod");

            migrationBuilder.DropTable(
                name: "Seat");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "EmployeeRole");

            migrationBuilder.DropTable(
                name: "SeatClass");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "Fare");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Flight");

            migrationBuilder.DropTable(
                name: "Aircraft");

            migrationBuilder.DropTable(
                name: "Crew");

            migrationBuilder.DropTable(
                name: "Route");

            migrationBuilder.DropTable(
                name: "SystemStatus");

            migrationBuilder.DropTable(
                name: "AircraftModel");

            migrationBuilder.DropTable(
                name: "Airline");

            migrationBuilder.DropTable(
                name: "Airport");

            migrationBuilder.DropTable(
                name: "Manufacturer");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "DocumentType");

            migrationBuilder.DropTable(
                name: "Gender");

            migrationBuilder.DropTable(
                name: "PersonAddress");
        }
    }
}
