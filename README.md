# FlowForge

FlowForge is a backend platform for managing business workflows and order operations.

The project is designed as a practical enterprise-oriented .NET backend rather than a simple CRUD application. It starts as a modular monolith and evolves incrementally as real business and distributed-system requirements emerge.

The main goal is to explore how a modern .NET backend is designed, tested, observed, secured, and eventually distributed in a production-like environment.

---

## Overview

FlowForge models an order-driven business workflow.

A simplified version of the initial workflow is:

```text
Customer
   │
   ▼
Create Order
   │
   ▼
Validate Products
   │
   ▼
Calculate Total
   │
   ▼
Persist Order
   │
   ▼
Order Created
```

As the system evolves, the workflow will be extended with components such as inventory, payment, shipping, notifications, and asynchronous event processing.

The project is intentionally built incrementally. Technologies are introduced when the system has a real problem that justifies them.

---

## Goals

FlowForge is primarily a learning and engineering project focused on real-world backend development.

The project aims to cover:

* Modern C# development
* ASP.NET Core Web API
* REST API design
* Domain modeling
* Dependency Injection
* Entity Framework Core
* PostgreSQL
* Database transactions and concurrency
* Authentication and authorization
* Background processing
* Distributed caching
* Messaging and event-driven architecture
* Resilience and fault tolerance
* Observability
* Automated testing
* Containerization
* CI/CD
* Kubernetes
* Distributed system design

The objective is not to use every technology possible.

Each technology should solve an actual problem in the system.

---

## Tech Stack

### Current

| Technology            | Purpose                      |
| --------------------- | ---------------------------- |
| C#                    | Primary programming language |
| .NET 10               | Application runtime and SDK  |
| ASP.NET Core          | Web API framework            |
| Entity Framework Core | ORM and data access          |
| PostgreSQL            | Relational database          |
| OpenAPI / Swagger     | API documentation            |
| xUnit                 | Automated testing            |
| Git                   | Version control              |

### Planned

The following technologies and concepts will be introduced progressively:

| Technology / Concept   | Purpose                                       |
| ---------------------- | --------------------------------------------- |
| Redis                  | Distributed caching and concurrency scenarios |
| BackgroundService      | Background processing                         |
| Kafka / RabbitMQ       | Asynchronous messaging                        |
| Outbox Pattern         | Reliable event publishing                     |
| CQRS                   | Separation of command and query models        |
| Domain Events          | Decoupling domain actions                     |
| DDD                    | Rich domain modeling                          |
| gRPC                   | Internal service-to-service communication     |
| SignalR                | Real-time client updates                      |
| OpenTelemetry          | Distributed observability                     |
| Prometheus             | Metrics                                       |
| Grafana                | Metrics visualization                         |
| Serilog                | Structured logging                            |
| Elasticsearch / Kibana | Centralized log management                    |
| Docker                 | Application containerization                  |
| Docker Compose         | Local infrastructure orchestration            |
| Kubernetes             | Container orchestration                       |
| Helm                   | Kubernetes package management                 |
| .NET Aspire            | Local distributed application orchestration   |
| GitHub Actions         | CI/CD                                         |
| Testcontainers         | Integration testing with real infrastructure  |

Not all planned technologies will necessarily become production dependencies. Some will be evaluated against alternatives as the architecture evolves.

---

## Architecture

The initial version of FlowForge intentionally starts as a **modular monolith**.

```text
┌───────────────────────────────┐
│        FlowForge.Api          │
│                               │
│  Controllers                 │
│  Application Logic           │
│  Domain Logic                │
│  Infrastructure              │
│                               │
└───────────────┬───────────────┘
                │
                ▼
         ┌─────────────┐
         │ PostgreSQL  │
         └─────────────┘
```

The architecture will evolve based on actual requirements.

Potential future architecture:

```text
                         ┌──────────────────┐
                         │    API Gateway   │
                         └────────┬─────────┘
                                  │
                 ┌────────────────┼────────────────┐
                 │                │                │
                 ▼                ▼                ▼
          ┌────────────┐   ┌────────────┐   ┌────────────┐
          │ Order      │   │ Inventory  │   │ Payment    │
          │ Service    │   │ Service    │   │ Service    │
          └─────┬──────┘   └─────┬──────┘   └─────┬──────┘
                │                │                │
                └────────────────┼────────────────┘
                                 ▼
                         ┌──────────────┐
                         │    Kafka     │
                         └──────────────┘
                                 │
                    ┌────────────┼────────────┐
                    ▼            ▼            ▼
              Notification   Shipping     Analytics
```

This architecture is **not implemented yet**. It represents the direction in which the system may evolve.

---

## Initial Domain

The initial domain focuses on orders and products.

### Customer

Represents a customer who can create orders.

### Product

Represents a product that can be purchased.

### Order

Represents a customer's purchase request and its lifecycle.

### OrderItem

Represents a product and quantity belonging to an order.

An important domain decision is that `OrderItem` stores the **unit price at the time of purchase**.

For example:

```text
Product price today:
$100

Customer creates order:
UnitPrice = $100

Product price later changes:
$150

Existing order:
UnitPrice = $100
```

This prevents historical orders from changing when the current product price changes.

---

## API

The initial API will expose REST endpoints.

Example:

```http
POST /api/orders
```

Example request:

```json
{
  "customerId": 1,
  "items": [
    {
      "productId": 10,
      "quantity": 2
    }
  ]
}
```

The server is responsible for:

* Validating the customer
* Validating products
* Validating quantities
* Calculating order totals
* Applying business rules
* Persisting the order

Client-provided totals will not be trusted.

---

## Engineering Principles

FlowForge follows several engineering principles.

### Business logic belongs to the domain

Controllers should coordinate HTTP concerns rather than contain business rules.

### Persistence is an implementation detail

Database access should not dictate the domain model unnecessarily.

### Validate at the boundary

Invalid requests should be rejected as early as possible.

### Never trust calculated values from clients

Important values such as order totals and prices are calculated server-side.

### Prefer explicit business rules

Business rules should be readable and testable rather than hidden inside controllers or database queries.

### Introduce complexity only when justified

The project will not adopt microservices, Kafka, Redis, Kubernetes, or other infrastructure simply because they are popular.

A technology should exist because the system has a problem that the technology solves.

### Test behavior, not implementation details

Tests should primarily verify observable application behavior and business requirements.

---

## Testing Strategy

Testing will evolve together with the architecture.

Planned testing layers include:

```text
Unit Tests
    │
    ├── Domain rules
    ├── Business logic
    └── Pure functions
          │
          ▼
Integration Tests
    │
    ├── API
    ├── PostgreSQL
    ├── Redis
    └── Messaging
          │
          ▼
Contract Tests
    │
    └── Service boundaries
```

The project will favor real infrastructure for integration tests when infrastructure behavior itself is part of what needs to be verified.

---

## Observability

As FlowForge becomes distributed, observability will become a core part of the system.

The planned observability stack includes:

```text
Application
    │
    ├── Logs
    ├── Metrics
    └── Traces
          │
          ▼
     OpenTelemetry
          │
     ┌────┼─────┐
     ▼    ▼     ▼
   Logs Metrics Traces
     │    │     │
     ▼    ▼     ▼
 Elastic Grafana  ...
```

The objective is to make failures diagnosable across service boundaries rather than relying only on application logs.

---

## Development Roadmap

### Phase 1 — Core API

* [ ] Domain discovery
* [ ] Customer model
* [ ] Product model
* [ ] Order model
* [ ] OrderItem model
* [ ] PostgreSQL integration
* [ ] EF Core migrations
* [ ] Order API
* [ ] Validation
* [ ] Unit tests
* [ ] Integration tests

### Phase 2 — Production API

* [ ] Global exception handling
* [ ] ProblemDetails
* [ ] Structured logging
* [ ] Configuration and Options pattern
* [ ] Health checks
* [ ] Authentication
* [ ] Authorization
* [ ] Rate limiting
* [ ] API versioning
* [ ] Idempotency
* [ ] Concurrency handling

### Phase 3 — Background Processing

* [ ] BackgroundService
* [ ] Asynchronous jobs
* [ ] Retry policies
* [ ] Cancellation handling
* [ ] Failure recovery

### Phase 4 — Distributed Systems

* [ ] Redis
* [ ] Distributed locking
* [ ] Messaging
* [ ] Kafka / RabbitMQ evaluation
* [ ] Domain events
* [ ] Integration events
* [ ] Outbox pattern
* [ ] Idempotent consumers
* [ ] Dead-letter handling

### Phase 5 — Service Decomposition

Potential service boundaries:

```text
Order Service
Inventory Service
Payment Service
Shipping Service
Notification Service
```

Services will be extracted only when there is a clear architectural reason to do so.

### Phase 6 — Observability

* [ ] OpenTelemetry
* [ ] Distributed tracing
* [ ] Metrics
* [ ] Prometheus
* [ ] Grafana
* [ ] Centralized logging
* [ ] Elasticsearch
* [ ] Kibana
* [ ] Correlation / Trace IDs

### Phase 7 — Infrastructure

* [ ] Docker
* [ ] Docker Compose
* [ ] Kubernetes
* [ ] Helm
* [ ] ConfigMaps
* [ ] Secrets
* [ ] Health probes
* [ ] Horizontal scaling
* [ ] Rolling deployments

### Phase 8 — Delivery

* [ ] GitHub Actions
* [ ] CI pipeline
* [ ] Automated tests
* [ ] Container image builds
* [ ] Deployment pipeline
* [ ] Environment management

---

## Project Structure

The initial structure is intentionally small:

```text
FlowForge/
│
├── FlowForge.sln
│
├── src/
│   └── FlowForge.Api/
│       ├── Controllers/
│       ├── Properties/
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── FlowForge.Api.csproj
│
└── tests/
    └── FlowForge.Api.Tests/
        ├── GlobalUsings.cs
        └── FlowForge.Api.Tests.csproj
```

As architectural boundaries become necessary, the solution may evolve into:

```text
src/
├── FlowForge.Api
├── FlowForge.Domain
├── FlowForge.Application
├── FlowForge.Infrastructure
└── FlowForge.Worker
```

These projects are intentionally **not created at the beginning**.

---

## Running Locally

### Requirements

* .NET 10 SDK
* PostgreSQL
* Git

### Clone

```bash
git clone <repository-url>
cd FlowForge
```

### Restore

```bash
dotnet restore
```

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run --project src/FlowForge.Api
```

### Test

```bash
dotnet test
```

---

## Development Philosophy

FlowForge is developed as an evolving backend system rather than a collection of isolated tutorials.

The implementation starts simple and becomes more sophisticated as new requirements introduce real engineering problems.

The intended progression is:

```text
Simple API
    ↓
Business Rules
    ↓
Reliable Persistence
    ↓
Testing
    ↓
Production Concerns
    ↓
Background Processing
    ↓
Messaging
    ↓
Distributed Systems
    ↓
Observability
    ↓
Containerization
    ↓
Kubernetes
    ↓
CI/CD
```

The purpose is to understand **why** each architectural decision exists, not simply how to configure the technology.

---

## Status

🚧 **In Development**

The project is currently in the initial domain discovery and core API design phase.
