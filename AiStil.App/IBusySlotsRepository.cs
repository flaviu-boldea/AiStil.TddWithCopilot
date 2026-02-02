namespace AiStil.App;

/// <summary>
/// Provides access to busy time slots for a stylist.
/// </summary>
/// <remarks>
/// This is an application-layer abstraction. Implementations typically query a database and return only the
/// minimum set of slots needed to evaluate overlap for a requested slot (same stylist, relevant time window).
/// </remarks>
public interface ISlotsRepository
{
    /// <summary>
    /// Loads the stylist aggregate for booking.
    /// </summary>
    /// <param name="requestedSlot">The slot we want to schedule.</param>
    /// <returns>The stylist with the relevant busy slots preloaded.</returns>
    /// <remarks>
    /// Implementations typically query a database and preload only slots that could overlap the
    /// <paramref name="requestedSlot"/> (same stylist, relevant time window).
    /// </remarks>
    Stylist LoadStylistForBooking(Slot requestedSlot);

    /// <summary>
    /// Persists a newly created appointment.
    /// </summary>
    /// <param name="appointment">The appointment to persist.</param>
    /// <remarks>
    /// Implementations should persist the appointment atomically with any related slot/busy-slot changes
    /// (one transaction/unit of work). Storage-level constraints (indexes/locking) should also prevent
    /// double-booking under concurrent requests.
    /// </remarks>
    void SaveAppointment(Appointment appointment);
}
