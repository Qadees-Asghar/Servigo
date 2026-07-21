# AGENTS.md

This file provides guidance to **Antigravity** (and other AI coding assistants) when working with code in this repository.

> For Claude-specific guidance, see [`CLAUDE.md`](./CLAUDE.md). The architectural notes there apply equally here.

---

## Project Overview

**SERVIGO** is a C# WinForms desktop application (`net10.0-windows`) for service booking and management, backed by SQL Server. It follows a strict three-layer architecture:

```
Forms (UI)  →  DAL (Data Access Layer)  →  SQL Server
```

---

## Key Rules for AI Contributors

### 1. Never Bypass the DAL
- Forms must **only** call `DAL.*` static classes.
- DAL classes must **only** use `Helpers.DatabaseHelper` for all DB access.
- Never write raw `SqlConnection` / `SqlCommand` code inside a Form.

### 2. Always Use Parameterized Queries
```csharp
// ✅ Correct
DatabaseHelper.Param("@UserID", userId)

// ❌ Never do this
$"SELECT * FROM Users WHERE UserID = '{userId}'"
```

### 3. Respect the Role/User Hierarchy
- `Models.User` is **abstract** — never instantiate it directly.
- Use `UserDAL.MapToUser(reader)` to get the correct concrete subclass (`AdminUser`, `CustomerUser`, `ServiceProviderUser`) based on `RoleID`.

### 4. Password Handling
- **Always** use `Helpers.PasswordHelper` (BCrypt, work factor 12).
- Never store, log, or compare plaintext passwords.

### 5. Validation
- All user input **must** pass through `Helpers.ValidationHelper`.
- It returns `bool` + `out string error` — display errors inline, never throw for validation failures.

### 6. UI / Theme
- Use `Theme.AppTheme` factory methods for all new UI controls.
- Do **not** hand-style WinForms controls. Use: `MakePrimaryButton`, `MakeTextBox`, `MakeDataGrid`, `MakeStatCard`, etc.

### 7. User ID Generation
- Always use `fn_GenerateUserID` (SQL) / `UserDAL.GenerateUserID()` (C#) for new user IDs.
- Never generate IDs manually.

### 8. Atomic Multi-step Writes
- Use manual `SqlTransaction` for any operation touching multiple tables.
- Follow the FK-order cascade pattern in `UserDAL.DeleteUser` as a reference.

---

## Database

- Full schema script: `Database/SERVIGO_Database.sql` (drop-and-recreate).
- Connection config: `appsettings.json` → `ConnectionStrings:DefaultConnection`.
- Default admin seeded on first run: `admin@servigo.com` / `Admin@123`.

---

## Build & Run

```bash
dotnet build
dotnet run
```

No test suite exists — manual verification via the WinForms UI is the current approach.

---

## Commit Convention

When Antigravity makes commits, commits will include:

```
Co-Authored-By: Antigravity <antigravity@google.com>
```

This ensures AI contributions are transparent and attributed in the GitHub contributor graph.
