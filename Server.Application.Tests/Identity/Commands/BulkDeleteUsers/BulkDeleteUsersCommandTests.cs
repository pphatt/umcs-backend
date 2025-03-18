using FluentAssertions;

using Server.Application.Features.Identity.Commands.BulkDeleteUsers;
using Server.Contracts.Identity.BulkDeleteUsers;

namespace Server.Application.Tests.Identity.Commands.BulkDeleteUsers;

[Trait("Identity", "Bulk Delete")]
public class BulkDeleteUsersCommandTests : BaseTest
{
    [Fact]
    public void BulkDeleteUsersCommandTests_BulkDeleteUsers_MapCorrectly()
    {
        // Arrange
        var request = new BulkDeleteUsersRequest
        {
            UserIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = _mapper.Map<BulkDeleteUsersCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.UserIds.Should().BeEquivalentTo(request.UserIds);
    }
}
