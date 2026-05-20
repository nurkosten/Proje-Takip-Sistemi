# Proje Havuzu Sistemi

Bu proje, öğrencilerin proje fikirlerini sisteme ekleyebildiği, akademisyenlerin kendilerine gelen projeleri değerlendirebildiği ve yöneticilerin tüm proje sürecini takip edebildiği web tabanlı bir proje takip sistemidir.

Proje; üniversite ortamında proje başvuru, değerlendirme, danışman onayı ve yönetim süreçlerini daha düzenli, güvenli ve izlenebilir hale getirmek amacıyla geliştirilmiştir.

> Bu README dosyası hazırlanırken gizlilik ve veri güvenliği ön planda tutulmuştur. Gerçek bağlantı bilgileri, şifreler, kişisel kullanıcı verileri veya özel sistem bilgileri paylaşılmamıştır.

---

## İçindekiler

- [Projenin Amacı](#projenin-amacı)
- [Kullanılan Teknolojiler](#kullanılan-teknolojiler)
- [Sistem Rolleri](#sistem-rolleri)
- [Temel Özellikler](#temel-özellikler)
- [Veritabanı Yapısı](#veritabanı-yapısı)
- [Gizlilik ve Veri Güvenliği](#gizlilik-ve-veri-güvenliği)
- [Kurulum](#kurulum)
- [Ortam Değişkenleri ve Yapılandırma](#ortam-değişkenleri-ve-yapılandırma)
- [GitHub'a Yüklerken Dikkat Edilmesi Gerekenler](#githuba-yüklerken-dikkat-edilmesi-gerekenler)
- [Proje Klasör Yapısı](#proje-klasör-yapısı)
- [Geliştirici Notları](#geliştirici-notları)

---

## Projenin Amacı

Proje Havuzu Sistemi'nin temel amacı, öğrencilerin proje önerilerini dijital ortamda oluşturabilmesini, akademisyenlerin bu projeleri inceleyerek onay veya red işlemi yapabilmesini ve yöneticilerin tüm süreci tek panel üzerinden takip edebilmesini sağlamaktır.

Bu sistem sayesinde:

- Proje başvuruları düzenli şekilde toplanır.
- Öğrenci ve akademisyen süreçleri birbirinden ayrılır.
- Proje durumları takip edilebilir.
- Yönetici paneli üzerinden sistem kontrolü sağlanır.
- Kullanıcı ve proje işlemleri kayıt altına alınabilir.

---

## Kullanılan Teknolojiler

Projede kullanılan temel teknolojiler şunlardır:

- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- HTML
- CSS
- JavaScript
- Bootstrap
- Git / GitHub

---

## Sistem Rolleri

Projede rol tabanlı kullanıcı yönetimi bulunmaktadır.

### Admin

Admin, sistem üzerinde yönetim yetkisine sahip kullanıcıdır.

Adminin yapabileceği işlemler:

- Kullanıcıları görüntüleme
- Öğrenci ve akademisyen kayıtlarını yönetme
- Projeleri görüntüleme
- Proje durumlarını takip etme
- Sistem loglarını inceleme
- Yönetim paneli üzerinden genel kontrol sağlama

### Öğrenci

Öğrenci, sisteme kayıt olup proje önerisi oluşturabilen kullanıcıdır.

Öğrencinin yapabileceği işlemler:

- Sisteme kayıt olma
- Giriş yapma
- Proje oluşturma
- Kendi projelerini görüntüleme
- Proje durumunu takip etme
- Danışman akademisyen seçme

### Akademisyen

Akademisyen, kendisine yönlendirilen projeleri inceleyen ve değerlendiren kullanıcıdır.

Akademisyenin yapabileceği işlemler:

- Kendisine gelen projeleri görüntüleme
- Proje detaylarını inceleme
- Projeyi onaylama veya reddetme
- Danışmanlık sürecini takip etme

---

## Temel Özellikler

- Kullanıcı kayıt sistemi
- Giriş ve çıkış işlemleri
- Rol bazlı yetkilendirme
- Şifre sıfırlama işlemleri
- Mail doğrulama sistemi
- Proje ekleme
- Proje listeleme
- Proje düzenleme
- Proje silme
- Akademisyen onay süreci
- Admin paneli
- Öğrenci paneli
- Akademisyen paneli
- Log kayıtları
- Veritabanı ilişkileri
- Responsive arayüz tasarımı

---

## Veritabanı Yapısı

Projede SQL Server veritabanı kullanılmıştır. Veritabanı işlemleri Entity Framework Core ile gerçekleştirilmiştir.

Sistemde temel olarak aşağıdaki veri yapıları bulunmaktadır:

- Kullanıcılar
- Roller
- Projeler
- Fakülteler
- Bölümler
- Proje talepleri
- Log kayıtları

Veritabanında kullanıcılar, roller, fakülteler, bölümler ve projeler arasında ilişkisel bir yapı bulunmaktadır.

> Not: Bu README dosyasında gerçek veritabanı adı, sunucu adı, kullanıcı adı, şifre veya bağlantı bilgileri paylaşılmamıştır.

---

## Gizlilik ve Veri Güvenliği

Bu projede gizlilik ve veri güvenliği açısından aşağıdaki noktalara dikkat edilmelidir:

### 1. Hassas Bilgiler GitHub'a Yüklenmemelidir

Aşağıdaki bilgiler kesinlikle GitHub'a yüklenmemelidir:

- Veritabanı bağlantı adresleri
- SQL Server kullanıcı adı ve şifreleri
- Mail adresi şifreleri
- SMTP kullanıcı bilgileri
- Admin kullanıcı bilgileri
- Gerçek kullanıcı verileri
- Öğrenci numarası, telefon, adres gibi kişisel bilgiler
- API key veya token bilgileri

### 2. `appsettings.json` Dosyasına Dikkat Edilmelidir

`appsettings.json` içerisinde gerçek bağlantı bilgileri varsa bu dosya GitHub'a yüklenmemelidir.

Bunun yerine örnek bir yapı paylaşılabilir:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=YOUR_DATABASE_NAME;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "MailSettings": {
    "Email": "your-email@example.com",
    "Password": "your-email-password",
    "Host": "smtp.example.com",
    "Port": 587
  }
}
```

Bu bilgiler sadece örnektir. Gerçek bilgiler proje deposuna eklenmemelidir.

### 3. Ortam Değişkenleri Kullanılmalıdır

Gizli bilgiler mümkün olduğunca ortam değişkenleri üzerinden yönetilmelidir.

Örneğin:

```bash
DB_CONNECTION_STRING=your_connection_string
MAIL_USERNAME=your_mail_username
MAIL_PASSWORD=your_mail_password
```

### 4. Gerçek Kullanıcı Verileri Paylaşılmamalıdır

Projeyi GitHub'a yüklemeden önce veritabanı yedeği, test verileri veya kişisel bilgiler kontrol edilmelidir.

Paylaşılmaması gereken örnek bilgiler:

- Gerçek öğrenci adları
- Gerçek akademisyen bilgileri
- Gerçek e-posta adresleri
- Öğrenci numaraları
- Şifreler
- Log kayıtlarındaki kişisel bilgiler

### 5. Şifreler Açık Metin Olarak Tutulmamalıdır

Kullanıcı şifreleri açık metin olarak saklanmamalıdır. ASP.NET Identity ile şifreler hashlenmiş şekilde tutulmalıdır.

### 6. Yetkilendirme Kontrolleri Yapılmalıdır

Her kullanıcı sadece kendi rolüne uygun sayfalara erişebilmelidir.

Örneğin:

- Öğrenci admin paneline erişememelidir.
- Öğrenci başka bir öğrencinin projesini düzenleyememelidir.
- Akademisyen sadece kendisine yönlendirilen projeleri görüntüleyebilmelidir.
- Admin işlemleri sadece admin rolündeki kullanıcılar tarafından yapılmalıdır.

### 7. Log Kayıtlarında Hassas Veri Tutulmamalıdır

Log kayıtlarında aşağıdaki bilgiler tutulmamalıdır:

- Şifre
- Token
- Mail doğrulama linki
- Şifre sıfırlama linki
- Veritabanı bağlantı bilgisi
- Kişisel veri içeren hata detayları

---

## Kurulum

Projeyi kendi bilgisayarınızda çalıştırmak için aşağıdaki adımları takip edebilirsiniz.

### 1. Projeyi Klonlayın

```bash
git clone REPOSITORY_URL
```

### 2. Proje Klasörüne Girin

```bash
cd proje-klasoru
```

### 3. Visual Studio ile Açın

Proje dosyasını Visual Studio ile açın.

### 4. Veritabanı Bağlantısını Ayarlayın

Kendi bilgisayarınıza uygun SQL Server bağlantısını yapılandırın.

Örnek bağlantı yapısı:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=ProjeHavuzuDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

> Bu bağlantı sadece örnektir. Gerçek bağlantı bilgileriniz GitHub'a yüklenmemelidir.

### 5. Migration İşlemlerini Çalıştırın

Package Manager Console üzerinden:

```bash
Update-Database
```

veya terminal üzerinden:

```bash
dotnet ef database update
```

### 6. Projeyi Çalıştırın

Visual Studio üzerinden projeyi başlatabilirsiniz.

---

## Ortam Değişkenleri ve Yapılandırma

Projede hassas bilgileri doğrudan kod içinde tutmak yerine ortam değişkenleri veya kullanıcı gizli ayarları kullanılması önerilir.

Örnek güvenli yaklaşım:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING"
dotnet user-secrets set "MailSettings:Email" "YOUR_EMAIL"
dotnet user-secrets set "MailSettings:Password" "YOUR_PASSWORD"
```

Bu yöntem sayesinde gizli bilgiler proje dosyaları içinde tutulmaz.

---

## GitHub'a Yüklerken Dikkat Edilmesi Gerekenler

Projeyi GitHub'a yüklemeden önce aşağıdaki kontroller yapılmalıdır:

- `appsettings.json` içerisinde gerçek şifre olmadığından emin olunmalıdır.
- Mail şifresi veya SMTP bilgileri paylaşılmamalıdır.
- Veritabanı yedeği yüklenmemelidir.
- `bin/` ve `obj/` klasörleri yüklenmemelidir.
- `.vs/` klasörü yüklenmemelidir.
- Kullanıcıya ait özel dosyalar yüklenmemelidir.
- Gerçek öğrenci veya akademisyen verileri paylaşılmamalıdır.
- Commit geçmişinde şifre olup olmadığı kontrol edilmelidir.

Önerilen `.gitignore` içeriği:

```gitignore
bin/
obj/
.vs/
*.user
*.suo
*.db
*.sqlite
*.bak
.env
appsettings.Production.json
appsettings.Development.json
```

> Eğer daha önce yanlışlıkla şifre veya bağlantı bilgisi commitlendiyse, sadece dosyayı silmek yeterli olmayabilir. Bu durumda şifreler değiştirilmelidir ve commit geçmişi temizlenmelidir.

---

## Proje Klasör Yapısı

Genel proje yapısı aşağıdaki gibidir:

```bash
ProjeHavuzu/
│
├── Controllers/
│   └── Sayfa yönlendirmeleri ve işlem kontrolleri
│
├── Models/
│   └── ViewModel ve yardımcı model sınıfları
│
├── Views/
│   └── Kullanıcı arayüz sayfaları
│
├── Data/
│   └── Veritabanı işlemleri, DbContext ve entity yapıları
│
├── Migrations/
│   └── Veritabanı migration dosyaları
│
├── wwwroot/
│   └── CSS, JavaScript ve görsel dosyalar
│
├── Mapping/
│   └── Entity ve ViewModel dönüşüm işlemleri
│
└── appsettings.json
    └── Proje yapılandırma ayarları
```

---

## Örnek Kullanım Akışı

1. Kullanıcı sisteme kayıt olur.
2. Kullanıcı rolüne göre ilgili panele yönlendirilir.
3. Öğrenci proje oluşturur.
4. Proje, seçilen akademisyenin paneline düşer.
5. Akademisyen projeyi onaylar veya reddeder.
6. Admin tüm proje sürecini sistem üzerinden takip eder.

---

## Geliştirici Notları

Bu proje eğitim amacıyla geliştirilmiştir. Proje geliştirilirken katmanlı yapı, rol bazlı yetkilendirme, Entity Framework Core, ASP.NET Identity ve veritabanı ilişkileri gibi temel yazılım geliştirme konularına dikkat edilmiştir.

Geliştirme sürecinde aşağıdaki noktalara önem verilmiştir:

- Temiz ve anlaşılır kod yazımı
- Rol bazlı erişim kontrolü
- Güvenli kullanıcı yönetimi
- Proje süreçlerinin takip edilebilir olması
- Veritabanı ilişkilerinin düzenli kurulması
- Arayüzlerin kullanıcı dostu olması

---

## Güvenlik Uyarısı

Bu proje GitHub üzerinde paylaşılmadan önce hassas bilgiler mutlaka kontrol edilmelidir. Özellikle bağlantı cümleleri, mail şifreleri, admin bilgileri ve gerçek kullanıcı verileri proje deposunda bulunmamalıdır.

---

## Lisans

Bu proje eğitim amacıyla hazırlanmıştır.
