using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Api.Controllers.ManagerApi;

[Route("api/manager/[controller]")]
[Authorize]
public class ManagerApiController : ApiController
{
    public ManagerApiController(ISender mediatorSender) : base(mediatorSender)
    {
    }
}
