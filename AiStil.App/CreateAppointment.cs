namespace AiStil.App;

public sealed class CreateAppointmentCommand(
    CreateAppointmentRequest request,
    ISlotsRepository busySlotsRepository)
{
    public CreateAppointmentResponse Execute()
    {
        IEnumerable<AppointmentSlot> busySlots = busySlotsRepository.GetBusySlotsForOverlap(request.Slot);
        if (IsSlotBusy(request.Slot, busySlots))
        {
            throw new SlotIsBusyException();
        }

        busySlotsRepository.SaveBusySlot(request.Slot);

        var appointment = new Appointment(
            Slot: request.Slot,
            ClientId: request.ClientId);

        busySlotsRepository.SaveAppointment(appointment);

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
