# Görev Dağılımı — 4 Kişilik Backend Takımı

> Herkes backend yapıyor. Görevler modül bazlı bölünmüştür.
> Her kişi kendi modülünün **Entity → Repository → DTO → Service → Controller** akışını uçtan uca yazar.

---

## 👥 Takım Rolleri

| Kişi | Rol | Modüller |
|---|---|---|
| **Rumeysa** | Tech Lead + Auth & User | Auth, User, Person, HealthFacility (Dış API), Program.cs yönetimi |
| **Kişi 2** | Backend Dev | Medicine, Reminder |
| **Kişi 3** | Backend Dev | Child, Vaccine, VaccineSchedule |
| **Kişi 4** | Backend Dev | Illness, Allergy, Notification |

---

## 🤔 Neden Bu Dağılım?

### Rumeysa — Auth & User & Person & HealthFacility (Dış API)
- Auth zaten hazır ✅ (Login, Register, Refresh Token, JWT)
- User zaten hazır ✅
- Person eklenecek (User ile 1:1 ilişkili — en karmaşık relation)
- **HealthFacility → Dış API entegrasyonu** (DB'de entity YOK!)
  - Yakındaki hastane/sağlık ocağı → Google Places API veya Overpass API
  - Nöbetçi eczane → nosyapi.com veya collectapi.com gibi ücretsiz API'ler
  - Farklı iş: Entity/Repository yok, HttpClient ile dış API çağrısı var
  - Service + DTO + Controller yeterli (veritabanı katmanı yok)
- Program.cs'e herkesin DI kaydını ekleyecek
- Code review yapacak

### Kişi 2 — Medicine & Reminder
- İkisi de Person'a bağlı (PersonId foreign key)
- CRUD yapısı benzer — birini öğrenince diğeri kolay
- Medicine'de ekstra: MedicineReminderTime (1:N ilişki öğrenir)
- Orta zorluk

### Kişi 3 — Child & Vaccine & VaccineSchedule
- En karmaşık ilişki zinciri: Child → VaccineSchedule → Vaccine
- VaccineScheduleGenerator iş mantığı (aşı takvimi otomatik oluşturma)
- İleri seviye öğrenme fırsatı
- Orta-zor

### Kişi 4 — Illness, Allergy, Notification
- Illness ve Allergy basit CRUD (Person'a bağlı, DB'de saklanır)
- NotificationSetting basit (User'a 1:1)
- En kolay modüller — backend'e yeni başlayanlar için ideal

---

## 📅 Sprint Planı

### Sprint 1 (Hafta 1-2): Temel Yapı + İlk Entity'ler

**Herkesin yapacağı ilk iş (DAY 1):**
1. `LEARNING_NOTES.md` dosyasını oku
2. Mevcut kodu incele (User akışını takip et: Entity → Repository → DTO → Service → Controller)
3. Git branch oluştur: `feature/kendi-modul-adi`

| Kişi | Görev | Detay |
|---|---|---|
| **Rumeysa** | Person entity + ilişkiler | Person Entity, PersonRepository, PersonDto, PersonService, PersonsController |
| **Kişi 2** | Medicine entity | Medicine Entity, MedicineReminderTime Entity, MedicineRepository, MedicineDto, MedicineService, MedicinesController |
| **Kişi 3** | Child entity | Child Entity, ChildRepository, ChildDto, ChildService, ChildrenController |
| **Kişi 4** | Illness + Allergy entity | Her ikisinin Entity, Repository, DTO, Service, Controller'ını yaz + Dış API araştırması |

**Sprint 1 sonunda:** 4 yeni entity çalışır durumda, Postman'de test edilmiş.

---

### Sprint 2 (Hafta 3-4): İlişkiler + İş Mantığı

| Kişi | Görev | Detay |
|---|---|---|
| **Rumeysa** | Person-User ilişkisi + HealthFacility | Navigation property, Include sorgular, profil oluşturma + Dış API entegrasyonu (HttpClient) |
| **Kişi 2** | Reminder entity + Medicine ilişkileri | Reminder CRUD, Medicine'e ReminderTime ekleme (1:N) |
| **Kişi 3** | VaccineSchedule + Vaccine | Schedule-Vaccine ilişkisi, VaccineScheduleGenerator |
| **Kişi 4** | NotificationSetting | Basit CRUD + diğer takım arkadaşlarına yardım |

**Sprint 2 sonunda:** Tüm entity'ler ve ilişkiler çalışır durumda.

---

### Sprint 3 (Hafta 5-6): Entegrasyon + Test + Flutter Bağlantısı

| Kişi | Görev |
|---|---|
| **Herkes** | Kendi modülünü Flutter'a bağla |
| **Herkes** | Postman test collection oluştur |
| **Rumeysa** | API dokümantasyonunu güncelle |
| **Herkes** | Bug fix + code review |

---

## 📝 Her Kişinin İzleyeceği Adımlar (Şablon)

Yeni bir entity ekleme adımları (User örneğini takip et):

```
1. DOMAIN KATMANI
   └── HealthApp.Domain/Entities/XxxEntity.cs oluştur
       → BaseEntity'den türet
       → Property'leri ekle

2. DATAACCESS KATMANI
   └── AppDbContext.cs'e DbSet<Xxx> ekle
   └── HealthApp.DataAccess/Repositories/XxxRepository.cs oluştur
       → GenericRepository<Xxx>'den türet
       → Varsa özel query metotları ekle

3. BUSINESS KATMANI
   └── HealthApp.Business/DTOs/XxxDto.cs oluştur (dışarıya dönen)
   └── HealthApp.Business/DTOs/XxxCreateDTO.cs oluştur (dışarıdan gelen)
   └── HealthApp.Business/Services/XxxService.cs oluştur
       → Repository'yi constructor'dan al (DI)
       → CRUD metotları yaz
       → DTO ↔ Entity dönüşümü yap

4. API KATMANI
   └── HealthApp.API/Controllers/XxxController.cs oluştur
       → [Route("api/[controller]")]
       → [Authorize] ekle
       → Service'i constructor'dan al (DI)
       → Endpoint'leri yaz

5. PROGRAM.CS
   └── Repository DI kaydı ekle: builder.Services.AddScoped<XxxRepository>();
   └── Service DI kaydı ekle: builder.Services.AddScoped<XxxService>();

6. MİGRATİON
   └── dotnet ef migrations add AddXxx --project HealthApp.DataAccess --startup-project HealthApp.API
   └── dotnet ef database update --project HealthApp.DataAccess --startup-project HealthApp.API

7. TEST
   └── Postman'de CRUD test et
   └── Token ile [Authorize] test et
```

---

## 🌿 Git Kuralları

### Branch Stratejisi
```
main (korunur — direkt push YASAK)
  │
  ├── feature/person-api         (Rumeysa)
  ├── feature/medicine-api       (Kişi 2)
  ├── feature/child-vaccine-api  (Kişi 3)
  ├── feature/illness-health-api (Kişi 4)
  │
  └── Bitince → Pull Request → Rumeysa review → main'e merge
```

### Commit Mesajı Formatı
```
feat: Person entity ve repository eklendi
feat: MedicineController CRUD endpoint'leri
fix: Medicine DTO'da CreatedAt eksikti
docs: API dokümantasyonu güncellendi
```

### Çakışma Önleme Kuralları
- **Program.cs** → Sadece Rumeysa düzenler (diğerleri kendi DI kaydını söyler, Rumeysa ekler)
- **AppDbContext.cs** → Herkes kendi DbSet'ini ekler ama dikkatli olsun
- **Migration** → Herkes kendi branch'inde oluşturur, merge sonrası Rumeysa kontrol eder

---

## ✅ Her Sprint Sonunda Kontrol Listesi

- [ ] Entity oluşturuldu ve BaseEntity'den türetildi
- [ ] AppDbContext'e DbSet eklendi
- [ ] Repository oluşturuldu (gerekirse özel metotlar)
- [ ] DTO'lar oluşturuldu (Create, Response ayrı)
- [ ] Service oluşturuldu (CRUD + iş mantığı)
- [ ] Controller oluşturuldu ([Authorize] var mı?)
- [ ] Program.cs'e DI kaydı eklendi
- [ ] Migration oluşturuldu ve uygulandı
- [ ] Postman'de test edildi (CRUD + Auth)
- [ ] Pull Request açıldı

---

## 📚 Takıma Önerilen Öğrenme Sırası

Herkes başlamadan önce şu sırayla okumalı:

| Sıra | Ne | Nerede |
|---|---|---|
| 1 | Öğrenme notları | `docs/LEARNING_NOTES.md` |
| 2 | API dokümantasyonu | `docs/API_DOCUMENTATION.md` |
| 3 | Mevcut User kodu | Entity → Repository → Service → Controller sırasıyla |
| 4 | Teknik doküman (kendi modülü) | `BACKEND_TECHNICAL_DOCUMENT.md` |

---

## 💡 Önemli Hatırlatmalar

1. **Entity'yi ASLA dışarıya döndürme** → DTO kullan
2. **İş mantığını Controller'da yazma** → Service'te yaz
3. **Password'u DTO'da döndürme** → Güvenlik açığı
4. **Her entity için migration oluştur** → Unutursan DB güncellenmez
5. **[Authorize] eklemeyi unutma** → Auth endpoint'leri hariç her yere koy
6. **Takıldığın yerde mevcut User kodunu incele** → En iyi şablon o
