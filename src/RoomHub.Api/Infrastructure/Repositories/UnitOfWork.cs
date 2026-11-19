using RoomHub.Api.Domain.Repositories;
using RoomHub.Api.Infrastructure.Database;

namespace RoomHub.Api.Infrastructure.Repositories;

public class UnitOfWork(RoomHubDbContext context) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
