using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public record GetBookingResponse(
    Guid Id,
    Guid RoomId,
    string BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees,
    string Status);

public static class GetBookingEndpoint
{
    public static void MapGetBooking(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/bookings/{id:guid}", async (
            Guid id,
            IQueryHandler<GetBookingQuery, Result<Booking>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetBookingQuery(id), cancellationToken);

            if (result.IsFailure)
                return result.Error.ToProblem();

            var b = result.Value;
            return Results.Ok(new GetBookingResponse(
                b.Id,
                b.RoomId,
                b.BookedBy,
                DateTime.SpecifyKind(b.StartUtc, DateTimeKind.Utc),
                DateTime.SpecifyKind(b.EndUtc, DateTimeKind.Utc),
                b.Attendees,
                b.Status.ToString()));
        }).WithTags("Bookings");
    }
}
