using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Server.Application.Wrapper;

namespace Server.Application.Features.ContributionApp.Commands.UpdateContribution;

public class UpdateContributionCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid Id { get; set; }

    public Guid FacultyId { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = default!;

    public IFormFile? Thumbnail { get; set; }

    public string Content { get; set; }

    public string ShortDescription { get; set; }

    public List<IFormFile>? Files { get; set; }

    public bool IsConfirmed { get; set; }
}
