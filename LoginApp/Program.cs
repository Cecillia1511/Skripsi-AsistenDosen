using LoginApp.Api.Data;
using Microsoft.EntityFrameworkCore;
using LoginApp.Api.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🌐 Dengarkan semua IP di port 44356
builder.WebHost.UseUrls("https://0.0.0.0:44356");

// 📦 Tambahkan controller dan DB context
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AsistenDosen")
        ?? throw new InvalidOperationException("Connection 'AsistenDosen' is not found"));
});


// 🧪 Swagger untuk testing API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<JwtHelper>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"];
if (string.IsNullOrEmpty(secretKey))
    throw new InvalidOperationException("JWT secret key is missing in configuration");


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

var app = builder.Build();

// 🛠️ Konfigurasi pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi(); // opsional, kalau pakai NSwag
}

// 🔐 Redirect HTTP ke HTTPS
app.UseHttpsRedirection();

// 📡 Override Host agar cocok dengan IP lokal
app.Use(async (context, next) =>
{
    context.Request.Host = new HostString("192.168.1.8:44356");
    await next();
});

// 🔐 Authorization (kalau pakai [Authorize] nanti)
app.UseAuthentication();
app.UseAuthorization();


// 🚀 Aktifkan semua controller
app.MapControllers();

// 🏁 Jalankan aplikasi
app.Run();
