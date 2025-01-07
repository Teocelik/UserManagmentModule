# User Management Module

ASP.NET Core MVC ile geliştirilmiş bir Kullanıcı Yönetim Modülü. Bu modül, kullanıcı kayıt, giriş, şifre sıfırlama ve rol tabanlı yetkilendirme gibi temel özellikleri içerir. Güvenlik, veri doğrulama ve hata yönetimi gibi becerileri geliştirmek amacıyla oluşturulmuştur.

## Özellikler

- **Kullanıcı Kaydı**: Yeni kullanıcıların sisteme kayıt olmasını sağlar.
- **Kullanıcı Girişi**: Kayıtlı kullanıcıların sisteme giriş yapmasını sağlar.
- **Şifre Sıfırlama**: Kullanıcıların unutulan şifrelerini sıfırlamalarına olanak tanır.
- **Rol Tabanlı Yetkilendirme**: Farklı kullanıcı rolleri tanımlayarak, her role özel yetkilendirme sağlar.

## Kullanılan Kütüphaneler

- **ASP.NET Core Identity**: Kullanıcı kimlik doğrulama ve yetkilendirme işlemleri için kullanılmıştır.
- **Entity Framework Core**: Veritabanı işlemlerini gerçekleştirmek ve ORM (Object-Relational Mapping) desteği sağlamak için kullanılmıştır.
- **Microsoft.AspNetCore.Mvc**: MVC mimarisi için temel altyapıyı sağlamak için kullanılmıştır.
- **Newtonsoft.Json**: Verilerin JSON formatında işlenmesi için kullanılır.
- **Microsoft.Extensions.DependencyInjection**: Bağımlılık enjeksiyonu desteği sağlamak için kullanılmıştır.

## Kurulum

### 1. Depoyu Klonlayın

```bash
git clone https://github.com/Teocelik/UserManagmentModule.git
```

### 2. Gerekli Bağımlılıkları Yükleyin

Proje klasöründe, gerekli NuGet paketlerini yüklemek için aşağıdaki komutu çalıştırın:

```bash
dotnet restore
```

### 3. Veritabanı Ayarlarını Yapın

`appsettings.json` dosyasında, veritabanı bağlantı dizesini kendi veritabanı bilgilerinizle güncelleyin.

### 4. Veritabanını Güncelleyin

Entity Framework Core kullanarak veritabanını güncelleyin:

```bash
dotnet ef database update
```

### 5. Uygulamayı Çalıştırın

```bash
dotnet run
```

## Katkıda Bulunma

Katkılarınızı memnuniyetle karşılıyorum. Lütfen önce bir konu açarak neyi değiştirmek istediğinizi tartışın. Değişiklikleriniz onaylandıktan sonra bir pull request oluşturabilirsiniz.
