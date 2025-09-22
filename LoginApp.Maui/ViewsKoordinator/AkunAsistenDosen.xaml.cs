using LoginApp.Maui.Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoginApp.Maui.ViewsKoordinator
{
    public partial class AkunAsistenDosen : ContentPage
    {
        public List<string> SemesterList { get; set; } = new List<string>
        {
            "GJ2526", "GN2425", "GJ2425"
        };

        private string _selectedSemesterText = "Pilih Semester";
        public string SelectedSemesterText
        {
            get => _selectedSemesterText;
            set
            {
                if (_selectedSemesterText != value)
                {
                    _selectedSemesterText = value;
                    OnPropertyChanged(nameof(SelectedSemesterText));
                }
            }
        }

        private bool _isDropdownVisible;
        public bool IsDropdownVisible
        {
            get => _isDropdownVisible;
            set
            {
                if (_isDropdownVisible != value)
                {
                    _isDropdownVisible = value;
                    OnPropertyChanged(nameof(IsDropdownVisible));
                }
            }
        }

        public AkunAsistenDosen()
        {
            InitializeComponent();
            BindingContext = this; // ✅ biar binding ke property di atas jalan
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadAsistenBySemester("GJ2526"); // Default semester saat halaman muncul
            SelectedSemesterText = "GJ2526";      // Teks tombol juga ikut berubah
        }

        private async Task LoadAsistenBySemester(string semester)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using var client = new HttpClient(handler);
                var url = $"https://localhost:44356/api/Users/persemester?semester={semester}";

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var data = await response.Content.ReadFromJsonAsync<List<User>>(options);

                    AsistenList.ItemsSource = data;

                    Console.WriteLine("✅ Jumlah data: " + data?.Count);
                }
                else
                {
                    await DisplayAlert("Error", "Gagal mengambil data dari server", "OK");
                    Console.WriteLine("❌ Status Code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.Message, "OK");
                Console.WriteLine("🔥 Exception: " + ex.Message);
            }
        }

        private async void OnSemesterSelected(object sender, SelectionChangedEventArgs e)
        {
            var selectedSemester = e.CurrentSelection.FirstOrDefault()?.ToString();
            if (!string.IsNullOrEmpty(selectedSemester))
            {
                SelectedSemesterText = selectedSemester; // ubah teks tombol
                IsDropdownVisible = false;               // sembunyikan dropdown
                await LoadAsistenBySemester(selectedSemester);
            }
        }

        private void OnToggleDropdownClicked(object sender, EventArgs e)
        {
            IsDropdownVisible = !IsDropdownVisible;
        }

        private async void OnTambahAkunClicked(object sender, EventArgs e)
        {
            var username = UsernameEntry.Text?.Trim();
            var name = NameEntry.Text?.Trim();
            var password = PasswordEntry.Text?.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Semua field harus diisi.", "OK");
                return;
            }

            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using var client = new HttpClient(handler);
                var url = $"https://localhost:44356/api/Users/register";

                var newUser = new User
                {
                    Username = username,
                    Name = name,
                    Password = password,
                    Role = "AsistenDosen"            // biar fix asisten dosen
                };

                var response = await client.PostAsJsonAsync(url, newUser);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Sukses", "Akun asisten berhasil ditambahkan.", "OK");

                    // refresh daftar asisten sesuai semester aktif
                    await LoadAsistenBySemester(SelectedSemesterText);

                    // reset field input
                    UsernameEntry.Text = string.Empty;
                    NameEntry.Text = string.Empty;
                    PasswordEntry.Text = string.Empty;
                }
                else
                {
                    await DisplayAlert("Error", "Gagal menambahkan akun asisten.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.Message, "OK");
            }
        }

    }
}
