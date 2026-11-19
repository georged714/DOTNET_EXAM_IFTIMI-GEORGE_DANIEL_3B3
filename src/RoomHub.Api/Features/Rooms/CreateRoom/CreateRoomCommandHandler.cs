using RoomHub.Api.Common;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Repositories;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public record CreateRoomCommand(string? Name, int Capacity);

public class CreateRoomCommandHandler(IRoomRepository roomRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateRoomCommand, Result<Room>>
{
    public async Task<Result<Room>> HandleAsync(CreateRoomCommand command, CancellationToken cancellationToken = default)
    {
        var roomResult = Room.Create(command.Name, command.Capacity);
        if (roomResult.IsFailure)
            return roomResult.Error;

        var room = roomResult.Value;
        await roomRepository.AddAsync(room, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return room;
    }
}
