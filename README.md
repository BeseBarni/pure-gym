# 🏋️ PureGym Management System (v1)

A system design and implementation of a 24/7 gym, featuring multiple locations and hardware entities.

This project follows **Clean Architecture** and its constraints, utilizing **CQRS** and **Event-Driven Architecture** to ensure high scalability and domain decoupling.

---

<div align="center">

![.NET 10](https://img.shields.io/badge/.NET_10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-20232A?style=for-the-badge&logo=react&logoColor=61DAFB)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![LGTM Stack](https://img.shields.io/badge/LGTM_Stack-F46800?style=for-the-badge&logo=grafana&logoColor=white)

<br/>

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)
![Architecture](https://img.shields.io/badge/Architecture-Clean_&_CQRS-success?style=for-the-badge)

</div>

---

## 🏛️ System Architecture

Below is the C4 Level 2 (Container) diagram illustrating the high-level architecture, component interactions, and data flow of the PureGym Management System.

![PureGym C4 Level 2 Architecture Diagram](./docs/diagrams/PureGym-C4-LVL2-v1.png)

---

## 🚀 Key Features

* **Event-Driven Architecture:** Implements the Outbox pattern to safely decouple the core API from physical door access hardware.
* **Comprehensive Observability:** Full LGTM tracing spans the entire request lifecycle (React Client -> .NET API -> RabbitMQ -> Hardware Mock).
* **Real-Time Feedback:** Utilizes Server-Sent Events (SSE) to push live updates and access statuses directly to the client.

---

## 🏗️ Architecture Decision Records (ADRs)

We document significant design decisions to track the project's evolution:

| ID | Title | Status |
|:---|:---|:---|
| [001](./docs/adr/ADR_001.md) | **Async Hardware Decoupling** | Accepted |
| [002](./docs/adr/ADR_002.md) | **PostgreSQL as Primary Store** | Accepted |
| [003](./docs/adr/ADR_003.md) | **Omission of Distributed Cache** | Accepted |
| [004](./docs/adr/ADR_004.md) | **LGTM Stack for Telemetry** | Accepted |
| [005](./docs/adr/ADR_005.md) | **Server-Sent Events (SSE) for Real-Time Feedback** | Accepted |

---

## 📐 Specifications

* **[Functional Specification (PDF)](./docs/specs/pure_gym_fs_v1.pdf):** A formal breakdown of the key features, business rules, and requirements of the system.