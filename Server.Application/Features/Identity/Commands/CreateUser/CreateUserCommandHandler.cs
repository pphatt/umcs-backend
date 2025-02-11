using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ErrorOr<ResponseWrapper>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

        Console.WriteLine(password);

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Create new use successfully."
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
