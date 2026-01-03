# Trendyol Komisyon Hesaplama (TKH)

![.NET](https://img.shields.io/badge/.NET-10-blueviolet) ![PostgreSQL](https://img.shields.io/badge/PostgreSQL-blue) ![Docker](https://img.shields.io/badge/Docker-2496ED?logo=docker&logoColor=white) ![Hangfire](https://img.shields.io/badge/Hangfire-grey) ![Serilog](https://img.shields.io/badge/Serilog-green)

**Trendyol Komisyon Hesaplama (TKH)**, Trendyol pazaryerinde satış yapan işletmelerin finansal süreçlerini otomatize etmek, sipariş, ürün ve finansal hareket verilerini senkronize etmek ve detaylı komisyon hesaplamaları yapmak amacıyla geliştirilmiş modern bir web uygulamasıdır.

Bu proje, karmaşık pazaryeri entegrasyon süreçlerini basitleştirerek, işletmelerin karlılık analizlerini daha şeffaf bir şekilde yapabilmelerini sağlar. Özellikle Trendyol API ile tam entegre çalışarak finansal mutabakat süreçlerindeki insan hatasını minimize etmeyi hedefler.

<a href="https://buymeacoffee.com/karabeyogluonur" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>

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
- **Arkaplan İş Yönetimi (Background Jobs):** Hangfire entegrasyonu ile veri senkronizasyonu gibi uzun süren işlemlerin asenkron yönetimi, zamanlanması ve hata toleransı (retry mechanism).
- **Merkezi Loglama ve İzleme:** Serilog ve Seq entegrasyonu ile uygulama loglarının yapılandırılmış (structured) şekilde toplanması, görselleştirilmesi ve analizi.
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

- **Hangfire:** Arkaplan işleri (Background Jobs) ve tekrarlayan görevlerin yönetimi için.
- **Serilog:** Yapılandırılmış loglama (structured logging) altyapısı için.
- **Seq:** Logların merkezi sunucuda toplanması ve dashboard üzerinden izlenmesi için.
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
│   └── TKH.Web/   # UI, Controllerlar, Viewlar, Program.cs (Web Katmanı)
├── docker-compose.yml      # Docker orkestrasyon dosyası (App, Db, Seq)
├── Dockerfile              # Web uygulaması için Docker imaj tanımı
└── .gitignore
```

#### 📁 Önemli Klasörler
- **TKH.Business/Integrations:** Trendyol API ile haberleşen Provider ve Service sınıflarını barındırır.
- **TKH.Business/Concrete/FinanceSyncService.cs:** Finansal verilerin çekilmesi ve işlenmesinden sorumlu ana servis.
- **TKH.Web/Controllers:** Kullanıcı isteklerini karşılayan MVC controller yapıları.

## 🚀 Kurulum

Projeyi yerel ortamınızda çalıştırmak için aşağıdaki adımları izleyin.

### Gereksinimler
- .NET SDK (Proje hedef sürümü)
- Docker Desktop (Önerilen)
- PostgreSQL (Docker kullanılmayacaksa yerel kurulum)
- Visual Studio 2022 veya VS Code

### Adım Adım Kurulum

1.  **Repoyu Klonlayın**
    ```bash
    git clone https://github.com/karabeyogluonur/trendyol-komisyon-hesaplama.git
    cd trendyol-komisyon-hesaplama
    ```

2.  **Bağımlılıkları Yükleyin**
    ```bash
    dotnet restore
    ```

3.  **Yapılandırma Dosyasını Düzenleyin**
    `src/TKH.Web/appsettings.json` (veya `Development.json`) dosyasındaki veritabanı ve loglama bağlantı bilgilerini kontrol edin.

## 🏃 Çalıştırma

Projeyi çalıştırmak için iki yöntem bulunmaktadır: Docker ile veya Yerel (.NET CLI) ile.

### Seçenek 1: Docker ile Çalıştırma (Önerilen)

```bash
docker-compose up --build
```

Komut sonrası aşağıdaki servisler ayağa kalkacaktır:

- **Web Uygulaması:** http://localhost:8081
- **PostgreSQL Veritabanı:** 5434 portu.
- **Seq Log Arayüzü:** http://localhost:5341 (Logları buradan izleyebilirsiniz)
- **Hangfire Dashboard:** http://localhost:8081/hangfire (Arkaplan işlerini buradan yönetebilirsiniz)

### Seçenek 2: Local Ortamda Çalıştırma

```bash
cd src/TKH.Web
dotnet run
```

Varsayılan adresler:
- http://localhost:5000
- https://localhost:5001

> **Not:** Local çalıştırmada Seq sunucusunun Docker üzerinde veya yerel olarak 5341 portunda çalışır durumda olduğundan emin olun.

## ⚙️ Ortam Değişkenleri

```yaml
environment:
  ASPNETCORE_ENVIRONMENT: Development
  ConnectionStrings__DefaultConnection: "Host=tkh.db;Port=5432;Database=TKHDb;Username=tkh_user;Password=StrongPostgresPass123!"
  Serilog__WriteTo__0__Args__serverUrl: "http://seq:5341"
```

#### Kritik Değişkenler
- `ConnectionStrings__DefaultConnection`
- `Serilog__WriteTo__...__serverUrl` (Seq Sunucu Adresi)
- `Trendyol API anahtarları` (SupplierId, ApiKey, ApiSecret)

## 🧪 Geliştirme Rehberi

- Clean Code prensiplerine uyulmalıdır.
- Yeni servisler `ServiceRegistration.cs` dosyasına eklenmelidir.
- Migration işlemleri:

  ```bash
  dotnet ef migrations add <MigrationName> --project ../TKH.DataAccess --startup-project .
  dotnet ef database update
  ```

## 🤝 Katkıda Bulunma

1.  Fork'layın
2.  Branch oluşturun (`feature/yeni-ozellik`)
3.  Commit alın
4.  Push edin
5.  Pull Request açın

## 📄 Lisans

Bu proje şu an için herhangi bir açık kaynak lisansı (MIT, Apache 2.0 vb.) barındırmamaktadır veya lisans dosyası paylaşılmamıştır. Kodların kullanımı ve dağıtımı ile ilgili haklar repo sahibine aittir.
