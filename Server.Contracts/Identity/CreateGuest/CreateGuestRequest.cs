namespace Server.Contracts.Identity.CreateGuest;

public class CreateGuestRequest
{
    public string Email { get; set; } = default!;

    public string UserName { get; set; } = default!;

    public Guid FacultyId { get; set; }
}
