using RoomHub.Api.Common;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public record GetRoomResponse(Guid Id, string Name, int Capacity);

public static class GetRoomEndpoint
{
    public static void MapGetRoom(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/rooms/{id:guid}", async (
            Guid id,
            IQueryHandler<GetRoomQuery, Result<Room>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetRoomQuery(id), cancellationToken);

            if (result.IsFailure)
                return result.Error.ToProblem();

            var room = result.Value;
            return Results.Ok(new GetRoomResponse(room.Id, room.Name, room.Capacity));
        }).WithTags("Rooms");
    }
}
