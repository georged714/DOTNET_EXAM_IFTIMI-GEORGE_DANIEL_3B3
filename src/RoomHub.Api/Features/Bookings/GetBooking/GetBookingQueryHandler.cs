using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public record GetBookingQuery(Guid Id);

public class GetBookingQueryHandler(IBookingRepository bookingRepository)
    : IQueryHandler<GetBookingQuery, Result<Booking>>
{
    public async Task<Result<Booking>> HandleAsync(GetBookingQuery query, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(query.Id, cancellationToken);
        if (booking is null)
            return Error.NotFound("BookingNotFound", "The booking does not exist.");

        return booking;
    }
}
