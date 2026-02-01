namespace AiStil.App;

public sealed class CreateAppointmentCommand(
    CreateAppointmentRequest request,
    IEnumerable<AppointmentSlot>? busySlots = null)
{
    public CreateAppointmentResponse Execute()
    {
        if (IsSlotBusy(request.Slot, busySlots ?? []))
        {
            throw new SlotIsBusyException();
        }

        var appointment = new Appointment(
            Slot: request.Slot,
            ClientId: request.ClientId);

        return new CreateAppointmentResponse(appointment);
    }

    private static bool IsSlotBusy(AppointmentSlot requestedSlot, IEnumerable<AppointmentSlot> busySlots) =>
        busySlots.Any(busySlot => IsOverlappingForSameStylist(requestedSlot, busySlot));

    private static bool IsOverlappingForSameStylist(AppointmentSlot requestedSlot, AppointmentSlot busySlot) =>
        requestedSlot.StylistId == busySlot.StylistId &&
        Overlaps(requestedSlot.StartTime, requestedSlot.EndTime, busySlot.StartTime, busySlot.EndTime);

    private static bool Overlaps(DateTime startA, DateTime endA, DateTime startB, DateTime endB) =>
        startA < endB && startB < endA;
}

public sealed record CreateAppointmentRequest(
    AppointmentSlot Slot,
    Guid ClientId
);

public sealed record CreateAppointmentResponse(Appointment Appointment);

public sealed class SlotIsBusyException : Exception;
