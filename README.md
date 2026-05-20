# Proje Takip Sistemi (ProjeHavuzu)

Malatya Turgut Özal Üniversitesi öğrenci proje süreçlerini yönetmek için geliştirilmiş web tabanlı bir **ASP.NET Core MVC** uygulamasıdır. Öğrenciler proje oluşturur, danışman onayı alır; akademisyenler ve yöneticiler süreci takip eder.

## Özellikler

### Öğrenci
- Kurumsal e-posta (`@ozal.edu.tr`) ile kayıt ve e-posta doğrulama
- Proje oluşturma, düzenleme ve yol haritası (faz) tanımlama
- Danışman seçimi ve proje havuzundan başvuru
- Proje teslimi (dosya yükleme)
- Kendi projelerini ve onay durumunu görüntüleme

### Akademisyen (Teacher)
- Danışmanı olduğu projeleri onaylama / reddetme
- Öğrenci listesi, proje atama ve pasifleştirme
- Proje isteklerini yönetme
- Proje teslimlerini inceleme

### Admin
- Tüm projeler, akademisyenler ve öğrenciler
- Akademisyen ekleme (Teacher rolü)
- Öğrenci aktif / pasif etme
- Kategori, fakülte, bölüm yönetimi
- Sistem logları ve Hangfire arka plan işleri
- Proje çöp kutusu (soft delete)

## Teknoloji Yığını

| Katman | Teknoloji |
|--------|-----------|
| UI | ASP.NET Core MVC, Razor, Bootstrap 5, DataTables |
| İş katmanı | ProjeHavuzu.Business |
| Veri | Entity Framework Core 10, SQL Server |
| Kimlik | ASP.NET Core Identity |
| Doğrulama | FluentValidation |
| Arka plan | Hangfire |
| E-posta | SMTP (yapılandırılabilir) |
| Haritalama | AutoMapper |

**Hedef çerçeve:** .NET 10

## Mimari

```
ProjeHavuzu.MVCUI      → Controllers, Views, wwwroot
ProjeHavuzu.Business   → Servisler, iş kuralları
ProjeHavuzu.Data       → Entity, DbContext, Repository, Migrations
```

## Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server veya **SQL Server LocalDB** (geliştirme için)
- (İsteğe bağlı) Visual Studio 2022 / VS Code / Cursor

## Kurulum

### 1. Depoyu klonlayın

```bash
git clone <repo-url>
cd ProjeTakip
```

### 2. Bağlantı dizesini yapılandırın

`ProjeHavuzu.MVCUI/appsettings.json` içindeki `ConnectionStrings:DefaultServer` değerini kendi SQL Server örneğinize göre güncelleyin.

Geliştirme için örnek (LocalDB):

```json
"ConnectionStrings": {
  "DefaultServer": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ProjeHavuzu2025Abkos;Integrated Security=True;Trust Server Certificate=True"
}
```

> **Güvenlik:** E-posta şifresi ve admin bilgilerini repoya koymayın. Üretimde `appsettings.Development.json` veya [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) kullanın.

### 3. Veritabanını oluşturun

```bash
dotnet ef database update --project ProjeHavuzu.Data\ProjeHavuzu.Data.csproj --startup-project ProjeHavuzu.MVCUI\ProjeHavuzu.MVCUI.csproj
```

### 4. Uygulamayı çalıştırın

```bash
dotnet run --project ProjeHavuzu.MVCUI\ProjeHavuzu.MVCUI.csproj --launch-profile https
```

Tarayıcı adresleri:

| Profil | Adres |
|--------|--------|
| HTTPS | https://localhost:7067 |
| HTTP | http://localhost:5234 |

İlk çalıştırmada `IdentitySeed` rolleri ve varsayılan kullanıcıları oluşturur.

## Varsayılan test hesapları (seed)

| Rol | E-posta | Şifre (varsayılan) |
|-----|---------|-------------------|
| Admin | `appsettings.json` → `SeedSettings:AdminEmail` | `SeedSettings:AdminPassword` |
| Akademisyen | `teacher@ozal.edu.tr` | `Teacher123+` |
| Öğrenci | `student@ozal.edu.tr` | `Student123+` |

Üretim ortamında bu şifreleri mutlaka değiştirin.

## Yapılandırma

### E-posta (`EmailSettings`)

| Alan | Açıklama |
|------|----------|
| `Mail` | Gönderen SMTP hesabı |
| `Password` | SMTP uygulama şifresi |
| `Host` / `Port` | SMTP sunucusu |
| `TestMode` | `true` ise tüm mailler `TestEmail` adresine gider |

### Öğrenci kaydı

- Yalnızca `@ozal.edu.tr` uzantılı e-posta
- Öğrenci numarası, e-posta ön eki ile aynı olmalıdır
- Kayıt sonrası e-posta doğrulama zorunludur

### Akademisyen

- Akademisyenler **yalnızca admin** tarafından eklenir (`Admin → Akademisyen Ekle`)
- Öğrenci self-register ile akademisyen olamaz

## Hangfire

Arka plan işleri (e-posta kuyruğu, sistem sağlık kontrolü) için:

- Panel: `/hangfire` (yalnızca **Admin** rolü)

## Proje yapısı (özet)

```
ProjeTakip/
├── ProjeHavuzu.MVCUI/          # Web arayüzü
│   ├── Controllers/
│   ├── Views/
│   └── wwwroot/
├── ProjeHavuzu.Business/       # İş mantığı
├── ProjeHavuzu.Data/            # EF Core, entity, migration
├── ProjeHavuzu.slnx
└── README.md
```

## Faydalı komutlar

```bash
# Derleme
dotnet build ProjeHavuzu.MVCUI\ProjeHavuzu.MVCUI.csproj

# Yeni migration
dotnet ef migrations add MigrationAdi --project ProjeHavuzu.Data\ProjeHavuzu.Data.csproj --startup-project ProjeHavuzu.MVCUI\ProjeHavuzu.MVCUI.csproj

# Migration uygula
dotnet ef database update --project ProjeHavuzu.Data\ProjeHavuzu.Data.csproj --startup-project ProjeHavuzu.MVCUI\ProjeHavuzu.MVCUI.csproj
```

## Roller

| Rol | Açıklama |
|-----|----------|
| `Admin` | Tam yönetim paneli |
| `Teacher` | Danışman / akademisyen paneli |
| `Student` | Öğrenci paneli |

## Lisans

Bu proje eğitim / kurum içi kullanım amacıyla geliştirilmiştir. Lisans bilgisi depo sahibi tarafından eklenebilir.

## Katkı

1. Dal oluşturun (`git checkout -b feature/yeni-ozellik`)
2. Değişiklikleri commit edin
3. Pull request açın

---

**Not:** `appsettings.json` içindeki hassas bilgileri (SMTP şifresi, admin parolası) paylaşılan repolarda tutmayın; ortam değişkenleri veya User Secrets kullanın.
