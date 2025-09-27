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

    private async Task<List<JadwalOption>> GetOptionsAsync(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);
        var raw = await response.Content.ReadAsStringAsync();

        System.Diagnostics.Debug.WriteLine($"[GET {endpoint}] Status: {response.StatusCode}");
        System.Diagnostics.Debug.WriteLine($"[GET {endpoint}] Body: {raw}");

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

    // 🔹 FIXED: GetJadwalByTahunAsync dengan mapping manual
    public async Task<List<JadwalTampil>> GetJadwalByTahunAsync(string tahunId)
    {
        var endpoint = $"Jadwal/tahun/{tahunId}";
        var response = await _client.GetAsync(endpoint);
        var raw = await response.Content.ReadAsStringAsync();

        // Log raw JSON
        System.Diagnostics.Debug.WriteLine($"[DEBUG] Full JSON Response for {endpoint}: {raw}");
        System.Diagnostics.Debug.WriteLine($"[GET {endpoint}] Status: {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Request gagal: {response.StatusCode}, body: {raw}");

        if (string.IsNullOrWhiteSpace(raw) || raw == "[]")
        {
            System.Diagnostics.Debug.WriteLine("[SERVICE] Raw JSON is empty or []. Returning empty list.");
            return new();
        }

        var result = new List<JadwalTampil>();

        try
        {
            using var document = JsonDocument.Parse(raw);
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Array)
            {
                System.Diagnostics.Debug.WriteLine("[SERVICE] JSON root is not array. Returning empty.");
                return new();
            }

            int count = 0;
            foreach (var jadwalElement in root.EnumerateArray())
            {
                // Extract exact field names dari JSON raw (match log kamu)
                string mataKuliah = GetJsonString(jadwalElement, new[] { "mataKuliah", "mata_kuliah", "NamaMK", "namaMK" });
                string dosen = GetJsonString(jadwalElement, new[] { "dosen", "Dosen", "namaDosen", "NamaDosen" });
                string hari = GetJsonString(jadwalElement, new[] { "hari", "Hari", "namaHari", "NamaHari" });
                string jamMulai = GetJsonString(jadwalElement, new[] { "jamMulai", "jam_mulai", "WaktuMulai", "waktuMulai", "start_time" });
                string jamSelesai = GetJsonString(jadwalElement, new[] { "jamSelesai", "jam_selesai", "WaktuSelesai", "waktuSelesai", "end_time" });
                string sksStr = GetJsonString(jadwalElement, new[] { "sks", "SKS", "sks_mk" });

                int sks = 0;
                if (!string.IsNullOrWhiteSpace(sksStr))
                {
                    if (int.TryParse(sksStr.Trim(), out int parsedSks))
                        sks = parsedSks;
                }

                string waktuMulai = FormatTime(jamMulai);  // "09:30"
                string waktuSelesai = FormatTime(jamSelesai);  // "11:10"

                // Missing: KodeMK & NIP (gak ada di JSON – butuh backend update)
                string kodeMK = GetJsonString(jadwalElement, new[] { "KodeMK", "kodeMK", "kode_mk" });  // Masih empty
                string nip = GetJsonString(jadwalElement, new[] { "NIP", "nip", "nip_dosen" });  // Masih empty

                // Optional: Extract ruangan & kelas (tambah ke model kalau mau)
                string ruangan = GetJsonString(jadwalElement, new[] { "ruangan", "Ruangan", "namaRuangan" });
                string kelas = GetJsonString(jadwalElement, new[] { "kelas", "Kelas", "namaKelas" });

                // Log extracted untuk debug
                System.Diagnostics.Debug.WriteLine($"[SERVICE DYNAMIC Item {++count}] NamaMK='{mataKuliah}', NamaDosen='{dosen}', Hari='{hari}', WaktuMulai='{waktuMulai}', WaktuSelesai='{waktuSelesai}', SKS={sks}, KodeMK='{kodeMK}', NIP='{nip}', Ruangan='{ruangan}', Kelas='{kelas}'");

                result.Add(new JadwalTampil
                {
                    KodeMK = kodeMK,
                    NamaMK = mataKuliah,
                    SKS = sks,
                    Kelas = kelas,
                    NIP = nip,
                    NamaDosen = dosen,
                    Hari = hari,
                    WaktuMulai = waktuMulai,
                    WaktuSelesai = waktuSelesai,
                    Ruangan = ruangan
                });
            }

            System.Diagnostics.Debug.WriteLine($"[SERVICE] Dynamic parsing success. Processed {count} items.");
        }
        catch (JsonException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SERVICE] JSON parsing error: {ex.Message}");
            return new();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SERVICE] Unexpected error: {ex.Message}");
            return new();
        }

        return result;
    }

    // 🔹 Helper: Format waktu (optional, hilangin detik)
    private string FormatTime(string timeStr)
    {
        if (string.IsNullOrWhiteSpace(timeStr) || timeStr.Length < 5)
            return timeStr ?? "";

        // Asumsi format "HH:mm:ss" → "HH:mm"
        if (timeStr.Contains(':') && timeStr.Length == 8)
        {
            return timeStr.Substring(0, 5);  // "09:30:00" → "09:30"
        }
        return timeStr;
    }

    // 🔹 Updated GetJsonString (sama, tapi handle lebih)
    private string GetJsonString(JsonElement element, string[] possibleNames)
    {
        foreach (var name in possibleNames)
        {
            if (element.TryGetProperty(name, out var prop))
            {
                if (prop.ValueKind == JsonValueKind.String)
                    return prop.GetString()?.Trim() ?? "";  // Trim spaces otomatis
                if (prop.ValueKind == JsonValueKind.Number)
                    return prop.GetInt32().ToString();
            }
        }
        return "";
    }

    // GetJsonInt gak dipakai lagi (SKS sekarang dari string)




    public async Task<(bool Success, string Message)> SimpanJadwalAsync(JadwalInputModel jadwal)
    {
        var endpoint = "Jadwal/Input";
        System.Diagnostics.Debug.WriteLine($"[POST {endpoint}] Sending: {JsonSerializer.Serialize(jadwal)}");

        var response = await _client.PostAsJsonAsync(endpoint, jadwal);
        var content = await response.Content.ReadAsStringAsync();

        System.Diagnostics.Debug.WriteLine($"[POST {endpoint}] Status: {response.StatusCode}");
        System.Diagnostics.Debug.WriteLine($"[POST {endpoint}] Body: {content}");

        return (response.IsSuccessStatusCode, content);
    }
}
