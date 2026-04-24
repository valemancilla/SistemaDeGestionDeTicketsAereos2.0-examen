using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class Semillas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BaggageType",
                columns: new[] { "IdBaggageType", "TypeName" },
                values: new object[,]
                {
                    { 1, "Equipaje de mano" },
                    { 2, "Equipaje de bodega" },
                    { 3, "Equipaje especial" }
                });

            migrationBuilder.InsertData(
                table: "CheckInChannel",
                columns: new[] { "IdChannel", "ChannelName" },
                values: new object[,]
                {
                    { 1, "Web" },
                    { 2, "Móvil" },
                    { 3, "Kiosco aeropuerto" },
                    { 4, "Mostrador" }
                });

            migrationBuilder.InsertData(
                table: "Country",
                columns: new[] { "IdCountry", "ISOCode", "Name" },
                values: new object[,]
                {
                    { 1, "CO", "Colombia" },
                    { 2, "VE", "Venezuela" },
                    { 3, "EC", "Ecuador" },
                    { 4, "PE", "Perú" },
                    { 5, "BR", "Brasil" },
                    { 6, "AR", "Argentina" },
                    { 7, "CL", "Chile" },
                    { 8, "MX", "México" },
                    { 9, "PA", "Panamá" },
                    { 10, "ES", "España" },
                    { 11, "US", "Estados Unidos" }
                });

            migrationBuilder.InsertData(
                table: "DocumentType",
                columns: new[] { "IdDocumentType", "Name" },
                values: new object[,]
                {
                    { 1, "Cédula de Ciudadanía" },
                    { 2, "Pasaporte" },
                    { 3, "Cédula de Extranjería" },
                    { 4, "Tarjeta de Identidad" }
                });

            migrationBuilder.InsertData(
                table: "EmployeeRole",
                columns: new[] { "IdRole", "RoleName" },
                values: new object[,]
                {
                    { 1, "Piloto" },
                    { 2, "Copiloto" },
                    { 3, "Auxiliar de vuelo" },
                    { 4, "Agente de aeropuerto" },
                    { 5, "Supervisor de operaciones" }
                });

            migrationBuilder.InsertData(
                table: "Gender",
                columns: new[] { "IdGender", "Description" },
                values: new object[,]
                {
                    { 1, "Masculino" },
                    { 2, "Femenino" },
                    { 3, "No binario" },
                    { 4, "Prefiero no decir" }
                });

            migrationBuilder.InsertData(
                table: "PaymentMethod",
                columns: new[] { "IdPaymentMethod", "MethodName" },
                values: new object[,]
                {
                    { 1, "Tarjeta de crédito" },
                    { 2, "Tarjeta de débito" },
                    { 3, "Transferencia bancaria" },
                    { 4, "Efectivo" },
                    { 5, "PSE" }
                });

            migrationBuilder.InsertData(
                table: "SeatClass",
                columns: new[] { "IdClase", "ClassName" },
                values: new object[,]
                {
                    { 1, "Económica" },
                    { 2, "Ejecutiva" },
                    { 3, "Primera Clase" }
                });

            migrationBuilder.InsertData(
                table: "SystemStatus",
                columns: new[] { "IdStatus", "EntityType", "StatusName" },
                values: new object[,]
                {
                    { 1, "Flight", "Programado" },
                    { 2, "Flight", "En vuelo" },
                    { 3, "Flight", "Aterrizado" },
                    { 4, "Flight", "Cancelado" },
                    { 5, "Flight", "Demorado" },
                    { 6, "Booking", "Confirmada" },
                    { 7, "Booking", "Pendiente" },
                    { 8, "Booking", "Cancelada" },
                    { 9, "Ticket", "Activo" },
                    { 10, "Ticket", "Usado" },
                    { 11, "Ticket", "Cancelado" },
                    { 12, "CheckIn", "Completado" },
                    { 13, "CheckIn", "Pendiente" },
                    { 14, "Baggage", "Registrado" },
                    { 15, "Baggage", "En tránsito" },
                    { 16, "Baggage", "Entregado" },
                    { 17, "Payment", "Aprobado" },
                    { 18, "Payment", "Pendiente" },
                    { 19, "Payment", "Rechazado" }
                });

            migrationBuilder.InsertData(
                table: "TimeZone",
                columns: new[] { "IdTimeZone", "Name", "UTCOffset" },
                values: new object[,]
                {
                    { 1, "Bogotá / Lima / Quito", "UTC-5" },
                    { 2, "Buenos Aires / Santiago", "UTC-3" },
                    { 3, "Ciudad de México", "UTC-6" },
                    { 4, "Nueva York / Miami", "UTC-5" },
                    { 5, "Madrid / París", "UTC+1" },
                    { 6, "Londres", "UTC+0" },
                    { 7, "Caracas", "UTC-4" }
                });

            migrationBuilder.InsertData(
                table: "City",
                columns: new[] { "IdCity", "IdCountry", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Bogotá" },
                    { 2, 1, "Medellín" },
                    { 3, 1, "Cali" },
                    { 4, 1, "Cartagena" },
                    { 5, 1, "Barranquilla" },
                    { 6, 2, "Caracas" },
                    { 7, 3, "Quito" },
                    { 8, 3, "Guayaquil" },
                    { 9, 4, "Lima" },
                    { 10, 5, "São Paulo" },
                    { 11, 5, "Río de Janeiro" },
                    { 12, 6, "Buenos Aires" },
                    { 13, 7, "Santiago" },
                    { 14, 8, "Ciudad de México" },
                    { 15, 8, "Cancún" },
                    { 16, 9, "Ciudad de Panamá" },
                    { 17, 10, "Madrid" },
                    { 18, 10, "Barcelona" },
                    { 19, 11, "Miami" },
                    { 20, 11, "Nueva York" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BaggageType",
                keyColumn: "IdBaggageType",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "BaggageType",
                keyColumn: "IdBaggageType",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "BaggageType",
                keyColumn: "IdBaggageType",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CheckInChannel",
                keyColumn: "IdChannel",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CheckInChannel",
                keyColumn: "IdChannel",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CheckInChannel",
                keyColumn: "IdChannel",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CheckInChannel",
                keyColumn: "IdChannel",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "IdCity",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "DocumentType",
                keyColumn: "IdDocumentType",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DocumentType",
                keyColumn: "IdDocumentType",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DocumentType",
                keyColumn: "IdDocumentType",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DocumentType",
                keyColumn: "IdDocumentType",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EmployeeRole",
                keyColumn: "IdRole",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EmployeeRole",
                keyColumn: "IdRole",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EmployeeRole",
                keyColumn: "IdRole",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EmployeeRole",
                keyColumn: "IdRole",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EmployeeRole",
                keyColumn: "IdRole",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Gender",
                keyColumn: "IdGender",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Gender",
                keyColumn: "IdGender",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Gender",
                keyColumn: "IdGender",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Gender",
                keyColumn: "IdGender",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PaymentMethod",
                keyColumn: "IdPaymentMethod",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PaymentMethod",
                keyColumn: "IdPaymentMethod",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PaymentMethod",
                keyColumn: "IdPaymentMethod",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PaymentMethod",
                keyColumn: "IdPaymentMethod",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PaymentMethod",
                keyColumn: "IdPaymentMethod",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SeatClass",
                keyColumn: "IdClase",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SeatClass",
                keyColumn: "IdClase",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SeatClass",
                keyColumn: "IdClase",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "TimeZone",
                keyColumn: "IdTimeZone",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TimeZone",
                keyColumn: "IdTimeZone",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TimeZone",
                keyColumn: "IdTimeZone",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TimeZone",
                keyColumn: "IdTimeZone",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TimeZone",
                keyColumn: "IdTimeZone",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TimeZone",
                keyColumn: "IdTimeZone",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TimeZone",
                keyColumn: "IdTimeZone",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "IdCountry",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "IdCountry",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "IdCountry",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "IdCountry",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "IdCountry",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "IdCountry",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "IdCountry",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "IdCountry",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "IdCountry",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "IdCountry",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "IdCountry",
                keyValue: 11);
        }
    }
}
