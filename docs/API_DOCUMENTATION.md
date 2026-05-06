# HealthApp API Dokümantasyonu

> Flutter geliştiricileri için backend API referansı.

## 🔗 Base URL

```
https://localhost:7105
```

> ⚠️ Production'da bu URL değişecektir.

---

## 📋 İçindekiler

1. [Authentication (Kimlik Doğrulama)](#-authentication)
   - [Register (Kayıt Ol)](#1-register---kayıt-ol)
   - [Login (Giriş Yap)](#2-login---giriş-yap)
   - [Refresh Token (Token Yenile)](#3-refresh-token---token-yenile)
2. [User (Kullanıcı İşlemleri)](#-user-işlemleri-authorize-gerekli)
   - [Tüm Kullanıcıları Listele](#4-tüm-kullanıcıları-listele)
   - [Kullanıcı Oluştur](#5-kullanıcı-oluştur)
3. [Token Kullanım Rehberi](#-token-kullanım-rehberi)
4. [Hata Yönetimi](#-hata-yönetimi)
5. [Flutter Entegrasyon Rehberi](#-flutter-entegrasyon-rehberi)

---

## 🔐 Authentication

Bu endpoint'ler **herkese açıktır** — token gerekmez.

### 1. Register - Kayıt Ol

Yeni kullanıcı oluşturur. Başarılıysa access token + refresh token döndürür.

```
POST /api/Auth/register
```

**Request Body:**

```json
{
    "name": "Rumeysa",
    "surname": "Semiz",
    "email": "rumeysa@test.com",
    "password": "123456",
    "confirmPassword": "123456"
}
```

| Alan | Tip | Zorunlu | Açıklama |
|---|---|---|---|
| `name` | string | ✅ | Kullanıcı adı |
| `surname` | string | ✅ | Kullanıcı soyadı |
| `email` | string | ✅ | E-posta adresi (benzersiz olmalı) |
| `password` | string | ✅ | Şifre |
| `confirmPassword` | string | ✅ | Şifre tekrarı (password ile aynı olmalı) |

**Başarılı Response (200 OK):**

```json
{
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "a8Kx9f2mNpQrStUvWxYz...",
    "user": {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Rumeysa",
        "surname": "Semiz",
        "email": "rumeysa@test.com",
        "createdAt": "2025-03-05T20:38:20Z"
    }
}
```

**Hata Response'ları (400 Bad Request):**

```json
{ "message": "Bu e-posta adresi zaten kullanımda!" }
```
```json
{ "message": "Şifreler eşleşmiyor!" }
```

---

### 2. Login - Giriş Yap

Mevcut kullanıcı ile giriş yapar. Başarılıysa access token + refresh token döndürür.

```
POST /api/Auth/login
```

**Request Body:**

```json
{
    "email": "rumeysa@test.com",
    "password": "123456"
}
```

| Alan | Tip | Zorunlu | Açıklama |
|---|---|---|---|
| `email` | string | ✅ | Kayıtlı e-posta adresi |
| `password` | string | ✅ | Şifre |

**Başarılı Response (200 OK):**

```json
{
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "b9Ly0g3nOpRsStUvWxYz...",
    "user": {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Rumeysa",
        "surname": "Semiz",
        "email": "rumeysa@test.com",
        "createdAt": "2025-03-05T20:38:20Z"
    }
}
```

**Hata Response (400 Bad Request):**

```json
{ "message": "E-posta veya şifre hatalı!" }
```

---

### 3. Refresh Token - Token Yenile

Access token süresi dolduğunda (60 dk), yeni token almak için kullanılır.
Kullanıcı tekrar login yapmak zorunda kalmaz.

```
POST /api/Auth/refresh
```

**Request Body:**

```json
{
    "refreshToken": "a8Kx9f2mNpQrStUvWxYz..."
}
```

| Alan | Tip | Zorunlu | Açıklama |
|---|---|---|---|
| `refreshToken` | string | ✅ | Login/Register'dan alınan refresh token |

**Başarılı Response (200 OK):**

```json
{
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6NEWTOKEN...",
    "refreshToken": "c0Mz1h4oQpSsTtUuVvWw...",
    "user": {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Rumeysa",
        "surname": "Semiz",
        "email": "rumeysa@test.com",
        "createdAt": "2025-03-05T20:38:20Z"
    }
}
```

> ⚠️ **Önemli:** Her refresh işleminde **yeni bir refresh token** döner. Eski refresh token geçersiz olur (Token Rotation). Flutter tarafında yeni token'ı kaydetmeyi unutma!

**Hata Response'ları (400 Bad Request):**

```json
{ "message": "Geçersiz refresh token!" }
```
```json
{ "message": "Refresh token süresi dolmuş. Lütfen tekrar giriş yapın." }
```

---

## 👤 User İşlemleri (Authorize Gerekli!)

> 🔒 Bu endpoint'ler **JWT token gerektirir**.
> Header'a `Authorization: Bearer <token>` eklenmezse `401 Unauthorized` döner.

### 4. Tüm Kullanıcıları Listele

```
GET /api/User
```

**Headers:**

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Başarılı Response (200 OK):**

```json
[
    {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Rumeysa",
        "surname": "Semiz",
        "email": "rumeysa@test.com",
        "createdAt": "2025-03-05T20:38:20Z"
    },
    {
        "id": "7ba92c31-8823-4671-a1bc-3d852f77bfa1",
        "name": "Ahmet",
        "surname": "Yılmaz",
        "email": "ahmet@test.com",
        "createdAt": "2025-03-06T10:15:30Z"
    }
]
```

**Hata Response (401 Unauthorized):**
Token yoksa veya geçersizse — body olmadan sadece status code döner.

---

### 5. Kullanıcı Oluştur

```
POST /api/User
```

**Headers:**

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Request Body:**

```json
{
    "name": "Ahmet",
    "surname": "Yılmaz",
    "email": "ahmet@test.com",
    "password": "123456"
}
```

**Başarılı Response (200 OK):**

```json
{
    "id": "7ba92c31-8823-4671-a1bc-3d852f77bfa1",
    "name": "Ahmet",
    "surname": "Yılmaz",
    "email": "ahmet@test.com",
    "createdAt": "2025-03-06T10:15:30Z"
}
```

---

## 🔑 Token Kullanım Rehberi

### Token Süreleri

| Token | Süre | Kullanım |
|---|---|---|
| Access Token | 60 dakika | Her API isteğinde header'da gönderilir |
| Refresh Token | 7 gün | Sadece yeni access token almak için kullanılır |

### Token Akış Şeması

```
┌──────────────────────────────────────────────────────────┐
│ 1. LOGIN / REGISTER                                      │
│    POST /api/Auth/login                                  │
│    ← { token, refreshToken, user }                       │
│    → İkisini de güvenli şekilde sakla                    │
└──────────────────────────────────────────────────────────┘
                          │
                          ▼
┌──────────────────────────────────────────────────────────┐
│ 2. NORMAL İSTEKLER (60 dakika boyunca)                   │
│    GET /api/User                                         │
│    Header: Authorization: Bearer <accessToken>           │
│    ← 200 OK                                              │
└──────────────────────────────────────────────────────────┘
                          │
                   Token süresi doldu
                          ▼
┌──────────────────────────────────────────────────────────┐
│ 3. 401 UNAUTHORIZED ALINDI                               │
│    POST /api/Auth/refresh { refreshToken: "..." }        │
│    ← { token (YENİ), refreshToken (YENİ), user }        │
│    → Yeni token'ları sakla, eski istekleri tekrarla      │
└──────────────────────────────────────────────────────────┘
                          │
                   7 gün sonra refresh token da doldu
                          ▼
┌──────────────────────────────────────────────────────────┐
│ 4. REFRESH DA BAŞARISIZ                                  │
│    → Kullanıcıyı login ekranına yönlendir                │
└──────────────────────────────────────────────────────────┘
```

---

## ❌ Hata Yönetimi

### HTTP Status Code'ları

| Code | Anlamı | Ne Zaman Döner |
|---|---|---|
| `200` | Başarılı | İşlem başarıyla tamamlandı |
| `400` | Bad Request | Validasyon hatası (eksik/yanlış veri) |
| `401` | Unauthorized | Token yok, geçersiz veya süresi dolmuş |

### Hata Response Formatı

Tüm hatalar şu formatta döner:

```json
{
    "message": "Hata açıklaması burada"
}
```

### Olası Hata Mesajları

| Endpoint | Hata Mesajı |
|---|---|
| Register | `"Bu e-posta adresi zaten kullanımda!"` |
| Register | `"Şifreler eşleşmiyor!"` |
| Login | `"E-posta veya şifre hatalı!"` |
| Refresh | `"Geçersiz refresh token!"` |
| Refresh | `"Refresh token süresi dolmuş. Lütfen tekrar giriş yapın."` |

---

## 📱 Flutter Entegrasyon Rehberi

### Gerekli Modeller

**User modeli** (`createdAt` alanı backend ile eşleşiyor):

```dart
class User {
  final String id;
  final String name;
  final String surname;
  final String email;
  final DateTime createdAt;

  User({
    required this.id,
    required this.name,
    required this.surname,
    required this.email,
    required this.createdAt,
  });

  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      id: json['id'] as String,
      name: json['name'] as String,
      surname: json['surname'] as String,
      email: json['email'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
    );
  }
}
```

**AuthResponse modeli** (login/register/refresh response'u için):

```dart
class AuthResponse {
  final String token;
  final String refreshToken;
  final User user;

  AuthResponse({
    required this.token,
    required this.refreshToken,
    required this.user,
  });

  factory AuthResponse.fromJson(Map<String, dynamic> json) {
    return AuthResponse(
      token: json['token'] as String,
      refreshToken: json['refreshToken'] as String,
      user: User.fromJson(json['user'] as Map<String, dynamic>),
    );
  }
}
```

### API Endpoints

```dart
class ApiEndpoints {
  static const String login = '/api/Auth/login';
  static const String register = '/api/Auth/register';
  static const String refresh = '/api/Auth/refresh';
  static const String users = '/api/User';
}
```

### Header Formatı

Korunan endpoint'ler için her istekte:

```dart
headers: {
  'Content-Type': 'application/json',
  'Authorization': 'Bearer $accessToken',
}
```

### Postman Test Sırası

1. **Register** → `POST /api/Auth/register` → Token'ları al
2. **Login** → `POST /api/Auth/login` → Token'ları al
3. **Get Users (token ile)** → `GET /api/User` + `Authorization: Bearer <token>` → 200 OK
4. **Get Users (token olmadan)** → `GET /api/User` → 401 Unauthorized
5. **Refresh** → `POST /api/Auth/refresh` → Yeni token'ları al
