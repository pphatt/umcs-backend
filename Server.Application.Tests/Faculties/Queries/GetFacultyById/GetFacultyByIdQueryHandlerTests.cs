using FluentAssertions;

using Moq;

using Server.Application.Common.Dtos.Content.Faculty;
using Server.Application.Features.FacultyApp.Queries.GetFacultyById;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;

namespace Server.Application.Tests.Faculties.Queries.GetFacultyById;

[Trait("Faculty", "Get Faculty By Id")]
public class GetFacultyByIdQueryHandlerTests : BaseTest
{
    private readonly GetFacultyByIdQueryHandler _queryHandler;
    private readonly Faculty _faculty;

    public GetFacultyByIdQueryHandlerTests()
    {
        _faculty = new Faculty
        {
            Name = "IT"
        };

        _mockFacultyRepository
            .Setup(repo => repo.GetByIdAsync(_faculty.Id))
            .ReturnsAsync(_faculty);

        _queryHandler = new GetFacultyByIdQueryHandler(_mockUnitOfWork.Object, _mapper);
    }

    [Fact]
    public async Task GetFacultyByIdQueryHandler_GetFacultyById_Should_ReturnError_WhenSearchByFacultyIdCannotFound()
    {
        // Arrange
        var query = new GetFacultyByIdQuery
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.Faculty.CannotFound);
        result.FirstError.Code.Should().Be(Errors.Faculty.CannotFound.Code);
        result.FirstError.Description.Should().Be(Errors.Faculty.CannotFound.Description);
    }

    [Fact]
    public async Task GetFacultyByIdQueryHandler_GetFacultyById_ShouldQuerySuccessfully()
    {
        // Arrange
        var query = new GetFacultyByIdQuery
        {
            Id = _faculty.Id
        };

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper<FacultyDto>>();

        var response = result.Value;
        response.IsSuccessful.Should().BeTrue();
        response.ResponseData.Should().NotBeNull();

        response.ResponseData.Id.Should().Be(_faculty.Id);
        response.ResponseData.Name.Should().Be(_faculty.Name);
    }
}
