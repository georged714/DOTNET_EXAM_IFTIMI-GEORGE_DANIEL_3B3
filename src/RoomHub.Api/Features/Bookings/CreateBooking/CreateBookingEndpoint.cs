using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public record CreateBookingRequest(string? BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees);

public record CreateBookingResponse(Guid Id, Guid RoomId, string Status);

public static class CreateBookingEndpoint
{
    public static void MapCreateBooking(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms/{roomId:guid}/bookings", async (
            Guid roomId,
            CreateBookingRequest request,
            ICommandHandler<CreateBookingCommand, Result<Booking>> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateBookingCommand(
                roomId,
                request.BookedBy,
                request.StartUtc,
                request.EndUtc,
                request.Attendees);

            var result = await handler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToProblem();

            var booking = result.Value;
            return Results.Created(
                $"/api/bookings/{booking.Id}",
                new CreateBookingResponse(booking.Id, booking.RoomId, booking.Status.ToString()));
        }).WithTags("Bookings");
    }
}
