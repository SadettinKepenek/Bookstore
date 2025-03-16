# Getting Started

This project provides a simple book store API. Follow the steps below to get started with the project.

## Prerequisites

Before you begin, ensure you have the following tools installed:

- [Docker](https://www.docker.com/get-started) - To run the project in containers.
- [Make](https://www.gnu.org/software/make/) - To use the Makefile commands.
- [.NET SDK](https://dotnet.microsoft.com/download) - Required for building and running the .NET application. (.NET 8.0 is used for this app)

## Setting Up the Project

### 1. Start Docker Containers

To start the required Docker containers (PostgreSQL database), run the following command:

```bash
make run-docker-compose
```
This will start the necessary services in the background, including the PostgreSQL database.

### 2. Generate Migrations for the Book Database
To generate the database migration for the book module, run the following command:

```bash
make migrate-book-db
```
This will create the migration files for the BookDbContext in the Migrations folder.

### 3. Generate Migrations for the Order Database
To generate the database migration for the order module, run the following command:

```bash
make migrate-order-db
```
This will create the migration files for the OrderDbContext in the Migrations folder.

### 4. Apply Migrations for the Book Database
To apply the generated migration and update the book database, run the following command:

```bash
make update-book-db
```
This will update the book database schema according to the migration files.

### 5. Apply Migrations for the Order Database
To apply the generated migration and update the order database, run the following command:

```bash
make update-order-db
```
This will update the order database schema according to the migration files.

### 6. Run the Application
To run the application, run the following command:

```bash
make run-app
```
This will start the application locally.
You can access swagger from http://localhost:5089/swagger/index.html

## Unit Tests
To run the unit tests for the book and order modules, execute the following command:

```bash
make run-tests
```

# Project Architecture

This project follows a layered architecture, which includes the **Book**, **Order**, and **Saga** domains, as well as an **API** layer for controller management. The application is developed using Domain-Driven Design (DDD) principles and Entity Framework Core (EF Core) for data persistence. Below is an overview of the architecture and the reasoning behind the choices made.

## Layers in the Project

### 1. **Book.Application** and **Order.Application**

These layers contain the business logic for their respective domains. The application layer interacts with the domain layer to perform necessary actions and prepare data for the outside world.

- **Book.Application**: Handles business operations related to books, such as adding and listing. It serves as the bridge between the domain layer and the API layer.

- **Order.Application**: Manages business operations related to orders, including creating orders or updating orders. Similar to `Book.Application`, it provides services for interacting with the `Order` domain.

Both application layers ensure that the business logic is separated from the infrastructure, making it easier to maintain and extend the application.

### 2. **Book.Domain** and **Order.Domain**

These are the core domain layers where the actual business logic resides. DDD principles are followed to create rich domain models that represent real-world entities and their behaviors.

- **Book.Domain**: This layer contains the business rules for books, including operations like creating a book, managing stock levels, and handling book categories. This layer focuses on the domain's intrinsic behaviors and ensures that business rules are implemented properly.

- **Order.Domain**: This layer is responsible for managing orders. It includes operations such as creating an order, validating the order, and handling its lifecycle. Domain services and aggregates related to orders are found here.

In both domain layers, **Book** and **Order** models are defined as aggregate root, following the DDD principles to model the business processes.

### 3. **Book.Infrastructure** and **Order.Infrastructure**

These layers are responsible for the infrastructure-related concerns, such as data access and persistence. They contain implementations for repositories, migrations, and database context.

- **Book.Infrastructure**: This layer contains EF Core configurations for the `Book` entity and related database operations. It includes the `DbContext`, migrations, and repositories for interacting with the database.

- **Order.Infrastructure**: Similar to `Book.Infrastructure`, this layer handles the persistence of orders. It contains the `DbContext`, migrations, and repositories for managing the order-related data in the database.

The infrastructure layers provide the concrete implementations that interact with the database, while the application and domain layers focus on business logic.

### 4. **Bookstore.Api**

This is the presentation layer where controllers reside. The **Bookstore.Api** layer acts as an interface for clients, exposing endpoints to interact with the system.

- **Controllers**: They provide the endpoints for creating, updating, and retrieving books and orders. The controllers interact with the services provided by the `Book.Application` and `Order.Application` layers.

- **Dependency Injection**: Services and domain models are injected into the controllers, ensuring that the application layer's services are available to handle requests.

This layer serves as the API for external clients to interact with the application.

## Why and How Saga is Used

### **Saga Application Layer**

The **Saga.Application** layer is introduced to manage distributed transactions and complex workflows. In a microservices-based architecture, where different services need to collaborate to complete a business process, maintaining data consistency and managing failures are crucial.

**Why Saga?**
- **Distributed Transactions**: Since our system interacts with multiple bounded contexts (such as the `Book` and `Order` domains), managing transactions across these services requires a mechanism like Saga. A single transaction might span across multiple services, and if any step fails, the saga ensures that compensating actions are taken to maintain consistency.


**How Saga Works in This Project**
1. **Step Execution**: The saga orchestrator executes each step in the defined sequence, starting from book validation and order creation to stock updates and order completion.
2. **Failure Handling**: If any step fails, the saga orchestrator ensures that previous steps are rolled back by invoking compensating transactions (rollback actions).
3. **State Management**: The saga maintains the state of each step in the process, ensuring that any step failure is handled appropriately, and subsequent steps are skipped if needed.
4. **Reliability**: Using Saga patterns, the system ensures eventual consistency and resilience in the face of failures. Each step is processed independently, and the state is updated after each successful step, ensuring a robust workflow.

## Advantages of Using Entity Framework Core (EF Core)

EF Core provides a powerful ORM (Object-Relational Mapper) for interacting with databases in .NET. Here are some of the key advantages of using EF Core in this project:

### 1. **Change Tracking**

EF Core's built-in **change tracker** is highly compatible with DDD. It automatically tracks changes to entities during the lifecycle of an object, which aligns perfectly with DDD's focus on domain entities and aggregates.

- **Domain Object State Management**: EF Core automatically detects changes in domain objects and can persist those changes to the database. This means you can focus on your domain logic and let EF Core handle the persistence of state.

- **Optimistic Concurrency Control**: EF Core also supports optimistic concurrency control, where the change tracker ensures that updates to the same record by multiple processes don't conflict, providing a safe environment for concurrent operations. For this project, optistic concurrency features added for stock operations.

### 2. **Migrations and Schema Management**

EF Core's migrations are a key feature that allows for easy database schema evolution. The migrations system ensures that schema changes are tracked, and the database is updated consistently.

- **Seamless Database Evolution**: As your domain model evolves, EF Core's migrations allow you to safely apply database schema changes, ensuring that your database is in sync with your domain model.

- **Version Control**: Each migration is versioned, and you can easily roll back to previous migrations if necessary.


---

This architecture leverages the full power of DDD, EF Core, and Saga to build a robust and scalable application. The separation of concerns across layers ensures maintainability, while Saga provides a reliable approach to managing distributed transactions in a microservices-based system.

