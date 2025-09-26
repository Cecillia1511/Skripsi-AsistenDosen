using LoginApp.Maui.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
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
            // ⚠️ Development only, bypass SSL cert validation
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

    // 🔧 helper method biar nggak copy-paste
    private async Task<List<JadwalOption>> GetOptionsAsync(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);
        var raw = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"[GET {endpoint}] Status: {response.StatusCode}");
        Console.WriteLine($"[GET {endpoint}] Body: {raw}");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Request gagal: {response.StatusCode}, body: {raw}");

        if (string.IsNullOrWhiteSpace(raw))
            return new();

        return JsonSerializer.Deserialize<List<JadwalOption>>(raw, _jsonOptions) ?? new();
    }

    public Task<List<JadwalOption>> GetMataKuliahAsync() => GetOptionsAsync("MataKuliah/dropdown");
    public Task<List<JadwalOption>> GetTahunAkademikAsync() => GetOptionsAsync("TahunAkademik/dropdown");
    public Task<List<JadwalOption>> GetDosenAsync() => GetOptionsAsync("Dosen/dropdown");
    public Task<List<JadwalOption>> GetHariAsync() => GetOptionsAsync("Hari/dropdown");
    public Task<List<JadwalOption>> GetWaktuAsync() => GetOptionsAsync("Waktu/dropdown");
    public Task<List<JadwalOption>> GetRuanganAsync() => GetOptionsAsync("Ruangan/dropdown");
    public Task<List<JadwalOption>> GetKelasAsync() => GetOptionsAsync("Kelas/dropdown");

    public async Task<List<JadwalTampil>> GetJadwalByTahunAsync(int tahunId)
    {
        var endpoint = $"Jadwal/tahun/{tahunId}";
        var response = await _client.GetAsync(endpoint);
        var raw = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"[GET {endpoint}] Status: {response.StatusCode}");
        Console.WriteLine($"[GET {endpoint}] Body: {raw}");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Request gagal: {response.StatusCode}, body: {raw}");

        if (string.IsNullOrWhiteSpace(raw))
            return new();

        return JsonSerializer.Deserialize<List<JadwalTampil>>(raw, _jsonOptions) ?? new();
    }

    public async Task<(bool Success, string Message)> SimpanJadwalAsync(JadwalInputModel jadwal)
    {
        var endpoint = "Jadwal/Input";
        Console.WriteLine($"[POST {endpoint}] Sending: {JsonSerializer.Serialize(jadwal)}");

        var response = await _client.PostAsJsonAsync(endpoint, jadwal);
        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"[POST {endpoint}] Status: {response.StatusCode}");
        Console.WriteLine($"[POST {endpoint}] Body: {content}");

        return (response.IsSuccessStatusCode, content);
    }
}
