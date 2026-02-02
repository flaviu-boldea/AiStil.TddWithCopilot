namespace AiStil.App.Domain;

public sealed record Slot(DateTime StartTime, DateTime EndTime, Guid StylistId)
{
    public bool Overlaps(Slot other) =>
        StylistId == other.StylistId &&
        StartTime < other.EndTime && other.StartTime < EndTime;
}
