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
        Assert.Equal(request.Slot, response.Appointment.Slot);
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

public sealed record AppointmentSlot(DateTime StartTime, DateTime EndTime, Guid StylistId);

internal sealed record CreateAppointmentRequest(
    AppointmentSlot Slot,
    Guid ClientId
);

internal sealed class SlotIsBusyException : Exception;

internal sealed record CreateAppointmentResponse(Appointment Appointment);

public sealed record Appointment(AppointmentSlot Slot, Guid ClientId);