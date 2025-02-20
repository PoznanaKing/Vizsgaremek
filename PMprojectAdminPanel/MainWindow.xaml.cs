using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net.Http.Headers;

namespace PMprojectAdminPanel
{
    public partial class MainWindow : Window
    {
        private string _jwtToken;
        private HttpClient _httpClient;

        public MainWindow()
        {
            InitializeComponent();
            InitializeHttpClient();
        }

        // HttpClient inicializálása és alapbeállítások
        private void InitializeHttpClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7285/")
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Token beállítása minden kéréshez
        private void SetAuthorizationHeader()
        {
            if (!string.IsNullOrEmpty(_jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _jwtToken);
            }
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginRequest = new
            {
                Username = txtUsername.Text,
                Password = txtPassword.Password
            };

            try
            {
                var json = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>(jsonResponse);

                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        _jwtToken = loginResponse.Token;
                        SetAuthorizationHeader(); // Token beállítása

                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(_jwtToken);

                        // Szerepkörök ellenőrzése a tokenből
                        var roles = jwtToken.Claims
                            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                            .Select(c => c.Value)
                            .ToList();

                        if (roles.Contains("Admin"))
                        {
                            txtAdminName.Text = $"Bejelentkezve: {loginRequest.Username}";
                            topPanel.Visibility = Visibility.Visible;
                            mainGrid.Visibility = Visibility.Collapsed;
                            await LoadPostsAsync();
                        }
                        else
                        {
                            MessageBox.Show("Nincs admin jogosultságod.");
                        }
                    }
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba történt: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}");
            }
        }

        private async Task LoadPostsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Posttable/GetAllPostsWithComments");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var posts = JsonConvert.DeserializeObject<List<PostDto>>(jsonResponse);

                    if (posts != null && posts.Any())
                    {
                        postsListView.ItemsSource = posts;
                    }
                    else
                    {
                        MessageBox.Show("Nincsenek posztok.");
                    }
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba történt a posztok betöltésekor: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}");
            }
        }

        private async void DeletePost_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserInRole("Admin"))
            {
                MessageBox.Show("Nincs jogosultság!");
                return;
            }

            var button = sender as Button;
            if (button == null)
            {
                MessageBox.Show("Nem gombról érkezett kérés!");
                return;
            }

            var post = button.Tag as PostDto;
            if (post == null)
            {
                MessageBox.Show("Érvénytelen poszt adatok!");
                return;
            }

            // Create the DTO object for deletion
            var deletePostDTO = new { post_id = post.PostId };
            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(deletePostDTO),
                Encoding.UTF8,
                "application/json");

            // Use POST method instead of DELETE since you're sending a body
            var response = await _httpClient.PostAsync("Posttable/DeletePost", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                await LoadPostsAsync();
            }
            else
            {
                MessageBox.Show($"Hiba történt: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
        }





        private bool IsUserInRole(string roleName)
        {
            if (string.IsNullOrEmpty(_jwtToken))
            {
                MessageBox.Show("Nincs érvényes token. Jelentkezz be újra!");
                return false;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(_jwtToken);

                // Kinyerjük a szerepköröket a tokenből
                var roles = jwtToken.Claims
                    .Where(claim => claim.Type == ClaimTypes.Role || claim.Type == "role")
                    .Select(claim => claim.Value)
                    .ToList();

                // Ellenőrizzük, hogy a felhasználó rendelkezik-e a megadott szerepkörrel
                return roles.Contains(roleName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a token feldolgozása során: {ex.Message}");
                return false;
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            _jwtToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
            txtAdminName.Text = string.Empty;
            topPanel.Visibility = Visibility.Collapsed;
            mainGrid.Visibility = Visibility.Visible;
        }
    }

    // DTO osztályok
    public class LoginResponseDto
    {
        public string Token { get; set; }
    }

    public class PostDto
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostDescription { get; set; }
        public List<CommentDto> PostComments { get; set; }
    }

    public class CommentDto
    {
        public string CommentText { get; set; }
        public string UserName { get; set; }
    }
}