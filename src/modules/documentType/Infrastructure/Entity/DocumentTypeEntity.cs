// El tipo de documento se usa para saber qué clase de identificación tiene una persona
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.documentType.Infrastructure.Entity;

// Entidad que representa la tabla DocumentType en la base de datos
// Ejemplos: Cédula, Pasaporte, Tarjeta de identidad
public class DocumentTypeEntity
{
    // Clave primaria
    public int IdDocumentType { get; set; }

    // Nombre del tipo de documento (ej: Cédula de Ciudadanía, Pasaporte)
    public string Name { get; set; } = string.Empty;

    // Personas que tienen este tipo de documento
    public ICollection<PersonEntity> Persons { get; set; } = new List<PersonEntity>();
}
