namespace AiStil.App;

public sealed class CreateAppointmentCommand(
    CreateAppointmentRequest request,
    ISlotsRepository busySlotsRepository)
{
    public CreateAppointmentResponse Execute()
    {
        Stylist stylist = busySlotsRepository.LoadStylistForBooking(request.Slot);

        Appointment appointment = stylist.Book(request.Slot, request.ClientId);

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
