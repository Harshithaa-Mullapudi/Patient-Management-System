# Patient Management API

## 1. Project Overview

The Patient Management API is a backend application developed using **C# and ASP.NET Core Web API**.

The main purpose of this project is to learn and implement .NET concepts by building a real-world application.

The application allows users to:
- Register and log in using JWT authentication.
- Add, view, update and delete patient details.
- Manage patient profile information.
- Schedule and manage patient appointments.
- Filter, sort and paginate patient and appointment records.
- Store and retrieve data from a PostgreSQL database.

This project was developed as a **Proof of Concept (POC)** to understand how different .NET technologies work together in an application.

---

## 2. Technologies Used

| Technology | Where and why it is used |
|---|---|
| C# | Main programming language used to develop the application. |
| .NET 10 | Framework used to build and run the application. |
| ASP.NET Core Web API | Used to create REST API endpoints. |
| Entity Framework Core | Used to communicate with the database. |
| PostgreSQL | Database used to store application data. |
| pgAdmin 4 | Used to manage and view the PostgreSQL database. |
| Npgsql | PostgreSQL provider used by Entity Framework Core. |
| AutoMapper | Used to convert entities into DTOs and DTOs into entities. |
| LINQ | Used for filtering, sorting, grouping and querying records. |
| JWT | Used to authenticate users and protect API endpoints. |
| Swagger | Used to view and test API endpoints. |
| VS Code | Code editor used for development. |

---

## 3. Project Architecture

The application follows **Clean Architecture** with four separate projects.

### Project Structure

```text
PatientManagement
│
├── PatientManagement.Domain
│   ├── Entities
│   │   ├── Patient.cs
│   │   ├── PatientProfile.cs
│   │   ├── Appointment.cs
│   │   └── User.cs
│   │
│   └── Enums
│       ├── Gender.cs
│       └── AppointmentStatus.cs
│
├── PatientManagement.Application
│   ├── DTOs
│   │   ├── CreatePatientDto.cs
│   │   ├── UpdatePatientDto.cs
│   │   ├── PatientResponseDto.cs
│   │   ├── CreatePatientProfileDto.cs
│   │   ├── PatientProfileResponseDto.cs
│   │   ├── CreateAppointmentDto.cs
│   │   ├── AppointmentResponseDto.cs
│   │   ├── RegisterRequestDto.cs
│   │   ├── LoginRequestDto.cs
│   │   └── AuthResponseDto.cs
│   │
│   ├── Interfaces
│   │   └── ITokenService.cs
│   │
│   ├── Profiles
│   │   └── PatientMappingProfile.cs
│   │
│   └── Validation
│       └── NotFutureDateAttribute.cs
│
├── PatientManagement.Infrastructure
│   ├── Data
│   │   └── PatientDbContext.cs
│   │
│   ├── Migrations
│   │
│   └── Services
│       └── JwtTokenService.cs
│
└── PatientManagement.API
    ├── Controllers
    │   ├── PatientsController.cs
    │   ├── PatientProfilesController.cs
    │   ├── AppointmentsController.cs
    │   └── AuthController.cs
    │
    ├── Program.cs
    ├── appsettings.json
    └── appsettings.Development.json
```

### What each project does

**1. PatientManagement.Domain**

This is the core project. It contains the main entities and business rules.

Examples:
- Patient
- PatientProfile
- Appointment
- User
- Gender and AppointmentStatus enums

Business rules are also implemented here.

For example:
- Patient age validation.
- Updating patient contact information.
- Completing an appointment.
- Cancelling an appointment.

**2. PatientManagement.Application**

This project contains the DTOs, interfaces, mapping profiles and custom validation.

DTOs are used to transfer data between the API and the application.

For example, `CreatePatientDto` receives patient information, while `PatientResponseDto` returns patient details.

**3. PatientManagement.Infrastructure**

This project handles database communication and infrastructure-related services.

It contains:
- `PatientDbContext`
- Entity Framework Core configurations.
- Database migrations.
- JWT token generation service.

**4. PatientManagement.API**

This is the entry point of the application.

It contains:
- API controllers.
- API routes.
- Dependency injection registrations.
- JWT authentication configuration.
- Swagger configuration.
- Application startup configuration.

---

## 4. Database Design

The application uses PostgreSQL as its database.

Database name: `PatientManagementDB`

### Tables Created

| Table | Purpose |
|---|---|
| Patients | Stores patient details. |
| PatientProfiles | Stores address and emergency contact information. |
| Appointments | Stores appointment details. |
| Users | Stores registered user information and password hashes. |
| __EFMigrationsHistory | Stores the applied Entity Framework migrations. |

### Database Relationships

**Patient and PatientProfile – One-to-One**

One patient can have only one profile.

The PatientProfiles table contains `PatientId` as a foreign key.

**Patient and Appointment – One-to-Many**

One patient can have multiple appointments.

The Appointments table contains `PatientId` as a foreign key.

**Users**

The Users table stores login and registration details separately from patient records.

---

## 5. Concepts Implemented and Where They Are Used

### 5.1 C# and Object-Oriented Programming

The project uses basic C# and OOP concepts throughout the application.

| Concept | Where it is used |
|---|---|
| Classes and Objects | Patient, PatientProfile, Appointment and User entities. |
| Properties | Entity and DTO properties. |
| Constructors | Patient entity initialization. |
| Encapsulation | Private setters for Patient Age and Appointment Status. |
| Methods | UpdateAge(), UpdateContactInformation(), CompleteAppointment() and CancelAppointment(). |
| Enums | Gender and AppointmentStatus. |
| Interfaces | ITokenService. |
| Exception Handling | Invalid appointment status transitions and validation rules. |
| Collections | ICollection<Appointment> in Patient entity. |

### 5.2 ASP.NET Core Web API

Used to create REST API endpoints.

Concepts implemented:
- Controllers and routing.
- HTTP methods: GET, POST, PUT, PATCH and DELETE.
- IActionResult.
- Model binding.
- JSON request and response handling.
- HTTP status codes.
- Swagger/OpenAPI documentation.

The controllers are:
- PatientsController
- PatientProfilesController
- AppointmentsController
- AuthController

### 5.3 DTOs and Model Validation

DTOs are used instead of directly accepting and returning entity objects.

Examples:

- CreatePatientDto
- UpdatePatientDto
- PatientResponseDto
- CreatePatientProfileDto
- CreateAppointmentDto
- RegisterRequestDto
- LoginRequestDto
- AuthResponseDto

Data annotations used:
- `[Required]`
- `[Length]`
- `[Range]`
- `[EmailAddress]`
- `[Phone]`
- `[MinLength]`
- `[EnumDataType]`

A custom validation attribute, `NotFutureDateAttribute`, was also created to prevent future dates of birth.

The `[ApiController]` attribute enables automatic validation of invalid model data and returns a 400 Bad Request response.

### 5.4 AutoMapper

AutoMapper is used to avoid manually copying properties between DTOs and entities.

Implementation is available in:

`PatientManagement.Application/Profiles/PatientMappingProfile.cs`

Concepts implemented:

- `CreateMap()`
- `ForMember()`
- `MapFrom()`
- `ReverseMap()`
- `ForCtorParam()`
- `IMapper`
- `ProjectTo()`
- AutoMapper dependency injection registration.

Custom mapping example:

Patient FirstName and LastName are combined to create the FullName property in PatientResponseDto.

### 5.5 Entity Framework Core

Entity Framework Core is used to interact with PostgreSQL.

Implementation is available in:

`PatientManagement.Infrastructure/Data/PatientDbContext.cs`

Concepts implemented:

- DbContext.
- DbSet.
- Fluent API.
- Primary keys.
- Foreign keys.
- Unique indexes.
- One-to-one relationships.
- One-to-many relationships.
- AsNoTracking().
- Asynchronous database operations.
- SaveChangesAsync().
- EF Core migrations.

Entity configurations are written inside `OnModelCreating()`.

### 5.6 Database Migrations

EF Core migrations are used to create and update the PostgreSQL database schema.

Commands used:

Create migration:

```bash
dotnet ef migrations add InitialCreate --project PatientManagement.Infrastructure --startup-project PatientManagement.API --output-dir Migrations
```

Apply migration:

```bash
dotnet ef database update --project PatientManagement.Infrastructure --startup-project PatientManagement.API
```

An additional migration was created for the Users table during JWT implementation.

### 5.7 LINQ Implementation

LINQ is used in PatientsController and AppointmentsController.

The following LINQ methods were implemented:

| LINQ method | Usage |
|---|---|
| Where() | Filter patients by age and gender, and appointments by patient and status. |
| OrderBy() | Sort records in ascending order. |
| OrderByDescending() | Sort records in descending order. |
| ThenBy() | Apply additional sorting. |
| Select() | Select and shape query results, including grouped statistics. |
| Count() | Count patient and appointment records. |
| Average() | Calculate average patient age. |
| Min() | Find the minimum patient age. |
| Max() | Find the maximum patient age. |
| GroupBy() | Group patients by age range and appointments by status. |
| Any() | Check whether an email or related record exists. |
| FirstOrDefault() | Retrieve a matching record. |
| Skip() | Skip records for pagination. |
| Take() | Retrieve a limited number of records. |

Pagination, filtering and sorting are implemented using query parameters.

Statistics endpoints are also implemented for patients and appointments.

### 5.8 Configuration Management

`IConfiguration` is used to read application settings from `appsettings.json`.

A `PatientSettings` section was added with:
- MaxPatientsToReturn.
- ApplicationName.
- EnablePatientRegistration.

A configuration endpoint was created in PatientsController to read and return these values.

### 5.9 Dependency Injection

Dependency injection is used throughout the application.

Examples:
- PatientDbContext is injected into controllers.
- IMapper is injected into controllers.
- IConfiguration is injected where configuration values are needed.
- ITokenService is injected into AuthController.
- IPasswordHasher<User> is injected into AuthController.

Service registrations are configured in Program.cs.

### 5.10 JWT Authentication

JWT authentication is implemented to protect the API endpoints.

Files involved:

- `User.cs` – stores registered user details.
- `RegisterRequestDto.cs` – receives registration details.
- `LoginRequestDto.cs` – receives login details.
- `AuthResponseDto.cs` – returns the authentication token.
- `ITokenService.cs` – defines the token generation contract.
- `JwtTokenService.cs` – generates JWT tokens.
- `AuthController.cs` – handles registration and login.

Concepts implemented:

- User registration.
- Password hashing using PasswordHasher<User>.
- Password verification.
- JWT token generation.
- JWT signing.
- Token expiration.
- Issuer and audience validation.
- JWT Bearer authentication.
- `[Authorize]` attribute.
- Swagger Bearer token authorization.

The PatientsController, PatientProfilesController and AppointmentsController are protected using `[Authorize]`.

The AuthController registration and login endpoints are public.

Role-based authorization was not implemented as part of this POC.

---

## 6. API Endpoints

### Authentication – AuthController

| Method | Endpoint | Purpose |
|---|---|---|
| POST | /api/Auth/register | Register a new user. |
| POST | /api/Auth/login | Log in and receive a JWT. |

### Patient Management – PatientsController

| Method | Endpoint | Purpose |
|---|---|---|
| GET | /api/Patients | Get patients with filtering, sorting and pagination. |
| GET | /api/Patients/{id} | Get patient by ID. |
| POST | /api/Patients | Create a patient. |
| PUT | /api/Patients/{id} | Update patient details. |
| DELETE | /api/Patients/{id} | Delete a patient. |
| GET | /api/Patients/statistics | Get patient statistics. |
| GET | /api/Patients/check-email | Check whether an email already exists. |
| GET | /api/Patients/configuration | Read configured patient settings. |

### Patient Profile Management – PatientProfilesController

| Method | Endpoint | Purpose |
|---|---|---|
| POST | /api/PatientProfiles | Create a patient profile. |
| GET | /api/PatientProfiles/{patientId} | Get a profile by patient ID. |
| PUT | /api/PatientProfiles/{patientId} | Update a patient profile. |
| DELETE | /api/PatientProfiles/{patientId} | Delete a patient profile. |

### Appointment Management – AppointmentsController

| Method | Endpoint | Purpose |
|---|---|---|
| POST | /api/Appointments | Create an appointment. |
| GET | /api/Appointments | Get appointments with filtering, sorting and pagination. |
| GET | /api/Appointments/{id} | Get appointment by ID. |
| PUT | /api/Appointments/{id} | Update an appointment. |
| PATCH | /api/Appointments/{id}/complete | Complete an appointment. |
| PATCH | /api/Appointments/{id}/cancel | Cancel an appointment. |
| DELETE | /api/Appointments/{id} | Delete an appointment. |
| GET | /api/Appointments/statistics | Get appointment statistics. |
| GET | /api/Appointments/check-patient/{patientId} | Check whether a patient has appointments. |

All patient, profile and appointment endpoints require a valid JWT.

---

## 7. Appointment Business Rules

The Appointment entity contains an AppointmentStatus enum:

- Scheduled
- Completed
- Cancelled

New appointments are created with Scheduled status.

Two methods are implemented in Appointment.cs:

- `CompleteAppointment()`
- `CancelAppointment()`

Only appointments with Scheduled status can be completed or cancelled.

Trying to complete an already completed appointment or cancel an already cancelled appointment returns an error.

---

## 8. How to Run the Project

### Prerequisites

Install the following:

- .NET 10 SDK.
- PostgreSQL.
- pgAdmin 4.
- Visual Studio Code.
- EF Core CLI tool.

### Step 1: Clone the repository

```bash
git clone <your-repository-url>
```

### Step 2: Navigate to the solution

```bash
cd PatientManagement
```

### Step 3: Configure the database

Open `PatientManagement.API/appsettings.json` and configure the PostgreSQL connection string.

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5433;Database=PatientManagementDB;Username=postgres;Password=YOUR_PASSWORD"
}
```

Replace the password with your local PostgreSQL password.

Configure the JWT signing key and other JWT settings before running the application. Do not use a real signing key from source control.

### Step 4: Restore packages

```bash
dotnet restore
```

### Step 5: Apply migrations

```bash
dotnet ef database update --project PatientManagement.Infrastructure --startup-project PatientManagement.API
```

### Step 6: Build the solution

```bash
dotnet build
```

### Step 7: Run the API

```bash
dotnet run --project PatientManagement.API
```

### Step 8: Open Swagger

Use the HTTPS or HTTP URL shown in the terminal, followed by `/swagger`.

Example:

```text
https://localhost:7252/swagger
```

Register a user, log in, copy the JWT token, and click Authorize in Swagger to test the protected endpoints.

---

## 9. Testing

All implemented API operations were tested using Swagger.

The following areas were tested:

- Patient CRUD operations.
- Patient profile CRUD operations.
- Appointment CRUD operations.
- Appointment completion and cancellation.
- Patient and appointment filtering.
- Sorting and pagination.
- Patient and appointment statistics.
- Duplicate email and duplicate profile validation.
- Database relationships.
- User registration and login.
- Password hashing and verification.
- JWT token generation.
- Access to protected endpoints.
- Invalid authentication requests.

The application was also built successfully using the .NET CLI.

---

## 10. Conclusion

Through this project, I gained practical experience in developing a .NET Web API using C# and ASP.NET Core.

I learned how to structure an application using Clean Architecture, create REST API endpoints, work with PostgreSQL using Entity Framework Core, and implement CRUD operations.

I also implemented DTOs, model validation, AutoMapper, LINQ queries, filtering, sorting, pagination and JWT authentication.

This project helped me understand how different .NET concepts work together in a real application instead of learning them only through theory and individual examples.