using LoginApp.Maui.Models;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows.Input;

namespace LoginApp.Maui.ViewsKoordinator;

public partial class AsistenDosen : ContentPage, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public List<string> TahunList { get; set; } = new List<string> { "GJ2526", "GN2425", "GJ2425" };

    private string _selectedTahunText = "Pilih Tahun";
    public string SelectedTahunText
    {
        get => _selectedTahunText;
        set
        {
            _selectedTahunText = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTahunText)));
        }
    }

    private bool _isDropdownVisible;
    public bool IsDropdownVisible
    {
        get => _isDropdownVisible;
        set
        {
            _isDropdownVisible = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDropdownVisible)));
        }
    }

    public AsistenDosen()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private void OnToggleDropdownClicked(object sender, EventArgs e)
    {
        IsDropdownVisible = !IsDropdownVisible;
    }

    private async void OnTahunSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedTahun = e.CurrentSelection.FirstOrDefault()?.ToString();
        if (!string.IsNullOrEmpty(selectedTahun))
        {
            SelectedTahunText = $"{selectedTahun}";
            IsDropdownVisible = false;
            await LoadAsisten(selectedTahun);
        }
    }

    private async Task LoadAsisten(string tahun)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        var client = new HttpClient(handler);
        var url = $"https://localhost:44356/api/Users/persemester?semester={tahun}";

        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = await response.Content.ReadFromJsonAsync<List<AsistenDosenViewModel>>(options);
            AsistenList.ItemsSource = data;
        }
    }

    public ICommand NavigateToDetailCommand => new Command<AsistenDosenViewModel>(async (asisten) =>
    {
        if (asisten == null) return;
        await Navigation.PushAsync(new DetailAsistenPage(asisten));
    });

}
