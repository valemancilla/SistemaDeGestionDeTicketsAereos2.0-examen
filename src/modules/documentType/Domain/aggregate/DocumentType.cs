// El tipo de documento identifica qué clase de identificación presenta una persona (cédula, pasaporte, etc.)
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.aggregate;

// Agregado DocumentType: encapsula las reglas de negocio de un tipo de documento de identidad
public class DocumentType
{
    // ID del tipo de documento (Value Object)
    public DocumentTypeId Id { get; private set; }

    // Nombre del tipo de documento (ej: "Cédula de Ciudadanía", "Pasaporte")
    public DocumentTypeName Name { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private DocumentType(DocumentTypeId id, DocumentTypeName name)
    {
        Id = id;
        Name = name;
    }

    // Método de fábrica para crear o reconstruir un tipo de documento desde la base de datos
    public static DocumentType Create(int id, string name)
    {
        // Regla: el nombre es validado por su Value Object (no vacío)
        return new DocumentType(
            DocumentTypeId.Create(id),
            DocumentTypeName.Create(name)
        );
    }

    // Método de fábrica para crear un tipo de documento nuevo (ID = 0, la BD lo asigna después)
    public static DocumentType CreateNew(string name) => Create(0, name);
}
