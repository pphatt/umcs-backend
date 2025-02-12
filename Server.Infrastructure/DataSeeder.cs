using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Authorization;
using Server.Application.Common.Extensions;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;
using System.Security.Claims;

namespace Server.Infrastructure;

public partial class DataSeeder
{
    public async static Task SeedAsync(AppDbContext context, RoleManager<AppRole> roleManager)
    {
        // seed faculies.
        var faculties = await FacultyList(context);

        // seed roles.
        var roles = await RoleList(context);

        // seed users.
        var passwordHasher = new PasswordHasher<AppUser>();

        var adminId = Guid.NewGuid();

        var studentList = StudentList(faculties);

        if (!await context.Users.AnyAsync())
        {
            // create admin account.
            var email = "phatvtgcs21@gmail.com";
            var username = "admin";

            var admin = new AppUser()
            {
                Id = adminId,
                FirstName = "Tien Phat",
                LastName = "Vu",
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                UserName = username,
                NormalizedUserName = username.ToUpperInvariant(),
                // không có SecurityStamp, user không thể login được.
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                IsActive = true,
                DateCreated = DateTime.Now,
                FacultyId = faculties[0].Id,
            };

            admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin@123");

            await context.Users.AddAsync(admin);

            await context.UserRoles.AddAsync(new IdentityUserRole<Guid>
            {
                RoleId = roles[0].Id,
                UserId = admin.Id,
            });

            // create student accounts.
            foreach (var user in studentList)
            {
                user.PasswordHash = passwordHasher.HashPassword(user, "Student@123");

                await context.Users.AddAsync(user);
                await context.UserRoles.AddAsync(new IdentityUserRole<Guid>
                {
                    RoleId = roles[1].Id,
                    UserId = user.Id,
                });
            }

            await context.SaveChangesAsync();
        }

        if (!context.RoleClaims.Any())
        {
            // seed admin role claims permissions.
            var adminPermissions = await roleManager.GetClaimsAsync(roles[0]);

            if (!adminPermissions.Any())
            {
                var adminPermissionList = new List<RoleClaimsDto>();

                var types = typeof(Permissions).GetNestedTypes().ToList();

                types.ForEach(adminPermissionList.GetPermissionByType);

                foreach (var adminPermission in adminPermissionList)
                {
                    await roleManager.AddClaimAsync(roles[0], new Claim(UserClaims.Permissions, adminPermission.Value!));
                }
            }
        }
    }

    private static async Task<List<Faculty>> FacultyList(AppDbContext context)
    {
        #region Faculty List

        var faculties = new List<Faculty>
        {
            new() { Id = Guid.NewGuid(), Name = Faculties.IT },
            new() { Id = Guid.NewGuid(), Name = Faculties.Business },
            new() { Id = Guid.NewGuid(), Name = Faculties.Marketing },
            new() { Id = Guid.NewGuid(), Name = Faculties.GraphicDesign },
            new() { Id = Guid.NewGuid(), Name = Faculties.ElectricalEngineer },
        };

        if (!await context.Faculties.AnyAsync())
        {
            await context.Faculties.AddRangeAsync(faculties);
            await context.SaveChangesAsync();
        }

        return faculties;

        #endregion Faculty List
    }

    private static async Task<List<AppRole>> RoleList(AppDbContext context)
    {
        #region Role List

        var roles = new List<AppRole>
        {
            new() { Id = Guid.NewGuid(), Name = Roles.Admin, NormalizedName = Roles.Admin.ToUpperInvariant(), DisplayName = "Administrator", },
            new() { Id = Guid.NewGuid(), Name = Roles.Student, NormalizedName = Roles.Student.ToUpperInvariant(), DisplayName = "Student" },
        };

        if (!await context.Roles.AnyAsync())
        {
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        return roles;

        #endregion Role List
    }

    private static List<AppUser> StudentList(List<Faculty> faculties)
    {
        #region Student List

        var list = new List<AppUser>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Tien Phat",
                LastName = "Vu",
                Email = "student1@gmail.com",
                NormalizedEmail = "student1@gmail.com".ToUpperInvariant(),
                UserName = "student1",
                NormalizedUserName = "student1".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[0].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Meryl",
                LastName = "Streep",
                Email = "student2@gmail.com",
                NormalizedEmail = "student2@gmail.com".ToUpperInvariant(),
                UserName = "student2",
                NormalizedUserName = "student2".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[0].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Cate",
                LastName = "Blanchett",
                Email = "student3@gmail.com",
                NormalizedEmail = "student3@gmail.com".ToUpperInvariant(),
                UserName = "student3",
                NormalizedUserName = "student3".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[0].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Viola",
                LastName = "Davis",
                Email = "student4@gmail.com",
                NormalizedEmail = "student4@gmail.com".ToUpperInvariant(),
                UserName = "student4",
                NormalizedUserName = "student4".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[0].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Scarlett",
                LastName = "Johansson",
                Email = "student5@gmail.com",
                NormalizedEmail = "student5@gmail.com".ToUpperInvariant(),
                UserName = "student5",
                NormalizedUserName = "student5".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[0].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Angelina",
                LastName = "Jolie",
                Email = "student6@gmail.com",
                NormalizedEmail = "student6@gmail.com".ToUpperInvariant(),
                UserName = "student6",
                NormalizedUserName = "student6".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[0].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Jennifer",
                LastName = "Lawrence",
                Email = "student7@gmail.com",
                NormalizedEmail = "student7@gmail.com".ToUpperInvariant(),
                UserName = "student7",
                NormalizedUserName = "student7".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[0].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Nicole",
                LastName = "Kidman",
                Email = "student8@gmail.com",
                NormalizedEmail = "student8@gmail.com".ToUpperInvariant(),
                UserName = "student8",
                NormalizedUserName = "student8".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[0].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Emma",
                LastName = "Stone",
                Email = "student9@gmail.com",
                NormalizedEmail = "student9@gmail.com".ToUpperInvariant(),
                UserName = "student9",
                NormalizedUserName = "student9".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[0].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Charlize",
                LastName = "Theron",
                Email = "student10@gmail.com",
                NormalizedEmail = "student10@gmail.com".ToUpperInvariant(),
                UserName = "student10",
                NormalizedUserName = "student10".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[0].Id,
            },
        };

        return list;

        #endregion Student List
    }
}
