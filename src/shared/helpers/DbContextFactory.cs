using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.shared.helpers;

public class DbContextFactory
{
    /// <summary>Busca la carpeta del proyecto que contiene appsettings.json (útil si el cwd no es la raíz del .csproj).</summary>
    public static string ResolveConfigurationBasePath()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        for (var d = dir; d is not null; d = d.Parent)
        {
            if (File.Exists(Path.Combine(d.FullName, "appsettings.json")))
                return d.FullName;
        }

        return Directory.GetCurrentDirectory();
    }

    public static AppDbContext Create()
    {
        var basePath = ResolveConfigurationBasePath();
        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION")
                               ?? config.GetConnectionString("MySqlDB");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("No se encontró una cadena de conexión válida (MySqlDB o MYSQL_CONNECTION).");

        var detectedVersion = MySqlVersionResolver.DetectVersion(connectionString);
        var minVersion = new Version(8, 0, 0);
        if (detectedVersion < minVersion)
            throw new NotSupportedException($"Versión de MySQL no soportada: {detectedVersion}. Requiere {minVersion} o superior.");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connectionString, new MySqlServerVersion(detectedVersion))
            .Options;

        return new AppDbContext(options);
    }
}
