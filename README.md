# My-CV (.NET 9 Portfolio API)

A backend RESTful API developed with **.NET 9**, showcasing personal information, skills, projects, and experience. This project follows **Clean Architecture** and **DDD (Domain-Driven Design)** principles and is intended to serve as a professional portfolio backend.

---

---

## 👇 Visit now

- Explore the portfolio: 🌐 [Click here to view](https://portfolio-one-sigma-16.vercel.app/trung-thanh)
- Explore the APIs: 🌐 [Click here to view](https://my-cv-suxl.onrender.com/swagger/index.html)

---

## 📌 Features

- Personal profile and CV data management
- Modular & maintainable DDD structure
- Clean Architecture (Domain → Application → Infrastructure → API)
- RESTful endpoints with versioning (optional)
- Minimal API
- PostgreSQL via [Neon](https://neon.tech/)

---

## ⚙️ Tech Stack

- **.NET SDK**: .NET 9.0
- **Architecture**: Clean Architecture, DDD
- **Cloud Hosting**: Render (Docker-based deployment)
- **Database**: PostgreSQL (hosted on Neon)
- **ORM**: Entity Framework Core
- **Platform**: WebApp
- **Containerization**: Docker
- **Caching**: Redis Cloud
- **Media Storage**: Cloudinary (image hosting, optimization)
- **Email Service**: SendGrid
- **CI/CD**: GitHub Actions

---

## 🚀 Getting Started

### 📦 Prerequisites

- [.NET SDK 9.0+](https://dotnet.microsoft.com/download)
- Init .env, docker-compose.yml in ZEN.Startup
- PostgreSQL connection string (Neon or PostgreSQL)
- Cloudinary connection string
- Redis cloud connectionstring

### 🛠 Setup

```bash
cd /Applications/Personal\ Project/CV/My-CV
dotnet restore
cd src/My-CV/ZEN.Startup
dotnet run
```

### 🛠 Setup with Docker

```bash
cd .../src
docker compose up --build
```
