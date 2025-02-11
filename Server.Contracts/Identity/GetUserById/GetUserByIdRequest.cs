using Microsoft.AspNetCore.Mvc;

namespace Server.Contracts.Identity.GetUserById;

public class GetUserByIdRequest
{
    [FromRoute]
    public Guid Id { get; set; }
};
