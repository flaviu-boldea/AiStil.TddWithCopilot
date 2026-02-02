namespace AiStil.App;

public sealed class Stylist
{
    private readonly List<Slot> busySlots;

    public Stylist(Guid stylistId, IEnumerable<Slot> busySlots)
    {
        StylistId = stylistId;
        this.busySlots = busySlots.ToList();
    }

    public Guid StylistId { get; }

    public Appointment Book(Slot requestedSlot, Guid clientId)
    {
        if (requestedSlot.StylistId != StylistId)
        {
            throw new ArgumentException("Slot stylist does not match.", nameof(requestedSlot));
        }

        if (busySlots.Any(busySlot => requestedSlot.Overlaps(busySlot)))
        {
            throw new SlotIsBusyException();
        }

        busySlots.Add(requestedSlot);
        return new Appointment(requestedSlot, clientId);
    }
}
