using OverstromingsApp.Core.Models;

namespace OverstromingsApp.Helpers;

/// <summary>
/// Marker-message die wordt verzonden wanneer de
/// <see cref="DataModel"/>-tabel is gewijzigd
/// (record toegevoegd, verwijderd of aangepast).
/// Pagina's die data tonen kunnen hierop luisteren
/// om zichzelf te verversen.
/// </summary>
public sealed class NeerslagChangedMessage { }
