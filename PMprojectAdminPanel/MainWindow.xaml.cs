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
using System.Windows.Controls.Primitives;

namespace PMprojectAdminPanel
{
    public class LoginRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Role { get; set; }
    }


    public partial class MainWindow : Window
    {

        private string _jwtToken;

        public MainWindow()
        {
            InitializeComponent();
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
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7285/auth/login");

                    var json = JsonConvert.SerializeObject(loginRequest);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>(jsonResponse);

                        if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                        {
                            _jwtToken = loginResponse.Token;
                            var handler = new JwtSecurityTokenHandler();
                            var jwtToken = handler.ReadJwtToken(_jwtToken);

                            var roleClaim = jwtToken.Claims.FirstOrDefault(c =>
                                c.Type == "role" ||
                                c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

                            if (roleClaim != null && roleClaim.Value == "Admin")
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
                        else
                        {
                            MessageBox.Show("Hibás felhasználónév vagy jelszó.");
                        }
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Hiba történt: {errorResponse}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private async Task LoadPostsAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7285/Posttable/GetAllPostsWithComments");

                    // Beállítjuk az Authorization fejlécet a JWT tokennel
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

                    // A GetAsync hívásban nem szükséges ismételten megadni a teljes URL-t
                    var response = await client.GetAsync("Posttable/GetAllPostsWithComments");

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
                        MessageBox.Show("Hiba történt a posztok betöltésekor.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt: " + ex.Message);
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            _jwtToken = null;
            txtAdminName.Text = string.Empty;
            topPanel.Visibility = Visibility.Collapsed;
            mainGrid.Visibility = Visibility.Visible;
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }
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
