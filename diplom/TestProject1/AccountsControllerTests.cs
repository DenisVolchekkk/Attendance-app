using System.Net;
using System.Net.Http.Json;
using Domain.DTO;
using Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using StudyProj;
using System.IdentityModel.Tokens.Jwt;
using StudyProj.Controllers;
using System.Reflection;
using Assert = Xunit.Assert;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using StudyProj.JwtFeatures;
using EmailService;

namespace TestProject1
{
    public class AccountsControllerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<JwtHandler> _mockJwtHandler;
        private readonly Mock<IEmailSender> _mockEmailSender;
        private readonly AccountsController _controller;
        private readonly IConfiguration _configuration;

        public AccountsControllerTests()
        {
            // Mock UserManager
            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Mock JwtHandler configuration
            var inMemorySettings = new Dictionary<string, string> {
                {"JWTSettings:securityKey", "SecurityKey1235255423252544234434123413!!!"},
                {"JWTSettings:validIssuer", "StudyProj"},
                {"JWTSettings:validAudience", "https://localhost:7112"},
                {"JWTSettings:expiryInMinutes", "60"},
                {"EmailConfiguration:From", "test@test.com"},
                {"EmailConfiguration:SmtpServer", "smtp.test.com"},
                {"EmailConfiguration:Port", "465"},
                {"EmailConfiguration:UserName", "test@test.com"},
                {"EmailConfiguration:Password", "password"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _mockMapper = new Mock<IMapper>();
            _mockJwtHandler = new Mock<JwtHandler>(_configuration);
            _mockEmailSender = new Mock<IEmailSender>();

            _controller = new AccountsController(
                _mockUserManager.Object,
                _mockMapper.Object,
                _mockJwtHandler.Object,
                _mockEmailSender.Object);
        }


        [Fact]
        public async Task RegisterUser_WithNullModel_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.RegisterUser(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task RegisterUser_WithInvalidModel_ReturnsBadRequestWithErrors()
        {
            // Arrange
            var userDto = new UserForRegistrationDto
            {
                Email = "test@test.com",
                Password = "password",
                ClientUri = "https://localhost/confirm"
            };

            var user = new User { Email = userDto.Email };
            var identityErrors = new[]
            {
                new IdentityError { Description = "Error1" },
                new IdentityError { Description = "Error2" }
            };

            _mockMapper.Setup(x => x.Map<User>(userDto)).Returns(user);
            _mockUserManager.Setup(x => x.CreateAsync(user, userDto.Password))
                .ReturnsAsync(IdentityResult.Failed(identityErrors));

            // Act
            var result = await _controller.RegisterUser(userDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<RegistrationResponseDto>(badRequestResult.Value);
            Assert.Equal(2, response.Errors.Count());
        }

        [Fact]
        public async Task RegisterUser_WithValidModel_ReturnsCreatedAndSendsEmail()
        {
            // Arrange
            var userDto = new UserForRegistrationDto
            {
                Email = "test@test.com",
                Password = "password",
                ClientUri = "https://localhost/confirm"
            };

            var user = new User { Email = userDto.Email };
            var roles = new List<string> { "Chief" };
            var jwtHandler = new JwtHandler(_configuration);
            var token = jwtHandler.CreateToken(user, roles);
            _mockMapper.Setup(x => x.Map<User>(userDto)).Returns(user);
            _mockUserManager.Setup(x => x.CreateAsync(user, userDto.Password))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(user))
                .ReturnsAsync(token);
            _mockUserManager.Setup(x => x.AddToRoleAsync(user, "Chief"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.RegisterUser(userDto);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, ((StatusCodeResult)result).StatusCode);

            _mockUserManager.Verify(x => x.AddToRoleAsync(user, "Chief"), Times.Once);
            _mockEmailSender.Verify(x => x.SendEmailAsync(It.Is<Message>(m =>
                m.To.Any(t => t.Address == userDto.Email) &&
                m.Subject == "Token to confirm email" &&
                m.Content.Contains(token))), Times.Once);
        }

        [Fact]
        public async Task EmailConfirmation_WithInvalidUser_ReturnsBadRequest()
        {
            // Arrange
            var email = "nonexistent@test.com";
            var token = "token";

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.EmailConfirmation(email, token);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error email confirm", badRequestResult.Value);
        }

        [Fact]
        public async Task EmailConfirmation_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var email = "test@test.com";
            var token = "invalid-token";
            var user = new User { Email = email };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _controller.EmailConfirmation(email, token);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error email confirm", badRequestResult.Value);
        }

        [Fact]
        public async Task EmailConfirmation_WithValidData_ReturnsOk()
        {
            // Arrange
            var email = "test@test.com";
            var token = "valid-token";
            var user = new User { Email = email };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.EmailConfirmation(email, token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Now you can autorize", okResult.Value);
        }

        [Fact]
        public async Task EmailConfirmation_UnescapesToken()
        {
            // Arrange
            var email = "test@test.com";
            var escapedToken = "escaped%20token";
            var unescapedToken = "escaped token";
            var user = new User { Email = email };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, unescapedToken))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.EmailConfirmation(email, escapedToken);

            // Assert
            _mockUserManager.Verify(x => x.ConfirmEmailAsync(user, unescapedToken), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }
    [Fact]
        public async Task Authenticate_WithInvalidUser_ReturnsBadRequest()
        {
            // Arrange
            var authDto = new UserForAuthenticationDto { Email = "nonexistent@test.com", Password = "password" };
            _mockUserManager.Setup(x => x.FindByNameAsync(authDto.Email)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.Authenticate(authDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Authenticate_WithUnconfirmedEmail_ReturnsUnauthorized()
        {
            // Arrange
            var authDto = new UserForAuthenticationDto { Email = "test@test.com", Password = "password" };
            var user = new User { UserName = authDto.Email };

            _mockUserManager.Setup(x => x.FindByNameAsync(authDto.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(false);

            // Act
            var result = await _controller.Authenticate(authDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<AuthResponseDto>(unauthorizedResult.Value);
            Assert.Equal("Confirm email", response.ErrorMessage);
        }

        [Fact]
        public async Task Authenticate_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var authDto = new UserForAuthenticationDto { Email = "test@test.com", Password = "wrongpassword" };
            var user = new User { UserName = authDto.Email };

            _mockUserManager.Setup(x => x.FindByNameAsync(authDto.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, authDto.Password)).ReturnsAsync(false);

            // Act
            var result = await _controller.Authenticate(authDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<AuthResponseDto>(unauthorizedResult.Value);
            Assert.Equal("Autentication error", response.ErrorMessage);
        }

        [Fact]
        public async Task Authenticate_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var authDto = new UserForAuthenticationDto { Email = "test@test.com", Password = "Correctpassword123!" };
            var user = new User
            {
                UserName = authDto.Email,
                FirstName = "John",
                LastName = "Doe",
                FatherName = "Smith"
            };
            var roles = new List<string> { "Dean" };
            var jwtHandler = new JwtHandler(_configuration);
            var expectedToken = jwtHandler.CreateToken(user, roles);

            _mockUserManager.Setup(x => x.FindByNameAsync(authDto.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, authDto.Password)).ReturnsAsync(true);
            _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

            _mockJwtHandler.Setup(x => x.CreateToken(user, roles)).Returns(expectedToken);

            // Act
            var result = await _controller.Authenticate(authDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.True(response.IsAuthSuccessful);
            Assert.Equal(expectedToken, response.Token);
        }

    }
}