namespace AiStil.Tests;

public class CreateAppointmentTests
{
    [Fact]
    public void ShouldAcceptWhenSlotIsFree()
    {
        // Arrange
        var request = CreateRequest();

        // Act
        CreateAppointmentResponse response = new CreateAppointmentCommand(request).Execute();

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Appointment);
        Assert.Equal(request.Slot.StartTime, response.Appointment.StartTime);
        Assert.Equal(request.Slot.EndTime, response.Appointment.EndTime);
        Assert.Equal(request.Slot.StylistId, response.Appointment.StylistId);
        Assert.Equal(request.ClientId, response.Appointment.ClientId);
    }

    [Fact]
    public void ShouldRejectWhenSlotIsBusy()
    {
        // Arrange
        var request = CreateRequest();

        var busySlots = new[]
        {
            new AppointmentSlot(
                request.Slot.StartTime.AddMinutes(30),
                request.Slot.EndTime.AddMinutes(30),
                request.Slot.StylistId)
        };

        // Act
        var act = () => new CreateAppointmentCommand(request, busySlots).Execute();

        // Assert
        Assert.Throws<SlotIsBusyException>(act);
    }

    private static CreateAppointmentRequest CreateRequest()
    {
        var startTime = new DateTime(2024, 7, 1, 10, 0, 0);
        var endTime = startTime.AddHours(1);
        var stylistId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var clientId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var slot = new AppointmentSlot(startTime, endTime, stylistId);
        return new CreateAppointmentRequest(slot, clientId);
    }
}

internal sealed class CreateAppointmentCommand(
    CreateAppointmentRequest request,
    IEnumerable<AppointmentSlot>? busySlots = null)
{
    internal CreateAppointmentResponse Execute()
    {
        if (IsSlotBusy(request.Slot, busySlots ?? []))
        {
            throw new SlotIsBusyException();
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

    private static bool IsSlotBusy(AppointmentSlot requestedSlot, IEnumerable<AppointmentSlot> busySlots) =>
        busySlots.Any(busySlot => IsOverlappingForSameStylist(requestedSlot, busySlot));

    private static bool IsOverlappingForSameStylist(AppointmentSlot requestedSlot, AppointmentSlot busySlot) =>
        requestedSlot.StylistId == busySlot.StylistId &&
        Overlaps(requestedSlot.StartTime, requestedSlot.EndTime, busySlot.StartTime, busySlot.EndTime);

    private static bool Overlaps(DateTime startA, DateTime endA, DateTime startB, DateTime endB) =>
        startA < endB && startB < endA;
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