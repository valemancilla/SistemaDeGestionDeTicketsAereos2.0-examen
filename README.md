# Sistema de Gestión de Tiquetes Aéreos

## Descripción del Proyecto

Aplicación de **consola en C# (.NET)** para la **venta, reserva, consulta y administración de tiquetes aéreos**. El sistema permite registrar y gestionar aerolíneas, aeropuertos, rutas, vuelos, clientes, reservas, tiquetes y pagos, con la información persistida en **MySQL**. La interacción es 100 % por menús en consola (sin interfaz web ni móvil).

Enfoque **formativo y profesional**: modelado del dominio, **Entity Framework Core** como acceso a datos, **LINQ** para consultas y reportes, validaciones y reglas de negocio. Los pagos y ciertos estados se manejan de forma **simulada** (no hay pasarela de pago real ni integración con sistemas externos de aerolíneas).

---

## Características Destacadas

- Menús de consola con navegación por rol (**administrador** vs **cliente**).
- Persistencia en **MySQL** con **EF Core** (entidades, migraciones, repositorios).
- Módulos funcionales: aerolíneas, aeropuertos y destinos, vuelos y tarifas, clientes, reservas, tiquetes y check-in, pagos, tripulación, reportes.
- Control de **disponibilidad de asientos** por vuelo al crear o cancelar reservas.
- **Emisión de tiquetes** a partir de reservas en estado adecuado (reglas de negocio en casos de uso).
- **Reportes operativos** con LINQ (ocupación, disponibilidad, ingresos por aerolínea, reservas por estado, tiquetes por fechas, clientes con más reservas, destinos más solicitados, entre otros).
- Trazabilidad básica de estados (reservas, tiquetes, vuelos) mediante historiales donde aplica.
- Autenticación contra base de datos y **control de acceso por rol** (RBAC).
- **Examen 3 — Check-in y pase de abordar** (flujo completo en consola, validaciones, persistencia en MySQL y consultas operativas).

---

## Examen : Check-in y pase de abordar (implementado)

Requerimiento académico integrado al sistema principal. Resumen de lo que hace la aplicación y dónde está el código.

### Objetivos cubiertos

- Check-in para **tiquetes emitidos** con reserva **pagada** y pago **aprobado**.
- Validación de **vuelo vigente** (salida futura), **no cancelado** y en estado permitido (**Programado** o **Demorado**).
- Ventana de tiempo de check-in (apertura amplia para datos de prueba; cierre típico **45 minutos** antes de la salida).
- **Pase de abordar** persistido (`BoardingPass` en MySQL): código único, puerta, asiento, hora de abordaje, nombre del pasajero, estado en catálogo (**Generado**); en consola se indica además el **estado operativo Activo** hasta el registro de abordaje en puerta.
- Tiquete pasa a estado **Check-in realizado**; registro de **CheckIn** en estado **Completado**.
- **Consulta de pase** por código de tiquete, código de pase o documento; si hay **varios pases** para el mismo documento, la consola pide elegir cuál.
- **Varios pasajeros** en la misma reserva (mismo PNR / mismo tiquete de reserva): la consola pide **elegir pasajero** antes de validar y completar el check-in.
- **Listado de pasajeros listos para abordar** (tiquetes en *Check-in realizado*), con filtro por vuelo y orden configurable (menú administrador).
- **Registrar abordaje** (admin): tiquete → **Abordado**, pase → **Activo** en base de datos (cierre del ciclo del examen).
- Mensajes de error alineados al enunciado cuando aplica: *Tiquete no emitido*, *Pago pendiente*, *Vuelo cancelado*, *El vuelo no está vigente*, *Vuelo no habilitado*, *Check-in ya realizado*, *Fuera del tiempo permitido*.

### Menús en consola

| Rol | Ruta en consola |
|-----|------------------|
| **Cliente** | Inicio de sesión → **Check-in y tiquetes** → *Realizar check-in* / *Consultar pase de abordar* / *Ver mis tiquetes* / *Ver mis check-ins*. |
| **Administrador** | Menú principal → **Gestión de Tiquetes y Check-In** → opciones de reserva en sesión, *Consultar pasajeros listos para abordar*, *Registrar abordaje*, etc. |

### Emisión del tiquete al pagar (relación con el examen)

Tras un **pago aprobado**, el sistema deja la reserva en **Pagada** (o equivalente si falta el estado en catálogo) y ejecuta **`TicketIssuanceAfterPaymentService.EmitTicketsForBookingAsync`**, que actualiza los tiquetes de la reserva de **Activo** a **Emitido**. Así el check-in del examen puede exigir tiquete **Emitido** sin pasos manuales extra.

### Archivos principales (referencia rápida)

| Archivo | Rol |
|---------|-----|
| `src/modules/ticket/Application/Services/ExamCheckInService.cs` | Reglas de negocio del check-in: búsqueda por código o PNR, validaciones, elección de pasajero, asiento, creación de check-in, actualización de tiquete y pase. |
| `src/modules/ticket/UI/ClientPnrCheckInMenu.cs` | UI del cliente para *Realizar check-in* (delega en `ExamCheckInService`). |
| `src/modules/ticket/Application/Services/BoardingPassQueryService.cs` | Consulta de pase con control de acceso y desambiguación por documento. |
| `src/modules/ticket/UI/TicketMenu.cs` | *Consultar pase de abordar*, listado de pasajeros listos, herramientas admin de check-in y pase. |
| `src/modules/ticket/UI/RegisterPassengerBoardingMenu.cs` | Abordaje en puerta: tiquete **Abordado**, pase **Activo**. |
| `src/modules/payment/UI/PaymentMenu.cs` | Tras pago aprobado, llama a la emisión automática de tiquetes. |
| `src/modules/payment/Application/Services/TicketIssuanceAfterPaymentService.cs` | Pasa tiquetes a **Emitido** cuando el pago dejó la reserva lista. |
| `src/modules/boardingPass/` | Dominio, casos de uso y persistencia del **pase de abordar**. |
| `src/modules/CheckIn/` | Dominio y persistencia del **check-in**. |

### Documentación en el código (comentarios)

Por el **tamaño del repositorio**, los comentarios en **español** se concentran en: punto de entrada (`Program.cs`), **pagos** ligados a emisión, **servicios y menús del Examen 3**, y casos de uso mínimos del **pase** y **check-in**. El resto del código sigue el estilo ya existente (módulos por capas, nombres descriptivos). Si el curso exige XML en cada clase, se puede ampliar con plantillas del IDE sin duplicar lógica en comentarios.

---

## Objetivo

**Objetivo general:** desarrollar una aplicación de consola en C# conectada a **MySQL** que permita administrar vuelos, clientes, reservas y emisión de tiquetes, usando **LINQ** para consultas, filtros, agrupaciones y procesamiento de datos.

**Objetivos específicos (resumen):**

- Modelar datos de aerolíneas, vuelos, rutas, clientes, reservas, tiquetes y pagos.
- Implementar consola con menús y submenús; conexión app–MySQL para guardar y consultar información.
- Registrar y administrar clientes, vuelos, destinos y reservas; emitir tiquetes desde reservas válidas.
- Controlar disponibilidad de asientos por vuelo; generar reportes con LINQ.
- Organizar el código por capas/módulos (presentación, aplicación, dominio, infraestructura), con validaciones y persistencia coherente del estado.

**Alcance (procesos cubiertos):** registro de aerolíneas; aeropuertos y destinos; creación de vuelos (origen, destino, fecha, horarios, capacidad, disponibilidad, estado); clientes; reservas (crear, confirmar/cancelar, cupos); emisión de tiquetes; consultas desde menú; reportes LINQ. **Fuera de alcance:** interfaz web/móvil, pasarelas de pago reales y APIs externas de aerolíneas.

---

## Tecnologías Utilizadas

| Tecnología | Uso en el proyecto |
|------------|-------------------|
| **C# / .NET** | Aplicación de consola ejecutable. |
| **MySQL 8** | Motor relacional (recomendado documentación oficial con EF Core / conector compatible). |
| **Entity Framework Core** | O/RM: `DbContext`, entidades, migraciones, repositorios. |
| **LINQ** | Consultas en casos de uso y sobre todo en el módulo de reportes (filtros, ordenamiento, agrupación, agregaciones). |
| **Pomelo.EntityFrameworkCore.MySql** | Proveedor EF Core para MySQL. |
| **Spectre.Console** | Menús, tablas y prompts en consola. |
| **Git** | Control de versiones del repositorio. |

La cadena de conexión se configura en `appsettings.json` (`ConnectionStrings:MySqlDB`). Cada entorno debe definir su propio servidor, base de datos y credenciales.

---

## Estructura del Sistema

```
SistemaDeGestionDeTicketsAereos/
├── Program.cs                    # Punto de entrada
├── appsettings.json              # Cadena de conexión MySQL
├── Migrations/                   # Migraciones EF Core
└── src/
    ├── shared/
    │   ├── context/              # AppDbContext, fábrica en diseño
    │   ├── helpers/            # DbContextFactory, utilidades
    │   └── ui/menus/           # Login, orquestador de menús principal
    └── modules/                 # Un módulo por dominio funcional
        └── <módulo>/
            ├── UI/             # Menús y pantallas de consola (*Menu.cs)
            ├── Application/    # Casos de uso (*UseCase.cs), servicios
            ├── Domain/         # Agregados, value objects, interfaces de repositorio
            └── Infrastructure/ # Entidades EF, configuraciones, repositorios
```

Cada **módulo** sigue la separación **UI → Application → Domain ← Infrastructure**, de modo que la lógica de negocio y el dominio no dependan directamente de la consola ni de detalles de SQL.

---

## Qué Hace Cada Archivo (principales)

| Archivo / carpeta | Función |
|-------------------|---------|
| `Program.cs` | Crea el `DbContext`, inicia `ConsoleMenuOrchestrator.StartAsync()` y captura errores críticos en consola. |
| `appsettings.json` | Contiene `ConnectionStrings:MySqlDB` para conectar a MySQL. |
| `src/shared/helpers/DbContextFactory.cs` | Construye `AppDbContext` leyendo configuración (uso desde menús y casos de uso). |
| `src/shared/context/AppDbContext.cs` | Contexto EF: `DbSet` de entidades y aplicación de configuraciones por módulo. |
| `src/shared/ui/menus/LoginMenu.cs` | Solicita usuario y contraseña, ejecuta login y deja rol e id en `AppState`. |
| `src/shared/ui/menus/ConsoleMenuOrchestrator.cs` | Bucle principal: según rol muestra menú de administrador o de cliente y despacha a cada `*Menu`. Incluye acceso a **Check-in y tiquetes** (cliente) y **Gestión de Tiquetes y Check-In** (admin), cubierto por el Examen 3. |
| `src/shared/ui/menus/AppState.cs` | Estado de sesión (autenticado, id de usuario, rol) compartido por la UI. |
| `src/modules/user/Application/UseCases/LoginUseCase.cs` | Valida credenciales contra el repositorio de usuarios. |
| `src/modules/user/Infrastructure/Repositories/UserRepository.cs` | Acceso a tabla `User` con EF Core. |
| `src/modules/report/UI/ReportsMenu.cs` | Reportes operativos implementados con LINQ sobre datos cargados desde el contexto. |
| `src/modules/*/UI/*Menu.cs` | Punto de entrada de consola de cada módulo (vuelos, reservas, clientes, pagos, etc.). |
| `src/modules/*/Application/UseCases/*.cs` | Orquestación de un caso de uso (crear reserva, emitir tiquete, etc.). |
| `src/modules/*/Infrastructure/Repositories/*.cs` | Implementación concreta de acceso a datos del módulo. |
| `src/modules/ticket/Application/Services/ExamCheckInService.cs` | **Examen 3:** validaciones y persistencia del check-in + generación del pase. |
| `src/modules/ticket/Application/Services/BoardingPassQueryService.cs` | **Examen 3:** consulta de pase con control de acceso y varios resultados por documento. |
| `src/modules/ticket/UI/ClientPnrCheckInMenu.cs` | **Examen 3:** consola de check-in del cliente (delega en `ExamCheckInService`). |
| `Migrations/*.cs` | Evolución del esquema y datos iniciales donde aplica. |

El resto de archivos del repositorio extiende este patrón por entidad (configuración EF, entidades, agregados de dominio).

---

## Credenciales del Administrador

Usuario sembrado en base de datos (configuración `UserEntityConfiguration`, tabla `User`):

| Campo | Valor |
|--------|--------|
| **Usuario** | `admin` |
| **Contraseña** | `12345678` |
| **Rol** | Administrador (`IdUserRole = 1`) |

Los clientes finales se gestionan con rol distinto (por ejemplo registro desde el flujo de login según implementación actual). Cambiar contraseñas en entorno real debe hacerse por menú de usuarios o actualizando datos de forma segura; no uses estas credenciales en producción.

---

## Autores


- valentina mancilla

---

