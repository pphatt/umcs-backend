using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Api.Controllers.AdminApi;

[Route("api/admin/[controller]")]
[Authorize]
public class AdminApiController : ApiController
{
    public AdminApiController(ISender mediatorSender) : base(mediatorSender)
    {
    }
}
