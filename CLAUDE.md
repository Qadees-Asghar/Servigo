# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

SERVIGO is a C# WinForms desktop app (net10.0-windows) for service booking/management, backed by SQL Server. Three-layer architecture: Forms (UI) → DAL (data access) → SQL Server (tables, stored procedures, functions, triggers, views).

## Commands

Build and run (no test suite exists):
```
dotnet build
dotnet run
```

Database: `Database/SERVIGO_Database.sql` is a full drop-and-recreate script (drops the `SERVIGO` database if it exists, then rebuilds it). Run it against a SQL Server instance to (re)provision the schema, stored procedures, functions, triggers, views, and seed reference data (Roles, BookingStatuses, ServiceCategories). Connection is configured in `appsettings.json` → `ConnectionStrings:DefaultConnection` (currently a `Trusted_Connection` pointing at a local server named `techsyndicate`; update this to match the dev machine's SQL Server instance name).

On first run, `Program.cs` seeds a default admin user (`admin@servigo.com` / `Admin@123`, UserID `SRV-00001`) if no admin exists yet.

## Architecture

**Layering is strict**: Forms call DAL static classes; DAL classes call `Helpers.DatabaseHelper`; nothing bypasses this chain. There is no ORM — all SQL is either inline parameterized queries/`ExecuteNonQuery`/`ExecuteScalar`/`ExecuteQuery`, or stored-procedure calls via `ExecuteStoredProcedure*`. Always use `DatabaseHelper.Param(name, value)` for parameters — never string-concatenate SQL.

**Role model**: `Models.User` is an abstract base class; `AdminUser`, `CustomerUser`, `ServiceProviderUser` are the concrete subclasses, selected polymorphically by `RoleID` (1 = Admin, 2 = Customer, 3 = Provider — see `Roles` table / `UserDAL.MapToUser`). Each subclass overrides `ShowDashboard()` and `GetRoleName()`. `Helpers.SessionManager` holds the single currently-logged-in `User` (and `CurrentProviderID` for providers) as static state for the process lifetime — there's no DI container, everything is accessed statically (`DatabaseHelper`, `SessionManager`, `PasswordHelper`, `ValidationHelper`, `AppTheme`).

**Forms** are organized by role under `Forms/Admin`, `Forms/Customer`, `Forms/Provider`, plus shared entry forms (`frmIntro`, `frmLogin`, `frmSignup`) directly under `Forms/`. Each form pairs a `.cs` (logic) with a `.Designer.cs` (generated layout) and `.resx`.

**DAL classes** (`DAL/*.cs`) are static, one per aggregate (`UserDAL`, `BookingDAL`, `ServiceDAL`, `ProviderDAL`, `RatingDAL`, `FeedbackDAL`, `NotificationDAL`). Multi-step writes that must be atomic use manual `SqlTransaction` (see `UserDAL.DeleteUser`, which cascades child-table deletes in FK order — Notifications/FeedbackReports → Ratings → Bookings → Services → TimeSlots → ServiceProviders → Users — before committing).

**Database-side logic**: business rules that must hold regardless of caller live in SQL, not C# — e.g. `sp_CreateBooking`, `sp_UpdateBookingStatus`, `sp_GetDashboardStats` (stored procedures), `fn_GenerateUserID`, `fn_GetCustomerBookingCount`, `fn_GetProviderCompletedCount` (functions), audit-logging triggers (`trg_Users_Audit`, `trg_Bookings_Audit`, `trg_ServiceProviders_Audit` write to `AuditLogs`), and `trg_UpdateAverageRating` (keeps `ServiceProviders.AverageRating` in sync with `Ratings`). `vw_BookingSummary` and `vw_ProviderBookingStats` back reporting/dashboard queries. When adding a new user-ID-consuming feature, use `fn_GenerateUserID` / `UserDAL.GenerateUserID()` rather than generating IDs in C#.

**Security**: passwords are BCrypt-hashed via `PasswordHelper` (work factor 12) — never store or compare plaintext. All user input validation (CNIC, phone, email, password, full name) goes through `Helpers.ValidationHelper`, which returns `bool` + `out string error` rather than throwing, so forms can display validation errors inline.

**Theme**: `Theme.AppTheme` is the single source of colors/fonts and a factory for pre-styled controls (`MakePrimaryButton`, `MakeTextBox`, `MakeDataGrid`, `MakeStatCard`, etc.) implementing a dark theme. New UI should be built by composing these factory methods rather than constructing/styling WinForms controls by hand, to keep visuals consistent across Admin/Customer/Provider dashboards.
