using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Application.Wrapper;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Identity.Commands.CreateGuest;

public class CreateGuestCommandHandler : IRequestHandler<CreateGuestCommand, ErrorOr<ResponseWrapper>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateGuestCommandHandler(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IDateTimeProvider dateTimeProvider)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(CreateGuestCommand request, CancellationToken cancellationToken)
    {
        var IsEmailExisted = await _userManager.FindByEmailAsync(request.Email);

        if (IsEmailExisted is not null)
        {
            return Errors.User.DuplicateEmail;
        }

        var role = await _roleManager.FindByNameAsync(Roles.Guest);

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
        newUser.DateCreated = _dateTimeProvider.UtcNow;

        string password = GenerateRandomPassword(12);
        newUser.PasswordHash = new PasswordHasher<AppUser>().HashPassword(newUser, password);

        newUser.LockoutEnabled = false;

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
            Body = $@"Hi {newUser.UserName},<br><br>
                    Your account has been successfully created. Below are your login credentials:<br><br>
                    <strong>Email:</strong> {newUser.Email}<br>
                    <strong>Password:</strong> {password}<br><br>
                    For security reasons, we recommend changing your password upon first login.<br><br>
                    Best regards,<br>
                    The Account Security Team"
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
