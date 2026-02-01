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

        // Act
        AppointmentResponse response = new CreateAppointmentCommand().Execute(startTime, endTime, stylistId, clientId);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Appointment);
        Assert.Equal(startTime, response.Appointment.StartTime);
        Assert.Equal(endTime, response.Appointment.EndTime);
        Assert.Equal(stylistId, response.Appointment.StylistId);
        Assert.Equal(clientId, response.Appointment.ClientId);
    }
}

internal class CreateAppointmentCommand
{
    public CreateAppointmentCommand()
    {
    }

    internal AppointmentResponse Execute(DateTime startTime, DateTime endTime, Guid stylistId, Guid clientId)
    {
        return new AppointmentResponse
        {
            Appointment = new Appointment
            {
                StartTime = startTime,
                EndTime = endTime,
                StylistId = stylistId,
                ClientId = clientId
            }
        };
    }
}

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