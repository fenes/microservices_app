# Microservices App

Welcome to the **Microservices App** repository! This project demonstrates a microservices architecture with a focus on managing articles and reviews. The application is built using ASP.NET Core and Docker to containerize services.

## Table of Contents

- [Project Overview](#project-overview)
- [Architecture](#architecture)
- [Requirements](#requirements)
- [Technologies Used](#technologies-used)
- [Setup and Installation](#setup-and-installation)
- [Endpoints](#endpoints)
- [Testing](#testing)
- [Caching](#caching)
- [Contributing](#contributing)

## Project Overview

This project includes two microservices:

1. **Article Service**: Manages CRUD operations for articles.
2. **Review Service**: Manages CRUD operations for reviews and verifies the existence of the referenced article via a REST call to the Article Service.

An **API Gateway** is also included to route requests between the microservices.

## Architecture

The architecture includes:

- **API Gateway**: Routes incoming requests to the appropriate microservice.
- **Article Service**: Handles CRUD operations for articles.
- **Review Service**: Handles CRUD operations for reviews and checks article existence.
- **Database**: Each microservice uses its own PostgreSQL database.
- **Caching**: Redis is used to cache frequently read data.

## Requirements

To get started with this project, you will need:

- Docker
- .NET 6 SDK
- PostgreSQL
- Redis

## Technologies Used

- **ASP.NET Core**: Framework for building the microservices.
- **Entity Framework Core**: ORM for database operations.
- **PostgreSQL**: Relational database for storing data.
- **Redis**: In-memory data structure store for caching.
- **Docker**: Platform for containerizing the application.
- **Swagger**: Tool for API documentation.

## Setup and Installation

### 1. Clone the Repository

```bash
git clone https://github.com/fenes/microservices_app.git
cd microservices_app
```

### 2. Build and Run the Docker Containers

```bash
docker-compose build
docker-compose up
```

### 3. Apply Migrations

Before running the application, you need to apply the database migrations. Run the following commands in the respective service directories:

```bash
# For Article Service
cd ArticleService
dotnet ef migrations add InitialCreate
dotnet ef database update

# For Review Service
cd ReviewService
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Access the Services

- **API Gateway**: http://localhost:5000
- **Article Service**: http://localhost:5001
- **Review Service**: http://localhost:5002

## Endpoints

### Article Service

- `POST /articles`: Create a new article.
- `PUT /articles/{articleId}`: Update an existing article.
- `DELETE /articles/{articleId}`: Delete an article.
- `GET /articles/{articleId}`: Get a specific article.
- `GET /articles`: Get a list of articles.
- `GET /articles?title=ExampleTitle&starCount=5`: Get articles with specific query parameters.

### Review Service

- `POST /reviews`: Create a new review.
- `PUT /reviews/{reviewId}`: Update an existing review.
- `DELETE /reviews/{reviewId}`: Delete a review.
- `GET /reviews/{reviewId}`: Get a specific review.
- `GET /reviews`: Get a list of reviews.
- `GET /reviews?reviewer=Doe&reviewCount=7&articleId=100`: Get reviews with specific query parameters.

## Testing

Unit tests are included for both services.  To run the tests:

```bash
docker-compose.test.yml build 
docker-compose.test.yml up
```

## Caching

Redis is used to cache frequently read data in both services to improve performance. The caching implementation follows these principles:
- Centralized Redis management through a shared Redis service
- Consistent caching patterns across services
- Configurable cache duration and invalidation strategies
- Proper error handling and fallback mechanisms

The caching layer is implemented using the following pattern:
```csharp
public interface ICacheService<T>
{
    Task<T> GetOrSetAsync(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
}
```

## Contributing

To contribute to this project:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/your-feature`).
3. Commit your changes (`git commit -am 'Add your feature'`).
4. Push to the branch (`git push origin feature/your-feature`).
5. Create a new Pull Request.
