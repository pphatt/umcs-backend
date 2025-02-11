using Microsoft.AspNetCore.Mvc;

namespace Server.Contracts.Identity.DeleteUser;

public class DeleteUserRequest
{
    [FromRoute]
    public Guid Id { get; set; }
};
