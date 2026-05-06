# HealthApp Backend — Öğrenme Notları

> Bu proje boyunca adım adım öğrendiklerimizin özeti.

---

## 📐 1. Katmanlı Mimari (Layered Architecture)

Projeyi 4 katmana ayırdık. Neden? Çünkü her şey tek yerde olursa değişiklik yapmak kabus olur.

```
HealthApp.API          → Dış dünyayla konuşan kapı (Controller'lar)
HealthApp.Business     → İş mantığı (Service'ler, DTO'lar)
HealthApp.DataAccess   → Veritabanı işlemleri (Repository'ler, DbContext)
HealthApp.Domain       → Veri modelleri (Entity'ler)
```

**Kural:** Üst katman alt katmanı bilir, alt katman üstü bilmez.
```
API → Business → DataAccess → Domain
```

---

## 🧱 2. Entity ve BaseEntity

**Ne:** Veritabanı tablolarını temsil eden C# sınıfları.

**BaseEntity** — Her tabloda ortak olan alanları tek yerde toplar:
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();        // Benzersiz kimlik
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
}
```

**User** — BaseEntity'den türer, kendi alanlarını ekler:
```csharp
public class User : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }         // BCrypt hash olarak saklanır
    public string? RefreshToken { get; set; }     // JWT yenileme anahtarı
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
```

**Neden `abstract`?** BaseEntity'den doğrudan nesne oluşturulmasın, sadece türetilsin diye.

---

## 🗄️ 3. Repository Pattern

**Ne:** Veritabanı işlemlerini tek yerden yönetmek.

**GenericRepository<T>** — Tüm entity'ler için ortak CRUD işlemleri:
```
GetAllAsync()    → Hepsini getir
GetByIdAsync()   → ID ile bul
AddAsync()       → Ekle
UpdateAsync()    → Güncelle
DeleteAsync()    → Sil
```

**UserRepository** — User'a özel metotlar (Generic'te olamaz):
```
GetByEmailAsync()        → Email ile kullanıcı bul
EmailExistsAsync()       → Email var mı kontrol et
GetByRefreshTokenAsync() → Refresh token ile kullanıcı bul
```

**Neden ayrı?** Her tablonun email'i yok, ama User'ın var. Genel işlemler generic'te, özel işlemler kendi repository'sinde.

---

## 📦 4. DTO (Data Transfer Object)

**Ne:** Dış dünyaya gönderilen/alınan veri yapıları. Entity'nin kendisi ASLA dışarıya verilmez.

```
Dışarıdan gelen:                    Dışarıya giden:
├── LoginRequestDTO (email, pass)   ├── UserDto (id, name, email, createdAt)
├── RegisterRequestDTO              ├── AuthResponseDTO (token, refreshToken, user)
├── UserCreateDTO                   
├── RefreshTokenRequestDTO          
```

**Neden Entity değil de DTO?**
- Entity'de Password var → Dışarıya şifre sızar!
- Entity'de RefreshToken var → Güvenlik açığı!
- DTO sadece gerekli alanları taşır.

**`record` nedir?** DTO'lar için ideal. Kısa yazılır, immutable (değiştirilemez):
```csharp
public record UserDto(Guid Id, string Name, string Surname, string Email, DateTime CreatedAt);
```

---

## ⚙️ 5. Service Katmanı

**Ne:** İş mantığının yazıldığı yer. Controller ile Repository arasında köprü.

**Neden Controller'da iş mantığı olmaz?**
```
❌ Controller'da:  "Email var mı?" + "Şifreyi hashle" + "DB'ye kaydet" → Şişer, test edilemez
✅ Service'te:     Controller sadece servisi çağırır, temiz kalır
```

**AuthService** — Login, Register, RefreshToken
**UserService** — Kullanıcı CRUD işlemleri

---

## 🔐 6. BCrypt — Şifre Hashleme

**Ne:** Şifreleri geri döndürülemez şekilde hashler.

```
Register: "123456" → BCrypt.HashPassword() → "$2a$11$xPJqk..." (DB'ye bu kaydedilir)
Login:    "123456" → BCrypt.Verify("123456", "$2a$11$xPJqk...") → true ✅
```

**Neden düz metin olmaz?** DB sızarsa herkesin şifresi okunur. Hash'ten şifreye geri dönmek imkansız.

**Neden MD5/SHA256 değil?** Çok hızlılar → brute force'a açık. BCrypt kasıtlı olarak yavaş.

---

## 🎫 7. JWT (JSON Web Token)

**Ne:** Kullanıcının kimliğini kanıtlayan dijital giriş kartı.

**İki token sistemi:**
```
Access Token  (60 dk)  → Her istekte header'da gönderilir
Refresh Token (7 gün)  → Access token dolunca yenisini almak için kullanılır
```

**Access Token'ın içinde ne var?**
```
eyJhbGciOiJI...  →  { sub: "user-id", email: "...", name: "...", exp: "..." }
```

**Refresh Token nedir?** Rastgele üretilmiş uzun bir string. JWT gibi yapılandırılmış değil. DB'de saklanır.

**Token Rotation:** Her refresh'te yeni refresh token üretilir, eski geçersiz olur → Çalınan token kullanılamaz.

**Akış:**
```
Login → Access + Refresh token al
  ↓
60 dk boyunca Access token ile istek at
  ↓
401 aldın → Refresh token ile yeni Access token al
  ↓
7 gün sonra Refresh da doldu → Tekrar login
```

---

## 🛡️ 8. [Authorize] Attribute

**Ne:** Endpoint'i token olmadan erişime kapatır.

```csharp
[Authorize]  // Bu controller'a token olmadan erişilemez → 401 Unauthorized
public class UserController

// AuthController'da [Authorize] YOK → Login/Register herkese açık (token yok ki daha)
```

---

## 🌐 9. CORS (Cross-Origin Resource Sharing)

**Ne:** Farklı adreslerden gelen isteklere izin verme mekanizması.

**Neden lazım?** Flutter (10.0.2.2) ile Backend (localhost:5105) farklı adresler → CORS izni lazım.

```csharp
policy.AllowAnyOrigin()   // Herkes erişebilir (development için)
      .AllowAnyMethod()   // GET, POST, PUT, DELETE
      .AllowAnyHeader();  // Authorization header'ı dahil
```

**Production'da** sadece kendi uygulamanın adresine izin verilir.

---

## 🔌 10. Dependency Injection (DI)

**Ne:** Sınıfların ihtiyaç duyduğu bağımlılıkları dışarıdan almak.

**Program.cs'te kayıt:**
```csharp
builder.Services.AddScoped<UserRepository>();  // "UserRepository istenirse oluştur"
builder.Services.AddScoped<AuthService>();      // "AuthService istenirse oluştur"
```

**Controller'da kullanım:**
```csharp
public AuthController(AuthService authService)  // ASP.NET otomatik verir
```

**`AddScoped` ne demek?** Her HTTP isteği için yeni bir instance oluştur, istek bitince at.

---

## 📡 11. Middleware Sırası

**Ne:** Her HTTP isteğinin geçtiği boru hattı. Sıra ÖNEMLİ!

```
İstek geldi
    ↓
UseCors           → Farklı adresten gelen isteğe izin ver mi?
    ↓
UseAuthentication → Token geçerli mi? (Kim bu?)
    ↓
UseAuthorization  → Bu kişinin yetkisi var mı?
    ↓
Controller        → İşlemi yap
    ↓
Response          → Cevabı döndür
```

---

## 🔄 12. Migration

**Ne:** C# entity'lerindeki değişiklikleri veritabanına yansıtan talimat dosyası.

```bash
# Entity değişti → Migration oluştur
dotnet ef migrations add MigrationAdi --project DataAccess --startup-project API

# Migration'ı veritabanına uygula
dotnet ef database update --project DataAccess --startup-project API
```

---

## 📱 13. Flutter ↔ C# Eşleşme

```
Flutter                         C# Backend
──────                          ──────────
User.fromJson()            ←    UserDto (record)
AuthResponse.fromJson()    ←    AuthResponseDTO (record)
ApiEndpoints.login         →    AuthController.Login()
"Authorization: Bearer"   →    [Authorize] middleware kontrolü
http://10.0.2.2:5105       →    localhost:5105 (Android emulator)
```

---

## 📁 Dosya Haritası

```
HealthApp/
├── HealthApp.Domain/           # Veri modelleri
│   └── Entities/
│       ├── BaseEntity.cs       # Id + CreatedAt (ortak)
│       └── User.cs             # Name, Email, Password, RefreshToken
│
├── HealthApp.DataAccess/       # Veritabanı erişimi
│   ├── Context/
│   │   └── AppDbContext.cs     # EF Core DbContext
│   └── Repositories/
│       ├── GenericRepository.cs    # CRUD (tüm entity'ler için)
│       └── UserRepository.cs      # User'a özel metotlar
│
├── HealthApp.Business/         # İş mantığı
│   ├── DTOs/
│   │   ├── UserDto.cs              # Dışarıya dönen kullanıcı bilgisi
│   │   ├── UserCreateDTO.cs        # Kullanıcı oluşturma girdi
│   │   ├── LoginRequestDTO.cs      # Login girdi
│   │   ├── RegisterRequestDTO.cs   # Register girdi
│   │   ├── AuthResponseDTO.cs      # Token + User çıktı
│   │   └── RefreshTokenRequestDTO.cs
│   └── Services/
│       ├── UserService.cs      # Kullanıcı CRUD iş mantığı
│       ├── AuthService.cs      # Login, Register, Refresh iş mantığı
│       └── JwtService.cs       # Token üretimi
│
├── HealthApp.API/              # HTTP endpoint'ler
│   ├── Controllers/
│   │   ├── AuthController.cs   # /api/Auth/login, register, refresh
│   │   └── UserController.cs   # /api/User (Authorize korumalı)
│   ├── Program.cs              # DI, JWT, CORS, Middleware
│   └── appsettings.json        # Connection string, JWT ayarları
│
└── docs/
    └── API_DOCUMENTATION.md    # Flutter için API referansı
```
