using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Globalization;
using System.IO;
using System.Windows.Data;

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
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Kérem töltse ki mindkét mezőt!");
                return;
            }

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
                        SetAuthorizationHeader();

                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(_jwtToken);

                        var roles = jwtToken.Claims
                            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                            .Select(c => c.Value)
                            .ToList();

                        if (roles.Contains("Admin"))
                        {
                            txtLoggedInLabel.Text = $"Bejelentkezve: {loginRequest.Username}";
                            topPanel.Visibility = Visibility.Visible;
                            mainGrid.Visibility = Visibility.Collapsed;
                            navBar.Visibility = Visibility.Visible;

                            // Alapértelmezetten a posztokat töltjük be
                            await LoadPostsAsync();
                        }
                        else
                        {
                            MessageBox.Show("Nincs admin jogosultságod.");
                            _jwtToken = null;
                        }
                    }
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        MessageBox.Show("Hibás felhasználónév vagy jelszó!");
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Hiba történt: {errorResponse}");
                    }
                }
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Nem sikerült kapcsolódni a szerverhez. Ellenőrizze az internetkapcsolatot.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Váratlan hiba történt: {ex.Message}");
            }
        }

        private async Task LoadPostsAsync()
        {
            try
            {
                // Edzőtermek panel elrejtése
                gymPanel.Visibility = Visibility.Collapsed;
                // Posztok panel megjelenítése
                topPanel.Visibility = Visibility.Visible;

                var response = await _httpClient.GetAsync("Posttable/GetAllPostsWithComments");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var posts = JsonConvert.DeserializeObject<List<PostDto>>(jsonResponse);

                    if (posts != null)
                    {
                        // Felhasználónév kiegészítése
                        foreach (var post in posts)
                        {
                            var userResponse = await _httpClient.GetAsync($"User/GetUsernameById/{post.UserId}");
                            if (userResponse.IsSuccessStatusCode)
                            {
                                var username = await userResponse.Content.ReadAsStringAsync();
                                post.UploaderUsername = $"Feltöltő: {username}";
                            }
                        }

                        postsListView.ItemsSource = posts;
                    }
                    else
                    {
                        MessageBox.Show("Nincsenek megjeleníthető posztok.");
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

            if (sender is Button button && button.Tag is PostDto post)
            {
                try
                {
                    var deletePostDTO = new { post_id = post.PostId };
                    var jsonContent = new StringContent(
                        JsonConvert.SerializeObject(deletePostDTO),
                        Encoding.UTF8,
                        "application/json");

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
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a törlés során: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Érvénytelen poszt adatok!");
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Token törlése, UI visszaállítása
            _jwtToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
            txtLoggedInLabel.Text = string.Empty;

            topPanel.Visibility = Visibility.Collapsed;
            gymPanel.Visibility = Visibility.Collapsed;
            mainGrid.Visibility = Visibility.Visible;
            navBar.Visibility = Visibility.Collapsed;

            postsListView.ItemsSource = null;
            gymsListView.ItemsSource = null;
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

                var roles = jwtToken.Claims
                    .Where(claim => claim.Type == ClaimTypes.Role || claim.Type == "role")
                    .Select(claim => claim.Value)
                    .ToList();

                return roles.Contains(roleName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a token feldolgozása során: {ex.Message}");
                return false;
            }
        }

        private async void NavigateToPosts(object sender, RoutedEventArgs e)
        {
            await LoadPostsAsync();
        }

        private async void NavigateToGyms(object sender, RoutedEventArgs e)
        {
            try
            {
                topPanel.Visibility = Visibility.Collapsed;
                gymPanel.Visibility = Visibility.Visible;

                var response = await _httpClient.GetAsync("PlaceTable/GetAllPlaces");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var gyms = JsonConvert.DeserializeObject<List<PlaceDto>>(jsonResponse);

                    if (gyms != null)
                    {
                        gymsListView.ItemsSource = gyms;
                    }
                    else
                    {
                        MessageBox.Show("Nincsenek megjeleníthető edzőtermek.");
                    }
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Hiba történt az edzőtermek betöltésekor: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}");
            }
        }

        private async void DeleteGym_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserInRole("Admin"))
            {
                MessageBox.Show("Nincs jogosultság!");
                return;
            }
            if (sender is Button button && button.Tag is PlaceDto gym)
            {
                try
                {
                   
                    var response = await _httpClient.DeleteAsync($"PlaceTable/DeletePost/{gym.placeid}");

                    

                    if (response.IsSuccessStatusCode)
                    {
                        NavigateToGyms(null, null);
                    }
                    else
                    {
                        MessageBox.Show($"Hiba történt: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a törlés során: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Érvénytelen edzőterem adatok!");
            }
        }


        private async void EditGym_Click(object sender, RoutedEventArgs e)
        {
            if (!IsUserInRole("Admin"))
            {
                MessageBox.Show("Nincs jogosultság!");
                return;
            }

            // A gomb Tag property-jében kapjuk meg a PlaceDto-t
            if (sender is Button button && button.Tag is PlaceDto gym)
            {
                try
                {
                    // Létrehozunk egy új ablakot, ahol a mezőket be lehet állítani
                    var editWindow = new Window
                    {
                        Title = "Edzőterem módosítása",
                        Width = 400,
                        Height = 500,
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E2E2E"))
                    };

                    var grid = new Grid { Margin = new Thickness(20) };

                    // 7 sor a mezőknek, +1 sor a gombnak
                    for (int i = 0; i < 7; i++)
                    {
                        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    }

                    // Létrehozzuk a mezőket
                    var nameTextBox = new TextBox { Text = gym.placename };
                    var postalCodeTextBox = new TextBox { Text = gym.postalcode.ToString() };
                    var townNameTextBox = new TextBox { Text = gym.townname };
                    var streetNameTextBox = new TextBox { Text = gym.streetname };
                    var storyLevelTextBox = new TextBox { Text = gym.storylevel?.ToString() ?? "" };
                    var descriptionTextBox = new TextBox { Text = gym.description, TextWrapping = TextWrapping.Wrap, Height = 100 };
                    var ratingTextBox = new TextBox { Text = gym.rating?.ToString() ?? "" };

                    // Felcímkézzük a mezőket
                    AddFormField(grid, 0, "Név:", nameTextBox);
                    AddFormField(grid, 1, "Irányítószám:", postalCodeTextBox);
                    AddFormField(grid, 2, "Város:", townNameTextBox);
                    AddFormField(grid, 3, "Utca:", streetNameTextBox);
                    AddFormField(grid, 4, "Emelet:", storyLevelTextBox);
                    AddFormField(grid, 5, "Leírás:", descriptionTextBox);
                    AddFormField(grid, 6, "Értékelés:", ratingTextBox);

                    // Hozzáadjuk a Mentés gombot
                    var saveButton = new Button
                    {
                        Content = "Mentés",
                        Margin = new Thickness(0, 20, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    // Mentés gomb eseménykezelő
                    saveButton.Click += async (s, args) =>
                    {
                        try
                        {
                            var updatedGym = new
                            {
                                placeid = gym.placeid,
                                placename = nameTextBox.Text,
                                postalcode = int.Parse(postalCodeTextBox.Text),
                                townname = townNameTextBox.Text,
                                streetname = streetNameTextBox.Text,
                                storylevel = string.IsNullOrEmpty(storyLevelTextBox.Text) ? (int?)null : int.Parse(storyLevelTextBox.Text),
                                description = descriptionTextBox.Text,
                                rating = string.IsNullOrEmpty(ratingTextBox.Text) ? (double?)null : double.Parse(ratingTextBox.Text)
                            };

                            var jsonContent = new StringContent(
                                JsonConvert.SerializeObject(updatedGym),
                                Encoding.UTF8,
                                "application/json");

                            // Kérés elküldése a szerver felé
                            // In the saveButton.Click handler, modify the PUT request:
                            var response = await _httpClient.PutAsync($"PlaceTable/EditPlaceData/{updatedGym.placeid}", jsonContent);

                            if (response.IsSuccessStatusCode)
                            {
                                editWindow.Close();
                                // Siker után frissítjük az edzőtermek listáját
                                NavigateToGyms(null, null);
                            }
                            else
                            {
                                var err = await response.Content.ReadAsStringAsync();
                                MessageBox.Show($"Hiba történt: {response.StatusCode} - {err}");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Hiba a módosítás során: {ex.Message}");
                        }
                    };

                    // A gombnak is létrehozunk egy sort
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    Grid.SetRow(saveButton, 7);
                    grid.Children.Add(saveButton);

                    editWindow.Content = grid;
                    editWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Segédfüggvény: címke + beviteli mező (TextBox) egy sorba illesztése a Gridben.
        /// </summary>
        private void AddFormField(Grid grid, int row, string label, TextBox textBox)
        {
            var stackPanel = new StackPanel { Margin = new Thickness(0, 5, 0, 5) };

            var labelTextBlock = new TextBlock
            {
                Text = label,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 5)
            };

            stackPanel.Children.Add(labelTextBlock);
            stackPanel.Children.Add(textBox);

            Grid.SetRow(stackPanel, row);
            grid.Children.Add(stackPanel);
        }
    }

    /// <summary>
    /// Segédosztály a Base64-es kép dekódolásához.
    /// </summary>
    public class Base64ToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            try
            {
                byte[] imageBytes = System.Convert.FromBase64String(value.ToString());
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                }
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// DTO a bejelentkezés válaszához.
    /// </summary>
    public class LoginResponseDto
    {
        public string Token { get; set; }
    }

    /// <summary>
    /// DTO a posztokhoz.
    /// </summary>
    public class PostDto
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostDescription { get; set; }
        public string PostImage { get; set; }
        public string UserId { get; set; }
        public string UploaderUsername { get; set; }
        public List<CommentDto> PostComments { get; set; }
    }

    public class CommentDto
    {
        public string CommentText { get; set; }
        public string UserName { get; set; }
    }

    /// <summary>
    /// DTO az edzőtermekhez (Place).
    /// </summary>
    public class PlaceDto
    {
        public int placeid { get; set; }
        public string placename { get; set; }
        public int postalcode { get; set; }
        public string townname { get; set; }
        public string streetname { get; set; }
        public int? storylevel { get; set; }
        public string description { get; set; }
        public double? rating { get; set; }
    }
}
