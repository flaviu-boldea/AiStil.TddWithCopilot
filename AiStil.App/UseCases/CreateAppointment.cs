using AiStil.App;
using AiStil.App.Domain;

namespace AiStil.App.UseCases;

public sealed class CreateAppointment(
    CreateAppointmentRequest request,
    ISchedulingRepository repository)
{
    public CreateAppointmentResponse Execute()
    {
        Stylist stylist = repository.LoadStylistForBooking(request.Slot);

        Appointment appointment = stylist.Book(request.Slot, request.ClientId);

        repository.SaveAppointment(appointment);

        return new CreateAppointmentResponse(appointment);
    }
}

public sealed record CreateAppointmentRequest(
    Slot Slot,
    Guid ClientId
);

public sealed record CreateAppointmentResponse(Appointment Appointment);
