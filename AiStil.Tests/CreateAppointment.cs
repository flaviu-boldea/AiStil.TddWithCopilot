namespace AiStil.Tests;

public class CreateAppointmentTests
{
    [Fact]
    public void CanCreateAppointment()
    {
        // Arrange
        var startTime = new DateTime(2024, 7, 1, 10, 0, 0);
        var endTime = startTime.AddHours(1);
        var stylistId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var clientId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var appointmentSlot = new AppointmentSlot(startTime, endTime, stylistId);
        var request = new CreateAppointmentRequest(appointmentSlot, clientId);

        // Act
        CreateAppointmentResponse response = new CreateAppointmentCommand(request).Execute();

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Appointment);
        Assert.Equal(startTime, response.Appointment.StartTime);
        Assert.Equal(endTime, response.Appointment.EndTime);
        Assert.Equal(stylistId, response.Appointment.StylistId);
        Assert.Equal(clientId, response.Appointment.ClientId);
    }

    [Fact]
    public void CannotCreateAppointmentWhenSlotIsBusy()
    {
        // Arrange
        var startTime = new DateTime(2024, 7, 1, 10, 0, 0);
        var endTime = startTime.AddHours(1);
        var stylistId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var clientId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var appointmentSlot = new AppointmentSlot(startTime, endTime, stylistId);

        var busySlots = new[]
        {
            new AppointmentSlot(startTime.AddMinutes(30), endTime.AddMinutes(30), stylistId)
        };

        var request = new CreateAppointmentRequest(appointmentSlot, clientId);

        // Act
        var act = () => new CreateAppointmentCommand(request, busySlots).Execute();

        // Assert
        Assert.Throws<SlotIsBusyException>(act);
    }
}

internal sealed class CreateAppointmentCommand(
    CreateAppointmentRequest request,
    IEnumerable<AppointmentSlot>? busySlots = null)
{
    internal CreateAppointmentResponse Execute()
    {
        busySlots ??= [];
        foreach (var busySlot in busySlots)
        {
            bool isOverlapping = request.Slot.StartTime < busySlot.EndTime &&
                                 busySlot.StartTime < request.Slot.EndTime &&
                                 request.Slot.StylistId == busySlot.StylistId;

            if (isOverlapping)
            {
                throw new SlotIsBusyException();
            }
        }
        
        return new CreateAppointmentResponse
        {
            Appointment = new Appointment
            {
                StartTime = request.Slot.StartTime,
                EndTime = request.Slot.EndTime,
                StylistId = request.Slot.StylistId,
                ClientId = request.ClientId
            }
        };
    }
}

internal sealed record AppointmentSlot(DateTime StartTime, DateTime EndTime, Guid StylistId);

internal sealed record CreateAppointmentRequest(
    AppointmentSlot Slot,
    Guid ClientId
);

internal sealed class SlotIsBusyException : Exception;

internal class CreateAppointmentResponse
{
    public Appointment? Appointment { get; internal set; }
}

public class Appointment
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Guid StylistId { get; set; }
    public Guid ClientId { get; set; }
}