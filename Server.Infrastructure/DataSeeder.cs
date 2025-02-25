using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Identity.Role;
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
        var adminId = Guid.NewGuid();

        // seed faculies.
        var faculties = await FacultyList(context);

        // seed academic years.
        var academicYears = await AcademicYearList(context, adminId);

        // seed roles.
        var roles = await RoleList(context);

        // seed users.
        var passwordHasher = new PasswordHasher<AppUser>();

        var studentList = StudentList(faculties);
        var coordinatorList = CoordinatorList(faculties);

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

            // create coordinator account.
            foreach (var user in coordinatorList)
            {
                user.PasswordHash = passwordHasher.HashPassword(user, "Admin@123");

                await context.Users.AddAsync(user);
                await context.UserRoles.AddAsync(new IdentityUserRole<Guid>
                {
                    RoleId = roles[2].Id,
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

            // seed student role claims permissions.
            var studentPermissions = await roleManager.GetClaimsAsync(roles[1]);

            if (!studentPermissions.Any())
            {
                var studentPermissionList = new List<RoleClaimsDto>
                {
                    new()
                    {
                        Selected = true,
                        Value = "Permissions.Contributions.Create"
                    },
                    new()
                    {
                        Selected = true,
                        Value = "Permissions.Contributions.Edit"
                    },
                    new()
                    {
                        Selected = true,
                        Value = "Permissions.Contributions.Download"
                    }
                };

                foreach (var studentPermission in studentPermissionList)
                {
                    await roleManager.AddClaimAsync(roles[1], new Claim(UserClaims.Permissions, studentPermission.Value!));
                }
            }

            // seed coordinator role claims permissions.
            var coordinatorPermissions = await roleManager.GetClaimsAsync(roles[2]);

            if (!coordinatorPermissions.Any())
            {
                var coordinatorPermissionList = new List<RoleClaimsDto>
                {
                    new()
                    {
                        Selected = true,
                        Value = "Permissions.Contributions.Manage"
                    }
                };
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
            new() { Id = Guid.NewGuid(), Name = Roles.Coordinator, NormalizedName = Roles.Coordinator.ToUpperInvariant(), DisplayName = "Marketing Coordinator" },
            new() { Id = Guid.NewGuid(), Name = Roles.Manager, NormalizedName = Roles.Manager.ToUpperInvariant(), DisplayName = "Manager" },
            new() { Id = Guid.NewGuid(), Name = Roles.Guest, NormalizedName = Roles.Guest.ToUpperInvariant(), DisplayName = "Guest" },
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

    private static List<AppUser> CoordinatorList(List<Faculty> faculties)
    {
        #region Cooridnator List

        var list = new List<AppUser>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Tien Phat",
                LastName = "Vu",
                Email = "phatvtgcs210973@fpt.edu.vn",
                NormalizedEmail = "phatvtgcs210973@fpt.edu.vn".ToUpperInvariant(),
                UserName = "coordinator",
                NormalizedUserName = "coordinator".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[0].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Anh Duong",
                LastName = "Trinh",
                Email = "cooridnator1@gmail.com",
                NormalizedEmail = "cooridnator1@gmail.com".ToUpperInvariant(),
                UserName = "coordinator1",
                NormalizedUserName = "coordinator1".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[1].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Thien An",
                LastName = "Do",
                Email = "cooridnator2@gmail.com",
                NormalizedEmail = "cooridnator2@gmail.com".ToUpperInvariant(),
                UserName = "coordinator2",
                NormalizedUserName = "coordinator2".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[2].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Vien Hao",
                LastName = "Dang",
                Email = "cooridnator3@gmail.com",
                NormalizedEmail = "cooridnator3@gmail.com".ToUpperInvariant(),
                UserName = "coordinator3",
                NormalizedUserName = "coordinator3".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[3].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Nguyen Quoc Khanh",
                LastName = "Le",
                Email = "cooridnator4@gmail.com",
                NormalizedEmail = "cooridnator4@gmail.com".ToUpperInvariant(),
                UserName = "coordinator4",
                NormalizedUserName = "coordinator4".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[4].Id,
            },
        };

        return list;

        #endregion Cooridnator List
    }

    private static async Task<List<AcademicYear>> AcademicYearList(AppDbContext context, Guid userId)
    {
        #region Academic Year List

        var academicYears = new List<AcademicYear>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2021-2022",
                StartClosureDate = new DateTime(2021, 1, 1),
                EndClosureDate = new DateTime(2021, 2, 1),
                FinalClosureDate = new DateTime(2021, 3, 1),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2022-2023",
                StartClosureDate = new DateTime(2022, 1, 15),
                EndClosureDate = new DateTime(2022, 5, 20),
                FinalClosureDate = new DateTime(2022, 6, 30),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2023-2024",
                StartClosureDate = new DateTime(2023, 1, 10),
                EndClosureDate = new DateTime(2023, 8, 15),
                FinalClosureDate = new DateTime(2023, 9, 20),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2024-2025",
                StartClosureDate = new DateTime(2024, 1, 5),
                EndClosureDate = new DateTime(2024, 4, 10),
                FinalClosureDate = new DateTime(2024, 5, 15),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2025-2026",
                StartClosureDate = new DateTime(2025, 1, 1),
                EndClosureDate = new DateTime(2025, 10, 5),
                FinalClosureDate = new DateTime(2025, 11, 10),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2026-2027",
                StartClosureDate = new DateTime(2026, 1, 15),
                EndClosureDate = new DateTime(2026, 3, 20),
                FinalClosureDate = new DateTime(2026, 4, 25),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2027-2028",
                StartClosureDate = new DateTime(2027, 1, 5),
                EndClosureDate = new DateTime(2027, 7, 10),
                FinalClosureDate = new DateTime(2027, 8, 15),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2028-2029",
                StartClosureDate = new DateTime(2028, 1, 10),
                EndClosureDate = new DateTime(2028, 12, 15),
                FinalClosureDate = new DateTime(2029, 1, 20),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2029-2030",
                StartClosureDate = new DateTime(2029, 1, 1),
                EndClosureDate = new DateTime(2029, 6, 5),
                FinalClosureDate = new DateTime(2029, 7, 10),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2030-2031",
                StartClosureDate = new DateTime(2030, 1, 15),
                EndClosureDate = new DateTime(2030, 9, 20),
                FinalClosureDate = new DateTime(2030, 10, 25),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2031-2032",
                StartClosureDate = new DateTime(2031, 1, 1),
                EndClosureDate = new DateTime(2031, 3, 10),
                FinalClosureDate = new DateTime(2031, 4, 20),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2032-2033",
                StartClosureDate = new DateTime(2032, 1, 5),
                EndClosureDate = new DateTime(2032, 5, 15),
                FinalClosureDate = new DateTime(2032, 6, 25),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2033-2034",
                StartClosureDate = new DateTime(2033, 1, 10),
                EndClosureDate = new DateTime(2033, 8, 20),
                FinalClosureDate = new DateTime(2033, 9, 30),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2034-2035",
                StartClosureDate = new DateTime(2034, 1, 1),
                EndClosureDate = new DateTime(2034, 11, 5),
                FinalClosureDate = new DateTime(2034, 12, 10),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2035-2036",
                StartClosureDate = new DateTime(2035, 1, 15),
                EndClosureDate = new DateTime(2035, 4, 20),
                FinalClosureDate = new DateTime(2035, 5, 25),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2036-2037",
                StartClosureDate = new DateTime(2036, 1, 5),
                EndClosureDate = new DateTime(2036, 10, 15),
                FinalClosureDate = new DateTime(2036, 11, 25),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2037-2038",
                StartClosureDate = new DateTime(2037, 1, 10),
                EndClosureDate = new DateTime(2037, 2, 15),
                FinalClosureDate = new DateTime(2037, 3, 20),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2038-2039",
                StartClosureDate = new DateTime(2038, 1, 5),
                EndClosureDate = new DateTime(2038, 6, 10),
                FinalClosureDate = new DateTime(2038, 7, 15),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2039-2040",
                StartClosureDate = new DateTime(2039, 1, 1),
                EndClosureDate = new DateTime(2039, 9, 5),
                FinalClosureDate = new DateTime(2039, 10, 10),
                IsActive = true,
                UserIdCreated = userId,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "2040-2041",
                StartClosureDate = new DateTime(2040, 1, 10),
                EndClosureDate = new DateTime(2040, 12, 15),
                FinalClosureDate = new DateTime(2041, 1, 20),
                IsActive = true,
                UserIdCreated = userId,
            },
        };

        if (!await context.AcademicYears.AnyAsync())
        {
            await context.AcademicYears.AddRangeAsync(academicYears);
            await context.SaveChangesAsync();
        }

        return academicYears;

        #endregion Academic Year List
    }
}
