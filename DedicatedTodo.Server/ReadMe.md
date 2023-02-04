# Simple Todo App Backend

## Stack
- .NET 7
- ASP.NET Core MVC: Currently, Swashbuckle, the OpenAPI document generator package for .NET, didn't support the major F# frameworks, such as Giraffe, Saturn, Falco.
- Dapper: Lightweight ADO.NET object mapper
- PostgreSQL: DB
- Polly: .NET resilience library

## Project Structure
- DbMapping: types for Mapping query result or parameter object.
- TodoDb: data access layer
- Domain: handle todo operations
- DTO: definitions, mapping
- Controllers: API endpoints and integrate with DI container
- Program: configuration part

## Considerations
### Use Dapper
Entity Framework Core, the ORM for .NET Core, was purely object orientated and worked awkwardly with F#.
Also, it's much less performant than Java based ORMs like Hibernate.
Dapper used the basic ADO.NET with some convenient object mapping features, so iss performance penalty was minimized.

Dapper didn't support positional parameter, so named parameter must be used.

Many articles about adapting Dapper in F# used dict<string,object> for parameter, After F# 4.6, anonymous record just work well.

### Use PostgreSQL
Cross-platform, free to use, and related packages were more abundant than other providers.

## Features
- Query / Query with Conditions / Create / Update / Delete
- OpenAPI document

## Todos
- Exception handling
- Filter
- OrderBy
- Unit Tests

## Further Optimization
- Paged
- Rate limit
- Health Check, Metrics
- Example in Open API document ([SwaggerRequestExample], [SwaggerResponseExample])
- Split Error into Normal and DB
- Set Backend url by environment
- Run Table creation script using Background service or Host OnStarted
- Authentication / Authorization

