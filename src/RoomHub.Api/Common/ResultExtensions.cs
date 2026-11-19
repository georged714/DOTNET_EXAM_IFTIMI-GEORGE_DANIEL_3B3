using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Common;

public static class ResultExtensions
{
    public static IResult ToProblem(this Error error)
    {
        return Results.Problem(
            detail: error.Description,
            statusCode: error.StatusCode,
            title: error.Code);
    }
}
