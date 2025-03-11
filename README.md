# ECommerceApp

A simple e-commerce application built with ASP.NET Core, featuring a Razor Pages frontend and a Web API backend. Users can browse products, create orders, and view order history, with admin capabilities for managing products and orders.

## Features

- **Frontend**: Razor Pages with Bootstrap 5 for a responsive UI.
- **Backend**: RESTful Web API with JWT authentication.
- **Order Processing**: Transactional order creation with stock updates via a stored procedure.
- **Admin Area**: Role-based dropdown for managing products and orders.
- **Data Access**: EF Core with a repository pattern and CQRS (MediatR).

## Architecture

- **WebApp**: Client-side rendering with Razor Pages, communicates with WebApi via HTTP.
- **WebApi**: Handles business logic, persistence, and authentication.
- **Application**: CQRS commands/queries with MediatR.
- **Domain**: Entities and interfaces.
- **Infrastructure**: EF Core and repository implementations.

## Setup

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Git

### Steps

1. **Clone the Repo**
   ```bash
   git clone https://github.com/yourusername/ECommerceApp.git
   cd ECommerceApp
   ```
2. **Configure Database**
   Update ECommerceApp.WebApi/appsettings.json with your connection string.
   Apply migrations:

```bash
Apply migrations:
dotnet ef migrations add InitialCreate -p src/ECommerceApp.Infrastructure -s src/ECommerceApp.WebApi -o Data/Migrations
dotnet ef database update -p src\ECommerceApp.Infrastructure -s src\ECommerceApp.WebApi
```

3. **Add Stored Procedure**
   Run in SQL Server:

```sql
CREATE TYPE dbo.OrderItemType AS TABLE
(
    ProductId INT,
    Quantity INT
);
GO

CREATE PROCEDURE UpdateProductStock
    @OrderItems dbo.OrderItemType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE p
        SET p.Quantity = p.Quantity - oi.Quantity
        FROM Products p
        INNER JOIN @OrderItems oi ON p.Id = oi.ProductId
        WHERE p.Quantity >= oi.Quantity;

        IF @@ROWCOUNT != (SELECT COUNT(*) FROM @OrderItems)
            THROW 50001, 'Insufficient stock for one or more products.', 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
```

4. **Set JWT Config**
   In ECommerceApp.WebApi/appsettings.json:

```json
"Jwt": {
    "Secret": "MySecretKeyHere1234567890MySecretKeyHere1234567890",
    "Issuer": "ECommerceApp",
    "Audience": "ECommerceApp",
    "ExpirationInMinutes": "60"
  }
```

5. **Run Locally**

- WebApi: cd ECommerceApp.WebApi && dotnet run
- WebApp: cd ECommerceApp.WebApp && dotnet run
- Visit WebApi at https://localhost:7051 and WebApp at https://localhost:7041.

**Docker Setup**

1. Docker Compose

- Run:

```
docker-compose up --build
```

- Apply migrations in webapi container

```
docker exec -it <webapi-container-id> dotnet ef database update
```

**_CI/CD Setup_**

1. **GitHub Actions**

- Workflow: .github/workflows/ci-cd.yml builds, publishes, and pushes Docker images to Docker Hub.
- Add secrets in GitHub:
  - DOCKER_USERNAME
  - DOCKER_PASSWORD (access token)

2. Trigger: Pushes/PRs to main branch.

**_API Endpoints_**

- **POST** /api/auth/login: { "username": "string", "password": "string" } → { "token": "jwt" }
- **GET** /api/products: Returns product list.
- **POST** /api/orders: { "orderItems": [{ "productId": int, "quantity": int }] } → Order ID (requires JWT).
- **GET** /api/orders: Returns user’s orders (requires JWT).

**License**

- MIT License
