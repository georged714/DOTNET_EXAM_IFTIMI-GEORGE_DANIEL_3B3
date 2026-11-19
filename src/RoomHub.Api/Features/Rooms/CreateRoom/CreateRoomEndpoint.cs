using RoomHub.Api.Common;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public record CreateRoomRequest(string? Name, int Capacity);

public record CreateRoomResponse(Guid Id, string Name, int Capacity);

public static class CreateRoomEndpoint
{
    public static void MapCreateRoom(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms", async (
            CreateRoomRequest request,
            ICommandHandler<CreateRoomCommand, Result<Room>> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateRoomCommand(request.Name, request.Capacity);
            var result = await handler.HandleAsync(command, cancellationToken);

            if (result.IsFailure)
                return result.Error.ToProblem();

            var room = result.Value;
            return Results.Created(
                $"/api/rooms/{room.Id}",
                new CreateRoomResponse(room.Id, room.Name, room.Capacity));
        }).WithTags("Rooms");
    }
}
