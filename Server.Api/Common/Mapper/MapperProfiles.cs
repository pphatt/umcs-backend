using AutoMapper;
using Microsoft.AspNetCore.Identity.Data;
using Server.Application.Common.Dtos.Content.AcademicYear;
using Server.Application.Common.Dtos.Content.Faculty;
using Server.Application.Common.Dtos.Identity.Role;
using Server.Application.Common.Dtos.Identity.Users;
using Server.Application.Features.AcademicYearsApp.Commands.ActivateAcademicYear;
using Server.Application.Features.AcademicYearsApp.Commands.BulkDeleteAcademicYears;
using Server.Application.Features.AcademicYearsApp.Commands.CreateAcademicYear;
using Server.Application.Features.AcademicYearsApp.Commands.DeleteAcademicYear;
using Server.Application.Features.AcademicYearsApp.Commands.InactivateAcademicYear;
using Server.Application.Features.AcademicYearsApp.Commands.UpdateAcademicYear;
using Server.Application.Features.AcademicYearsApp.Queries.GetAcademicYearById;
using Server.Application.Features.AcademicYearsApp.Queries.GetAllAcademicYearsPagination;
using Server.Application.Features.Authentication.Commands.Login;
using Server.Application.Features.Authentication.Commands.RefreshToken;
using Server.Application.Features.ContributionApp.Commands.CreateContribution;
using Server.Application.Features.ContributionApp.Commands.UpdateContribution;
using Server.Application.Features.FacultyApp.Commands.BulkDeleteFaculty;
using Server.Application.Features.FacultyApp.Commands.CreateFaculty;
using Server.Application.Features.FacultyApp.Commands.DeleteFaculty;
using Server.Application.Features.FacultyApp.Commands.UpdateFaculty;
using Server.Application.Features.FacultyApp.Queries.GetAllFacultiesPagination;
using Server.Application.Features.FacultyApp.Queries.GetFacultyById;
using Server.Application.Features.Identity.Commands.BulkDeleteUsers;
using Server.Application.Features.Identity.Commands.DeleteUser;
using Server.Application.Features.Identity.Commands.ForgotPassword;
using Server.Application.Features.Identity.Commands.UpdateUser;
using Server.Application.Features.Identity.Queries.GetAllUsersPagination;
using Server.Application.Features.Identity.Queries.GetUserById;
using Server.Application.Features.Role.Commands.BulkDeleteRoles;
using Server.Application.Features.Role.Commands.CreateRole;
using Server.Application.Features.Role.Commands.DeleteRole;
using Server.Application.Features.Role.Commands.SavePermissionsToRole;
using Server.Application.Features.Role.Commands.UpdateRole;
using Server.Application.Features.Role.Queries.GetAllRolePermissions;
using Server.Application.Features.Role.Queries.GetAllRolesPagination;
using Server.Application.Features.Role.Queries.GetRoleById;
using Server.Application.Features.Users.Commands.CreateUser;
using Server.Contracts.AcademicYears.ActivateAcademicYear;
using Server.Contracts.AcademicYears.BulkDeleteAcademicYears;
using Server.Contracts.AcademicYears.CreateAcademicYear;
using Server.Contracts.AcademicYears.DeleteAcademicYear;
using Server.Contracts.AcademicYears.GetAcademicYearById;
using Server.Contracts.AcademicYears.GetAllAcademicYearsPagination;
using Server.Contracts.AcademicYears.InactivateAcademicYear;
using Server.Contracts.AcademicYears.UpdateAcademicYear;
using Server.Contracts.Authentication.RefreshToken;
using Server.Contracts.Common.Media;
using Server.Contracts.Contributions.CreateContribution;
using Server.Contracts.Contributions.UpdateContribution;
using Server.Contracts.Faculties.BulkDeleteFaculties;
using Server.Contracts.Faculties.CreateFaculty;
using Server.Contracts.Faculties.DeleteFaculty;
using Server.Contracts.Faculties.GetAllFacultiesPagination;
using Server.Contracts.Faculties.GetFacultyById;
using Server.Contracts.Faculties.UpdateFaculty;
using Server.Contracts.Identity.BulkDeleteUsers;
using Server.Contracts.Identity.CreateUser;
using Server.Contracts.Identity.DeleteUser;
using Server.Contracts.Identity.GetAllUsersPagination;
using Server.Contracts.Identity.GetUserById;
using Server.Contracts.Identity.UpdateUser;
using Server.Contracts.Roles.BulkDeleteRoles;
using Server.Contracts.Roles.CreateRole;
using Server.Contracts.Roles.DeleteRole;
using Server.Contracts.Roles.GetAllRolePermissions;
using Server.Contracts.Roles.GetAllRolesPagination;
using Server.Contracts.Roles.GetRoleById;
using Server.Contracts.Roles.UpdateRole;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

using File = Server.Domain.Entity.Content.File;
using LoginRequest = Server.Contracts.Authentication.Login.LoginRequest;

namespace Server.Api.Common.Mapper;

public class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        // Authentication.
        CreateMap<LoginRequest, LoginCommand>();
        CreateMap<RefreshTokenRequest, RefreshTokenCommand>();

        // User.
        CreateMap<AppUser, UserDto>().ReverseMap();

        CreateMap<CreateUserRequest, CreateUserCommand>();
        CreateMap<CreateUserCommand, AppUser>();
        CreateMap<UpdateUserRequest, UpdateUserCommand>();
        CreateMap<UpdateUserCommand, AppUser>();
        CreateMap<DeleteUserRequest, DeleteUserCommand>();
        CreateMap<BulkDeleteUsersRequest, BulkDeleteUsersCommand>();

        CreateMap<GetUserByIdRequest, GetUserByIdQuery>();
        CreateMap<GetAllUsersPaginationRequest, GetAllUsersPaginationQuery>();

        CreateMap<ForgotPasswordRequest, ForgotPasswordCommand>();

        // Role.
        CreateMap<AppRole, RoleDto>().ReverseMap();

        CreateMap<CreateRoleRequest, CreateRoleCommand>();
        CreateMap<UpdateRoleRequest, UpdateRoleCommand>();
        CreateMap<DeleteRoleRequest, DeleteRoleCommand>();
        CreateMap<BulkDeleteRolesRequest, BulkDeleteRolesCommand>();

        CreateMap<GetRoleByIdRequest, GetRoleByIdQuery>();
        CreateMap<GetAllRolesPaginationRequest, GetAllRolesPaginationQuery>();

        CreateMap<GetAllRolePermissionsRequest, GetAllRolePermissionsQuery>();
        CreateMap<PermissionsDto, SavePermissionsToRoleCommand>();

        // Faculty.
        CreateMap<Faculty, FacultyDto>().ReverseMap();

        CreateMap<CreateFacultyRequest, CreateFacultyCommand>();
        CreateMap<UpdateFacultyRequest, UpdateFacultyCommand>();
        CreateMap<DeleteFacultyRequest, DeleteFacultyCommand>();
        CreateMap<BulkDeleteFacultiesRequest, BulkDeleteFacultiesCommand>();

        CreateMap<GetFacultyByIdRequest, GetFacultyByIdQuery>();
        CreateMap<GetAllFacultiesPaginationRequest, GetAllFacultiesPaginationQuery>();

        CreateMap<Faculty, FacultyDto>();

        // Academic Year.
        CreateMap<AcademicYear, AcademicYearDto>().ReverseMap();

        CreateMap<CreateAcademicYearRequest, CreateAcademicYearCommand>();
        CreateMap<UpdateAcademicYearRequest, UpdateAcademicYearCommand>();
        CreateMap<DeleteAcademicYearRequest, DeleteAcademicYearCommand>();
        CreateMap<BulkDeleteAcademicYearsRequest, BulkDeleteAcademicYearsCommand>();

        CreateMap<GetAcademicYearByIdRequest, GetAcademicYearByIdQuery>();
        CreateMap<GetAllAcademicYearsPaginationRequest, GetAllAcademicYearsPaginationQuery>();

        CreateMap<ActivateAcademicYearRequest, ActivateAcademicYearCommand>();
        CreateMap<InactivateAcademicYearRequest, InactivateAcademicYearCommand>();

        // Contribution.
        CreateMap<CreateContributionRequest, CreateContributionCommand>();
        CreateMap<UpdateContributionRequest, UpdateContributionCommand>();
        CreateMap<UpdateContributionCommand, Contribution>()
            .ForMember(dest => dest.Files, opt => opt.Ignore())
            .ForMember(dest => dest.AcademicYearId, opt => opt.Ignore())
            .ForMember(dest => dest.AcademicYear, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.SubmissionDate, opt => opt.Ignore())
            .ForMember(dest => dest.PublicDate, opt => opt.Ignore())
            .ForMember(dest => dest.Faculty, opt => opt.Ignore())
            .ForMember(dest => dest.ContributionTags, opt => opt.Ignore());

        // File.
        CreateMap<File, DeleteFilesRequest>();
    }
}
