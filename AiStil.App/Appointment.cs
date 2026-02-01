namespace AiStil.App;

public sealed record AppointmentSlot(DateTime StartTime, DateTime EndTime, Guid StylistId);

public sealed record Appointment(AppointmentSlot Slot, Guid ClientId);
