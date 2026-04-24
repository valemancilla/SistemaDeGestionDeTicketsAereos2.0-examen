namespace SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;

/// <summary>
/// Clase estática que mantiene el estado global de la aplicación
/// durante la ejecución (como si fuera una "memoria compartida").
/// </summary>
public static class AppState 
{
    /// <summary>
    /// Indica si el usuario ha iniciado sesión o no.
    /// true = autenticado, false = no autenticado.
    /// </summary>
    public static bool IsAuthenticated { get; set; } = false;

    /// <summary>
    /// Guarda el ID del usuario actualmente logueado.
    /// Se usa para identificarlo en operaciones (consultas, reservas, etc.).
    /// </summary>
    public static int IdUser { get; set; } = 0;
    
    /// <summary>
    /// Guarda el rol del usuario según la base de datos.
    /// Sirve para restringir acceso a menús o funcionalidades.
    /// Ejemplo:
    /// 1 = Administrador
    /// 2 = Cliente
    /// </summary>
    public static int IdUserRole { get; set; } = 0;

    /// <summary>
    /// Guarda el nombre de usuario actual (puede ser null si no hay sesión).
    /// Se usa para mostrar información en pantalla (ej: "Bienvenido, Juan").
    /// </summary>
    public static string? CurrentUser { get; set; }

    /// <summary>
    /// ID de la persona vinculada al usuario logueado (null si el usuario no tiene persona asociada).
    /// Se usa para que el cliente pueda ver y editar solo sus propios datos.
    /// </summary>
    public static int? IdPerson { get; set; }
}