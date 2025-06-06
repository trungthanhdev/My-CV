# My-CV (.NET 9 Portfolio API)

A backend RESTful API developed with **.NET 9**, showcasing personal information, skills, projects, and experience. This project follows **Clean Architecture** and **DDD (Domain-Driven Design)** principles and is intended to serve as a professional portfolio backend.

---

## 📌 Features

- Personal profile and CV data management
- Modular & maintainable DDD structure
- Clean Architecture (Domain → Application → Infrastructure → API)
- RESTful endpoints with versioning (optional)
- PostgreSQL via [Neon](https://neon.tech/)
- (Optional) Authentication/Authorization layer
- (Optional) Unit & integration test structure

---

## ⚙️ Tech Stack

- **.NET SDK**: .NET 9.0
- **Architecture**: Clean Architecture, DDD
- **Database**: PostgreSQL (hosted on Neon)
- **ORM**: Entity Framework Core (if applicable)
- **Platform**: macOS / cross-platform

---

## 🚀 Getting Started

### 📦 Prerequisites

- [.NET SDK 9.0+](https://dotnet.microsoft.com/download)
- PostgreSQL connection string (Neon or local)

### 🛠 Setup

```bash
cd /Applications/Personal\ Project/CV/My-CV
dotnet restore
cd src/My-CV/ZEN.Startup
dotnet run
```
