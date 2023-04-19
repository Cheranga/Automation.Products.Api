# Products API

## Context
To provide a Products API to use for automation.

## Using the API in your local environment
* Install Docker Desktop.
* Install Azure Storage Explorer.
* Open a command prompt and run the below command to download and run `Azurite` as a container in detached mode.

```dockerfile
docker run -d -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
```
* Open `Azure Storage Explorer` and create below. This table and queues are needed for the API to function.
  * `registrations` table.
  * `registrations` queue.
  * `update-registrations` queue.

## Features

### Register product
- [x] Validate request.
- [x] Handling failures.
- [x] Publishing domain event to Azure storage queue.
- [x] Save data in Azure table storage.

### Change location of a product
- [x] Validate request.
- [x] Handling failures.
- [x] Publishing domain event to Azure storage queue.
- [x] Save data in Azure table storage.

### Get product by id
- [x] Validate request.
- [x] Handling failures.

### Get all products
- [ ] Returns all the products.

### Authentication and Authorization
- [ ] Integrating with Azure AD for authentication and authorization.

### Logging
- [x] Using Serilog for structured logging.

### Versioning
- [ ] Support for default and specific versioning.

### CI/CD
- [ ] Using GHA for CI/CD pipelines.
- [ ] Using Azure Bicep for cloud resources deployment.

### API Documentation
- [x] Including comprehensive API documentation using Swagger.

### Automation tests
- [ ] Creating an integration test suite.

---

## References

* [Minimal API Tutorial](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-7.0&tabs=visual-studio)


