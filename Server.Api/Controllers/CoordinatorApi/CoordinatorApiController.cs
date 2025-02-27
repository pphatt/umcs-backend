using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Api.Controllers.CoordinatorApi;

[Route("api/coordinator/[controller]")]
[Authorize]
public class CoordinatorApiController : ApiController
{
    public CoordinatorApiController(ISender mediatorSender) : base(mediatorSender)
    {
    }
}
