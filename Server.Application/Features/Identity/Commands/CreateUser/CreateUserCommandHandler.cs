using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Media;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Application.Common.Interfaces.Services.Media;
using Server.Application.Wrapper;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ErrorOr<ResponseWrapper>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediaService _mediaService;

    public CreateUserCommandHandler(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IEmailService emailService, IUnitOfWork unitOfWork, IMapper mapper, IMediaService mediaService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mediaService = mediaService;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var IsEmailExisted = await _userManager.FindByEmailAsync(request.Email);

        if (IsEmailExisted is not null)
        {
            return Errors.User.DuplicateEmail;
        }

        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());

        if (role is null)
        {
            return Errors.Roles.CannotFound;
        }

        var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(request.FacultyId);

        if (role.Name != Roles.Admin || role.Name != Roles.Manager)
        {
            if (faculty is null)
            {
                return Errors.Faculty.CannotFound;
            }
        }

        var newUser = new AppUser();

        _mapper.Map(request, newUser);

        newUser.Id = Guid.NewGuid();
        newUser.FacultyId = faculty is not null ? faculty.Id : null;

        string password = GenerateRandomPassword(12);
        newUser.PasswordHash = new PasswordHasher<AppUser>().HashPassword(newUser, password);

        newUser.LockoutEnabled = false;

        // handle user avatar.
        if (request.Avatar is not null)
        {
            var files = new List<IFormFile> { request.Avatar };
            var required = new FileRequiredParamsDto
            {
                type = FileType.Avatar,
                userId = newUser.Id,
            };

            var uploadImageResult = await _mediaService.UploadFilesToCloudinary(files, required);

            foreach (var info in uploadImageResult)
            {
                newUser.Avatar = info.Path;
                newUser.AvatarPublicId = info.PublicId;
            }
        }

        var createNewUserResult = await _userManager.CreateAsync(newUser);

        if (!createNewUserResult.Succeeded)
        {
            return createNewUserResult.Errors.Select(error => Error.Validation(code: error.Code, description: error.Description)).ToArray();
        }

        var saveUserRoleResult = await _userManager.AddToRoleAsync(newUser, role.Name!);

        if (!saveUserRoleResult.Succeeded)
        {
            return saveUserRoleResult.Errors.Select(error => Error.Validation(code: error.Code, description: error.Description)).ToArray();
        }

        // handle send email.
        await _emailService.SendEmailAsync(new MailRequest
        {
            ToEmail = request.Email,
            Subject = "Account information",
            Body = $"Email: <h1>{newUser.Email}</h1> <br> Password: <h1>{password}</h1>."
        });

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Create new user successfully."
        };
    }

    private static Random random = new Random();

    private static string GenerateRandomPassword(int length)
    {
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string specialChars = "!@#$%^&*()";
        const string allChars = uppercase + lowercase + digits + specialChars;

        return new string(Enumerable.Repeat(allChars, length).Select(c => c[random.Next(c.Length)]).ToArray());
    }
}
