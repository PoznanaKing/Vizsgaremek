using NUnit.Framework;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;

namespace PMprojectAdminPanel.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private MainWindow _mainWindow;
        private Mock<HttpMessageHandler> _mockMessageHandler;
        private HttpClient _mockHttpClient;
        private Dispatcher _dispatcher;
        private Thread _dispatcherThread;

        [SetUp]
        public void Setup()
        {
            // Create and start dispatcher thread
            var dispatcherReadyEvent = new ManualResetEvent(false);
            _dispatcherThread = new Thread(() =>
            {
                _dispatcher = Dispatcher.CurrentDispatcher;
                dispatcherReadyEvent.Set();
                Dispatcher.Run();
            });
            _dispatcherThread.SetApartmentState(ApartmentState.STA);
            _dispatcherThread.IsBackground = true;
            _dispatcherThread.Start();
            dispatcherReadyEvent.WaitOne();

            // Initialize components on dispatcher thread
            _dispatcher.Invoke(() =>
            {
                _mainWindow = new MainWindow();
                _mockMessageHandler = new Mock<HttpMessageHandler>();
                _mockHttpClient = new HttpClient(_mockMessageHandler.Object)
                {
                    BaseAddress = new System.Uri("https://localhost:7285/")
                };
                _mainWindow._httpClient = _mockHttpClient;
                _mainWindow.InitializeComponent();
            });
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up on dispatcher thread
            _dispatcher.Invoke(() =>
            {
                _mainWindow?.Close();
                _mockHttpClient?.Dispose();
            });

            // Shutdown dispatcher
            _dispatcher.InvokeShutdown();
            _dispatcherThread.Join(1000);
        }

        [Test]
        public async Task Login_WithValidAdminCredentials_SetsTokenAndAuthorization()
        {
            // Arrange
            var token = new JwtSecurityToken(
                new JwtHeader(),
                new JwtPayload(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "123"),
                    new Claim(ClaimTypes.Role, "Admin")
                }));

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new LoginResponseDto
                {
                    Token = tokenString
                }))
            };

            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m =>
                        m.Method == HttpMethod.Post &&
                        m.RequestUri.ToString().EndsWith("auth/login")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            await _dispatcher.InvokeAsync(() =>
            {
                _mainWindow.txtUsername.Text = "Admin";
                _mainWindow.txtPassword.Password = "Admin123!";
            });

            // Act
            await _dispatcher.InvokeAsync(() => _mainWindow.btnLogin_Click(null, null));
            await Task.Delay(300); // Short delay for async operations

            // Assert
            Assert.That(_mainWindow._jwtToken, Is.EqualTo(tokenString));
            Assert.That(_mainWindow._currentUserId, Is.EqualTo("123"));
        }

        [Test]
        public async Task Login_WithInvalidCredentials_ShowsErrorMessage()
        {
            // Arrange
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized
            };

            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            await _dispatcher.InvokeAsync(() =>
            {
                _mainWindow.txtUsername.Text = "InvalidUser";
                _mainWindow.txtPassword.Password = "WrongPassword";
            });

            // Act
            await _dispatcher.InvokeAsync(() => _mainWindow.btnLogin_Click(null, null));
            await Task.Delay(300);

            // Assert
            Assert.That(_mainWindow._jwtToken, Is.Null);
        }

        

        [Test]
        public async Task IsUserInRoleAsync_WithAdminToken_ReturnsTrue()
        {
            // Arrange
            var token = new JwtSecurityToken(
                new JwtHeader(),
                new JwtPayload(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "123"),
                    new Claim(ClaimTypes.Role, "Admin")
                }));

            _mainWindow._jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Act
            var result = await _mainWindow.IsUserInRoleAsync("Admin");

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsUserInRoleAsync_WithNonAdminToken_ReturnsFalse()
        {
            // Arrange
            var token = new JwtSecurityToken(
                new JwtHeader(),
                new JwtPayload(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "456"),
                    new Claim(ClaimTypes.Role, "User")
                }));

            _mainWindow._jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Act
            var result = await _mainWindow.IsUserInRoleAsync("Admin");

            // Assert
            Assert.That(result, Is.False);
        }

        

        [Test]
        public async Task NavigateToGyms_SuccessfulRequest_UpdatesGymsListView()
        {
            // Arrange
            var gyms = new List<PlaceDto>
            {
                new PlaceDto { placeId = 1, placename = "Test Gym" }
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(gyms))
            };

            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m =>
                        m.Method == HttpMethod.Get &&
                        m.RequestUri.ToString().EndsWith("PlaceTable/GetAllPlaces")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            await _dispatcher.InvokeAsync(() => _mainWindow.NavigateToGyms(null, null));
            await Task.Delay(300);

            // Assert
            var itemsSource = await _dispatcher.InvokeAsync(() => _mainWindow.gymsListView.ItemsSource);
            Assert.That(itemsSource, Is.Not.Null);
        }

        [Test]
        public async Task NavigateToUsers_SuccessfulRequest_UpdatesUsersListView()
        {
            // Arrange
            var users = new List<UserDto>
            {
                new UserDto { userId = "1", username = "TestUser" }
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(users))
            };

            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m =>
                        m.Method == HttpMethod.Get &&
                        m.RequestUri.ToString().EndsWith("auth/users")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            await _dispatcher.InvokeAsync(() => _mainWindow.NavigateToUsers(null, null));
            await Task.Delay(300);

            // Assert
            var itemsSource = await _dispatcher.InvokeAsync(() => _mainWindow.usersListView.ItemsSource);
            Assert.That(itemsSource, Is.Not.Null);
        }

        

        

        [Test]
        public async Task Logout_Click_ClearsTokenAndResetsUI()
        {
            // Arrange
            await _dispatcher.InvokeAsync(() =>
            {
                _mainWindow._jwtToken = "test_token";
                _mainWindow._currentUserId = "123";
                _mainWindow._httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", "test_token");
            });

            // Act
            await _dispatcher.InvokeAsync(() => _mainWindow.btnLogout_Click(null, null));
            await Task.Delay(300);

            // Assert
            Assert.That(_mainWindow._jwtToken, Is.Null);
            Assert.That(_mainWindow._httpClient.DefaultRequestHeaders.Authorization, Is.Null);
        }

        
    }
}