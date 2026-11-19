using RoomHub.Api.Common;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public record GetRoomQuery(Guid Id);

public class GetRoomQueryHandler(IRoomRepository roomRepository)
    : IQueryHandler<GetRoomQuery, Result<Room>>
{
    public async Task<Result<Room>> HandleAsync(GetRoomQuery query, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdAsync(query.Id, cancellationToken);
        if (room is null)
            return Error.NotFound("RoomNotFound", "The room does not exist.");

        return room;
    }
}
