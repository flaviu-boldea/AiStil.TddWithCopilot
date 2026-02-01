namespace AiStil.Tests;

public class CreateAppointment
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
        AppointmentResponse response = new CreateAppointmentCommand(request).Execute();

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Appointment);
        Assert.Equal(startTime, response.Appointment.StartTime);
        Assert.Equal(endTime, response.Appointment.EndTime);
        Assert.Equal(stylistId, response.Appointment.StylistId);
        Assert.Equal(clientId, response.Appointment.ClientId);
    }
}

internal sealed class CreateAppointmentCommand(CreateAppointmentRequest request)
{
    internal AppointmentResponse Execute()
    {
        return new AppointmentResponse
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

internal sealed record CreateAppointmentRequest(AppointmentSlot Slot, Guid ClientId);

internal class AppointmentResponse
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