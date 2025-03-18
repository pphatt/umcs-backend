using AutoMapper;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Moq;

using Server.Application.Common.Interfaces.Services;
using Server.Domain.Entity.Content;
using Server.Infrastructure.Persistence.Repositories;
using Server.Infrastructure.Services;

namespace Server.Infrastructure.Tests.Repositories.AcademicYear;

using AcademicYear = Server.Domain.Entity.Content.AcademicYear;

public class AcademicYearRepositoryTests
{
    private readonly Mock<AppDbContext> _contextMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly IDateTimeProvider _mockDateTimeProvider;

    private readonly AcademicYearRepository _repository;
    private readonly Mock<DbSet<AcademicYear>> _academicYearsMock;
    private readonly Mock<DbSet<Contribution>> _contributionsMock;

    public AcademicYearRepositoryTests()
    {
        _contextMock = new Mock<AppDbContext>(MockBehavior.Loose, new DbContextOptions<AppDbContext>());
        _mapperMock = new Mock<IMapper>();

        _mockDateTimeProvider = new DateTimeProvider();

        _academicYearsMock = new Mock<DbSet<AcademicYear>>();
        _contributionsMock = new Mock<DbSet<Contribution>>();

        _contextMock.Setup(x => x.AcademicYears).Returns(_academicYearsMock.Object);
        _contextMock.Setup(x => x.Contributions).Returns(_contributionsMock.Object);

        _repository = new AcademicYearRepository(_contextMock.Object, _mapperMock.Object);
    }

    private void SetupDbSetMock<T>(Mock<DbSet<T>> dbSetMock, IQueryable<T> data) where T : class
    {
        dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
    }

    // TestMethod_Scenario_ExpectResult
    //[Theory]
    //[InlineData("2022-2023")]
    //public async Task GetAcademicYearByName_ExistingName_ShouldReturnAcademicYearAsync(string name)
    //{
    //    // Arrange
    //    var academicYear = new AcademicYear
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = name,
    //        UserIdCreated = Guid.NewGuid(),
    //        IsActive = true,
    //        StartClosureDate = _mockDateTimeProvider.UtcNow,
    //        EndClosureDate = _mockDateTimeProvider.UtcNow.AddMonths(1),
    //        FinalClosureDate = _mockDateTimeProvider.UtcNow.AddMonths(2)
    //    };

    //    var data = new List<AcademicYear> { academicYear }.AsQueryable();
    //    SetupDbSetMock(_academicYearsMock, data);

    //    // Act
    //    var result = await _repository.GetAcademicYearByNameAsync(name);

    //    // Assert
    //    result.Should().NotBeNull();
    //    result.Name.Should().Be(name);
    //}
}
