using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Content.Comment;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionPublicCommentRepository : RepositoryBase<ContributionPublicComment, Guid>, IContributionPublicCommentRepository
{
    private readonly AppDbContext _context;

    public ContributionPublicCommentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<CommentDto>> GetCommentsByContributionId(Guid contributionId)
    {
        var comments = await _context.ContributionPublicComments
            .Where(x => x.ContributionId == contributionId)
            .OrderBy(x => x.DateCreated)
            .Select(x => new
            {
                Comment = x,
                User = _context.Users.FirstOrDefault(u => u.Id == x.UserId)
            })
            .ToListAsync();

        var result = comments.Select(x => new CommentDto
        {
            Content = x.Comment.Content,
            Username = x.User.UserName,
            Avatar = x.User.Avatar,
            DateCreated = x.Comment.DateCreated
        }).ToList();

        return result;
    }
}
