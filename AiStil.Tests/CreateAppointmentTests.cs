using AiStil.App;

namespace AiStil.Tests;

public class CreateAppointmentTests
{
    [Fact]
    public void ShouldAcceptWhenSlotIsFree()
    {
        // Arrange
        var request = CreateRequest();
        var repository = new FakeBusySlotsRepository(_ => Array.Empty<Slot>());

        // Act
        CreateAppointmentResponse response = new CreateAppointmentCommand(request, repository).Execute();

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
            new Slot(
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

    [Fact]
    public void ShouldBookSlotWhenAppointmentIsCreated()
    {
        // Arrange
        var request = CreateRequest();
        var repository = new FakeBusySlotsRepository(_ => Array.Empty<Slot>());

        // Act
        _ = new CreateAppointmentCommand(request, repository).Execute();

        // Assert
        Assert.Single(repository.SavedSlots);
        Assert.Equal(request.Slot, repository.SavedSlots[0]);
    }

    [Fact]
    public void ShouldPersistAppointmentWhenIsCreated()
    {
        // Arrange
        var request = CreateRequest();
        var repository = new FakeBusySlotsRepository(_ => Array.Empty<Slot>());

        // Act
        _ = new CreateAppointmentCommand(request, repository).Execute();

        // Assert
        Assert.Single(repository.SavedAppointments);
        Assert.Equal(new Appointment(request.Slot, request.ClientId), repository.SavedAppointments[0]);
    }


    private static CreateAppointmentRequest CreateRequest()
    {
        var startTime = new DateTime(2024, 7, 1, 10, 0, 0);
        var endTime = startTime.AddHours(1);
        var stylistId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var clientId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var slot = new Slot(startTime, endTime, stylistId);
        return new CreateAppointmentRequest(slot, clientId);
    }

    private sealed class FakeBusySlotsRepository(Func<Slot, IEnumerable<Slot>> getBusySlots)
        : ISlotsRepository
    {
        public List<Slot> SavedSlots { get; } = [];
        public List<Appointment> SavedAppointments { get; } = [];

        public Stylist LoadStylistForBooking(Slot requestedSlot) =>
            new(requestedSlot.StylistId, getBusySlots(requestedSlot));

        public void SaveAppointment(Appointment appointment)
        {
            SavedSlots.Add(appointment.Slot);
            SavedAppointments.Add(appointment);
        }
    }
}