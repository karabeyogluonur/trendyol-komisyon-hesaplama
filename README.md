# Trendyol Komisyon Hesaplama (TKH)

**Trendyol Komisyon Hesaplama (TKH)**, Trendyol pazaryerinde satış yapan işletmelerin finansal süreçlerini otomatize etmek, sipariş, ürün ve finansal hareket verilerini senkronize etmek ve detaylı komisyon hesaplamaları yapmak amacıyla geliştirilmiş modern bir web uygulamasıdır.

Bu proje, karmaşık pazaryeri entegrasyon süreçlerini basitleştirerek, işletmelerin karlılık analizlerini daha şeffaf bir şekilde yapabilmelerini sağlar. Özellikle Trendyol API ile tam entegre çalışarak finansal mutabakat süreçlerindeki insan hatasını minimize etmeyi hedefler.

## 🎯 Projenin Amacı

- **Otomasyon:** Manuel olarak takip edilen Trendyol finansal hareketlerinin (hakedişler, kesintiler, komisyonlar) otomatik olarak sisteme çekilmesi.
- **Doğruluk:** Sipariş bazlı karlılık ve komisyon hesaplamalarının hatasız yapılması.
- **Entegrasyon:** Trendyol OMS (Order Management System) ve Finans servisleriyle çift yönlü veya tek yönlü veri akışının sağlanması.
- **İzlenebilirlik:** Gelişmiş bir dashboard üzerinden anlık durum takibi.

## ✨ Özellikler

- **Çok Katmanlı Mimari (N-Tier Architecture):** Sürdürülebilir ve genişletilebilir kod tabanı.
- **Trendyol API Entegrasyonu:**
  - Sipariş Senkronizasyonu
  - Ürün/Stok Senkronizasyonu
  - Finansal Hareketler (Settlement) Entegrasyonu
- **Finansal Hesaplama Motoru:** İşlem bazlı gelir/gider eşleştirmesi.
- **Docker Desteği:** Konteynerize edilmiş uygulama ve veritabanı yapısı.
- **Gelişmiş Veritabanı Yönetimi:** PostgreSQL üzerinde Code-First yaklaşımı.
- **Validasyon ve Mapping:** FluentValidation ve AutoMapper ile güvenli veri işleme.
- **Modern Arayüz:** ASP.NET Core MVC ve Metronic tema altyapısı (kısmi).

## 🛠 Kullanılan Teknolojiler

### Backend & Core

- **Framework:** .NET 10
- **Dil:** C#
- **Web Framework:** ASP.NET Core MVC
- **ORM:** Entity Framework Core
- **Veritabanı:** PostgreSQL
- **Dependency Injection:** Microsoft DI

### Kütüphaneler & Araçlar

- **AutoMapper:** Nesne eşleme işlemleri için.
- **FluentValidation:** Veri doğrulama kuralları için.
- **Refit:** Trendyol API gibi HTTP tabanlı servislerle tip güvenli ve deklaratif entegrasyon sağlamak için.
- **Docker & Docker Compose:** Dağıtım ve ortam yönetimi için.

> _Not: `csproj` dosyasında `net10.0` belirtilmiştir. Çalıştırma ortamınızın uygun SDK sürümüne sahip olduğundan emin olun veya hedef sürümü güncel LTS sürümüne (örn. .NET 8/9) çekin._

## 🏗 Proje Mimarisi

Proje, **N-Tier Architecture** (Çok Katmanlı Mimari) prensiplerine benzer, gevşek bağlı (loosely coupled) bir katmanlı yapı izler.

```text
trendyol-komisyon-hesaplama/
├── src/
│   ├── TKH.Core/           # Evrensel nesneler, arayüzler, sabitler (Bağımsız Katman)
│   ├── TKH.DataAccess/     # Veritabanı erişimi, EF Core Context, Migrations
│   ├── TKH.Business/       # İş mantığı, Servisler, DTO'lar, Validasyonlar, API Entegrasyonları
│   └── TKH.Presentation/   # UI, Controllerlar, Viewlar, Program.cs (Web Katmanı)
├── docker-compose.yml      # Docker orkestrasyon dosyası
├── Dockerfile              # Web uygulaması için Docker imaj tanımı
└── .gitignore
```

## 📁 Önemli Klasörler

- **TKH.Business/Integrations**
  Trendyol API ile haberleşen Provider ve Service sınıflarını barındırır.

- **TKH.Business/Concrete/FinanceSyncService.cs**
  Finansal verilerin çekilmesi ve işlenmesinden sorumlu ana servis.

- **TKH.Presentation/Controllers**
  Kullanıcı isteklerini karşılayan MVC controller yapıları.

## 🚀 Kurulum

Projeyi yerel ortamınızda çalıştırmak için aşağıdaki adımları izleyin.

### Gereksinimler

- .NET SDK (Proje hedef sürümü)
- Docker Desktop (Önerilen)
- PostgreSQL (Docker kullanılmayacaksa yerel kurulum)
- Visual Studio 2022 veya VS Code

### Adım Adım Kurulum

#### Repoyu Klonlayın

```bash
git clone https://github.com/karabeyogluonur/trendyol-komisyon-hesaplama.git
cd trendyol-komisyon-hesaplama
```

#### Bağımlılıkları Yükleyin

```bash
dotnet restore
```

#### Yapılandırma Dosyasını Düzenleyin

`src/TKH.Presentation/appsettings.json` (veya `Development.json`) dosyasındaki veritabanı bağlantı bilgilerini kontrol edin.

## 🏃 Çalıştırma

Projeyi çalıştırmak için iki yöntem bulunmaktadır: Docker ile veya Yerel (.NET CLI) ile.

### Seçenek 1: Docker ile Çalıştırma (Önerilen)

```bash
docker-compose up --build
```

- PostgreSQL veritabanını **5434** portunda ayağa kaldırır.
- Web uygulamasını **8081** portunda yayınlar.
- Veritabanı sağlık kontrollerini (healthcheck) yapar.

Tarayıcı: `http://localhost:8081`

### Seçenek 2: Local Ortamda Çalıştırma

```bash
cd src/TKH.Presentation
dotnet run
```

Varsayılan adresler:
- http://localhost:5000
- https://localhost:5001

## ⚙️ Ortam Değişkenleri

```yaml
environment:
  ASPNETCORE_ENVIRONMENT: Development
  ConnectionStrings__DefaultConnection: "Host=tkh.db;Port=5432;Database=TKHDb;Username=tkh_user;Password=StrongPostgresPass123!"
```

### Kritik Değişkenler

- `ConnectionStrings__DefaultConnection`
- Trendyol API anahtarları (`SupplierId`, `ApiKey`, `ApiSecret`)

## 🧪 Geliştirme Rehberi

- Clean Code prensiplerine uyulmalıdır.
- Yeni servisler `ServiceRegistration.cs` dosyasına eklenmelidir.
- Migration işlemleri:

```bash
dotnet ef migrations add <MigrationName> --project ../TKH.DataAccess --startup-project .
dotnet ef database update
```


## 🤝 Katkıda Bulunma

1. Fork'layın
2. Branch oluşturun (`feature/yeni-ozellik`)
3. Commit alın
4. Push edin
5. Pull Request açın

## 📄 Lisans

Bu proje şu an için herhangi bir açık kaynak lisansı (MIT, Apache 2.0 vb.) barındırmamaktadır veya lisans dosyası paylaşılmamıştır. Kodların kullanımı ve dağıtımı ile ilgili haklar repo sahibine aittir.
