using System.Security.Claims;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Server.Application.Common.Dtos.Identity.Role;
using Server.Application.Common.Extensions;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Enums;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Infrastructure;

using File = Server.Domain.Entity.Content.File;

public partial class DataSeeder
{
    public async static Task SeedAsync(
        AppDbContext context,
        RoleManager<AppRole> roleManager,
        IContributionRepository contributionRepository)
    {
        var adminId = Guid.NewGuid();

        // seed faculties.
        var faculties = await FacultyList(context);

        // seed academic years.
        var academicYears = await AcademicYearList(context, adminId);

        // seed roles.
        var roles = await RoleList(context);

        // seed users.
        var passwordHasher = new PasswordHasher<AppUser>();

        var managerId = Guid.NewGuid();
        var studentList = StudentList(faculties);
        var coordinatorList = CoordinatorList(faculties);
        var guestList = GuestList(faculties);

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

            await context.UserRoles.AddAsync(new IdentityUserRole<Guid> { RoleId = roles[0].Id, UserId = admin.Id, });

            // create manager account.
            var managerEmail = "snotright5@gmail.com";
            var managerUsername = "manager";

            var manager = new AppUser()
            {
                Id = managerId,
                FirstName = "Tien Phat",
                LastName = "Vu",
                Email = managerEmail,
                NormalizedEmail = managerEmail.ToUpperInvariant(),
                UserName = managerUsername,
                NormalizedUserName = managerUsername.ToUpperInvariant(),
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                IsActive = true,
                DateCreated = DateTime.Now,
                FacultyId = faculties[0].Id,
            };

            manager.PasswordHash = passwordHasher.HashPassword(manager, "Admin@123");

            await context.Users.AddAsync(manager);

            await context.UserRoles.AddAsync(new IdentityUserRole<Guid> { RoleId = roles[3].Id, UserId = manager.Id, });

            // create student accounts.
            foreach (var user in studentList)
            {
                user.PasswordHash = passwordHasher.HashPassword(user, "Admin@123");

                await context.Users.AddAsync(user);
                await context.UserRoles.AddAsync(new IdentityUserRole<Guid>
                {
                    RoleId = roles[1].Id, UserId = user.Id,
                });
            }

            // create coordinator account.
            foreach (var user in coordinatorList)
            {
                user.PasswordHash = passwordHasher.HashPassword(user, "Admin@123");

                await context.Users.AddAsync(user);
                await context.UserRoles.AddAsync(new IdentityUserRole<Guid>
                {
                    RoleId = roles[2].Id, UserId = user.Id,
                });
            }

            // create guest account.
            foreach (var guest in guestList)
            {
                guest.PasswordHash = passwordHasher.HashPassword(guest, "Admin@123");

                await context.Users.AddAsync(guest);
                await context.UserRoles.AddAsync(
                    new IdentityUserRole<Guid> { RoleId = roles[4].Id, UserId = guest.Id, });
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
                    await roleManager.AddClaimAsync(roles[0],
                        new Claim(UserClaims.Permissions, adminPermission.Value!));
                }
            }

            // seed student role claims permissions.
            var studentPermissions = await roleManager.GetClaimsAsync(roles[1]);

            if (!studentPermissions.Any())
            {
                var studentPermissionList = new List<RoleClaimsDto>
                {
                    new() { Selected = true, Value = "Permissions.StudentDashboard.View" },
                    new() { Selected = true, Value = "Permissions.Contributions.View" },
                    new() { Selected = true, Value = "Permissions.Contributions.Create" },
                    new() { Selected = true, Value = "Permissions.Contributions.Edit" },
                    new() { Selected = true, Value = "Permissions.Contributions.Download" }
                };

                foreach (var studentPermission in studentPermissionList)
                {
                    await roleManager.AddClaimAsync(roles[1],
                        new Claim(UserClaims.Permissions, studentPermission.Value!));
                }
            }

            // seed coordinator role claims permissions.
            var coordinatorPermissions = await roleManager.GetClaimsAsync(roles[2]);

            if (!coordinatorPermissions.Any())
            {
                var coordinatorPermissionList = new List<RoleClaimsDto>
                {
                    new() { Selected = true, Value = "Permissions.Dashboards.View" },
                    new() { Selected = true, Value = "Permissions.Contributions.View" },
                    new() { Selected = true, Value = "Permissions.Contributions.Approve" },
                    new() { Selected = true, Value = "Permissions.ManageContributions.View" },
                    new() { Selected = true, Value = "Permissions.SettingGAC.View" },
                    new() { Selected = true, Value = "Permissions.Contributions.Download" },
                };

                foreach (var coordinatorPermission in coordinatorPermissionList)
                {
                    await roleManager.AddClaimAsync(roles[2],
                        new Claim(UserClaims.Permissions, coordinatorPermission.Value!));
                }
            }

            // seed guest role claims permissions.
            var guestPermissions = await roleManager.GetClaimsAsync(roles[4]);

            if (!coordinatorPermissions.Any())
            {
                var guestPermissionList = new List<RoleClaimsDto>
                {
                    new() { Selected = true, Value = "Permissions.StudentDashboard.View" },
                };

                foreach (var guestPermission in guestPermissionList)
                {
                    await roleManager.AddClaimAsync(roles[4],
                        new Claim(UserClaims.Permissions, guestPermission.Value!));
                }
            }
        }

        var contributions = ContributionsList(academicYears, faculties, studentList);

        if (!context.Contributions.Any())
        {
            foreach (var contribution in contributions)
            {
                await context.Contributions.AddAsync(contribution);
            }

            await context.SaveChangesAsync();

            foreach (var contribution in contributions)
            {
                await contributionRepository.SendToApproved(contribution.Id, adminId);
            }

            await context.SaveChangesAsync();
        }

        var files = ContributionFilesList(contributions);

        if (!context.Files.Any())
        {
            await context.Files.AddRangeAsync(files);

            await context.SaveChangesAsync();
        }

        if (!context.ContributionPublics.Any())
        {
            for (var i = 0; i <= 250; i++)
            {
                await contributionRepository.ApproveContribution(contributions[i], adminId);
            }

            for (var i = 251; i <= 300; i++)
            {
                await contributionRepository.RejectContribution(contributions[i], adminId, "default reject reason");
            }

            for (var i = 301; i <= 500; i++)
            {
                await contributionRepository.ApproveContribution(contributions[i], adminId);
            }

            await context.SaveChangesAsync();
        }

        if (!context.ContributionPublicComments.Any())
        {
            for (var i = 0; i <= 250; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    await context.ContributionPublicComments.AddAsync(new ContributionPublicComment
                    {
                        UserId = studentList[j].Id,
                        ContributionId = contributions[i].Id,
                        Content = $"test comment {i}",
                    });
                }
            }

            for (var i = 301; i <= 500; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    await context.ContributionPublicComments.AddAsync(new ContributionPublicComment
                    {
                        UserId = studentList[j].Id,
                        ContributionId = contributions[i].Id,
                        Content = $"test comment {i}",
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        if (!context.Likes.Any())
        {
            for (var i = 0; i <= 250; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    await context.Likes.AddAsync(new Like
                    {
                        UserId = studentList[j].Id,
                        ContributionId = contributions[i].Id,
                    });
                }
            }

            for (var i = 301; i <= 500; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    await context.Likes.AddAsync(new Like
                    {
                        UserId = studentList[j].Id,
                        ContributionId = contributions[i].Id,
                    });
                }
            }

            await context.SaveChangesAsync();
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
            new()
            {
                Id = Guid.NewGuid(),
                Name = Roles.Admin,
                NormalizedName = Roles.Admin.ToUpperInvariant(),
                DisplayName = "Administrator",
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = Roles.Student,
                NormalizedName = Roles.Student.ToUpperInvariant(),
                DisplayName = "Student"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = Roles.Coordinator,
                NormalizedName = Roles.Coordinator.ToUpperInvariant(),
                DisplayName = "Marketing Coordinator"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = Roles.Manager,
                NormalizedName = Roles.Manager.ToUpperInvariant(),
                DisplayName = "Manager"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = Roles.Guest,
                NormalizedName = Roles.Guest.ToUpperInvariant(),
                DisplayName = "Guest"
            },
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
                Email = "phatvu080903@gmail.com",
                NormalizedEmail = "phatvu080903@gmail.com".ToUpperInvariant(),
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
                FirstName = "student2",
                LastName = "student2",
                Email = "jettlaststand@gmail.com",
                NormalizedEmail = "jettlaststand@gmail.com".ToUpperInvariant(),
                UserName = "student2",
                NormalizedUserName = "student2".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[1].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "student3",
                LastName = "student3",
                Email = "student3@gmail.com",
                NormalizedEmail = "student3@gmail.com".ToUpperInvariant(),
                UserName = "student3",
                NormalizedUserName = "student3".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[2].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "student4",
                LastName = "student4",
                Email = "student4@gmail.com",
                NormalizedEmail = "student4@gmail.com".ToUpperInvariant(),
                UserName = "student4",
                NormalizedUserName = "student4".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[3].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "student5",
                LastName = "student5",
                Email = "student5@gmail.com",
                NormalizedEmail = "student5@gmail.com".ToUpperInvariant(),
                UserName = "student5",
                NormalizedUserName = "student5".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[4].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "student6",
                LastName = "student6",
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
                FirstName = "student7",
                LastName = "student7",
                Email = "student7@gmail.com",
                NormalizedEmail = "student7@gmail.com".ToUpperInvariant(),
                UserName = "student7",
                NormalizedUserName = "student7".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[1].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "student8",
                LastName = "student8",
                Email = "student8@gmail.com",
                NormalizedEmail = "student8@gmail.com".ToUpperInvariant(),
                UserName = "student8",
                NormalizedUserName = "student8".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[2].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "student9",
                LastName = "student9",
                Email = "student9@gmail.com",
                NormalizedEmail = "student9@gmail.com".ToUpperInvariant(),
                UserName = "student9",
                NormalizedUserName = "student9".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[3].Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "student10",
                LastName = "student10",
                Email = "student10@gmail.com",
                NormalizedEmail = "student10@gmail.com".ToUpperInvariant(),
                UserName = "student10",
                NormalizedUserName = "student10".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[4].Id,
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

    private static List<AppUser> GuestList(List<Faculty> faculties)
    {
        #region Guest List

        var list = new List<AppUser>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Phat",
                LastName = "Vu",
                Email = "guest@gmail.com",
                NormalizedEmail = "guest@gmail.com".ToUpperInvariant(),
                UserName = "guest",
                NormalizedUserName = "guest".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[0].Id,
                Avatar =
                    "https://res.cloudinary.com/dus70fkd3/image/upload/c_thumb,w_200,g_face/v1743472745/133858407972414310_tmpggi.jpg",
                AvatarPublicId = "avatar/user-aebb36e4-de2f-4d97-8985-199910cedb9e/y4u6isgtypvplhekz3d5"
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Vu",
                LastName = "Nguyen",
                Email = "guest1@gmail.com",
                NormalizedEmail = "guest1@gmail.com".ToUpperInvariant(),
                UserName = "guest1",
                NormalizedUserName = "guest1".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[1].Id,
                Avatar =
                    "https://res.cloudinary.com/dus70fkd3/image/upload/c_thumb,w_200,g_face/v1743472724/DALLE2024-04-0507.40_ebloch.webp",
                AvatarPublicId = "avatar/user-aebb36e4-de2f-4d97-8985-199910cedb9e/y4u6isgtypvplhekz3d5"
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Duong",
                LastName = "Anh",
                Email = "guest2@gmail.com",
                NormalizedEmail = "guest2@gmail.com".ToUpperInvariant(),
                UserName = "guest2",
                NormalizedUserName = "guest2".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[2].Id,
                Avatar =
                    "https://res.cloudinary.com/dus70fkd3/image/upload/c_thumb,w_200,g_face/v1743472714/CustomBlogCover_oevxl3.avif",
                AvatarPublicId = "avatar/user-aebb36e4-de2f-4d97-8985-199910cedb9e/y4u6isgtypvplhekz3d5"
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Tri",
                LastName = "Sieu",
                Email = "guest3@gmail.com",
                NormalizedEmail = "guest3@gmail.com".ToUpperInvariant(),
                UserName = "guest3",
                NormalizedUserName = "guest3".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[3].Id,
                Avatar =
                    "https://res.cloudinary.com/dus70fkd3/image/upload/c_thumb,w_200,g_face/v1743472705/ab67616d0000b2731841e5f0a180d90a17e38c89_dxkaxs.jpg",
                AvatarPublicId = "avatar/user-aebb36e4-de2f-4d97-8985-199910cedb9e/y4u6isgtypvplhekz3d5"
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Bao",
                LastName = "Gia",
                Email = "guest4@gmail.com",
                NormalizedEmail = "guest4@gmail.com".ToUpperInvariant(),
                UserName = "guest4",
                NormalizedUserName = "guest4".ToUpperInvariant(),
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                DateCreated = DateTime.Now,
                FacultyId = faculties[4].Id,
                Avatar =
                    "https://res.cloudinary.com/dus70fkd3/image/upload/c_thumb,w_200,g_face/v1743472697/sora-no-game-no-life_fjvodi.jpg",
                AvatarPublicId = "avatar/user-aebb36e4-de2f-4d97-8985-199910cedb9e/y4u6isgtypvplhekz3d5"
            }
        };

        return list;

        #endregion Guest List
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

    public static List<Contribution> ContributionsList(List<AcademicYear> academicYears, List<Faculty> faculties,
        List<AppUser> studentList)
    {
        #region Contribution List

        var contributions = new List<Contribution>();
        int contributionCount = 1;

        for (int studentIndex = 0; studentIndex < studentList.Count; studentIndex++) // 10 students
        {
            for (int facultyIndex = 0; facultyIndex < faculties.Count; facultyIndex++) // 5 faculties
            {
                for (int yearIndex = 0; yearIndex < academicYears.Count; yearIndex++) // 20 academic years
                {
                    var contribution = new Contribution
                    {
                        AcademicYearId = academicYears[yearIndex].Id,
                        FacultyId = faculties[facultyIndex].Id,
                        UserId = studentList[studentIndex].Id,
                        Id = Guid.NewGuid(),
                        IsConfirmed = true,
                        DateCreated = DateTime.Now,
                        Title = $"test {contributionCount}",
                        Slug = $"test-{contributionCount}",
                        SubmissionDate = DateTime.Now,
                        Status = ContributionStatus.Pending,
                        Content =
                            "<p>\r\n  <meta charset=\"utf-8\"><span data-metadata=\"\"></span><span data-buffer=\"\"></span><span style=\"white-space:pre-wrap;\"><strong></strong></span>\r\n</p>\r\n<p>\r\n  <meta charset=\"utf-8\"><span data-metadata=\"\"></span><span data-buffer=\"\"></span><span style=\"white-space:pre-wrap;\">Gastronomy atmosphere set aside. Slice butternut cooking home. Delicious romantic undisturbed raw platter will meld. Thick Skewers skillet natural, smoker soy sauce wait roux. slices rosette bone-in simmer precision alongside baby leeks. Crafting renders aromatic enjoyment, then slices taco. Minutes undisturbed cuisine lunch magnificent mustard curry. Juicy share baking sheet pork. Meals ramen rarities selection, raw pastries richness magnificent atmosphere. Sweet soften dinners, cover mustard infused skillet, Skewers on culinary experience.</span><br><br><span style=\"white-space:pre-wrap;\">Juicy meatballs brisket slammin' baked shoulder. Juicy smoker soy sauce burgers brisket. polenta mustard hunk greens. Wine technique snack skewers chuck excess. Oil heat slowly. slices natural delicious, set aside magic tbsp skillet, bay leaves brown centerpiece. fruit soften edges frond slices onion snack pork steem on wines excess technique cup; Cover smoker soy sauce fruit snack. Sweet one-dozen scrape delicious, non sheet raw crunch mustard. Minutes clever slotted tongs scrape, brown steem undisturbed rice.</span><br><br><span style=\"white-space:pre-wrap;\">Food qualities braise chicken cuts bowl through slices butternut snack. Tender meat juicy dinners. One-pot low heat plenty of time adobo fat raw soften fruit. sweet renders bone-in marrow richness kitchen, fricassee basted pork shoulder. Delicious butternut squash hunk. Flavor centerpiece plate, delicious ribs bone-in meat, excess chef end. sweet effortlessly pork, low heat smoker soy sauce flavor meat, rice fruit fruit. Romantic fall-off-the-bone butternut chuck rice burgers.</span>\r\n</p>\r\n<p>\r\n  <meta charset=\"utf-8\"><span data-metadata=\"\"></span><span data-buffer=\"\"></span>\r\n  </strong><span style=\"white-space:pre-wrap;\"><strong></strong></span>\r\n</p>\r\n<p>\r\n  <meta charset=\"utf-8\"><span data-metadata=\"\"></span><span data-buffer=\"\"></span>\r\n</p>\r\n<p><span style=\"white-space:pre-wrap;\">Gastronomy atmosphere set aside. Slice butternut cooking home. Delicious romantic undisturbed raw platter will meld. Thick Skewers skillet natural, smoker soy sauce wait roux. slices rosette bone-in simmer precision alongside baby leeks. Crafting renders aromatic enjoyment, then slices taco. Minutes undisturbed cuisine lunch magnificent mustard curry. Juicy share baking sheet pork. Meals ramen rarities selection, raw pastries richness magnificent atmosphere. Sweet soften dinners, cover mustard infused skillet, Skewers on culinary experience.</span></p>",
                        ShortDescription =
                            "Lorem ipsum dolor sit, amet consectetur adipisicing elit. Quaerat similique at molestias deleniti tempore consectetur commodi ab, beatae soluta dolorem nemo, quo totam repudiandae corporis distinctio voluptatibus accusamus sint ad.\r\n          Magni culpa quia quis asperiores ipsum molestias aspernatur, laboriosam possimus? Mollitia laudantium iste autem placeat aspernatur. Ducimus aperiam, adipisci excepturi quo officiis nisi et rem in, animi quod, eaque nihil.\r\n"
                    };

                    contributions.Add(contribution);
                    contributionCount++;
                }
            }
        }

        return contributions;

        #endregion Contribution List
    }

    public static List<File> ContributionFilesList(List<Contribution> contributions)
    {
        #region Contribution Files List

        var files = new List<File>();

        for (var i = 0; i < 150; i++)
        {
            files.AddRange(new List<File>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ContributionId = contributions[i].Id,
                    DateCreated = DateTime.UtcNow,
                    Path = "https://res.cloudinary.com/dus70fkd3/image/upload/v1743514391/Elon_Musk_fvuxcu.jpg",
                    Name = "Elon_Musk_fvuxcu.jpg",
                    Type = FileType.Thumbnail,
                    PublicId = "Elon_Musk_fvuxcu",
                    Extension = ".jpg"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ContributionId = contributions[i].Id,
                    DateCreated = DateTime.UtcNow,
                    Path =
                        "https://res.cloudinary.com/dus70fkd3/raw/upload/v1743514900/COMP786_CW1_Logbook_gd4i3q.docx",
                    Name = "COMP786_CW1_Logbook_gd4i3q.docx",
                    Type = FileType.File,
                    PublicId = "COMP786_CW1_Logbook_gd4i3q.docx",
                    Extension = ".docx"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ContributionId = contributions[i].Id,
                    DateCreated = DateTime.UtcNow,
                    Path =
                        "https://res.cloudinary.com/dus70fkd3/raw/upload/v1743514899/COMP1786_CW1_REPORT_Logbook_Template_f3odic.docx",
                    Name = "1_UnitCOMP1786_CW1_REPORT_Logbook_Template_f3odic.docx",
                    Type = FileType.File,
                    PublicId = "COMP1786_CW1_REPORT_Logbook_Template_f3odic.docx",
                    Extension = ".docx"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ContributionId = contributions[i].Id,
                    DateCreated = DateTime.UtcNow,
                    Path =
                        "https://res.cloudinary.com/dus70fkd3/image/upload/v1743514900/Final_COMP1787_Submission_layout_wuiwzm.pdf",
                    Name = "Final_COMP1787_Submission_layout_wuiwzm.pdf",
                    Type = FileType.File,
                    PublicId = "Final_COMP1787_Submission_layout_wuiwzm.pdf",
                    Extension = ".pdf"
                },
            });
        }
        
        for (var i = 250; i < 400; i++)
        {
            files.AddRange(new List<File>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ContributionId = contributions[i].Id,
                    DateCreated = DateTime.UtcNow,
                    Path = "https://res.cloudinary.com/dus70fkd3/image/upload/v1743514391/Elon_Musk_fvuxcu.jpg",
                    Name = "Elon_Musk_fvuxcu.jpg",
                    Type = FileType.Thumbnail,
                    PublicId = "Elon_Musk_fvuxcu",
                    Extension = ".jpg"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ContributionId = contributions[i].Id,
                    DateCreated = DateTime.UtcNow,
                    Path =
                        "https://res.cloudinary.com/dus70fkd3/raw/upload/v1743514900/COMP786_CW1_Logbook_gd4i3q.docx",
                    Name = "COMP786_CW1_Logbook_gd4i3q.docx",
                    Type = FileType.File,
                    PublicId = "COMP786_CW1_Logbook_gd4i3q.docx",
                    Extension = ".docx"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ContributionId = contributions[i].Id,
                    DateCreated = DateTime.UtcNow,
                    Path =
                        "https://res.cloudinary.com/dus70fkd3/raw/upload/v1743514899/COMP1786_CW1_REPORT_Logbook_Template_f3odic.docx",
                    Name = "1_UnitCOMP1786_CW1_REPORT_Logbook_Template_f3odic.docx",
                    Type = FileType.File,
                    PublicId = "COMP1786_CW1_REPORT_Logbook_Template_f3odic.docx",
                    Extension = ".docx"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ContributionId = contributions[i].Id,
                    DateCreated = DateTime.UtcNow,
                    Path =
                        "https://res.cloudinary.com/dus70fkd3/image/upload/v1743514900/Final_COMP1787_Submission_layout_wuiwzm.pdf",
                    Name = "Final_COMP1787_Submission_layout_wuiwzm.pdf",
                    Type = FileType.File,
                    PublicId = "Final_COMP1787_Submission_layout_wuiwzm.pdf",
                    Extension = ".pdf"
                },
            });
        }
        
        for (var i = 500; i < 650; i++)
        {
            files.AddRange(new List<File>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ContributionId = contributions[i].Id,
                    DateCreated = DateTime.UtcNow,
                    Path = "https://res.cloudinary.com/dus70fkd3/image/upload/v1743514391/Elon_Musk_fvuxcu.jpg",
                    Name = "Elon_Musk_fvuxcu.jpg",
                    Type = FileType.Thumbnail,
                    PublicId = "Elon_Musk_fvuxcu",
                    Extension = ".jpg"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ContributionId = contributions[i].Id,
                    DateCreated = DateTime.UtcNow,
                    Path =
                        "https://res.cloudinary.com/dus70fkd3/raw/upload/v1743514900/COMP786_CW1_Logbook_gd4i3q.docx",
                    Name = "COMP786_CW1_Logbook_gd4i3q.docx",
                    Type = FileType.File,
                    PublicId = "COMP786_CW1_Logbook_gd4i3q.docx",
                    Extension = ".docx"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ContributionId = contributions[i].Id,
                    DateCreated = DateTime.UtcNow,
                    Path =
                        "https://res.cloudinary.com/dus70fkd3/raw/upload/v1743514899/COMP1786_CW1_REPORT_Logbook_Template_f3odic.docx",
                    Name = "1_UnitCOMP1786_CW1_REPORT_Logbook_Template_f3odic.docx",
                    Type = FileType.File,
                    PublicId = "COMP1786_CW1_REPORT_Logbook_Template_f3odic.docx",
                    Extension = ".docx"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ContributionId = contributions[i].Id,
                    DateCreated = DateTime.UtcNow,
                    Path =
                        "https://res.cloudinary.com/dus70fkd3/image/upload/v1743514900/Final_COMP1787_Submission_layout_wuiwzm.pdf",
                    Name = "Final_COMP1787_Submission_layout_wuiwzm.pdf",
                    Type = FileType.File,
                    PublicId = "Final_COMP1787_Submission_layout_wuiwzm.pdf",
                    Extension = ".pdf"
                },
            });
        }

        return files;

        #endregion Contribution Files List
    }
}
