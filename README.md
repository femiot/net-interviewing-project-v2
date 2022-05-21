[![Coolblue](https://github.com/femiot/net-interviewing-project-v2/blob/master/logo.png)](https://www.coolblue.nl/en)

# Insurance API .Net 6.0 C#
## _Solution Documentation_
![main workflow](https://github.com/femiot/net-interviewing-project-v2/actions/workflows/dotnet.yml/badge.svg)

Table of contents
=================

<!--ts-->
   * [1. Technical Details on the API](#1-technical-details-on-the-api)
      * [1.1 Solution Architecture](#11-solution-architecture)
      * [1.2 Design Patterns](#12-design-patterns)
      * [1.3 Monitoring](#13--monitoring)
      * [1.4 Exception Handling](#14-exception-handling)
      * [1.5 Data Access](#15-data-access)
      * [1.6 Testing](#16-testing)
      * [1.7 CI/CD](#17-cicd)
<!--te-->

Insurance API is a service for calculating insurance costs on single product and multiple products. There are set rules based on the product, product type and if the product can be insured. The rules are stored in a sqlite database to avoid hardcoded values in the solution:

- Cost range rule will check if the product sales cost is within a certain range and add a cost to the insurance 
- Extra insurance cost are allocated specific products identified by the business. For example digital cameras are getting lost more than usual and an extra cost of 500 Euros should be added to the insurance cost
- Surcharge cost are also allocated to specific products based on their product type. This cost will be added to a product if a surcharge is captured against it's product type.

Let's dive into the code and get a sense of the structure and the components built.

## 1. Technical Details on the API

Assumptions:
- This API will be hosted behind firewall and not on the internet
- Database utilized is Sqlite to ensure the database can run as part of the application
- No security such as OAuth is required if it is hosted behind a firewall
- Product Data Api is healthy and active


##  1.1 Solution Architecture:
The solution consists of multiple layers to ensure seperation of concern, high cohesion and low coupling. The clean architecture is adopted to ensure that code can be tested properly, easily scaled and maintained.

[![Clean Architecture](https://github.com/femiot/net-interviewing-project-v2/blob/master/clean_architecture.drawio.svg)]()

- Interface Layer contains the API that exposes the endpoints that can be consumed. This application is the entry point into the system. It consists of Startups, Controllers, Middlewares (logger, exception, correlation handlers), Filters (action and expection handlers). 
- Domain Layer contains the core project where the business rules are implemented. This layer only depends on the shared layer and nothing else. This is to ensure that other layers can be modified without any negative effects on the domain. This layer rely on abstraction and the use of interface with other layers.
- Persistence layer contains the infrastructure project which consists of the database context, migration scripts, repositories and the unit of work. This project is mainly focused on data access management within the solution.
- Integration layer helps to ensure that integration with external services for example Product Data Api is handled in abstraction without any coupling to the domain layer. In a scenario where the Product Api is about to be upgraded, the only major impact will be on Integration layer. 
- Shared Layer contains components such as helper methods, constants, entities, models, dto, app configuration. The main focus of this project is to ensure common components are centralized.

## 1.2 Design Patterns
The design patterns adopted are Service Facade and Cache aside.

1. Service Facade pattern helps to abstract complexities from the client. In the solution, the controllers only rely on a single service which under the hood communicates multiple services. It improves code readability and usability.
2. Cache aside pattern is adopted to ensure that large data such as getting all products from Product Data Api is cached and reused to improve the performance of the application. The idea is to invalidate the cache after a duration of time and fetch new data as they emerge. 

## 1.3  Monitoring
The monitoring implemented in the solution utilizes serilog. The key factor of this implementation is that it uses a correlation Id to chain the logs together. This correlation Id can be passed into the API via the HTTP Request Header by specifying the header key as:
```sh
"Coolblue-Correlation-ID"
```
and the logger middleware will check the header and see if any exists. If no correlation Id is found, it would generate a new one that can be used in the solution and apply it to the header request and response. Name of the middleware classes below:
```sh
CorrelationIdMiddleware.cs
```
The logger middleware is responsible for ensuring that the logs are scope based on the correlation Id and this will ensure that logs can be traced easily through the use of the correlation Id. Code snippet below:
```cs
LoggerMiddleware.cs
var header = context.Request.Headers[appConfigurationOptions.Value.CorrelationKey];
if (header.Count > 0)
{
    using (logger.BeginScope("{@CorrelationId}", header[0]))
    {
        await _next(context);
    }
}
```


Another thing to bear in mind is that the correlation Id is sent to external APIs in order to be ensure it can be traced should there be any issues during processing a request. Here is an example of the log output below with correlation Id `fc1ca750-32a7-4e85-a024-967fe6e3a360`
```sh
[2022-05-20 09:09:39.377 +02:00 fc1ca750-32a7-4e85-a024-967fe6e3a360 INF] Executing endpoint 'Insurance.Api.Controllers.SurchargeController.UploadSurchargeRates (Insurance.Api)'

[2022-05-20 09:09:39.413 +02:00 fc1ca750-32a7-4e85-a024-967fe6e3a360 INF] Route matched with {action = "UploadSurchargeRates", controller = "Surcharge"}. Executing controller action with signature System.Threading.Tasks.Task`1[Microsoft.AspNetCore.Mvc.IActionResult] UploadSurchargeRates(Insurance.Shared.Payload.Requests.SurchargeUploadRequest) on controller Insurance.Api.Controllers.SurchargeController (Insurance.Api).
```
Log directory path can be found: `./src/logs`. In a scenario where this will be hosted on a cloud platform for example `AWS`, the app settings will be configured to use cloudwatch and all the logs will be provided on the cloud platform instead of the directory specified above.

In terms of the health checks, there is an endpoint configured to check if the API is running. This can be configured on a load balancer or kubernetes cluster to ensure that the health of the api is monitored at all times. 

## 1.4 Exception Handling
There are two exception handlers in the solution namely:
1. `ExceptionFilter.cs`
2. `ExceptionHandlerMiddleware.cs`

The exception filter is mainly to handle any exceptions that may occur based on the code implemented. The filter checks the environment and sends the appropriate error message. The exception handler middleware is more of ensuring that any unhandled exception that may occur in the framework can be handled without the API crashing. 

When in development mode. All exceptions are returned but in a production environment, only friendly message will be shown to the end user and the real error will be logged for tracing purpose.

## 1.5 Data Access
The application use Sqlite but ideally should be connecting to a more optimal SQL server such MSSQL Server. All cost such as range cost, extra costs, surcharge are stored in the sqlite database. The database file can be found with the API project `.\src\Insurance.Api` name of the db file should be `InsuranceDatabase.db`.

The application uses a code first approach. There are migration scripts that are configured on first load to seed the database. In a scenario where any of the entity models is changed, the migration script should be executed within the solution to create new migration that would update the database on the new run. Here is the command to invoke at the root of the solution:
```sh
dotnet ef migrations add <TitleOfchange> --project src/Insurance.Infrastructure --startup-project src/Insurance.Api --context InsuranceContext -o 'MigrationData'
```
Please note that the necessary tools will have to be installed on your machine for the migration script above to work. Use the link below as a guide to setup your machine:
[Migrations Overview](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)

`<TitleOfchange>` must be replaced with a title for modify the entity. This will be stored in the database after it has been executed to prevent it from running again. Section below of where the database migration are automatically triggered:
```cs
Startup.cs
private static void UpdateDatabases(IApplicationBuilder app)
{
    using (var serviceScope = app.ApplicationServices
        .GetRequiredService<IServiceScopeFactory>()
        .CreateScope())
    {
        using (var context = serviceScope.ServiceProvider.GetService<InsuranceContext>())
        {
            context.Database.Migrate();
        }
    }
}
```
## 1.6 Testing
The solution consists of both Integration and Unit tests. The integration tests are mainly geared towards the behaviour of the system based on input provided. 
Unit tests are used to compliment the integration tests in cases where there are no code coverage.

## 1.7 CI/CD
There is a build pipeline to check the code coverage and ensure that it is at least 75%. An example can be found here: https://github.com/femiot/net-interviewing-project-v2/runs/6536982844?check_suite_focus=true

Please ensure to always update the document as the system evolves. 
✨Happy coding ✨



