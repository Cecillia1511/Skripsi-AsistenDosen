using LoginApp.Maui.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace LoginApp.Maui.Services;

public class JadwalService : IJadwalService
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public JadwalService(IConfiguration configuration)
    {
        var baseUrl = configuration["ApiUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl) || !Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
            throw new InvalidOperationException("ApiUrl is missing or invalid");

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        _client = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/")
        };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<JadwalOption>> GetMataKuliahAsync()
    {
        var response = await _client.GetAsync("MataKuliah/dropdown");
        var raw = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<JadwalOption>>(raw, _jsonOptions) ?? new();
    }

    public async Task<List<JadwalOption>> GetTahunAkademikAsync()
    {
        var response = await _client.GetAsync("TahunAkademik/dropdown");
        var raw = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<JadwalOption>>(raw, _jsonOptions) ?? new();
    }

    public async Task<List<JadwalOption>> GetDosenAsync()
    {
        var response = await _client.GetAsync("Dosen/dropdown");
        var raw = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<JadwalOption>>(raw, _jsonOptions) ?? new();
    }

    public async Task<List<JadwalOption>> GetHariAsync()
    {
        var response = await _client.GetAsync("Hari/dropdown");
        var raw = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<JadwalOption>>(raw, _jsonOptions) ?? new();
    }

    public async Task<List<JadwalOption>> GetWaktuAsync()
    {
        var response = await _client.GetAsync("Waktu/dropdown");
        var raw = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<JadwalOption>>(raw, _jsonOptions) ?? new();
    }

    public async Task<List<JadwalOption>> GetRuanganAsync()
    {
        var response = await _client.GetAsync("Ruangan/dropdown");
        var raw = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<JadwalOption>>(raw, _jsonOptions) ?? new();
    }

    public async Task<List<JadwalOption>> GetKelasAsync()
    {
        var response = await _client.GetAsync("Kelas/dropdown");
        var raw = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<JadwalOption>>(raw, _jsonOptions) ?? new();
    }

    public async Task<List<JadwalTampil>> GetJadwalByTahunAsync(int tahunId)
    {
        var response = await _client.GetAsync($"Jadwal/tahun/{tahunId}");
        var raw = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<JadwalTampil>>(raw, _jsonOptions) ?? new();
    }

    public async Task<bool> SimpanJadwalAsync(JadwalInputModel jadwal)
    {
        Console.WriteLine("Sending Jadwal: " + JsonSerializer.Serialize(jadwal));
        var response = await _client.PostAsJsonAsync("Jadwal/Input", jadwal);
        Console.WriteLine("Status Code: " + response.StatusCode);
        return response.IsSuccessStatusCode;
    }
}
