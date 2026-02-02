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
    /// Returns the busy slots that could overlap the <paramref name="requestedSlot"/>.
    /// </summary>
    /// <param name="requestedSlot">The slot we want to schedule.</param>
    /// <returns>
    /// Busy slots for the same stylist that may overlap the requested slot.
    /// Implementations should filter by stylist and time window to avoid loading unnecessary data.
    /// </returns>
    /// <remarks>
    /// Expected overlap rule (half-open interval): two slots overlap if
    /// <c>requested.StartTime &lt; busy.EndTime</c> and <c>busy.StartTime &lt; requested.EndTime</c>.
    /// </remarks>
    IEnumerable<Slot> GetBusySlotsForOverlap(Slot requestedSlot);

    /// <summary>
    /// Persists a newly-created busy slot.
    /// </summary>
    /// <param name="slot">The slot to mark as busy.</param>
    /// <remarks>
    /// Called only after the use case has determined there is no overlap.
    /// Implementations should persist the slot in a way that prevents double-booking.
    /// </remarks>
    void SaveBusySlot(Slot slot);

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
