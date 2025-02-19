using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Server.Application.Wrapper;

namespace Server.Application.Features.ContributionApp.Commands.CreateContribution;

public class CreateContributionCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid FacultyId { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public IFormFile? Thumbnail { get; set; }

    public string Content { get; set; }

    public string ShortDescription { get; set; }

    public List<IFormFile>? Files { get; set; }

    public bool IsConfirmed { get; set; }
}
