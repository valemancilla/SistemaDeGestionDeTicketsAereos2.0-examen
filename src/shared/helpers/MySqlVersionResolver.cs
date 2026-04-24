using MySqlConnector;

namespace SistemaDeGestionDeTicketsAereos.src.shared.helpers;

public class MySqlVersionResolver
{
    public static Version DetectVersion(string connectionString)
    {
        try
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var raw = conn.ServerVersion;
            var clean = raw.Split('-')[0];
            return Version.Parse(clean);
        }
        catch
        {
            // Para design-time (migraciones) no es estrictamente necesario conectarse a la BD:
            // si falla la conexión, asumimos un mínimo compatible.
            return new Version(8, 0, 0);
        }
    }
}
