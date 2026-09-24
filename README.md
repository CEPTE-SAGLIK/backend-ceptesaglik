# Sağlık Pusulası – Backend Architecture (API & Services)

A modular, cross-platform healthcare management and patient-tracking backend built with **C# / ASP.NET Core (.NET 8)**, **PostgreSQL**, and **Docker** containerization.

---

## 🏛 Architecture Overview
The solution is organized using **N-Tier / Clean Architecture** principles to maintain strict separation of concerns, high testability, and enterprise-grade maintainability:

* **HealthApp.Domain:** Core entities, database models, enums, and domain interfaces.
* **HealthApp.DataAccess:** Entity Framework Core configuration, DbContext, Fluent API mappings, and migrations for PostgreSQL.
* **HealthApp.Business:** Business rules, service interfaces/implementations, Generic Repository patterns, and DTO object mappings.
* **HealthApp.API:** Secure RESTful API endpoints, dependency injection configuration, and Swagger documentation.
* **Infrastructure / DevOps:** Containerized execution using `Dockerfile` and `docker-compose.yml`.

---

## 👥 Engineering Team & Module Ownership

Developed by a 4-person engineering team at **Konya Food and Agriculture University** (Computer Engineering Capstone Project):

### • Zeki Zafer Aydınlı ([@zeki582](https://github.com/zeki582)) – Backend & Data Engineering
* **Relational Health Modules:** Designed and developed business logic, DTO mappings, and CRUD/filtering REST endpoints for core patient tracking services:
  * `IllnessService` / `IllnessesController` (Chronic & diagnosed medical records)
  * `AllergyService` / `AllergiesController` (Patient allergen profiles)
  * `MedicineReminderService` / `MedicineRemindersController` (Prescription schedules & intake reminders)
  * `VaccineScheduleService` / `VaccineSchedulesController` (Immunization timelines)
* **Data Persistence & DevOps:** Configured Entity Framework Core entities, repository implementations, and assisted with Docker service configuration for containerized deployment.
* **System Design & Prototyping:** Co-modeled application system flows, requirements, and interactive interface prototypes in Figma.

### • Team Members & Contributions
* **Emir Ata Karagenç ([@ONLYPREVIEW](https://github.com/ONLYPREVIEW)):** Core Architecture, Authentication/JWT, User/Profile Services, and Branch Management.
* **Rümeysa Semiz ([@rumeysa-semiz](https://github.com/rumeysa-semiz)):** Docker environment orchestration, API integration, and health service endpoints.
* **Beyza ([@777beyza](https://github.com/777beyza)):** Health metrics, notification logic, and business validation rules.

---

## 🛠 Tech Stack
* **Language & Runtime:** C# | .NET 8 (ASP.NET Core Web API)
* **ORM & Database:** Entity Framework Core | PostgreSQL
* **Containerization:** Docker | Docker Compose
* **Design & Collaboration:** Figma | Git / GitHub
