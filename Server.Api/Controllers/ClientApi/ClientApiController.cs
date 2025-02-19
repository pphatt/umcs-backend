using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Api.Controllers.ClientApi;

[Route("api/client/[controller]")]
[Authorize]
public class ClientApiController : ApiController
{
    public ClientApiController(ISender mediatorSender) : base(mediatorSender)
    {
    }
}
