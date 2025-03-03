using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Wrapper;

namespace Server.Application.Features.PublicContributionApp.Queries.GetPublicContributionBySlug;

public class GetPublicContributionBySlugQuery : IRequest<ErrorOr<ResponseWrapper<PublicContributionWithCommentsDto>>>
{
    public string Slug { get; set; } = default!;

    public Guid UserId { get; set; }
}
