using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Infrastructure.Database;

namespace RoomHub.Api.Infrastructure.Repositories;

public class RoomRepository(RoomHubDbContext context) : IRoomRepository
{
    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Rooms.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        await context.Rooms.AddAsync(room, cancellationToken);
    }
}
