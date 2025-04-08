using AuthApi.Controllers;
using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApi.Services.IService;
using emailApi.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace AuthApi.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IMemoryCache> _mockMemoryCache;
        private readonly Mock<IAuth> _mockAuth;
        private readonly Mock<IEmail> _mockEmail;
        private readonly AppDbContext _dbContext;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockMemoryCache = new Mock<IMemoryCache>();
            _mockAuth = new Mock<IAuth>();
            _mockEmail = new Mock<IEmail>();

            // Use real in-memory database for DbContext
            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use unique name for test isolation
                .Options;

            _dbContext = new AppDbContext(dbContextOptions);

            _controller = new AuthController(
                _mockMemoryCache.Object,
                _mockAuth.Object,
                _mockEmail.Object,
                _dbContext
            );
        }

        [Fact]
        public async Task LoginPost_ReturnsOk_WhenLoginSuccessful()
        {
            // Arrange
            var loginDto = new LoginRequestDto("testuser", "Test123!");
            var expectedResult = new { Token = "test-token", User = new { Id = "1", UserName = "testuser" } };

            _mockAuth.Setup(a => a.Login(It.IsAny<LoginRequestDto>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.LoginPost(loginDto);

            // Xunit.Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task LoginPost_ReturnsNotFound_WhenLoginFails()
        {
            // Arrange
            var loginDto = new LoginRequestDto("invaliduser", "WrongPass");

            var failResult = new { result = "" };
            _mockAuth.Setup(a => a.Login(It.IsAny<LoginRequestDto>()))
                .ReturnsAsync(failResult);

            // Act
            var result = await _controller.LoginPost(loginDto);

            // Xunit.Assert
            Xunit.Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RegisterPost_ReturnsOk_WhenRegistrationSuccessful()
        {
            // Arrange
            var registerDto = new RegisterRequestDto(
                "testuser",
                "Test123!",
                "test@example.com",
                "Test User"
            );

            var user = new ApplicationUser
            {
                Id = "user-id-1",
                UserName = registerDto.UserName,
                Email = registerDto.Email
            };

            _mockAuth.Setup(a => a.Register(It.IsAny<RegisterRequestDto>()))
                .ReturnsAsync(user);

            // Setup memory cache
            var memoryCacheEntryOptions = new MemoryCacheEntryOptions();
            object savedValue = null;
            string savedKey = null;

            _mockMemoryCache
                .Setup(m => m.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<MemoryCacheEntryOptions>()))
                .Callback<string, object, MemoryCacheEntryOptions>((key, value, options) =>
                {
                    savedKey = key;
                    savedValue = value;
                })
                .Returns((string key, object value, MemoryCacheEntryOptions options) => value);

            // Act
            var result = await _controller.RegisterPost(registerDto);

            // Xunit.Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var responseValue = okResult.Value as dynamic;
            Xunit.Assert.NotNull(responseValue);
            Xunit.Assert.Equal(user, responseValue.user);
            Xunit.Assert.InRange((int)responseValue.code, 100000, 999999);

            _mockEmail.Verify(e => e.SendMail(
                It.Is<EmailDTO>(dto => dto.To == registerDto.Email),
                It.IsAny<int>()),
                Times.Once);
        }

        [Fact]
        public async Task RegisterPost_ReturnsBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var registerDto = new RegisterRequestDto(
                "existinguser",
                "Test123!",
                "existing@example.com",
                "Existing User"
            );

            _mockAuth.Setup(a => a.Register(It.IsAny<RegisterRequestDto>()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.RegisterPost(registerDto);

            // Xunit.Assert
            Xunit.Assert.IsType<BadRequestResult>(result);
            _mockEmail.Verify(e => e.SendMail(It.IsAny<EmailDTO>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task AssignRole_ReturnsCreated_WhenRoleAssignedSuccessfully()
        {
            // Arrange
            var assignRoleDto = new AssignRoleRequestDto("testuser", "Admin");

            var user = new ApplicationUser
            {
                Id = "user-id-1",
                UserName = assignRoleDto.UserName,
                Email = "test@example.com"
            };

            _mockAuth.Setup(a => a.AssignRole(It.IsAny<AssignRoleRequestDto>()))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.AssignRole(assignRoleDto);

            // Xunit.Assert
            var statusCodeResult = Xunit.Assert.IsType<ObjectResult>(result);
            Xunit.Assert.Equal(201, statusCodeResult.StatusCode);
            Xunit.Assert.Equal(user, statusCodeResult.Value);
        }

        [Fact]
        public async Task AssignRole_ReturnsBadRequest_WhenRoleAssignmentFails()
        {
            // Arrange
            var assignRoleDto = new AssignRoleRequestDto("nonexistentuser", "Admin");

            _mockAuth.Setup(a => a.AssignRole(It.IsAny<AssignRoleRequestDto>()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.AssignRole(assignRoleDto);

            // Xunit.Assert
            Xunit.Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task EmailVerify_ReturnsOk_WhenVerificationSuccessful()
        {
            // Arrange
            string userId = "user-id-1";
            string email = "test@example.com";
            int verificationCode = 123456;

            var user = new ApplicationUser
            {
                Id = userId,
                Email = email,
                EmailConfirmed = false
            };

            // Add the user to the in-memory database
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Setup memory cache to return the verification code
            object cachedCode = verificationCode;
            _mockMemoryCache.Setup(m => m.TryGetValue($"EmailVerificationCode_{email}", out cachedCode))
                .Returns(true);

            // Act
            var result = await _controller.EmailVerify(verificationCode, userId);

            // Xunit.Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.Equal("Sikeres igazolás!", ((dynamic)okResult.Value).message);

            // Refresh user from context to check EmailConfirmed
            var updatedUser = await _dbContext.Users.FindAsync(userId);
            Xunit.Assert.True(updatedUser.EmailConfirmed);
        }

        [Fact]
        public async Task EmailVerify_ReturnsBadRequest_WhenCodeMismatch()
        {
            // Arrange
            string userId = "user-id-2";
            string email = "test2@example.com";
            int verificationCode = 123456;
            int invalidCode = 654321;

            var user = new ApplicationUser
            {
                Id = userId,
                Email = email,
                EmailConfirmed = false
            };

            // Add the user to the in-memory database
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Setup memory cache to return the correct verification code
            object cachedCode = verificationCode;
            _mockMemoryCache.Setup(m => m.TryGetValue($"EmailVerificationCode_{email}", out cachedCode))
                .Returns(true);

            // Act
            var result = await _controller.EmailVerify(invalidCode, userId);

            // Xunit.Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
            Xunit.Assert.Equal("Sikertelen igazolás, hibás a kód!", ((dynamic)badRequestResult.Value).message);

            // Refresh user from context to check EmailConfirmed
            var updatedUser = await _dbContext.Users.FindAsync(userId);
            Xunit.Assert.False(updatedUser.EmailConfirmed);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsListOfUsers()
        {
            // Arrange - Seed the database with users and roles
            var roles = new List<IdentityRole>
            {
                new IdentityRole { Id = "role1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "role2", Name = "User", NormalizedName = "USER" }
            };

            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", UserName = "user1", Email = "user1@test.com", NormalizedUserName = "USER1" },
                new ApplicationUser { Id = "2", UserName = "user2", Email = "user2@test.com", NormalizedUserName = "USER2" }
            };

            var userRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string> { UserId = "1", RoleId = "role1" },
                new IdentityUserRole<string> { UserId = "1", RoleId = "role2" },
                new IdentityUserRole<string> { UserId = "2", RoleId = "role1" }
            };

            _dbContext.Roles.AddRange(roles);
            _dbContext.Users.AddRange(users);
            await _dbContext.SaveChangesAsync();

            // We can't add UserRoles directly due to EF Core in-memory limitation
            // So we'll mock the necessary methods

            var mockApplicationUsersDbSet = new Mock<DbSet<ApplicationUser>>();
            mockApplicationUsersDbSet.As<IQueryable<ApplicationUser>>()
                .Setup(m => m.Provider).Returns(users.AsQueryable().Provider);
            mockApplicationUsersDbSet.As<IQueryable<ApplicationUser>>()
                .Setup(m => m.Expression).Returns(users.AsQueryable().Expression);
            mockApplicationUsersDbSet.As<IQueryable<ApplicationUser>>()
                .Setup(m => m.ElementType).Returns(users.AsQueryable().ElementType);
            mockApplicationUsersDbSet.As<IQueryable<ApplicationUser>>()
                .Setup(m => m.GetEnumerator()).Returns(users.AsQueryable().GetEnumerator());

            var mockUserRolesDbSet = new Mock<DbSet<IdentityUserRole<string>>>();
            mockUserRolesDbSet.As<IQueryable<IdentityUserRole<string>>>()
                .Setup(m => m.Provider).Returns(userRoles.AsQueryable().Provider);
            mockUserRolesDbSet.As<IQueryable<IdentityUserRole<string>>>()
                .Setup(m => m.Expression).Returns(userRoles.AsQueryable().Expression);
            mockUserRolesDbSet.As<IQueryable<IdentityUserRole<string>>>()
                .Setup(m => m.ElementType).Returns(userRoles.AsQueryable().ElementType);
            mockUserRolesDbSet.As<IQueryable<IdentityUserRole<string>>>()
                .Setup(m => m.GetEnumerator()).Returns(userRoles.AsQueryable().GetEnumerator());

            var mockRolesDbSet = new Mock<DbSet<IdentityRole>>();
            mockRolesDbSet.As<IQueryable<IdentityRole>>()
                .Setup(m => m.Provider).Returns(roles.AsQueryable().Provider);
            mockRolesDbSet.As<IQueryable<IdentityRole>>()
                .Setup(m => m.Expression).Returns(roles.AsQueryable().Expression);
            mockRolesDbSet.As<IQueryable<IdentityRole>>()
                .Setup(m => m.ElementType).Returns(roles.AsQueryable().ElementType);
            mockRolesDbSet.As<IQueryable<IdentityRole>>()
                .Setup(m => m.GetEnumerator()).Returns(roles.AsQueryable().GetEnumerator());

            // Create a new DbContext mock that uses our prepared DbSets
            var mockDb = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            mockDb.Setup(db => db.applicationUsers).Returns(mockApplicationUsersDbSet.Object);
            mockDb.Setup(db => db.UserRoles).Returns(mockUserRolesDbSet.Object);
            mockDb.Setup(db => db.Roles).Returns(mockRolesDbSet.Object);

            // Create a new controller with this mock
            var controller = new AuthController(
                _mockMemoryCache.Object,
                _mockAuth.Object,
                _mockEmail.Object,
                mockDb.Object
            );

            // Act
            var result = await controller.GetAllUsers();

            // Xunit.Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Xunit.Assert.IsType<List<object>>(okResult.Value);
            Xunit.Assert.Equal(2, returnedUsers.Count);
        }

        [Fact]
        public async Task DeleteUser_ReturnsOk_WhenUserDeleted()
        {
            // Arrange
            string userId = "user-id-to-delete";
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "DeleteMe",
                Email = "delete@test.com"
            };

            // Add the user to the in-memory database
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteUser(userId);

            // Xunit.Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.Equal("Felhasználó törölve.", ((dynamic)okResult.Value).message);

            // Verify user was deleted
            var deletedUser = await _dbContext.Users.FindAsync(userId);
            Xunit.Assert.Null(deletedUser);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            string userId = "nonexistent-user-id";

            // Act
            var result = await _controller.DeleteUser(userId);

            // Xunit.Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result);
            Xunit.Assert.Equal("Felhasználó nem található!", ((dynamic)notFoundResult.Value).message);
        }

        [Fact]
        public async Task GetUserById_ReturnsOk_WhenUserFound()
        {
            // Arrange
            string userId = "existing-user-id";
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "TestUser",
                Email = "test@example.com"
            };

            // Add the user to the in-memory database
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetUserById(userId);

            // Xunit.Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var userDto = Xunit.Assert.IsType<UserByIdDTO>(okResult.Value);
            Xunit.Assert.Equal(user.UserName, userDto.username);
            Xunit.Assert.Equal(user.Id, userDto.id);
            Xunit.Assert.Equal(user.Email, userDto.email);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            string userId = "nonexistent-user-id";

            // Act
            var result = await _controller.GetUserById(userId);

            // Xunit.Assert
            Xunit.Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SendMessage_ReturnsOk_WhenMessageSent()
        {
            // Arrange
            var request = new SendMessageRequestDto
            {
                SenderId = "sender-id",
                ReceiverId = "receiver-id",
                Content = "Test message content"
            };

            var sender = new ApplicationUser
            {
                Id = request.SenderId,
                UserName = "Sender",
                Email = "sender@example.com"
            };

            var receiver = new ApplicationUser
            {
                Id = request.ReceiverId,
                UserName = "Receiver",
                Email = "receiver@example.com"
            };

            // Add users to the in-memory database
            _dbContext.Users.Add(sender);
            _dbContext.Users.Add(receiver);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.SendMessage(request);

            // Xunit.Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.Equal("Üzenet sikeresen elküldve!", ((dynamic)okResult.Value).message);

            _mockEmail.Verify(e => e.SendMessageEmail(
                receiver.Email,
                sender.UserName,
                request.Content),
                Times.Once);
        }

        [Fact]
        public async Task UpdateUserData_ReturnsOk_WhenUpdateSuccessful()
        {
            // Arrange
            var updateDto = new UserDataUpdateDTO("UpdatedName", "updated@example.com", "user-id-1");

            var user = new ApplicationUser
            {
                Id = updateDto.id,
                UserName = "OldName",
                Email = "old@example.com",
                NormalizedEmail = "OLD@EXAMPLE.COM",
                NormalizedUserName = "OLDNAME"
            };

            // Add the user to the in-memory database
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.UpdateUserData(updateDto);

            // Xunit.Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.Equal("User data updated successfully", okResult.Value);

            // Refresh user from database and verify changes
            var updatedUser = await _dbContext.Users.FindAsync(updateDto.id);
            Xunit.Assert.Equal(updateDto.username, updatedUser.UserName);
            Xunit.Assert.Equal(updateDto.email, updatedUser.Email);
            Xunit.Assert.Equal(updateDto.email.ToUpper(), updatedUser.NormalizedEmail);
            Xunit.Assert.Equal(updateDto.username.ToUpper(), updatedUser.NormalizedUserName);
        }

        [Fact]
        public async Task UpdateUserData_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var updateDto = new UserDataUpdateDTO("UpdatedName", "updated@example.com", "nonexistent-user-id");

            // Act
            var result = await _controller.UpdateUserData(updateDto);

            // Xunit.Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result);
            Xunit.Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task UpdatePassword_ReturnsOk_WhenPasswordUpdated()
        {
            // Arrange
            var updateDto = new UserPasswordUpdateDTO
            {
                Id = "user-id-1",
                CurrentPassword = "OldPass123!",
                NewPassword = "NewPass123!"
            };

            var user = new ApplicationUser
            {
                Id = updateDto.Id,
                UserName = "TestUser",
                Email = "test@example.com"
            };

            // Add the user to the in-memory database
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            _mockAuth.Setup(a => a.UpdatePassword(updateDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateUserPassword(updateDto);

            // Xunit.Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.Equal("Jelszó sikeresen frissítve!", ((dynamic)okResult.Value).message);
        }

        [Fact]
        public async Task UpdatePassword_ReturnsBadRequest_WhenPasswordUpdateFails()
        {
            // Arrange
            var updateDto = new UserPasswordUpdateDTO
            {
                Id = "user-id-1",
                CurrentPassword = "WrongOldPass",
                NewPassword = "NewPass123!"
            };

            var user = new ApplicationUser
            {
                Id = updateDto.Id,
                UserName = "TestUser",
                Email = "test@example.com"
            };

            // Add the user to the in-memory database
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            _mockAuth.Setup(a => a.UpdatePassword(updateDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateUserPassword(updateDto);

            // Xunit.Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
            Xunit.Assert.Equal("Jelszó frissítése sikertelen!", ((dynamic)badRequestResult.Value).message);
        }
    }
}