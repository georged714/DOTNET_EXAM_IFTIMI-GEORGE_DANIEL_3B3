using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Repositories;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public record CreateBookingCommand(
    Guid RoomId,
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees);

public class CreateBookingCommandHandler(
    IRoomRepository roomRepository,
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : ICommandHandler<CreateBookingCommand, Result<Booking>>
{
    public async Task<Result<Booking>> HandleAsync(CreateBookingCommand command, CancellationToken cancellationToken = default)
    {
        // When the room does not exist, 404 is returned before any validation.
        var room = await roomRepository.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
            return Error.NotFound("RoomNotFound", "The room does not exist.");

        // Domain validation & creation (R2, R3, R4, R5, R6)
        var bookingResult = Booking.Create(
            command.RoomId,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            room.Capacity,
            timeProvider);

        if (bookingResult.IsFailure)
            return bookingResult.Error;

        var booking = bookingResult.Value;

        // R7: Overlap check with active bookings of the same room
        var activeBookings = await bookingRepository.GetActiveBookingsForRoomAsync(command.RoomId, cancellationToken);
        if (activeBookings.Any(b => b.OverlapsWith(booking.StartUtc, booking.EndUtc)))
            return Error.Conflict("RoomOccupied", "The room is occupied in this interval.");

        await bookingRepository.AddAsync(booking, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return booking;
    }
}
