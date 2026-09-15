# 🚀 Modern .NET Web API: Clean Architecture & Vertical Slice Template (မြန်မာဘာသာ)

[![CI](https://github.com/YellMinNaing-micro/CleanArchitecture.VerticalSlice.Template/actions/workflows/ci.yml/badge.svg)](https://github.com/YellMinNaing-micro/CleanArchitecture.VerticalSlice.Template/actions/workflows/ci.yml)
[![CodeQL](https://github.com/YellMinNaing-micro/CleanArchitecture.VerticalSlice.Template/actions/workflows/codeql.yml/badge.svg)](https://github.com/YellMinNaing-micro/CleanArchitecture.VerticalSlice.Template/actions/workflows/codeql.yml)
[![codecov](https://codecov.io/gh/YellMinNaing-micro/CleanArchitecture.VerticalSlice.Template/branch/main/graph/badge.svg)](https://codecov.io/gh/YellMinNaing-micro/CleanArchitecture.VerticalSlice.Template)

[Read in English (အင်္ဂလိပ်ဘာသာဖြင့် ဖတ်ရှုရန်)](README.md)

ဤ Repository သည် Enterprise-ready ဖြစ်ပြီး zero-configuration ဖြင့် မောင်းနှင်နိုင်သော **ASP.NET Core (.NET 10)** Web API Template ဖြစ်သည်။ ၎င်းသည် စနစ်တကျရှိသော layer boundary များ ရရှိစေရန် **Clean Architecture** အခြေခံစည်းမျဉ်းများဖြင့် ဖွဲ့စည်းထားပြီး၊ Core Application Layer အတွင်း၌ လုပ်ဆောင်ချက်များကို သန့်ရှင်းသပ်ရပ်စွာ ပြင်ဆင်နိုင်ရန် **Vertical Slice Architecture (Feature Folders)** တည်ဆောက်ပုံစနစ်ကို ပေါင်းစပ်အသုံးပြုထားသည်။

---

## 🏗️ Architecture Design နှင့် သဘောတရား

ဤ Template တွင် ခေတ်မီဆော့ဖ်ဝဲလ်ပရောဂျက်ကြီးများတွင် ကြုံတွေ့ရလေ့ရှိသော ပြဿနာများကို ဖြေရှင်းနိုင်ရန် ဖွဲ့စည်းပုံနှစ်ခုကို ပေါင်းစပ်အသုံးပြုထားပါသည်-
1. **Clean Architecture (Horizontal Project Boundaries)**: လုပ်ငန်းလုပ်ဆောင်ချက်ဆိုင်ရာ Core Business Rules (Domain & Application Logic) များကို ပြင်ပ database များ၊ UI frameworks များနှင့် အခြားသော ပြင်ပ tools များမှ ကင်းလွတ်ပြီး သီးခြားလွတ်လပ်စွာ တည်ရှိနိုင်ရန် Layer များ ခွဲခြားထားသည်။
2. **Vertical Slice / Feature Folders (Vertical Organization)**: Application Layer အတွင်း၌ Logic များကို technical types များအလိုက် စုစည်းခြင်းမပြုဘဲ **Features** (ဥပမာ- `Products`, `Orders`) အလိုက် စုစည်းထားသည်။ ထို့ကြောင့် Use Case တစ်ခု (ဥပမာ- Product အသစ်ဆောက်ခြင်း) အတွက် လိုအပ်သော Commands, Queries, DTOs နှင့် Validators အားလုံးသည် folder တစ်ခုတည်းအောက်တွင် စုစည်းတည်ရှိနေသည်။

```mermaid
graph TD
    %% Project Layers %%
    WebApi[CleanArch.WebApi<br>Presentation Layer] --> Infrastructure[CleanArch.Infrastructure<br>Infrastructure Layer]
    WebApi --> Application[CleanArch.Application<br>Core Business Logic]
    Infrastructure --> Application
    Application --> Domain[CleanArch.Domain<br>Enterprise Domain Layer]

    style Domain fill:#f9f,stroke:#333,stroke-width:2px
    style Application fill:#bbf,stroke:#333,stroke-width:2px
    style Infrastructure fill:#fbb,stroke:#333,stroke-width:2px
    style WebApi fill:#bfb,stroke:#333,stroke-width:2px
```

---

## 📂 အသေးစိတ် Folder ဖွဲ့စည်းပုံ

Project directory အတွင်း ဖိုင်များ မည်သို့တည်ရှိပြီး ဖွဲ့စည်းထားပုံကို အောက်ပါအတိုင်း တွေ့မြင်နိုင်သည်-

```text
CleanArch/
├── CleanArch.slnx                              # Modern XML-based Solution file
├── .gitignore                                  # C#/.NET, Visual Studio နှင့် SQLite ဆိုင်ရာ git ignore rules
├── README.md                                   # အင်္ဂလိပ်ဘာသာဖြင့် ရေးထားသော လမ်းညွှန်ချက် (Default)
├── README.my.md                                # မြန်မာဘာသာဖြင့် ရေးထားသော လမ်းညွှန်ချက်
└── src/
    ├── CleanArch.Domain/                       # Domain Layer (Pure C#, ပြင်ပ dependency မပါဝင်ပါ)
    │   ├── Common/
    │   │   └── BaseEntity.cs                   # Id နှင့် Audit fields များ ပါဝင်သော base abstract class
    │   └── Entities/
    │       └── Product.cs                      # Product domain entity
    │
    ├── CleanArch.Application/                  # Application Layer (Core business rules)
    │   ├── Common/
    │   │   ├── Behaviors/
    │   │   │   └── ValidationBehavior.cs       # MediatR validation pipeline interceptor
    │   │   └── Interfaces/
    │   │       ├── IProductRepository.cs       # database access contract interface
    │   │       └── IUser.cs                    # Auditing အတွက် current user abstraction
    │   ├── Features/
    │   │   └── Products/                       # Product features နှင့်ပတ်သက်သော vertical slices
    │   │       ├── Commands/
    │   │       │   ├── CreateProduct/
    │   │       │   │   ├── CreateProductCommand.cs
    │   │       │   │   └── CreateProductCommandValidator.cs
    │   │       │   ├── UpdateProduct/
    │   │       │   │   └── UpdateProductCommand.cs
    │   │       │   └── DeleteProduct/
    │   │       │       └── DeleteProductCommand.cs
    │   │       └── Queries/
    │   │           ├── GetProductById/
    │   │           │   └── GetProductByIdQuery.cs
    │   │           └── GetProducts/
    │   │               ├── GetProductsQuery.cs
    │   │               └── ProductDto.cs
    │   └── DependencyInjection.cs               # MediatR နှင့် FluentValidation များကို DI register လုပ်ခြင်း
    │
    ├── CleanArch.Infrastructure/               # Infrastructure Layer (ပြင်ပ Tools နှင့် Persistence)
    │   ├── Persistence/
    │   │   ├── Interceptors/
    │   │   │   └── AuditableEntityInterceptor.cs # Audit fields များ auto ဖြည့်ပေးမည့် SaveChangesInterceptor
    │   │   ├── Repositories/
    │   │   │   └── ProductRepository.cs        # EF Core သုံးပြီး ရေးထားသော database repo implementation
    │   │   ├── ApplicationDbContext.cs         # Entity Framework db context
    │   │   └── ApplicationDbContextInitializer.cs  # local development database Auto-Seed ပြုလုပ်ပေးသည့် class
    │   └── DependencyInjection.cs               # DbContext, Interceptors နှင့် repositories များကို register လုပ်ခြင်း
    │
    └── CleanArch.WebApi/                       # Presentation Layer (API Host & Host configuration)
        ├── Controllers/
        │   ├── ApiControllerBase.cs            # MediatR ISender ကို inject လုပ်ပေးထားသော base controller
        │   └── ProductsController.cs           # Product REST endpoints controllers
        ├── Middleware/
        │   └── ApiExceptionHandlingMiddleware.cs # Error များကို RFC 7807 problem details အဖြစ်ပြောင်းပေးသော middleware
        ├── Services/
        │   └── CurrentUser.cs                  # HttpContext မှတစ်ဆင့် User claim ရယူပေးမည့် service
        ├── Properties/
        │   └── launchSettings.json             # port များနှင့် browser autostart စာမျက်နှာကို သတ်မှတ်ခြင်း
        ├── appsettings.json                    # database connections နှင့် logs setup configurations
        └── Program.cs                          # project စတင်မောင်းနှင်ရာ ဝင်ပေါက် (Entry point)
└── tests/
    ├── CleanArch.ArchitectureTests/            # Layer dependency စည်းမျဉ်းများ (NetArchTest)
    ├── CleanArch.IntegrationTests/             # In-memory SQLite API tests (WebApplicationFactory)
    └── CleanArch.UnitTests/                    # Unit Tests ပရောဂျက် (xUnit, FluentAssertions, NSubstitute)
        ├── Domain/                             # Domain entity စမ်းသပ်ချက်များ
        ├── Application/                        # Command, query, validator နှင့် pipeline behavior စမ်းသပ်ချက်များ
        ├── Infrastructure/                     # EF Core interceptor နှင့် persistence စမ်းသပ်ချက်များ
        └── WebApi/                             # Controller နှင့် middleware စမ်းသပ်ချက်များ
```

---

## 🔄 Request လုပ်ဆောင်ချက် စီးဆင်းပုံ (Execution Flow)

Client ဆီမှ ဝင်လာသော HTTP Request သည် Mediator Pattern အား အသုံးပြု၍ Layer စည်းမျဉ်းများအတိုင်း အောက်ပါအတိုင်း စီးဆင်းလုပ်ဆောင်သွားသည်-

```text
 [ HTTP POST /api/products ]
            │
            ▼
┌───────────────────────┐
│  ProductsController   │  <-- Endpoint မှ JSON payload ကို လက်ခံရယူသည်
└───────────┬───────────┘
            │  (MediatR မှတစ်ဆင့် CreateProductCommand ကို ပို့ဆောင်သည်)
            ▼
┌───────────────────────┐
│   ValidationBehavior  │  <-- Pipeline Interceptor မှ FluentValidation စည်းမျဉ်းများကို စစ်ဆေးသည်။
└───────────┬───────────┘      အကယ်၍ validation မအောင်မြင်ပါက ValidationException ကို throw လုပ်သည်။
            │  (အောင်မြင်ပါက ရှေ့သို့ ဆက်သွားသည်)
            ▼
┌───────────────────────┐
│   CreateProductCmd    │
│        Handler        │  <-- လုပ်ဆောင်ချက် logic ကို run ပြီး DTO ကို Domain Entity အဖြစ်ပြောင်းသည်။
└───────────┬───────────┘
            │  (IProductRepository.AddAsync ကို ခေါ်ယူသည်)
            ▼
┌───────────────────────┐
│   ProductRepository   │  <-- Infrastructure implementation မှ EF Core သုံး၍ database sql queries ရေးသည်။
└───────────┬───────────┘
            │
            ▼
┌───────────────────────┐
│  SQLite Local DB      │  <-- Product data ကို SQLite local disk ဖိုင် (CleanArch.db) ထဲသို့ သိမ်းဆည်းသည်။
└───────────────────────┘
```

---

## 🛠️ အသုံးပြုထားသော အဓိကနည်းပညာများနှင့် Packages

- **Runtime:** .NET 10.0
- **Database Context (ORM):** Entity Framework Core 10
- **Database Engine:** SQLite (`Microsoft.EntityFrameworkCore.Sqlite`)
- **CQRS Pattern Orchestrator:** MediatR (`MediatR`)
- **Model Validation:** FluentValidation (`FluentValidation.DependencyInjectionExtensions`)
- **API Documentation & Testing UI:** Scalar (`Scalar.AspNetCore` နှင့် Native OpenAPI Document Generation)
- **Health Checks & Monitoring:** ASP.NET Core Diagnostics နှင့် EF Core DbContext Check (`Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore`)

---

## ✅ အလိုအလျောက် အရည်အသွေးစစ်ဆေးမှုများ

- **GitHub Actions CI:** `main` သို့ push သို့မဟုတ် pull request ပြုလုပ်တိုင်း dependencies restore၊ build၊ code formatting နှင့် tests အားလုံးကို စစ်ဆေးပေးသည်။
- **Codecov:** Line နှင့် branch coverage ကို တင်ပေးပြီး test မရောက်သေးသောနေရာများနှင့် pull request တစ်ခုချင်းစီ၏ coverage အပြောင်းအလဲကို ကြည့်ရှုနိုင်စေသည်။
- **CodeQL:** Push၊ pull request နှင့် အပတ်စဉ် schedule တို့တွင် C# source code ထဲရှိ security vulnerabilities များကို scan လုပ်ပေးသည်။
- **Dependabot:** NuGet packages နှင့် GitHub Actions versions များကို အပတ်စဉ်စစ်ပြီး version အသစ်ရှိလျှင် update pull request တင်ပေးသည်။

---

## 💡 လမ်းညွှန်ချက်များ

### ၁။ Feature အသစ်တစ်ခု ထပ်ထည့်ပုံ (ဥပမာ - `Order`)
ဤ Codebase ထဲသို့ database-backed aggregate အသစ်တစ်ခု ထည့်သွင်းလိုပါက အောက်ပါအဆင့်အတိုင်း လုပ်ဆောင်ပါ-
1. **Domain:** `CleanArch.Domain/Entities/` အောက်တွင် `BaseEntity` ကို အမွေဆက်ခံသည့် `Order` class ကို တည်ဆောက်ပါ။
2. **Application:**
   - Database access စာချုပ်အဖြစ် `IOrderRepository` ကို `CleanArch.Application/Common/Interfaces/` အောက်တွင် ဖန်တီးပါ။
   - `CleanArch.Application/Features/Orders/Commands/CreateOrder/` folder ဆောက်ပါ။
   - `CreateOrderCommand`, `CreateOrderCommandValidator`, နှင့် `CreateOrderCommandHandler` တို့ကို ရေးသားပါ။
3. **Infrastructure:**
   - `ApplicationDbContext` ထဲတွင် `DbSet<Order> Orders` ကို သတ်မှတ်ပါ။
   - `CleanArch.Infrastructure/Persistence/Repositories/` အောက်တွင် `OrderRepository` ကို ရေးပါ။
   - `CleanArch.Infrastructure/DependencyInjection.cs` ထဲတွင် repository ကို register လုပ်ပါ:
     ```csharp
     services.AddScoped<IOrderRepository, OrderRepository>();
     ```
4. **WebApi:**
   - `CleanArch.WebApi/Controllers/` ထဲတွင် `ApiControllerBase` ကို အမွေဆက်ခံသည့် `OrdersController.cs` ကို တည်ဆောက်ပြီး HTTP endpoints များ ပြင်ဆင်ပါ။

### ၂။ SQLite မှ SQL Server သို့ ပြောင်းလဲအသုံးပြုပုံ
စမ်းသပ်မှုအဆင့်မှ Production အဆင့်သို့ ပြောင်းလဲရန် database provider ကို အောက်ပါအတိုင်း ပြောင်းလဲနိုင်သည်-
1. NuGet Package ဖြစ်သည့် `Microsoft.EntityFrameworkCore.SqlServer` ကို `CleanArch.Infrastructure.csproj` ထဲသို့ သွင်းပါ။
2. `CleanArch.WebApi/appsettings.json` ရှိ connection string ကို ပြင်ဆင်ပါ-
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=CleanArchDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```
3. `CleanArch.Infrastructure/DependencyInjection.cs` ထဲတွင် SQL Server သုံးရန် ပြောင်းလဲပါ-
   ```csharp
   services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(connectionString));
   ```

### ၃။ Frontend Project (`.client`) ချိတ်ဆက်ပုံ
Angular, React, Vue, သို့မဟုတ် Svelte client project တစ်ခုကို ဤ solution directory အောက်တွင် အောက်ပါအတိုင်း ချိတ်ဆက်နိုင်သည်-
1. Root directory တွင် Vite သို့မဟုတ် Angular CLI သုံးပြီး client folder ကို တည်ဆောက်ပါ-
   ```bash
   npx create-vite@latest src/CleanArch.Client --template react-ts
   ```
2. Frontend dev-server (ဥပမာ- `vite.config.ts`) တွင် backend API endpoints calls များကို proxy forward လုပ်ရန် ပြင်ဆင်ပါ-
   ```typescript
   server: {
     proxy: {
       '/api': {
         target: 'http://localhost:5076',
         changeOrigin: true,
         secure: false
       }
     }
   }
   ```
3. Visual Studio တွင် WebApi နှင့် Client project နှစ်ခုစလုံး တစ်ပြိုင်နက်တည်း Run နိုင်ရန် **Multiple Startup Projects** အဖြစ် configure သတ်မှတ်ပေးနိုင်ပါသည်။

---

## 📦 စံသတ်မှတ်ထားသော API Response Format

Controller response အားလုံးကို English နှင့် Myanmar message ပါဝင်သော JSON envelope ပုံစံတစ်မျိုးတည်းဖြင့် ပြန်ပေးထားသည်။ Raw data ကို `Ok(...)` ဖြင့် တိုက်ရိုက်ပြန်မည့်အစား reusable `ApiResponse` helper ကို အသုံးပြုပါ။

### Data ပါသော success response

```csharp
return ApiResponse.OkResult(
    route: null,
    message: new MessageResponse
    {
        EN = "Products retrieved successfully.",
        MM = "ကုန်ပစ္စည်းများ ရယူပြီးပါပြီ။"
    },
    data: products);
```

```json
{
  "success": true,
  "message": {
    "en": "Products retrieved successfully.",
    "mm": "ကုန်ပစ္စည်းများ ရယူပြီးပါပြီ။"
  },
  "data": []
}
```

Client ကို resource location ပြန်ပေးရန်လိုပါက `route` ထည့်နိုင်သည်။ `route` သည် null ဖြစ်ပါက JSON response ထဲတွင် မပါဝင်ပါ။

```csharp
return ApiResponse.OkResult(
    route: $"/api/products/{productId}",
    message: new MessageResponse
    {
        EN = "Product created successfully.",
        MM = "ကုန်ပစ္စည်းအသစ် ဖန်တီးပြီးပါပြီ။"
    },
    data: productId);
```

### Data မပါသော success response

```csharp
return ApiResponse.OkResult(
    route: null,
    message: new MessageResponse
    {
        EN = "Product updated successfully.",
        MM = "ကုန်ပစ္စည်းအချက်အလက် ပြင်ဆင်ပြီးပါပြီ။"
    });
```

### Error response

```csharp
return ApiResponse.ErrorResult(
    StatusCodes.Status404NotFound,
    new MessageResponse
    {
        EN = "Product was not found.",
        MM = "ကုန်ပစ္စည်းကို ရှာမတွေ့ပါ။"
    });
```

Validation မှားယွင်းပါက `errors` dictionary ထပ်မံပါဝင်နိုင်သည်။ Global exception၊ invalid model state နှင့် unsupported content type response များသည်လည်း bilingual response envelope ပုံစံတူကို အသုံးပြုသည်။ Body ပါသော API request များတွင် `Content-Type: application/json` သတ်မှတ်ရမည်။ `application/json; charset=utf-8` ကိုလည်း လက်ခံသည်။

သက်ဆိုင်ရာ types များ၏တည်နေရာများမှာ—

- `CleanArch.WebApi/Helpers/ApiResponse.cs`
- `CleanArch.WebApi/Models/ApiResult.cs`
- `CleanArch.WebApi/Models/MessageResponse.cs`

---

## 🚀 စတင်အသုံးပြုပုံ

### လိုအပ်သော Software များ

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Git
- Visual Studio 2026၊ Visual Studio Code သို့မဟုတ် JetBrains Rider (မဖြစ်မနေမလိုပါ)

### ၁။ API စတင် Run ရန်
ပရောဂျက်၏ root လမ်းကြောင်းအောက်တွင် အောက်ပါအတိုင်း run ပါ-
```bash
dotnet run --project src/CleanArch.WebApi
```
သို့မဟုတ် **Visual Studio** တွင် `CleanArch.WebApi` ပေါ် Right-click နှိပ်ပြီး **Set as Startup Project** ရွေးချယ်ကာ **F5** ကို နှိပ်ပါ။

### ၂။ Scalar UI တွင် စမ်းသပ်ခြင်း
API စတင်ပွင့်လာပါက browser တွင် interactive **Scalar Documentation UI** ပွင့်လာပါလိမ့်မည်-
*   👉 **`http://localhost:5076/scalar/v1`** (သို့မဟုတ် `https://localhost:7013/scalar/v1`)

၎င်း UI တွင် JSON request payload များကို စမ်းသပ်ထည့်သွင်းပြီး endpoints များကို တိုက်ရိုက်စမ်းသပ်နိုင်သည်။

### စမ်းသပ်နိုင်သည့် Endpoint နမူနာများ-
- **`GET /api/products`** - Product အားလုံးကို list ပြပေးသည်။
- **`GET /api/products/{id}`** - သက်ဆိုင်ရာ Product ၏ အသေးစိတ်အချက်အလက်များကို ပြပေးသည်။
- **`POST /api/products`** - Product အသစ်တစ်ခု ဆောက်ပေးသည် (`Name` နှင့် `Price` ကို စစ်ဆေးပေးသည်)။
- **`PUT /api/products/{id}`** - လက်ရှိ Product ကို အချက်အလက်အသစ်ဖြင့် ပြင်ဆင်ပေးသည် (ID တူညီမှု၊ `Name` နှင့် `Price` ကို စစ်ဆေးပေးသည်)။
- **`DELETE /api/products/{id}`** - database ထဲမှ product ကို ဖျက်ပေးသည်။
- **`GET /health`** - API status နှင့် Database ချိတ်ဆက်မှု အခြေအနေကို JSON format ဖြင့် စစ်ဆေးပေးသည်။

### ၃။ Automated Tests များ စမ်းသပ် Run ရန်
Unit၊ Architecture နှင့် Integration test suites အားလုံးကို .NET CLI ဖြင့် အောက်ပါအတိုင်း run နိုင်သည်-
```bash
dotnet test CleanArch.slnx
```
သို့မဟုတ် code coverage အစီရင်ခံချက်နှင့်အတူ run ရန်-
```bash
dotnet test CleanArch.slnx --collect:"XPlat Code Coverage"
```

- **Unit tests** သည် Domain၊ Application၊ Infrastructure နှင့် Web API component တစ်ခုချင်းစီကို စစ်ဆေးသည်။
- **Architecture tests** သည် Clean Architecture layer dependencies များ အတွင်းဘက်သို့သာ မှီခိုကြောင်း စစ်ဆေးသည်။
- **Integration tests** သည် Web API အစစ်ကို သီးခြား in-memory SQLite database ဖြင့် boot လုပ်ပြီး HTTP endpoints များကို စစ်ဆေးသည်။
