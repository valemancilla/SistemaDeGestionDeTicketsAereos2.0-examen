namespace SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Domain;

public sealed class ClientFareBundleDisplayData
{
    public int Id { get; set; } = 1;
    public decimal RefCarryOnCop { get; set; }
    public decimal RefCheckedCop { get; set; }
    public decimal ClassicMultiplier { get; set; }
    public decimal FlexMultiplier { get; set; }
    public decimal UnpublishedFareReferenceCop { get; set; }
    public string SubtitleLine { get; set; } = string.Empty;
    public string ExplainerLine { get; set; } = string.Empty;
    public string BasicBodyMarkup { get; set; } = string.Empty;
    public string ClassicBodyMarkup { get; set; } = string.Empty;
    public string FlexBodyMarkup { get; set; } = string.Empty;
    public decimal SeatSelectionFromCop { get; set; } = 27_000m;
}
