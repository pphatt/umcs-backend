namespace Server.Application.Features;

/*
 * This is just for experiment unit test for the first time.
 * This class will not be implemented any where beside unit test.
 */
public record CurrentUser(string Id,
    string Email,
    IEnumerable<string> Roles,
    string? Nationality,
    DateOnly? DateOfBirth)
{
    public bool IsInRole(string role) => Roles.Contains(role);
}
