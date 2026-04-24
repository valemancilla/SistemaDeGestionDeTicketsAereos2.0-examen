using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Migrations;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class SeedDemoCatalogAndFlight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Airline",
                columns: new[] { "IdAirline", "Active", "IATACode", "IdCountry", "Name" },
                values: new object[] { 1, true, "DM", 1, "Aero Demo S.A." });

            migrationBuilder.InsertData(
                table: "Airport",
                columns: new[] { "IdAirport", "Active", "IATACode", "IdCity", "Name" },
                values: new object[,]
                {
                    { 1, true, "BOG", 1, "Aeropuerto Internacional El Dorado" },
                    { 2, true, "MDE", 2, "Aeropuerto Internacional José María Córdova" },
                    { 3, true, "MIA", 19, "Miami International Airport" }
                });

            // INSERT estándar falla si la app ya creó la fila Id=1 (GetOrCreateSingletonAsync).
            InsertClientFareBundleDisplayRowIgnoreDuplicate(migrationBuilder);

            migrationBuilder.InsertData(
                table: "Crew",
                columns: new[] { "IdCrew", "GroupName" },
                values: new object[] { 1, "Tripulación demo - vuelo precargado" });

            migrationBuilder.InsertData(
                table: "Manufacturer",
                columns: new[] { "IdManufacturer", "Name" },
                values: new object[,]
                {
                    { 1, "Boeing" },
                    { 2, "Airbus" }
                });

            migrationBuilder.InsertData(
                table: "Person",
                columns: new[] { "IdPerson", "BirthDate", "DocumentNumber", "FirstName", "IdAddress", "IdCountry", "IdDocumentType", "IdGender", "LastName" },
                values: new object[,]
                {
                    { 1, new DateOnly(1985, 3, 10), "SEED-TRIP-00000001", "Juan", null, 1, 1, 1, "Pérez" },
                    { 2, new DateOnly(1988, 7, 22), "SEED-TRIP-00000002", "María", null, 1, 1, 2, "Gómez" }
                });

            migrationBuilder.Sql("""
                INSERT INTO `SystemStatus` (`IdStatus`, `EntityType`, `StatusName`)
                SELECT 20, 'Booking', 'Pagada'
                WHERE NOT EXISTS (SELECT 1 FROM `SystemStatus` WHERE `IdStatus` = 20);
                """);

            migrationBuilder.InsertData(
                table: "AircraftModel",
                columns: new[] { "IdModel", "IdManufacturer", "Model" },
                values: new object[] { 1, 1, "737-800" });

            migrationBuilder.InsertData(
                table: "AirportTimeZone",
                columns: new[] { "IdAirport", "IdTimeZone" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 4 }
                });

            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "IdEmployee", "IdAirline", "IdPerson", "IdRole" },
                values: new object[,]
                {
                    { 1, 1, 1, 1 },
                    { 2, 1, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "Fare",
                columns: new[] { "IdFare", "Active", "BasePrice", "ExpirationDate", "FareName", "IdAirline", "ValidFrom", "ValidTo" },
                values: new object[] { 1, true, 450000m, null, "Tarifa económica demo", 1, new DateOnly(2024, 1, 1), new DateOnly(2035, 12, 31) });

            migrationBuilder.InsertData(
                table: "Route",
                columns: new[] { "IdRoute", "Active", "DestinationAirport", "DistanceKm", "EstDuration", "OriginAirport" },
                values: new object[,]
                {
                    { 1, true, 3, 2440m, new TimeOnly(6, 15, 0), 1 },
                    { 2, true, 2, 240m, new TimeOnly(0, 55, 0), 1 }
                });

            migrationBuilder.InsertData(
                table: "Aircraft",
                columns: new[] { "IdAircraft", "Capacity", "IdAirline", "IdModel" },
                values: new object[] { 1, 20, 1, 1 });

            migrationBuilder.InsertData(
                table: "CrewMember",
                columns: new[] { "IdCrewMember", "IdCrew", "IdEmployee", "IdRole" },
                values: new object[,]
                {
                    { 1, 1, 1, 1 },
                    { 2, 1, 2, 2 }
                });

            // Algunas bases no tienen las filas de SeatClass del seed inicial; sin esto falla FK al insertar FareSeatClassPrice.
            migrationBuilder.Sql("""
                INSERT IGNORE INTO `SeatClass` (`IdClase`, `ClassName`) VALUES
                (1, 'Económica'),
                (2, 'Ejecutiva'),
                (3, 'Primera Clase');
                """);

            migrationBuilder.InsertData(
                table: "FareSeatClassPrice",
                columns: new[] { "IdClase", "IdFare", "Price" },
                values: new object[,]
                {
                    { 1, 1, 380000m },
                    { 2, 1, 620000m },
                    { 3, 1, 1200000m }
                });

            migrationBuilder.InsertData(
                table: "Flight",
                columns: new[] { "IdFlight", "ArrivalTime", "AvailableSeats", "Date", "DepartureTime", "FlightNumber", "IdAircraft", "IdCrew", "IdFare", "IdRoute", "IdStatus", "TotalCapacity" },
                values: new object[] { 1, new TimeOnly(14, 45, 0), 20, new DateOnly(2026, 12, 15), new TimeOnly(8, 30, 0), "DM101", 1, 1, 1, 1, 1, 20 });

            migrationBuilder.InsertData(
                table: "Seat",
                columns: new[] { "IdSeat", "IdAircraft", "IdClase", "Number" },
                values: new object[,]
                {
                    { 1, 1, 1, "1A" },
                    { 2, 1, 1, "2A" },
                    { 3, 1, 1, "3A" },
                    { 4, 1, 1, "4A" },
                    { 5, 1, 1, "5A" },
                    { 6, 1, 1, "6A" },
                    { 7, 1, 1, "7A" },
                    { 8, 1, 1, "8A" },
                    { 9, 1, 1, "9A" },
                    { 10, 1, 1, "10A" },
                    { 11, 1, 1, "11A" },
                    { 12, 1, 1, "12A" },
                    { 13, 1, 1, "13A" },
                    { 14, 1, 1, "14A" },
                    { 15, 1, 1, "15A" },
                    { 16, 1, 1, "16A" },
                    { 17, 1, 1, "17A" },
                    { 18, 1, 1, "18A" },
                    { 19, 1, 1, "19A" },
                    { 20, 1, 1, "20A" }
                });

            migrationBuilder.InsertData(
                table: "SeatFlight",
                columns: new[] { "IdSeatFlight", "Available", "IdFlight", "IdSeat" },
                values: new object[,]
                {
                    { 1, true, 1, 1 },
                    { 2, true, 1, 2 },
                    { 3, true, 1, 3 },
                    { 4, true, 1, 4 },
                    { 5, true, 1, 5 },
                    { 6, true, 1, 6 },
                    { 7, true, 1, 7 },
                    { 8, true, 1, 8 },
                    { 9, true, 1, 9 },
                    { 10, true, 1, 10 },
                    { 11, true, 1, 11 },
                    { 12, true, 1, 12 },
                    { 13, true, 1, 13 },
                    { 14, true, 1, 14 },
                    { 15, true, 1, 15 },
                    { 16, true, 1, 16 },
                    { 17, true, 1, 17 },
                    { 18, true, 1, 18 },
                    { 19, true, 1, 19 },
                    { 20, true, 1, 20 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AirportTimeZone",
                keyColumns: new[] { "IdAirport", "IdTimeZone" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "AirportTimeZone",
                keyColumns: new[] { "IdAirport", "IdTimeZone" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "AirportTimeZone",
                keyColumns: new[] { "IdAirport", "IdTimeZone" },
                keyValues: new object[] { 3, 4 });

            migrationBuilder.DeleteData(
                table: "CrewMember",
                keyColumn: "IdCrewMember",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CrewMember",
                keyColumn: "IdCrewMember",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FareSeatClassPrice",
                keyColumns: new[] { "IdClase", "IdFare" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "FareSeatClassPrice",
                keyColumns: new[] { "IdClase", "IdFare" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "FareSeatClassPrice",
                keyColumns: new[] { "IdClase", "IdFare" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "Manufacturer",
                keyColumn: "IdManufacturer",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Route",
                keyColumn: "IdRoute",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "SeatFlight",
                keyColumn: "IdSeatFlight",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Airport",
                keyColumn: "IdAirport",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employee",
                keyColumn: "IdEmployee",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employee",
                keyColumn: "IdEmployee",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Flight",
                keyColumn: "IdFlight",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Seat",
                keyColumn: "IdSeat",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Aircraft",
                keyColumn: "IdAircraft",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Crew",
                keyColumn: "IdCrew",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Fare",
                keyColumn: "IdFare",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Person",
                keyColumn: "IdPerson",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Person",
                keyColumn: "IdPerson",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Route",
                keyColumn: "IdRoute",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AircraftModel",
                keyColumn: "IdModel",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Airline",
                keyColumn: "IdAirline",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Airport",
                keyColumn: "IdAirport",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Airport",
                keyColumn: "IdAirport",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Manufacturer",
                keyColumn: "IdManufacturer",
                keyValue: 1);
        }

        /// <summary>Evita error 1062 si la fila singleton ya fue creada por la aplicación.</summary>
        private static void InsertClientFareBundleDisplayRowIgnoreDuplicate(MigrationBuilder migrationBuilder)
        {
            static string Q(string s) => "'" + s.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("'", "''", StringComparison.Ordinal) + "'";

            var basic = Q(ClientFareBundleDisplayDefaults.BasicBody());
            var classic = Q(ClientFareBundleDisplayDefaults.ClassicBody);
            var flex = Q(ClientFareBundleDisplayDefaults.FlexBody);
            var expl = Q(ClientFareBundleDisplayDefaults.ExplainerLineTemplate);
            var sub = Q(ClientFareBundleDisplayDefaults.SubtitleLine);

            var carry = ClientFareBundleDisplayDefaults.ReferenceCarryOnCop.ToString(CultureInfo.InvariantCulture);
            var check = ClientFareBundleDisplayDefaults.ReferenceCheckedCop.ToString(CultureInfo.InvariantCulture);
            var seatSel = ClientFareBundleDisplayDefaults.SeatSelectionFromCop.ToString(CultureInfo.InvariantCulture);
            var classicM = ClientFareBundleDisplayDefaults.ClassicMultiplier.ToString(CultureInfo.InvariantCulture);
            var flexM = ClientFareBundleDisplayDefaults.FlexMultiplier.ToString(CultureInfo.InvariantCulture);
            var unpub = ClientFareBundleDisplayDefaults.UnpublishedFareReferenceCop.ToString(CultureInfo.InvariantCulture);

            migrationBuilder.Sql(
                $"""
                INSERT IGNORE INTO `ClientFareBundleDisplay` (
                    `Id`, `BasicBodyMarkup`, `ClassicBodyMarkup`, `ClassicMultiplier`, `ExplainerLine`,
                    `FlexBodyMarkup`, `FlexMultiplier`, `RefCarryOnCop`, `RefCheckedCop`, `SeatSelectionFromCop`,
                    `SubtitleLine`, `UnpublishedFareReferenceCop`)
                VALUES (
                    1,
                    {basic},
                    {classic},
                    {classicM},
                    {expl},
                    {flex},
                    {flexM},
                    {carry},
                    {check},
                    {seatSel},
                    {sub},
                    {unpub}
                );
                """);
        }
    }
}
