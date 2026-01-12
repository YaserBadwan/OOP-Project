# 📞 PhoneBook – A Framework-like Console Application in C#

[![CSharp](https://img.shields.io/badge/C%23-11-blueviolet)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![.NET](https://img.shields.io/badge/.NET-8-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![xUnit](https://img.shields.io/badge/xUnit-2.4.1-yellowgreen)](https://xunit.net/)
[![Docker](https://img.shields.io/badge/Docker-20.10-blue)](https://www.docker.com/)

## 📖 Overview

**PhoneBook** is a single-user phone book application developed as an **Object-Oriented Programming (OOP) academic project**. It is intentionally designed as a **mini-framework**, not as a simple CRUD console app.

The focus of this project is on **clean architecture, domain modeling, and separation of concerns**, demonstrating mature OOP design rather than minimal implementation.

The application exposes a reusable **Core domain layer**, multiple interchangeable **persistence mechanisms**, and a **thin CLI UI**, with future support for a **thin Web UI (Blazor)**.

---

## 🎯 Architectural Goals

- **Strict Separation of Responsibilities**: Ensuring that each component has a single, well-defined purpose.
- **Pure Domain Logic**: Keeping the core business rules independent of UI and infrastructure.
- **Dependency Inversion**: Using abstractions to decouple high-level and low-level modules.
- **Framework-like Structure**: Designing the application to be extensible and reusable.
- **User-Friendly Error Handling**: Preventing crashes and providing a smooth user experience.
- **Professional Project Evolution**: Maintaining a clean commit history and comprehensive test coverage.

---

## 🏗️ Solution Structure

```
PhoneBook.sln
│
├── 📦 PhoneBook.Core           // Domain layer (pure)
├── 🗄️ PhoneBook.Infrastructure   // Storage & external integrations
├── 🖥️ PhoneBook.ConsoleApp     // Thin CLI UI
└── 🌐 PhoneBook.Web             // Thin Blazor UI (planned)
```

### Project Dependencies

| Project          | Depends On                                   |
| ---------------- | -------------------------------------------- |
| `PhoneBook.Core` | ❌ **Nothing**                               |
| `Infrastructure` | `PhoneBook.Core`                             |
| `ConsoleApp`     | `PhoneBook.Core`, `PhoneBook.Infrastructure` |
| `Web`            | `PhoneBook.Core`, `PhoneBook.Infrastructure` |

> **Important:** `PhoneBook.Core` has **zero dependencies** on UI, infrastructure, logging, or third-party libraries.

---

## 📦 PhoneBook.Core (Domain Layer)

### Responsibilities

- Domain models and business rules
- Application services to orchestrate domain logic
- Domain-specific exceptions and warnings
- Contracts (interfaces) for dependency inversion

### What Core does NOT contain

- ❌ UI logic
- ❌ File or database access
- ❌ Logging implementations
- ❌ Third-party libraries

---

## 🧩 Domain Model

### 📞 `PhoneNumber` (Value Object)

- An **immutable** value object representing a phone number.
- Stored in **E.164 canonical format** (e.g., `+40722123456`).
- Default region for normalization is **RO** (Romania).
- Equality is based **only on the E164 value**.

**Stored Data:**
- `Raw`: The original user input (e.g., "0722 123 456").
- `E164`: The normalized canonical value.

Validation and normalization are **delegated via an abstraction** to keep the Core pure.

```csharp
public interface IPhoneNumberNormalizer
{
    string ToE164(string raw, string defaultRegion);
}
```

### 👤 `Contact` (Entity)

- A `Contact` entity **cannot exist in an invalid state**.
- **Mandatory Fields**: `PhoneNumber`, `FirstName`.
- **Optional Fields**: `LastName`, `Email`, `Pronouns`, `Ringtone`, `Birthday`, `Notes`.
- Validation and normalization occur in the constructor.
- `.Trim()` is applied consistently to string inputs.
- Email is validated using a simple regex.

### ⚙️ `PhoneBookService` (Application Service)

- Acts as the orchestrator of the application.
- Holds the in-memory `PhoneBookState`.
- Loads state at startup and automatically persists it after mutations.
- Enforces domain rules, such as phone number uniqueness.

**API:**
- `ListAll()`
- `Add(Contact)` -> `(Contact, IReadOnlyList<DomainWarning>)`
- `GetByPhone(...)`
- `Update(originalPhone, updated)`
- `DeleteByPhone(...)`
- `SearchExact(query)`
- `CreatePhoneNumber(string raw)`

> **Note:** Duplicate names generate warnings, not errors, for a better user experience.

### ⚠️ Domain Exceptions (Framework-like)

All domain errors are modeled explicitly to ensure a crash-free and fluent UX.

**Hierarchy:**
```
DomainException
├── ValidationException
├── RuleViolationException
├── DuplicatePhoneNumberException
├── ContactNotFoundException
└── StorageException
```

**Design Rules:**
- `Message` is user-friendly.
- `InnerException` contains technical details for logging.
- The Core throws exceptions, and the UI handles them gracefully.

---

## 🗄️ PhoneBook.Infrastructure

Contains all concrete implementations of external dependencies.

### Storage Implementations

1.  **InMemory Storage**:
    - Used for tests and demos.
    - Provides isolation through deep clone semantics.

2.  **JSON File Storage**:
    - Uses DTOs to separate the domain from the storage model.
    - Ensures atomic writes using a temp file and replace strategy.
    - Handles missing files, corrupted JSON, and I/O errors by wrapping them in a `StorageException`.

3.  **MariaDB Storage (Docker)**:
    - Implemented using `MySqlConnector`.
    - The schema is created manually.
    - Persistence is a snapshot: `DELETE` + `INSERT` inside a transaction.
    - Database exceptions are wrapped in a `StorageException`.
    - The connection string is injected using the Options pattern.

---

## 🖥️ Console Application (PhoneBook.ConsoleApp)

### Design Philosophy

- **Thin UI**: No business logic.
- **Pure Composition Root**: All dependencies are wired up here.
- **Professional UX**: Clear, user-friendly, and robust.

### Features

- Pretty help command (`help`).
- `list`: Shows all contacts.
- `show <phone>`: Displays a single contact.
- `search <query>`: Searches for contacts (auto-shows on a single result).
- `add`: Adds a new contact.
- `edit <phone>`: Edits a contact with a draft and retry loop.
- `delete <phone>`: Deletes a contact with confirmation.
- Graceful handling of errors and warnings.

### Architecture

- `IConsole` abstraction for testability.
- `ConsoleLayout` for consistent formatting.
- `IContactPresenter` to display contacts.
- `CommandLoop` and `CommandDispatcher` for command handling.
- Commands are auto-discovered via reflection.

---

## 🔍 Logging

- Implemented using `.NET GenericHost` and `ILogger`.
- Logging exists **only in `ConsoleApp` and `Infrastructure`**.
- `PhoneBook.Core` remains logging-free.
- Logs are written to a file.
- The UI displays `DomainException.Message` and logs the `InnerException` for diagnostics.

---

## ⚙️ Configuration

- `appsettings.json`
- Launch profiles
- Environment variables
- Docker-based MariaDB

> **Note:** Storage selection happens via Dependency Injection, not conditional logic. The UI displays the actual injected storage type.

---

## 🧪 Testing

- **xUnit** is used for testing.
- Tests cover the `Contact` entity and `PhoneBookService`.
- `InMemory` storage is used for isolation.
- All tests are green. ✅

---

## ▶️ Running the Application

### InMemory

```bash
dotnet run --project PhoneBook.ConsoleApp --launch-profile InMemory
```

### JSON Storage

```bash
dotnet run --project PhoneBook.ConsoleApp --launch-profile Json
```

### MariaDB (Docker)

1.  Start the database:
    ```bash
    docker-compose up -d
    ```
2.  Run the application:
    ```bash
    dotnet run --project PhoneBook.ConsoleApp --launch-profile MariaDb
    ```

---

## 🧠 Design Rationale

- **Why no logging in Core?**
  - To keep the domain logic pure, reusable, and free of external concerns.

- **Why wrappers for external libraries?**
  - To isolate volatility, enable testability, and make dependencies swappable.

- **Why snapshot persistence?**
  - It provides simplicity, correctness, and transactional safety for this use case.

- **Why warnings instead of errors?**
  - To create a better user experience and model real-world scenarios more accurately.

---

## 🚀 Future Work (maybe)

- [ ] Thin Web UI (Blazor)

---

## 🎓 Academic Notes

This project intentionally prioritizes:
- **Architecture** over shortcuts.
- **Correct OOP** over minimal code.
- **Extensibility** over immediacy.

It is designed to be explained, defended, and extended.