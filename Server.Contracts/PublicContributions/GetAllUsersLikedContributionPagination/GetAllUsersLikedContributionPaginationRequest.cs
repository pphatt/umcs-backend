using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;

namespace Server.Contracts.PublicContributions.GetAllUsersLikedContributionPagination;

public class GetAllUsersLikedContributionPaginationRequest : PaginationRequest
{
    [FromRoute]
    public Guid ContributionId { get; set; }
}
