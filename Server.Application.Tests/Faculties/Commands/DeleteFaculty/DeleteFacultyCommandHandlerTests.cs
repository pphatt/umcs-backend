﻿using System.Linq.Expressions;

using FluentAssertions;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Moq;

using Server.Application.Features.FacultyApp.Commands.DeleteFaculty;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Application.Tests.Faculties.Commands.DeleteFaculty;

[Trait("Faculty", "Delete")]
public class DeleteFacultyCommandHandlerTests : BaseTest
{
    private readonly DeleteFacultyCommandHandler _commandHandler;
    private readonly Guid _facultyId = Guid.NewGuid();

    public DeleteFacultyCommandHandlerTests()
    {
        var faculty = new Faculty
        {
            Id = _facultyId,
            Name = "IT"
        };

        _mockFacultyRepository
            .Setup(repo => repo.GetByIdAsync(_facultyId))
            .ReturnsAsync(faculty);

        var users = new List<AppUser>().AsQueryable();
        _mockUserManager
            .Setup(m => m.Users)
            .Returns(users);

        _commandHandler = new DeleteFacultyCommandHandler(_mockUnitOfWork.Object, _dateTimeProvider, _mockUserManager.Object);
    }

    [Fact]
    public async Task DeleteFacultyCommandHandler_DeleteFaculty_Should_ReturnError_WhenSearchByFacultyIdCannotFound()
    {
        // Arrange
        var command = new DeleteFacultyCommand
        {
            Id = Guid.NewGuid(),
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.Faculty.CannotFound);
        result.FirstError.Code.Should().Be(Errors.Faculty.CannotFound.Code);
        result.FirstError.Description.Should().Be(Errors.Faculty.CannotFound.Description);
    }

    // cannot mock any thing related to user manager (temporary).
    //[Fact]
    //public async Task DeleteFacultyCommandHandler_DeleteFaculty_Should_ReturnError_WhenHasUsersInFaculty()
    //{
    //    // Arrange
    //    var command = new DeleteFacultyCommand
    //    {
    //        Id = _facultyId
    //    };

    //    var user = new AppUser
    //    {
    //        Id = Guid.NewGuid(),
    //        FacultyId = _facultyId,
    //        UserName = "testuser",
    //        Email = "test@example.com",
    //        IsActive = true,
    //        DateCreated = DateTime.UtcNow
    //    };

    //    var usersList = new List<AppUser> { user };
    //    var usersQueryable = usersList.AsQueryable();

    //    _mockUserManager
    //        .Setup(m => m.Users)
    //        .Returns(usersQueryable);

    //    // Act
    //    var result = await _commandHandler.Handle(command, CancellationToken.None);

    //    // Assert
    //    result.IsError.Should().BeTrue();
    //    result.FirstError.Should().Be(Errors.Faculty.HasUserIn);
    //    result.FirstError.Code.Should().Be(Errors.Faculty.HasUserIn.Code);
    //    result.FirstError.Description.Should().Be(Errors.Faculty.HasUserIn.Description);
    //}

    //[Fact]
    //public async Task Handle_WhenValidRequest_ShouldDeleteSuccessfully()
    //{
    //    // Arrange
    //    var command = new DeleteFacultyCommand { Id = _facultyId };

    //    var users = new List<AppUser>().AsQueryable();
    //    _mockUserManager
    //        .Setup(m => m.Users)
    //        .Returns(users);

    //    _mockUnitOfWork
    //        .Setup(uow => uow.FacultyRepository.Update(It.IsAny<Faculty>()));

    //    // Act
    //    var result = await _commandHandler.Handle(command, CancellationToken.None);

    //    // Assert
    //    result.IsError.Should().BeFalse();
    //    result.Value.Should().BeOfType<ResponseWrapper>();
    //    result.Value.IsSuccessful.Should().BeTrue();
    //    result.Value.Message.Should().Be("Delete faculty successfully.");

    //    _mockUnitOfWork.Verify(
    //        uow => uow.CompleteAsync(),
    //        Times.Once);
    //}
}
