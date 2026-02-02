namespace AiStil.App;

public sealed class CreateAppointmentCommand(
    CreateAppointmentRequest request,
    ISlotsRepository busySlotsRepository)
{
    public CreateAppointmentResponse Execute()
    {
        IEnumerable<Slot> busySlots = busySlotsRepository.GetBusySlotsForOverlap(request.Slot);
        var stylist = new Stylist(request.Slot.StylistId, busySlots);

        Appointment appointment = stylist.Book(request.Slot, request.ClientId);

        busySlotsRepository.SaveBusySlot(appointment.Slot);

        busySlotsRepository.SaveAppointment(appointment);

        return new CreateAppointmentResponse(appointment);
    }
}

public sealed record CreateAppointmentRequest(
    Slot Slot,
    Guid ClientId
);

public sealed record CreateAppointmentResponse(Appointment Appointment);

public sealed class SlotIsBusyException : Exception;
