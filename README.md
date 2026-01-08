# 🛒 Inno_Shop

**Inno_Shop** is a **microservices-based backend system** built with **ASP.NET Core**, designed to demonstrate modern backend engineering practices such as service separation, secure authentication, ownership-based authorization, and containerized deployment.

The project consists of two independent microservices — **User Service** and **Product Service** — communicating via REST APIs and secured with **JWT authentication**. It was developed as a **personal learning project** with a strong focus on clean architecture, security, and real-world API design.

## Architecture Overview

- **User Service**
  - Manages user accounts, roles, and authentication
  - Handles email confirmation and password recovery
  - Issues JWT tokens for authenticated access

- **Product Service**
  - Manages products owned by users
  - Enforces ownership-based access control
  - Provides search and filtering functionality

Each service:
- Has its **own database**
- Is **independently deployable**
- Exposes a **RESTful API**
- Can be run locally or via **Docker Compose**


## Key Features

- **Microservices architecture** with separated responsibilities
- **ASP.NET Core Web API**
- **JWT-based authentication and authorization**
- **Role-based and ownership-based access control**
- **Email confirmation and password recovery flows**
- **Full CRUD operations** for users and products
- **Search and filtering** with request validation
- **Global error handling** using Problem Details
- **Entity Framework Core (Code First)** with relational databases
- **Docker & Docker Compose** for services and databases
- **Unit tests and basic integration tests**

## Security Model

- Authentication implemented using **JWT tokens**
- Authorization based on:
  - **User roles**
  - **Resource ownership** (users can manage only their own products)
- Protected endpoints for create, update, and delete operations
- Email verification required for account activation
- Secure password reset mechanism



## Data Access

- **Entity Framework Core (Code First)**
- Separate database per microservice
- Validated request models
- Consistent error responses using **ProblemDetails**

## Testing

- **Unit tests** for business logic
- **Basic integration tests** for API endpoints
- Test structure prepared for further extension



## Deployment

The project is prepared for containerized deployment:

- Dockerfiles for each microservice
- `docker-compose.yml` including:
  - User Service
  - Product Service
  - Relational databases



## Purpose of the Project

This project was created to practice and demonstrate:
- Microservices architecture in .NET
- Secure API design
- Real-world authentication and authorization flows
- Clean project structure (Clean / Onion Architecture)
- Containerized development and deployment

It reflects skills directly applicable to **commercial .NET backend development**.


## Tech Stack

- **ASP.NET Core Web API**
- **JWT (JSON Web Tokens)**
- **Entity Framework Core**
- **FluentValidation**
- **Docker / Docker Compose**
- **xUnit and integration testing tools**
- **SQL-based relational databases**

## License

This project is for educational and portfolio purposes.
