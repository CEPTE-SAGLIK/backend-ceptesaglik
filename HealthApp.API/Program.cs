using HealthApp.DataAccess.Context;
using HealthApp.DataAccess.Repositories;
using HealthApp.Business.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HealthApp.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(OptionsBuilderConfigurationExtensions =>
    OptionsBuilderConfigurationExtensions.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Gerekli Servislerin Eklenmesi (AutoMapper ve HttpContextAccessor)
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<HealthApp.Business.Mappings.MappingProfile>());
builder.Services.AddHttpContextAccessor();

// ----- BİZİM EKLEDİĞİMİZ KISIM BURASI -----
// REPOSITORIES
builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<PersonRepository>();
builder.Services.AddScoped<IChildRepository, ChildRepository>();
builder.Services.AddScoped<VaccineRepository>();
builder.Services.AddScoped<VaccineScheduleRepository>();
builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ChildRepository>();
builder.Services.AddScoped<AllergyRepository>();
builder.Services.AddScoped<IllnessRepository>();
builder.Services.AddScoped<NotificationRepository>();
builder.Services.AddScoped<MedicineRepository>();

// SERVICES
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<ChildService>();
builder.Services.AddScoped<VaccineService>();
builder.Services.AddSingleton<VaccineScheduleGenerator>();
builder.Services.AddScoped<MedicineService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<PersonService>();
builder.Services.AddHttpClient<GeminiHealthService>();
builder.Services.AddScoped<AllergyService>();
builder.Services.AddScoped<IllnessService>();
builder.Services.AddScoped<NotificationService>();
// ------------------------------------------

// JWT AUTHENTICATION AYARLARI
// Her gelen istekte "Authorization: Bearer <token>" header'ını otomatik kontrol eder.
// Token geçersizse veya süresi dolmuşsa 401 (Unauthorized) döndürür.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Token'ı kimin ürettiğini doğrula (appsettings'teki Issuer ile eşleşmeli)
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            // Token kimin için üretildiğini doğrula (appsettings'teki Audience ile eşleşmeli)
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            // Token'ın imzasını doğrula (Key ile)
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

            // Token'ın süresinin dolup dolmadığını kontrol et
            ValidateLifetime = true
        };
    });

// CORS: Flutter uygulamasından gelen isteklere izin ver.
// Farklı adreslerden (origin) gelen istekler varsayılan olarak engellenir.
// Flutter emulator/cihaz farklı bir adreste çalıştığı için buna ihtiyaç var.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutter", policy =>
    {
        policy.AllowAnyOrigin()    // Tüm adreslerden gelen isteklere izin ver (development için)
              .AllowAnyMethod()    // GET, POST, PUT, DELETE hepsine izin ver
              .AllowAnyHeader();   // Authorization gibi tüm header'lara izin ver
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Flutter camelCase bekler; tüm property adlarını küçük harfe çevir
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        // Gelen istek body'lerinde büyük/küçük harf fark etmesin
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    // Production'da HTTPS zorunlu, Development'ta Flutter sertifika hatası almasın diye kapalı.
    app.UseHttpsRedirection();
}

// CORS middleware'i — Authentication'dan ÖNCE olmalı.
// Aksi halde Flutter'dan gelen preflight (OPTIONS) istekleri 401 alır.
app.UseCors("AllowFlutter");

// Sıra ÖNEMLİ! Önce Authentication (kim bu?), sonra Authorization (yetkisi var mı?)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
