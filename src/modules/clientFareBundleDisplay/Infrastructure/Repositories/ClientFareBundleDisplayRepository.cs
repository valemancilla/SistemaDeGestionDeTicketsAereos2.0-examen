using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Domain;
using SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;

namespace SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Infrastructure.Repositories;

public sealed class ClientFareBundleDisplayRepository
{
    private const int SingletonId = 1;
    private readonly AppDbContext _db;

    public ClientFareBundleDisplayRepository(AppDbContext db) => _db = db;

    public async Task<ClientFareBundleDisplayData> GetOrCreateSingletonAsync(CancellationToken ct = default)
    {
        var entity = await _db.Set<ClientFareBundleDisplayEntity>().AsTracking()
            .FirstOrDefaultAsync(x => x.Id == SingletonId, ct);
        if (entity is not null)
            return ToDomain(entity);

        entity = new ClientFareBundleDisplayEntity
        {
            Id = SingletonId,
            RefCarryOnCop = ClientFareBundleDisplayDefaults.ReferenceCarryOnCop,
            RefCheckedCop = ClientFareBundleDisplayDefaults.ReferenceCheckedCop,
            ClassicMultiplier = ClientFareBundleDisplayDefaults.ClassicMultiplier,
            FlexMultiplier = ClientFareBundleDisplayDefaults.FlexMultiplier,
            UnpublishedFareReferenceCop = ClientFareBundleDisplayDefaults.UnpublishedFareReferenceCop,
            SeatSelectionFromCop = ClientFareBundleDisplayDefaults.SeatSelectionFromCop,
            SubtitleLine = ClientFareBundleDisplayDefaults.SubtitleLine,
            ExplainerLine = ClientFareBundleDisplayDefaults.ExplainerLineTemplate,
            BasicBodyMarkup = ClientFareBundleDisplayDefaults.BasicBody(),
            ClassicBodyMarkup = ClientFareBundleDisplayDefaults.ClassicBody,
            FlexBodyMarkup = ClientFareBundleDisplayDefaults.FlexBody,
        };
        await _db.Set<ClientFareBundleDisplayEntity>().AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
        return ToDomain(entity);
    }

    public async Task UpdateAsync(ClientFareBundleDisplayData d, CancellationToken ct = default)
    {
        var entity = await _db.Set<ClientFareBundleDisplayEntity>().FirstOrDefaultAsync(x => x.Id == SingletonId, ct)
            ?? throw new InvalidOperationException("Falta el registro de pantalla; reiniciá o ejecutá la migración.");
        entity.RefCarryOnCop = d.RefCarryOnCop;
        entity.RefCheckedCop = d.RefCheckedCop;
        entity.ClassicMultiplier = d.ClassicMultiplier;
        entity.FlexMultiplier = d.FlexMultiplier;
        entity.UnpublishedFareReferenceCop = d.UnpublishedFareReferenceCop;
        entity.SeatSelectionFromCop = d.SeatSelectionFromCop;
        entity.SubtitleLine = d.SubtitleLine;
        entity.ExplainerLine = d.ExplainerLine;
        entity.BasicBodyMarkup = d.BasicBodyMarkup;
        entity.ClassicBodyMarkup = d.ClassicBodyMarkup;
        entity.FlexBodyMarkup = d.FlexBodyMarkup;
    }

    private static ClientFareBundleDisplayData ToDomain(ClientFareBundleDisplayEntity e) => new()
    {
        Id = e.Id,
        RefCarryOnCop = e.RefCarryOnCop,
        RefCheckedCop = e.RefCheckedCop,
        ClassicMultiplier = e.ClassicMultiplier,
        FlexMultiplier = e.FlexMultiplier,
        UnpublishedFareReferenceCop = e.UnpublishedFareReferenceCop,
        SeatSelectionFromCop = e.SeatSelectionFromCop,
        SubtitleLine = e.SubtitleLine,
        ExplainerLine = e.ExplainerLine,
        BasicBodyMarkup = e.BasicBodyMarkup,
        ClassicBodyMarkup = e.ClassicBodyMarkup,
        FlexBodyMarkup = e.FlexBodyMarkup,
    };
}
