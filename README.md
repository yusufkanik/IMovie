# IMovie - Full-Stack Movie & Recommendation Platform

IMovie is a full-stack web application designed for film enthusiasts to explore movies, manage personal watchlists, create custom public/private collections, write interactive reviews, and receive personalized movie recommendations. 

Built with **ASP.NET Core 8 Web API** on the backend and **Next.js 14 (App Router)** with **TypeScript** on the frontend, the platform integrates **The Movie Database (TMDB) API** with local database synchronization and custom rating algorithms.

---

## Key Features

- **Secure Authentication & Authorization:**
  - Custom password hashing and salting via `HMACSHA512`.
  - Dual-token authentication system using short-lived **JWT Access Tokens** and long-lived **Refresh Tokens**.
  - Secure token storage utilizing `HttpOnly` and `SameSite=Strict` HTTP cookies to mitigate XSS and CSRF risks.
  - Role-Based Access Control (RBAC) separating `Admin` and `User` privileges.

- **Movie Discovery & TMDB Synchronization:**
  - Real-time movie search and catalog browsing powered by TMDB API.
  - On-demand and batch database synchronization (`SyncPopularMovies`, `SyncSingleMovie`) with transaction safety (`BeginTransactionAsync`).
  - Advanced filtering (genres, rating bounds, release year, vote counts) and dynamic sorting.

- **Smart Recommendation Engine:**
  - **Personalized Recommendations:** Algorithmic movie recommendations tailored to the user's favorite films and top-watched genres.
  - **Similar Movies:** Content-based similarity filtering using matching genre weights and rating scores.

- **Interactive Reviews & Reactions:**
  - User rating (1–10) and comment system with character limits and inline editing/deletion.
  - Real-time aggregation of local movie ratings (`LocalVoteAverage`, `LocalVoteCount`).
  - Upvote / Downvote (Like / Dislike) reaction tracking for community reviews.

- **Custom Movie Lists & Social Features:**
  - Ability to create **Public** or **Private** custom movie collections.
  - Community feed for discovering public lists created by other users.
  - Ownership validation preventing unauthorized list or review tampering.

- **Watch Status & User Analytics:**
  - Track movie status (*Watched*, *Plan to Watch*, *Dropped*).
  - Personal dashboard displaying stats such as total movies watched, dropped counts, and average rating given.

---

## Tech Stack

### **Backend**
- **Framework:** .NET 8 (ASP.NET Core Web API)
- **Database:** MSSQL Server
- **ORM:** Entity Framework Core (Code-First Migration, Fluent API, Relational Junction Entities)
- **Security:** System.IdentityModel.Tokens.Jwt, HMACSHA512, Claims Principal Authentication
- **HTTP Client:** `HttpClient` factory consuming TMDB REST API

### **Frontend**
- **Framework:** Next.js 14 (App Router, Client Components)
- **Language:** TypeScript
- **Styling:** Tailwind CSS
- **HTTP & State Management:** Axios (Custom Interceptors), React Context API (`AuthContext`)

---

## Architecture & Security Highlights

1. **Defense Against Broken Object Level Authorization (BOLA/IDOR):**
   - Identity claims are extracted securely from JWT context (`User.GetUserId()`) rather than trusting user-supplied route parameters.
2. **Database Integrity & Query Optimization:**
   - Explicit `AsNoTracking()` applied to read-heavy queries for memory performance.
   - Cascading deletions, junction tables (`MovieGenre`, `MovieCast`, `MovieDirector`, `CustomListMovie`), and indexed foreign keys.
   - Parameterized EF Core queries protecting against SQL Injection (SQLi).
3. **Transaction Management:**
   - Atomic sync operations using EF Core database transactions to avoid partial writes when pulling cast, crew, and genre metadata from third-party APIs.

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js (v18+)](https://nodejs.org/)
- [SQL Server](https://www.microsoft.com/sql-server/)

### 1. Backend Setup
```bash
# Navigate to API project
cd MovieAPI

# Configure appsettings.json or User Secrets
# Add your TMDB API Key, ConnectionString, and JwtSettings

# Apply database migrations
dotnet ef database update

# Run backend server
dotnet run