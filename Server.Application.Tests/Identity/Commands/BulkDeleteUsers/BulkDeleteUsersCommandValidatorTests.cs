using FluentValidation.TestHelper;

using Server.Application.Features.Identity.Commands.BulkDeleteUsers;

namespace Server.Application.Tests.Identity.Commands.BulkDeleteUsers;

[Trait("Identity", "Bulk Delete")]
public class BulkDeleteUsersCommandValidatorTests : BaseTest
{
    private readonly BulkDeleteUsersCommandValidator _validator;

    public BulkDeleteUsersCommandValidatorTests()
    {
        _validator = new BulkDeleteUsersCommandValidator();
    }

    [Fact]
    public async Task BulkDeleteUsersCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new BulkDeleteUsersCommand
        {
            UserIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
