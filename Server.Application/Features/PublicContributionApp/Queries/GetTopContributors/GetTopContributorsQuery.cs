using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Wrapper;
using Server.Domain.Entity.Content;

namespace Server.Application.Features.PublicContributionApp.Queries.GetTopContributors;

public class GetTopContributorsQuery : PaginationDto, IRequest<ErrorOr<ResponseWrapper<List<ContributorDto>>>>
{
    public Guid UserId { get; set; } = default!;

    public string? FacultyName { get; set; }

    public string? OrderBy { get; set; }
}
