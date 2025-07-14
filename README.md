# CourseTech - Mini Online Kurs Satış Platformu
## İçindekiler

1. [🚀 Proje Hakkında](#-proje-hakkında)
2. [📌 Ana Özellikler](#-ana-özellikler)
   - [👥 Kullanıcı Rolleri ve Yetkileri](#-kullanıcı-rolleri-ve-yetkileri)
   - [🏗️ Teknik Mimari](#️-teknik-mimari)
   - [📊 Veritabanı Yapısı](#-veritabanı-yapısı)
3. [🔧 Backend Kurulumu](#-backend-kurulumu)
4. [🔧 Frontend Kurulumu](#-frontend-kurulumu)
5. [🛠️ Kullanılan Teknolojiler](#️-kullanılan-teknolojiler)
6. [👤 Varsayılan Kullanıcılar](#-varsayılan-kullanıcılar)
7. [🔄 Kullanım Akışı](#kullanım-akışı)
8. [📑 API Endpoints](#-some-api-endpoints)
   - [🔑 Authentication](#-authentication)
   - [🛒 Basket](#-basket)
   - [📚 Courses](#-courses)
   - [👤 Users](#-users)
   - [💳 Payments](#-payments)
   - [📦 Orders](#-orders)
9. [🛡️ Güvenlik Yapılandırması](#️-güvenlik-yapılandırması)
10. [📦 Kullanılan Pattern'ler](#-kullanılan-patternler)
11. [🔍 Önerilen Geliştirmeler](#-önerilen-geliştirmeler)
12. [📄 Lisans](#-lisans)

---

## 🚀 Proje Hakkında

```plaintext
Backend  : ASP.NET Core 9 Web API
Frontend : React.js
```
CourseTech, ASP.NET Core 9 Web API backend ve React.js frontend kullanılarak geliştirilmiş modern ve responsive bir mini online kurs satış platformudur. Platform, eğitmenlerin kurs oluşturup satmasına ve öğrencilerin eğitim içeriklerine kolayca erişmesine olanak tanır ve geliştirilebilir bir altyapı sunar.

## 📌 Ana Özellikler

### 👥 Kullanıcı Rolleri ve Yetkileri

- **Admin**: Tam sistem erişimi, yönetim yetkileri.
- **Instructor**: Kurs oluşturma, kurs yönetimi, kurs satın alma, profil yönetimi.
- **Student**: Kurs satın alma, kurs erişimi, profil yönetimi.
- **Guest**: Kurs inceleme, kategori detaylarına göz atma.


### 🏗️ Teknik Mimari

#### Backend Yapısı
Katmanlı mimari yapı uygulanmıştır:
```plaintext
Core        -> Entity'ler, DTO'lar, Service Interface'leri, UnitOfWork Interface'leri
Repository  -> Veritabanı işlemleri, Data Seed, Konfigürasyonlar
Service     -> İş Mantığı, Mapping İşlemleri
API         -> Controller'lar
Shared      -> Enumlar, Service Result, JWT Yapısı
Client      -> React.js Frontend
```

### 📊 Veritabanı Yapısı
```sql
Tables:
- Identity (AppUser, AppRole, AspNetUserRefreshToken)
- Course
- Category
- Basket
- BasketItem
- Order
- OrderItem
- Payment
- Enrollment
```

### 📊 Base Entity Yapısı
**Entityler arasındaki ilişkiler (One-to-Many, Many-to-Many, Many-to-One) tanımlanmıştır.**

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; private set; } = false;

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        Update();
    }

    public void Update()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
```

**Generic Repository Yapısı:**
```csharp
public interface IGenericRepository<TEntity>
    where TEntity : BaseEntity
{
    Task<TEntity> GetByIdAsync(Guid id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
    Task InsertAsync(TEntity entity);
    TEntity Update(TEntity entity);
    void SoftDelete(TEntity entity);
}
```
**UnitOfWork Yapısı:**
```csharp
 public interface IUnitOfWork
 {
     ICategoryRepository Category { get; }
     ICourseRepository Course { get; }
     IEnrollmentRepository Enrollment { get; }
     IBasketRepository Basket { get; }
     IAppUserRefreshTokenRepository AppUserRefreshToken { get; }
     IPaymentRepository Payment { get; }
     IOrderRepository Order { get; }
     Task SaveChangesAsync();
     void SaveChanges();
 }
```
- **Entitylere özel genişletilmiş ek servisler yazılmıştır**

### 📝 Enum Tanımları
```csharp
public enum BasketStatus
{
    Active,
    Passive
}

public enum PaymentStatus
{
    Pending,
    Success,
    Failed
}

public enum OrderStatus
{
    Pending,
    Completed,
    Canceled
}
```
---
# 🔧 Backend Kurulumu
### 1. Projeyi Klonlayın veya İndirin
GitHub deposunu bilgisayarınıza klonlayın:
```bash
git clone https://github.com/GithubUserName/CourseTechPlatform.git
```
1. **`appsettings.json`** dosyasında MSSQL Server bilgilerinizi girin.
2. Migration oluşturun ve veritabanını güncelleyin:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
3. Backend sunucusunu başlatın:
   ```bash
   dotnet run
   ```

# 🔧 Frontend Kurulumu
1. Frontend dizinine gidin:
   ```bash
   cd coursetech.Client.client
   ```
2. Paketleri yükleyin:
   ```bash
   npm install
   ```
3. Frontend sunucusunu başlatın:
   ```bash
   npm start
   ```
---

## 🛠️ Kullanılan Teknolojiler
### Backend Teknolojileri
- **ASP.NET Core 9**
- **Entity Framework Core** (EF Core)
- **JWT Authentication**
- **UnitOfWork Pattern**
- **Repository Pattern**

### Frontend Teknolojileri
- **React.js**
- **Axios**
- **Toastify**

CORS yapısı ile frontend-backend iletişimi sağlanmıştır.

---

## 👤 Varsayılan Kullanıcılar
```json
{
    "admin": {
        "email": "admin@admin.com",
        "role": "Admin",
        "password": "Admin123*"
    },
    "instructor1": {
        "email": "fatihcakiroglu@instructor.com",
        "role": "Instructor",
        "password": "Fatih123**"
    },
    "instructor2": {
        "email": "ahmet@instructor.com",
        "role": "Instructor",
        "password": "Ahmet123*"
    }
}
```

# Kullanım Akışı

- Kullanıcı Kaydı ve Giriş

Kullanıcı kaydı oluşturan herkes **Student** rolü ile atanır.


### 🔄 Akış: Sepet - Order - Ödeme - Kurs Erişim
1. Kullanıcıya aktif bir sepet tanımlanır.
2. Kurslar sepete eklenir.
3.  
    - Checkout yapıldıktan sonra sepet **Passive** duruma geçer, 
    - Sipariş kaydı **Pending** olarak oluşturulur. 
4. Ödeme yap butonuna tıklanır 
- Ödeme **Pending** durumuna gelir.
- Sipariş kaydı **Completed* olur.
- Kart bilgileri doldurulur (Luhn algoritmasına göre doğrulanır).
5. - Ödeme tamamlandıktan sonra Payment **Completed** durumuna geçer,  
   - Kullanıcı ile kurs arasında **Enrollment** ilişkisi kurulur. **Enrollment** kaydı oluşturulur.
6. Profil sayfasında Ödeme Geçmişi, Sipariş Geçmişi, ve Kurslarım özet halinde gösterilir.

---

# 📑 Some API Endpoints

### 🔑 Authentication
- **POST** `/api/Authentications/login` - Kullanıcı giriş yapar.
- **POST** `/api/Authentications/create-token-by-refresh-token` - Refresh token ile yeni bir erişim tokenı oluşturur.
- **POST** `/api/Authentications/revoke-refresh-token` - Refresh token'ı geçersiz kılar.

### 🛒 Basket
- **GET** `/api/Baskets/{userId}` - Kullanıcının sepetini getirir.
- **POST** `/api/Baskets/users/{userId}/courses/{courseId}` - Kursu sepete ekler.
- **DELETE** `/api/Baskets/users/{userId}/courses/{courseId}` - Sepetten kursu çıkarır.
- **DELETE** `/api/Baskets/users/{userId}` - Kullanıcının sepetini siler.
- **POST** `/api/Baskets/users/{userId}/complete` - Sepeti tamamlar.
- **GET** `/api/Baskets/{basketId}/admin` - Admin için sepet detaylarını getirir.

### 📚 Courses
- **GET** `/api/Courses/{id}` - Belirli bir kursu getirir.
- **DELETE** `/api/Courses/{id}` - Kursu siler.
- **GET** `/api/Courses` - Tüm kursları listeler.
- **POST** `/api/Courses` - Yeni bir kurs oluşturur.
- **PUT** `/api/Courses` - Mevcut kursu günceller.
- **GET** `/api/Courses/published` - Yayınlanmış kursları listeler.
- **GET** `/api/Courses/by-category/{categoryId}` - Belirli bir kategoriye ait kursları listeler.
- **GET** `/api/Courses/by-instructor/{instructorId}` - Belirli bir eğitmene ait kursları listeler.
- **GET** `/api/Courses/details/{id}` - Kursun detaylarını getirir.
- **GET** `/api/Courses/summaries` - Kurs özetlerini listeler.
- **PATCH** `/api/Courses/{id}/publish` - Kursu yayınlar.
- **PATCH** `/api/Courses/{id}/unpublish` - Kursun yayını kaldırılır.

### 👤 Users
- **GET** `/api/Users/{id}` - Kullanıcı bilgilerini getirir.
- **PUT** `/api/Users/{id}` - Kullanıcı bilgilerini günceller.
- **DELETE** `/api/Users/{id}` - Kullanıcıyı siler.
- **GET** `/api/Users/instructors` - Eğitmenleri listeler.
- **GET** `/api/Users/students` - Öğrencileri listeler.
- **GET** `/api/Users/all` - Tüm kullanıcıları listeler.
- **POST** `/api/Users/register` - Yeni bir kullanıcı kaydı oluşturur.
- **POST** `/api/Users/reset-password` - Kullanıcının şifresini sıfırlar.

### 💳 Payments
- **POST** `/api/Payments` - Ödeme işlemi başlatır.
- **GET** `/api/Payments/{userId}/user` - Kullanıcının tüm başarılı ödeme geçmişlerini getirir.
- **GET** `/api/Payments/{paymentId}` - Belirli bir ödemenin detaylarını getirir.

### 📦 Orders
- **POST** `/api/Orders/from-basket/{basketId}` - Sepetten siparişi oluşturur.
- **GET** `/api/Orders/{orderId}` - Sipariş detaylarını getirir.
- **GET** `/api/Orders/user/{userId}` - Kullanıcının siparişlerini listeler.

---
## 🛡️ Güvenlik Yapılandırması

- JWT tabanlı kimlik doğrulama ve yetkilendirme.

- Access Token ve Refresh Token desteği.

- Rol bazlı güvenlik sistemi.

- Entity Framework Identity ile entegre.


```javascript
const security = {
    authentication: "JWT Bearer Token",
    refreshToken: true,
    authorization: "Role Based Access Control",
    validation: {
        form: true,
        input: true
    },
    cors: "Configured",
    passwordStorage: "Hashed + Salted"
};
```



## 📦 Kullanılan Pattern'ler
```javascript
const patterns = {
    architecture: [
        "Layered Architecture",
        "Repository Pattern",
        "Unit of Work Pattern"
        "Service Result Pattern"
    ],
    principles: [
        "SOLID"
    ],
    features: [
        "Generic Repository",
        "Soft Delete",
        "Rich Domain Model", 
    ]
};
```

## 🔍 Önerilen Geliştirmeler
```javascript
const futureFeatures = [
    "RabbitMQ entegrasyonu",
    "Redis cache implementasyonu",
    "Admin paneli",
    "Kurs güncelleme",
    "Gerçek ödeme sistemi",
    "Email bildirimleri",
    "Gelişmiş arama",
    "Değerlendirme sistemi",
    "İlerleme takibi",
    "İnteraktif içerik görüntüleyici"
    "Unit testler",
    "Docker desteği",
    "CI/CD pipeline",
    "Performans optimizasyonu"
];
```

## 📄 Lisans
```plaintext
MIT License

Copyright (c) 2025 CourseTech

Bu yazılım MIT lisansı altında lisanslanmıştır.
```
