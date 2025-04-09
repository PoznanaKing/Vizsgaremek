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
using System.Diagnostics;
using System.Threading;

namespace PMprojectAdminPanel
{
    public partial class MainWindow : Window
    {
        public string _jwtToken;
        public HttpClient _httpClient;
        public string _currentUserId;

        public MainWindow()
        {
            // Biztosítjuk, hogy STA szálat használunk
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

            InitializeComponent();
            InitializeHttpClient();
        }

        public void InitializeHttpClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7285/")
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetAuthorizationHeader()
        {
            if (!string.IsNullOrEmpty(_jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _jwtToken);
            }
        }

        public async void btnLogin_Click(object sender, RoutedEventArgs e)
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
                        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                            ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");
                        if (userIdClaim != null)
                        {
                            _currentUserId = userIdClaim.Value;
                        }
                        else
                        {
                            MessageBox.Show("Nem található felhasználó azonosító a tokenben.");
                        }
                        if (roles.Contains("Admin"))
                        {
                            await Dispatcher.InvokeAsync(async () => {
                                txtLoggedInLabel.Text = $"Bejelentkezve: {loginRequest.Username}";
                                topPanel.Visibility = Visibility.Visible;
                                mainGrid.Visibility = Visibility.Collapsed;
                                navBar.Visibility = Visibility.Visible;
                            });

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

        public async Task LoadPostsAsync()
        {
            try
            {
                await Dispatcher.InvokeAsync(() => {
                    gymPanel.Visibility = Visibility.Collapsed;
                    usersPanel.Visibility = Visibility.Collapsed;
                    topPanel.Visibility = Visibility.Visible;
                });

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

                        await Dispatcher.InvokeAsync(() => {
                            postsListView.ItemsSource = posts;
                        });
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

        public async void DeletePost_Click(object sender, RoutedEventArgs e)
        {
            if (!await IsUserInRoleAsync("Admin"))
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

        public void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Token törlése, UI visszaállítása
            _jwtToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
            txtLoggedInLabel.Text = string.Empty;

            topPanel.Visibility = Visibility.Collapsed;
            gymPanel.Visibility = Visibility.Collapsed;
            usersPanel.Visibility = Visibility.Collapsed;
            mainGrid.Visibility = Visibility.Visible;
            navBar.Visibility = Visibility.Collapsed;

            postsListView.ItemsSource = null;
            gymsListView.ItemsSource = null;
            usersListView.ItemsSource = null;
        }

        public async Task<bool> IsUserInRoleAsync(string roleName)
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

        public async void NavigateToPosts(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(async () => {
                usersPanel.Visibility = Visibility.Collapsed;
                gymPanel.Visibility = Visibility.Collapsed;
                await LoadPostsAsync();
            });
        }

        public async void NavigateToGyms(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(async () => {
                usersPanel.Visibility = Visibility.Collapsed;
                topPanel.Visibility = Visibility.Collapsed;
                gymPanel.Visibility = Visibility.Visible;

                try
                {
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
            });
        }

        public async void DeleteGym_Click(object sender, RoutedEventArgs e)
        {
            if (!await IsUserInRoleAsync("Admin"))
            {
                MessageBox.Show("Nincs jogosultság!");
                return;
            }
            if (sender is Button button && button.Tag is PlaceDto gym)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"PlaceTable/DeletePost/{gym.placeId}");

                    if (response.IsSuccessStatusCode)
                    {
                        await Dispatcher.InvokeAsync(() => {
                            NavigateToGyms(null, null);
                        });
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

        public async void NavigateToUsers(object sender, RoutedEventArgs e)
        {
            await Dispatcher.InvokeAsync(async () => {
                topPanel.Visibility = Visibility.Collapsed;
                gymPanel.Visibility = Visibility.Collapsed;
                usersPanel.Visibility = Visibility.Visible;

                try
                {
                    var response = await _httpClient.GetAsync("auth/users");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var users = JsonConvert.DeserializeObject<List<UserDto>>(jsonResponse);

                        if (users != null)
                        {
                            usersListView.ItemsSource = users;
                        }
                        else
                        {
                            MessageBox.Show("Nincsenek megjeleníthető felhasználók.");
                        }
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Hiba történt a felhasználók betöltésekor: {errorResponse}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt: {ex.Message}");
                }
            });
        }

        public async void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (!await IsUserInRoleAsync("Admin"))
            {
                MessageBox.Show("Nincs jogosultság!");
                return;
            }

            if (sender is Button button && button.Tag is string userId)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"auth/users/{userId}");

                    if (response.IsSuccessStatusCode)
                    {
                        await Dispatcher.InvokeAsync(() => {
                            NavigateToUsers(null, null);
                        });
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
                MessageBox.Show("Érvénytelen felhasználói adatok!");
            }
        }

        public async void EditGym_Click(object sender, RoutedEventArgs e)
        {
            if (!await IsUserInRoleAsync("Admin"))
            {
                MessageBox.Show("Nincs jogosultság!");
                return;
            }

            if (sender is Button button && button.Tag is PlaceDto gym)
            {
                try
                {
                    var editWindow = new Window
                    {
                        Title = "Edzőterem módosítása",
                        Width = 400,
                        Height = 550,
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E2E2E")),
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        Owner = this
                    };

                    var grid = new Grid { Margin = new Thickness(20) };

                    for (int i = 0; i < 8; i++) // Módosítva 8-ra, hogy helyet hagyjunk a mentés gombnak
                    {
                        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    }

                    var nameTextBox = new TextBox { Text = gym.placename, Margin = new Thickness(0, 2, 0, 5) };
                    var postalCodeTextBox = new TextBox { Text = gym.postalcode.ToString(), Margin = new Thickness(0, 2, 0, 5) };
                    var townNameTextBox = new TextBox { Text = gym.townname, Margin = new Thickness(0, 2, 0, 5) };
                    var streetNameTextBox = new TextBox { Text = gym.streetname, Margin = new Thickness(0, 2, 0, 5) };
                    var storyLevelTextBox = new TextBox { Text = gym.storylevel?.ToString() ?? "", Margin = new Thickness(0, 2, 0, 5) };
                    var descriptionTextBox = new TextBox { Text = gym.description, TextWrapping = TextWrapping.Wrap, Height = 100, Margin = new Thickness(0, 2, 0, 5) };
                    var ratingTextBox = new TextBox { Text = gym.rating?.ToString() ?? "", Margin = new Thickness(0, 2, 0, 5) };

                    AddFormField(grid, 0, "Név:", nameTextBox);
                    AddFormField(grid, 1, "Irányítószám:", postalCodeTextBox);
                    AddFormField(grid, 2, "Város:", townNameTextBox);
                    AddFormField(grid, 3, "Utca:", streetNameTextBox);
                    AddFormField(grid, 4, "Emelet:", storyLevelTextBox);
                    AddFormField(grid, 5, "Leírás:", descriptionTextBox);
                    AddFormField(grid, 6, "Értékelés:", ratingTextBox);

                    var saveButton = new Button
                    {
                        Content = "Mentés",
                        Margin = new Thickness(0, 20, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Padding = new Thickness(10, 5, 10, 5)
                    };

                    saveButton.Click += async (s, args) =>
                    {
                        try
                        {
                            if (!int.TryParse(postalCodeTextBox.Text, out int postalCode))
                            {
                                MessageBox.Show("Az irányítószámnak számnak kell lennie!");
                                return;
                            }

                            int? storyLevel = null;
                            if (!string.IsNullOrEmpty(storyLevelTextBox.Text))
                            {
                                if (!int.TryParse(storyLevelTextBox.Text, out int sl))
                                {
                                    MessageBox.Show("Az emeletnek számnak kell lennie!");
                                    return;
                                }
                                storyLevel = sl;
                            }

                            double? rating = null;
                            if (!string.IsNullOrEmpty(ratingTextBox.Text))
                            {
                                if (!double.TryParse(ratingTextBox.Text, out double r))
                                {
                                    MessageBox.Show("Az értékelésnek számnak kell lennie!");
                                    return;
                                }
                                rating = r;
                            }

                            var updatedGym = new
                            {
                                placeId = gym.placeId,
                                placename = nameTextBox.Text,
                                postalcode = postalCode,
                                townname = townNameTextBox.Text,
                                streetname = streetNameTextBox.Text,
                                storylevel = storyLevel,
                                description = descriptionTextBox.Text,
                                rating = rating
                            };

                            var jsonContent = new StringContent(
                                JsonConvert.SerializeObject(updatedGym),
                                Encoding.UTF8,
                                "application/json");

                            var response = await _httpClient.PutAsync($"PlaceTable/EditPlaceData/{updatedGym.placeId}", jsonContent);

                            if (response.IsSuccessStatusCode)
                            {
                                editWindow.Close();
                                await Dispatcher.InvokeAsync(() => {
                                    NavigateToGyms(null, null);
                                });
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

        public async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (!await IsUserInRoleAsync("Admin"))
            {
                MessageBox.Show("Nincs jogosultság!");
                return;
            }

            if (sender is Button button && button.Tag is string receiverId)
            {
                var dialog = new Window
                {
                    Title = "Üzenet küldése",
                    Width = 300,
                    Height = 200,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E2E2E"))
                };

                var stackPanel = new StackPanel { Margin = new Thickness(10) };
                var textBox = new TextBox
                {
                    Margin = new Thickness(0, 0, 0, 10),
                    AcceptsReturn = true,
                    Height = 100,
                    Background = Brushes.White,
                    Foreground = Brushes.Black
                };
                var sendButton = new Button
                {
                    Content = "Küldés",
                    Style = (Style)FindResource(typeof(Button))
                };

                sendButton.Click += async (s, args) =>
                {
                    if (string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        MessageBox.Show("Üzenet szövege nem lehet üres!");
                        return;
                    }

                    var messageRequest = new
                    {
                        senderId = _currentUserId,
                        receiverId = receiverId,
                        content = textBox.Text
                    };

                    try
                    {
                        var json = JsonConvert.SerializeObject(messageRequest);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await _httpClient.PostAsync("auth/sendMessage", content);

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Üzenet sikeresen elküldve!");
                            dialog.Close();
                        }
                        else
                        {
                            var error = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Hiba történt: {error}");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hiba: {ex.Message}");
                    }
                };

                stackPanel.Children.Add(textBox);
                stackPanel.Children.Add(sendButton);
                dialog.Content = stackPanel;
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Érvénytelen felhasználói adatok!");
            }
        }

        public void AddFormField(Grid grid, int row, string label, TextBox textBox)
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

    public class LoginResponseDto
    {
        public string Token { get; set; }
    }

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

    public class PlaceDto
    {
        public int placeId { get; set; }
        public string placename { get; set; }
        public int postalcode { get; set; }
        public string townname { get; set; }
        public string streetname { get; set; }
        public int? storylevel { get; set; }
        public string description { get; set; }
        public double? rating { get; set; }
    }

    public class UserDto
    {
        public string userId { get; set; }
        public string username { get; set; }
        public string email { get; set; }
    }
}