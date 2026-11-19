using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public class Booking
{
    // EF Core parameterless constructor
    private Booking() { }

    private Booking(
        Guid id,
        Guid roomId,
        string bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        BookingStatus status)
    {
        Id = id;
        RoomId = roomId;
        BookedBy = bookedBy;
        StartUtc = startUtc;
        EndUtc = endUtc;
        Attendees = attendees;
        Status = status;
    }

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    public static Result<Booking> Create(
        Guid roomId,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        int roomCapacity,
        TimeProvider timeProvider)
    {
        // R2: bookedBy is required
        if (string.IsNullOrWhiteSpace(bookedBy))
            return Error.Validation("InvalidBookedBy", "BookedBy is required.");

        // Normalize DateTime to UTC kind
        var normalizedStart = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
        var normalizedEnd = DateTime.SpecifyKind(endUtc, DateTimeKind.Utc);

        // R3: endUtc must be strictly after startUtc
        if (normalizedEnd <= normalizedStart)
            return Error.Validation("InvalidRange", "The end must be after the start.");

        // R4: attendees is strictly positive and at most the room capacity
        if (attendees <= 0)
            return Error.Validation("InvalidAttendees", "Attendees must be positive.");

        if (attendees > roomCapacity)
            return Error.Validation("AttendeesExceedCapacity", "The room does not have enough seats.");

        // R5: booking cannot start in the past. Now comes from TimeProvider
        var now = timeProvider.GetUtcNow().UtcDateTime;
        if (normalizedStart < now)
            return Error.Validation("BookingInPast", "A booking cannot start in the past.");

        // R6: maximum duration is 4 hours (exactly 4 hours is allowed)
        if ((normalizedEnd - normalizedStart) > TimeSpan.FromHours(4))
            return Error.Validation("ExceedsMaxDuration", "The maximum duration of a booking is 4 hours.");

        return new Booking(
            Guid.NewGuid(),
            roomId,
            bookedBy.Trim(),
            normalizedStart,
            normalizedEnd,
            attendees,
            BookingStatus.Active);
    }

    // R7: Touching intervals do not overlap. Only active bookings overlap.
    public bool OverlapsWith(DateTime start, DateTime end)
    {
        if (Status != BookingStatus.Active)
            return false;

        var normalizedStart = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        var normalizedEnd = DateTime.SpecifyKind(end, DateTimeKind.Utc);

        return StartUtc < normalizedEnd && normalizedStart < EndUtc;
    }

    // R8: Cancelling moves to Cancelled. Already cancelled returns conflict (409).
    public Result Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            return Error.Conflict("AlreadyCancelled", "The booking is already cancelled.");

        Status = BookingStatus.Cancelled;
        return Result.Success();
    }
}
