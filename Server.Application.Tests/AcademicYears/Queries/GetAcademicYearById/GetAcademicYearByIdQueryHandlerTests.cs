using FluentAssertions;

using Moq;

using Server.Application.Common.Dtos.Content.AcademicYear;
using Server.Application.Features.AcademicYearsApp.Queries.GetAcademicYearById;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;

namespace Server.Application.Tests.AcademicYears.Queries.GetAcademicYearById;

[Trait("Academic Year", "Get Academic Year By Id")]
public class GetAcademicYearByIdQueryHandlerTests : BaseTest
{
    private readonly GetAcademicYearByIdQueryHandler _queryHandler;
    private readonly AcademicYear _academicYear;

    public GetAcademicYearByIdQueryHandlerTests()
    {
        _academicYear = new AcademicYear
        {
            Id = Guid.NewGuid(),
            Name = "2025-2026",
            UserIdCreated = Guid.NewGuid(),
            IsActive = true,
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
        };

        _mockAcademicYearRepository
            .Setup(repo => repo.GetByIdAsync(_academicYear.Id))
            .ReturnsAsync(_academicYear);

        _queryHandler = new GetAcademicYearByIdQueryHandler(_mockUnitOfWork.Object, _mapper);
    }

    [Fact]
    public async Task GetAcademicYearByIdQueryHandler_GetAcademicYearById_Should_ReturnError_WhenSearchByAcademicYearIdCannotFound()
    {
        // Arrange
        var query = new GetAcademicYearByIdQuery
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.AcademicYears.CannotFound);
        result.FirstError.Code.Should().Be(Errors.AcademicYears.CannotFound.Code);
        result.FirstError.Description.Should().Be(Errors.AcademicYears.CannotFound.Description);
    }

    [Fact]
    public async Task GetAcademicYearByIdQueryHandler_GetAcademicYearById_ShouldQuerySuccessfully()
    {
        // Arrange
        var query = new GetAcademicYearByIdQuery
        {
            Id = _academicYear.Id
        };

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper<AcademicYearDto>>();

        var response = result.Value;
        response.IsSuccessful.Should().BeTrue();
        response.ResponseData.Should().NotBeNull();

        response.ResponseData.Id.Should().Be(_academicYear.Id);
        response.ResponseData.Name.Should().Be(_academicYear.Name);
        response.ResponseData.IsActive.Should().Be(_academicYear.IsActive);
        response.ResponseData.StartClosureDate.Should().Be(_academicYear.StartClosureDate);
        response.ResponseData.EndClosureDate.Should().Be(_academicYear.EndClosureDate);
        response.ResponseData.FinalClosureDate.Should().Be(_academicYear.FinalClosureDate);
    }
}
