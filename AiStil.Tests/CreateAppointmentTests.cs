using AiStil.App;

namespace AiStil.Tests;

public class CreateAppointmentTests
{
    [Fact]
    public void ShouldAcceptWhenSlotIsFree()
    {
        // Arrange
        var request = CreateRequest();
        var repository = new FakeBusySlotsRepository(_ => Array.Empty<AppointmentSlot>());

        // Act
        CreateAppointmentResponse response = new CreateAppointmentCommand(request, repository).Execute();

        // Assert
        Assert.NotNull(response);
        Assert.Equal(request.Slot, response.Appointment.Slot);
        Assert.Equal(request.ClientId, response.Appointment.ClientId);
    }

    [Fact]
    public void ShouldSaveBusySlotWhenAppointmentIsCreated()
    {
        // Arrange
        var request = CreateRequest();
        var repository = new FakeBusySlotsRepository(_ => Array.Empty<AppointmentSlot>());

        // Act
        _ = new CreateAppointmentCommand(request, repository).Execute();

        // Assert
        Assert.Single(repository.SavedSlots);
        Assert.Equal(request.Slot, repository.SavedSlots[0]);
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

        ISlotsRepository repository = new FakeBusySlotsRepository(_ => busySlots);

        // Act
        var act = () => new CreateAppointmentCommand(request, repository).Execute();

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

    private sealed class FakeBusySlotsRepository(Func<AppointmentSlot, IEnumerable<AppointmentSlot>> getBusySlots)
        : ISlotsRepository
    {
        public List<AppointmentSlot> SavedSlots { get; } = [];

        public IEnumerable<AppointmentSlot> GetBusySlotsForOverlap(AppointmentSlot requestedSlot) =>
            getBusySlots(requestedSlot);

        public void SaveBusySlot(AppointmentSlot slot) => SavedSlots.Add(slot);
    }
}