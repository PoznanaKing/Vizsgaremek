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
        public async Task LoginPost_ReturnsNotFound_WhenLoginFails()
        {
            // Arrange
            var loginDto = new LoginRequestDto("invaliduser", "WrongPass");

            var failResult = new { result = "" };
            _mockAuth.Setup(a => a.Login(It.IsAny<LoginRequestDto>()))
                .ReturnsAsync(failResult);

            // Act
            var result = await _controller.LoginPost(loginDto);

            // Assert
            Xunit.Assert.IsType<NotFoundResult>(result);
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

            // Javítás: A null érték visszaadása nullable típusként
            _mockAuth.Setup(a => a.Register(It.IsAny<RegisterRequestDto>()))
                .ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _controller.RegisterPost(registerDto);

            // Assert
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

            // Assert
            var statusCodeResult = Xunit.Assert.IsType<ObjectResult>(result);
            Xunit.Assert.Equal(201, statusCodeResult.StatusCode);
            Xunit.Assert.Equal(user, statusCodeResult.Value);
        }

        [Fact]
        public async Task AssignRole_ReturnsBadRequest_WhenRoleAssignmentFails()
        {
            // Arrange
            var assignRoleDto = new AssignRoleRequestDto("nonexistentuser", "Admin");

            // Javítás: A null érték visszaadása nullable típusként
            _mockAuth.Setup(a => a.AssignRole(It.IsAny<AssignRoleRequestDto>()))
                .ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _controller.AssignRole(assignRoleDto);

            // Assert
            Xunit.Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            string userId = "nonexistent-user-id";

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            Xunit.Assert.IsType<NotFoundResult>(result);
        }

       
        [Fact]
        public async Task UpdateUserData_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var updateDto = new UserDataUpdateDTO("UpdatedName", "updated@example.com", "nonexistent-user-id");

            // Act
            var result = await _controller.UpdateUserData(updateDto);

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result);
            Xunit.Assert.Equal("User not found", notFoundResult.Value);
        }

       
    }
}