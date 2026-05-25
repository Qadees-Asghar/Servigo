<div align="center">

<img width="120" src="https://cdn-icons-png.flaticon.com/512/942/942748.png"/>

# SERVIGO

### Enterprise-Level Service Booking & Management Platform

<p align="center">
  <img src="https://img.shields.io/badge/C%23-Windows%20Forms-68217A?style=for-the-badge&logo=csharp&logoColor=white"/>
  <img src="https://img.shields.io/badge/.NET-Framework-512BD4?style=for-the-badge&logo=dotnet&logoColor=white"/>
  <img src="https://img.shields.io/badge/SQL%20Server-Database-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white"/>
  <img src="https://img.shields.io/badge/Architecture-3--Layer-blue?style=for-the-badge"/>
</p>

<p align="center">
A modern desktop-based platform designed to streamline service booking, provider management, appointment scheduling, and customer interaction through a secure role-based architecture.
</p>

</div>

---

# 🚀 Overview

SERVIGO is a professionally designed desktop application developed using **C# Windows Forms** and **SQL Server**.  
The system enables customers to discover services, schedule appointments, interact with providers, and manage bookings efficiently while giving administrators full operational control.

The project focuses on:
- Secure authentication
- Appointment automation
- Service provider management
- Scalable database architecture
- Professional desktop UI/UX
- Clean separation of concerns using 3-Layer Architecture

---

# ✨ Core Features

<table>
<tr>
<td width="50%">

## 👤 Customer Module
- Service browsing system
- Appointment booking
- Booking history management
- Ratings & reviews
- Real-time notifications
- Complaint & feedback system

</td>

<td width="50%">

## 🛠 Provider Module
- Service management
- Time-slot scheduling
- Incoming booking requests
- Booking approval/rejection
- Appointment completion workflow
- Provider notification system

</td>
</tr>
</table>

---

# 🛡 Admin Control Panel

```txt
✔ User Management
✔ Provider Verification
✔ Booking Monitoring
✔ Analytics Dashboard
✔ Report Management
✔ System Activity Tracking
✔ Account Activation / Deactivation
```

---

# 🧠 System Architecture

```text
┌────────────────────────────┐
│        UI LAYER            │
│  Windows Forms Interface   │
└─────────────┬──────────────┘
              │
┌─────────────▼──────────────┐
│         DAL LAYER          │
│ Database Access & Queries  │
└─────────────┬──────────────┘
              │
┌─────────────▼──────────────┐
│      DATABASE LAYER        │
│ SQL Server + Procedures    │
└────────────────────────────┘
```

---

# ⚙ Technology Stack

<div align="center">

| Technology | Purpose |
|---|---|
| **C#** | Application Logic |
| **Windows Forms** | Desktop User Interface |
| **SQL Server** | Relational Database |
| **ADO.NET** | Database Connectivity |
| **BCrypt** | Password Security |
| **Stored Procedures** | Transaction Management |
| **Triggers & Views** | Database Automation |

</div>

---

# 🔐 Security Implementation

```txt
✔ BCrypt Password Hashing
✔ SQL Injection Prevention
✔ Parameterized Queries
✔ Session-Based Authentication
✔ Role-Based Access Control
✔ Secure Account Management
```

---

# 🗄 Database Design

The database is fully normalized in **Third Normal Form (3NF)** to ensure:
- Reduced redundancy
- Improved scalability
- Better consistency
- Efficient relational mapping

### Main Tables

```text
Users
Roles
ServiceProviders
Services
TimeSlots
Bookings
Ratings
Notifications
FeedbackReports
AuditLogs
```

---

# 📊 Advanced Database Features

| Feature | Description |
|---|---|
| Stored Procedures | Booking & status automation |
| Triggers | Automatic audit logging |
| Views | Reporting & analytics |
| Functions | Dynamic calculations |
| Transactions | Safe booking operations |

---

# 📦 Core Functional Modules

<table>
<tr>
<th>Module</th>
<th>Description</th>
</tr>

<tr>
<td>Authentication System</td>
<td>Secure login/signup with role management</td>
</tr>

<tr>
<td>Customer Dashboard</td>
<td>Service discovery and appointment handling</td>
</tr>

<tr>
<td>Provider Dashboard</td>
<td>Service scheduling and booking workflow</td>
</tr>

<tr>
<td>Admin Dashboard</td>
<td>System-wide management and analytics</td>
</tr>

<tr>
<td>Ratings & Reviews</td>
<td>1–5 star feedback system</td>
</tr>

<tr>
<td>Notification System</td>
<td>Booking and status alerts</td>
</tr>

</table>

---

# 🧩 OOP Concepts Applied

```txt
✔ Abstraction
✔ Inheritance
✔ Encapsulation
✔ Polymorphism
✔ Static Helper Classes
```

---

# 📁 Project Structure

```text
SERVIGO/
│
├── Forms/
├── DAL/
├── Models/
├── Database/
├── Helpers/
├── Assets/
├── SQL Scripts/
└── Program.cs
```

---

# ⚡ Getting Started

### Requirements
- Visual Studio
- SQL Server
- .NET Framework
- NuGet Packages

### Setup

```bash
git clone https://github.com/your-username/SERVIGO.git
```

```bash
Open Solution → Configure Database → Run Project
```

---

# 📈 Future Enhancements

- Online payment gateway
- Mobile application support
- Email & SMS notifications
- AI-based provider recommendations
- Real-time chat system

---

<div align="center">

# 👨‍💻 Developer

## Qadees Asghar


</div>

---

<div align="center">

### ⭐ If you like this project, consider giving it a star.

</div>
