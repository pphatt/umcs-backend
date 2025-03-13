using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using AutoMapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using Moq;

using Server.Api.Common.Mapper;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Common.Interfaces.Services;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Entity.Identity;
using Server.Infrastructure.Services;

namespace Server.Application.Tests;

public class BaseTest
{
    internal Mock<IUnitOfWork> _mockUnitOfWork;
    internal Mock<UserManager<AppUser>> _mockUserManager;

    internal IMapper _mapper;
    internal IDateTimeProvider _dateTimeProvider;

    internal readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    internal IUserService _userService;

    // Repositories
    internal Mock<ITokenRepository> _mockTokenRepository;
    internal Mock<IFacultyRepository> _mockFacultyRepository;
    internal Mock<IAcademicYearRepository> _mockAcademicYearRepository;
    internal Mock<IContributionRepository> _mockContributionRepository;
    internal Mock<IContributionPublicRepository> _mockContributionPublicRepository;
    internal Mock<IContributionActivityLogRepository> _mockContributionActivityLogRepository;
    internal Mock<IContributionCommentRepository> _mockContributionCommentRepository;
    internal Mock<IContributionPublicCommentRepository> _mockContributionPublicCommentRepository;
    internal Mock<IContributionPublicReadLaterRepository> _mockContributionPublicReadLaterRepository;
    internal Mock<IContributionPublicBookmarkRepository> _mockContributionPublicBookmarkRepository;
    internal Mock<IContributionPublicRatingRepository> _mockContributionPublicRatingRepository;
    internal Mock<IFileRepository> _mockFileRepository;
    internal Mock<ILikeRepository> _mockLikeRepository;
    internal Mock<ITagRepository> _mockTagRepository;

    public BaseTest()
    {
        // Initialize mock unit of work
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        // Initialize mock user manager
        var store = new Mock<IUserStore<AppUser>>();
        _mockUserManager = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);

        // Initialize mapper
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MapperProfiles>();
        });
        _mapper = configuration.CreateMapper();

        // User Service
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _userService = new UserService(_httpContextAccessor.Object);

        // Set up the HttpUserContext
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "613DA9F6-FC5A-4E7F-AB2E-7FC89258A596"),
            new Claim(UserClaims.Id, "613DA9F6-FC5A-4E7F-AB2E-7FC89258A596"),
            new Claim(ClaimTypes.NameIdentifier, "test"),
            new Claim(JwtRegisteredClaimNames.Email, "test@gmail.com"),
            new Claim(ClaimTypes.Name, "test-user"),
            new Claim(UserClaims.Roles, string.Join(",", [Roles.Admin, Roles.Student])),
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()
        {
            User = user
        });

        // Initialize date time provider
        _dateTimeProvider = new DateTimeProvider();

        // Initialize repository mocks
        _mockTokenRepository = new Mock<ITokenRepository>();
        _mockFacultyRepository = new Mock<IFacultyRepository>();
        _mockAcademicYearRepository = new Mock<IAcademicYearRepository>();
        _mockContributionRepository = new Mock<IContributionRepository>();
        _mockContributionPublicRepository = new Mock<IContributionPublicRepository>();
        _mockContributionActivityLogRepository = new Mock<IContributionActivityLogRepository>();
        _mockContributionCommentRepository = new Mock<IContributionCommentRepository>();
        _mockContributionPublicCommentRepository = new Mock<IContributionPublicCommentRepository>();
        _mockContributionPublicReadLaterRepository = new Mock<IContributionPublicReadLaterRepository>();
        _mockContributionPublicBookmarkRepository = new Mock<IContributionPublicBookmarkRepository>();
        _mockContributionPublicRatingRepository = new Mock<IContributionPublicRatingRepository>();
        _mockFileRepository = new Mock<IFileRepository>();
        _mockLikeRepository = new Mock<ILikeRepository>();
        _mockTagRepository = new Mock<ITagRepository>();

        // Set up the unit of work to return the repository mocks
        SetupUnitOfWork();

        // Set up default CompleteAsync behavior to return 1 (if success)
        _mockUnitOfWork.Setup(uow => uow.CompleteAsync()).ReturnsAsync(1);
    }

    private void SetupUnitOfWork()
    {
        // Configure UnitOfWork to return our mocked repositories
        _mockUnitOfWork.Setup(uow => uow.TokenRepository).Returns(_mockTokenRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.FacultyRepository).Returns(_mockFacultyRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.AcademicYearRepository).Returns(_mockAcademicYearRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.ContributionRepository).Returns(_mockContributionRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.ContributionPublicRepository).Returns(_mockContributionPublicRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.ContributionActivityLogRepository).Returns(_mockContributionActivityLogRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.ContributionCommentRepository).Returns(_mockContributionCommentRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.ContributionPublicCommentRepository).Returns(_mockContributionPublicCommentRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.ContributionPublicReadLaterRepository).Returns(_mockContributionPublicReadLaterRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.ContributionPublicBookmarkRepository).Returns(_mockContributionPublicBookmarkRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.ContributionPublicRatingRepository).Returns(_mockContributionPublicRatingRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.FileRepository).Returns(_mockFileRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.LikeRepository).Returns(_mockLikeRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.TagRepository).Returns(_mockTagRepository.Object);
    }

    protected void ResetMocks()
    {
        // Helper method to reset repository mocks between tests if needed
        _mockTokenRepository.Reset();
        _mockFacultyRepository.Reset();
        _mockAcademicYearRepository.Reset();
        _mockContributionRepository.Reset();
        _mockContributionPublicRepository.Reset();
        _mockContributionActivityLogRepository.Reset();
        _mockContributionCommentRepository.Reset();
        _mockContributionPublicCommentRepository.Reset();
        _mockContributionPublicReadLaterRepository.Reset();
        _mockContributionPublicBookmarkRepository.Reset();
        _mockContributionPublicRatingRepository.Reset();
        _mockFileRepository.Reset();
        _mockLikeRepository.Reset();
        _mockTagRepository.Reset();

        // Reset unit of work mock
        _mockUnitOfWork.Reset();

        // Re-setup the unit of work
        SetupUnitOfWork();
        _mockUnitOfWork.Setup(uow => uow.CompleteAsync()).ReturnsAsync(1);
    }
}
