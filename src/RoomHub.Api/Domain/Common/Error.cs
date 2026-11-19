namespace RoomHub.Api.Domain.Common;

public readonly record struct Error(string Code, string Description, int StatusCode)
{
    public static Error Validation(string code, string description) => new(code, description, 400);
    public static Error NotFound(string code, string description) => new(code, description, 404);
    public static Error Conflict(string code, string description) => new(code, description, 409);
}
