# Lion Gym - Gym Management System 🦁💪

A modern, comprehensive, and premium Gym Management System built with a decoupled architecture using **ASP.NET Core Web API** and **Blazor WebAssembly (.NET 10)**.

The system is designed to streamline fitness club operations, trainee memberships, attendance logs, and financial calculations (including installments, custom pricing, and detailed operational expenses).

---

## 🌟 Key Features

### 1. Trainee Management
* **Registration & Profiles**: Add trainees with full name, phone number, gender, date of birth, and custom profile photo uploads.
* **Membership Status**: Automatic determination of active vs. inactive status based on subscription validity dates.
* **Flexible UI**: Localized drop-downs featuring bilingual Arabic/English descriptors for Plan and Period selection.

### 2. Custom Subscription Pricing Model (Refactored)
* **Predefined Price Defaults**: Define plan-period defaults (e.g. Basic Plan + Monthly = 300$).
* **Agreed Price Overrides**: Allows administrators to specify custom subscription prices per trainee (e.g., Trainee A pays 300$, Trainee B pays 350$, Trainee C pays 250$ for the exact same plan-period).
* **Validation**: Backend and frontend validations to prevent negative or zero pricing and validate installment payments.

### 3. Financial Tracking & Analytics
* **Remaining Balances**: Dedicated "المدفوعات المتبقية" (Finance) screen to manage trainees with outstanding balances.
* **Date & Search Filtering**: Filter unpaid installment rows by Trainee Name/Phone and Date Range filters (based on subscription start date).
* **Payment Installments**: Support for collecting partial payments/installments with real-time recalculation of remaining amounts.
* **Visual Trainee Identifiers**: Renders trainee profile pictures or dynamic initials-based avatars next to each row.

### 4. Financial Dashboard & Reporting
* **Key Metrics**: Real-time stats for Total Earnings, Monthly Earnings, and Active Subscriptions count.
* **Growth Analytics**: Automated calculation of Month-Over-Month (MoM) and Year-Over-Year (YoY) growth percentages.
* **Earnings Trend Chart**: Interactive monthly earnings graph visualizing gym revenue over time.
* **Transaction Logs**: Unified transaction history showing subscriptions, installments, and expenses.

### 5. Expense Management
* **Operational Expense Log**: Categorized tracking of gym expenses (rent, electricity, equipment maintenance) to dynamically compute net profits.

### 6. Role & Gender-Based Security
* **Gender Scoping**: Administrative accounts marked for female-only managers automatically restrict database records server-side to show only female trainees.

---

## 🛠️ Technology Stack

* **Frontend**: Blazor WebAssembly (.NET 10), Bootstrap 5, Bootstrap Icons, Animate.css
* **Backend API**: ASP.NET Core Web API (.NET 10), ASP.NET Core Identity
* **Database & ORM**: SQL Server, Entity Framework Core (EF Core)
* **Authentication**: JWT (JSON Web Tokens) with sliding refresh tokens

---

## 📂 Project Structure

```text
GymManagementSystem/
├── GymManagementSystem/                    # Startup ASP.NET Core Web API Project
│   ├── Controllers/                        # API Endpoint Controllers
│   └── Program.cs                          # API Configuration, Middleware, Services Bootstrapper
├── GymManagementSystem.Application/        # Application Layer (Services & Logic)
├── GymManagementSystem.Application.Contracts/ # DTOs, Commands, Queries, Service Interfaces
├── GymManagementSystem.Domain/             # Core Domain Entities
├── GymManagementSystem.Domain.Shared/      # Enums, Shared Constants
├── GymManagementSystem.Infrastructure/     # DbContext, EF Configurations, Migrations
└── GymManagementSystem.BlazorWASM/         # Blazor WebAssembly Frontend Web App
    ├── Pages/                              # Razor Components & UI pages
    ├── Services/                           # ApiClient (HTTP/JSON integration)
    └── wwwroot/                            # Static assets (CSS, images)
```

---

## 🚀 Getting Started

### Prerequisites
* [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* SQL Server LocalDB or full instance
* [EF Core CLI Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

### Database Setup
1. Open the solution in Visual Studio or your preferred IDE.
2. Verify the Connection String inside the backend project (`GymManagementSystem/appsettings.json` or equivalent environment configuration).
3. Apply the migrations to build your database schema:
   
   Using the **dotnet CLI**:
   ```bash
   dotnet ef database update --project GymManagementSystem.Infrastructure --startup-project GymManagementSystem
   ```
   
   Using the **Visual Studio Package Manager Console (PMC)**:
   ```powershell
   Update-Database -Project GymManagementSystem.Infrastructure -StartupProject GymManagementSystem
   ```

### Running Locally
To launch both the API Backend and Blazor WASM Frontend together:

1. Restore dependencies:
   ```bash
   dotnet restore
   ```
2. Run the application:
   ```bash
   dotnet run --project GymManagementSystem
   ```

The Blazor WASM client will launch and hook up to the ASP.NET Core backend to query data.