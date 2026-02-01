namespace AiStil.App;

public interface ISlotsRepository
{
    IEnumerable<AppointmentSlot> GetBusySlotsForOverlap(AppointmentSlot requestedSlot);
}
