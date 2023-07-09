# E-commerce RESTful Web API Backend

This repository contains the backend implementation of an e-commerce application, providing a RESTful Web API for managing various aspects of an online store. The project follows the model-view-controller (MVC) architectural design pattern, with a strong emphasis on decoupling the business logic from the data and user interface.

## Project Structure
The project's file structure adheres to the MVC pattern, consisting of controllers and models. The view component is implemented separately, utilizing different technologies and languages. The backend project focuses on implementing the business logic and exposing it to other applications through web APIs. This design choice allows for easy reuse of the backend in different applications without requiring significant modifications.

## Design Choices

### MVC Pattern
The model-view-controller (MVC) architectural pattern is employed to structure the backend project. This pattern separates the application into three main components:

1. **Model**: Represents the application's data and business logic. It encapsulates data structures, business rules, and operations related to the application's functionality. The model component is responsible for data access, manipulation, and validation.

2. **View**: Presents the user interface to the application's users. It focuses on displaying data and receiving user input. In this project, the view component is implemented separately, while still being able to access the model's data and business logic through web APIs.

3. **Controller**: Acts as an intermediary between the model and the view. It handles user interactions, such as receiving requests from the view, processing them, and updating the model or view accordingly. The controller coordinates the flow of data between the model and the view, ensuring their decoupling.

### Repository Pattern
The repository pattern is utilized to create an abstraction layer between the application's data access logic and the underlying data storage. This pattern promotes modularity, maintainability, and testability in the application architecture.

The repository pattern defines an interface that represents the contract for data operations, such as CRUD (Create, Read, Update, Delete) operations, filtering, sorting, and querying. It enables the application to work with data through a standardized interface, irrespective of the actual data storage implementation. This flexibility allows the application to adapt and switch between different data storage technologies without impacting the rest of the application.

By assigning the responsibility of data access to dedicated repository classes, the repository pattern adheres to the Single Responsibility Principle. Each repository is responsible for handling operations related to a specific entity or aggregate root in the domain model.

The repository pattern simplifies testing by allowing the application to use mock or in-memory implementations of the repositories during unit testing. This facilitates isolated testing of business logic without the need for actual database connectivity.

### Controllers and Web APIs
In this project, the term "controller" refers to the component responsible for handling API endpoints, as per the ASP.NET Core framework convention. Controllers receive incoming API requests, process them, and generate appropriate API responses. They execute necessary operations and return data or responses in a format suitable for the API consumer.

The API endpoints are defined using attribute-based routing and HTTP method attributes (HttpGet, HttpPost, HttpPut, HttpDelete). This convention-based approach allows developers to associate specific API endpoints with controller actions, specifying the supported HTTP methods. The controller actions can return various data types, such as objects, JSON, or XML, enabling flexibility in the format of API responses. ASP.NET Core also supports content negotiation, automatically serializing response data into the requested format based on client preferences and available formats.

### API Features

The e-commerce API backend implements several features to enhance the flexibility and functionality of the application:

#### Record-based filtering
Clients can filter records from the database based on specific criteria. The available filtering options include:

- Range of prices: Retrieve all records within a specific price range.
- Brand filtering: Filter products based on the brand they belong to.
- Category filtering: Filter products that fall under one or more specified categories.
- Vote rating: Retrieve products with a vote higher than a certain value.
- Date filtering: Retrieve products that have been added after a specified date.

#### Field-based filtering
Inspired by the GraphQL data query language, the API allows clients to specify the exact fields they want to retrieve in a single API request. This feature helps avoid over-fetching or under-fetching of data. Clients can use the "OnlySelectFields" parameter to specify a comma-separated list of fields they want to include in the API response. By default, all fields are returned, but with this feature, clients have granular control over the data they receive. This approach optimizes network usage, improves performance, and reduces the amount of data retrieved in the database queries. Additionally, clients can use the "FieldsToExclude" parameter to specify only the fields they do not want to retrieve, which can be useful for entities with many fields.

#### Pagination
The API supports pagination to efficiently handle large datasets. Pagination breaks down a large set of data into smaller, manageable chunks or pages. It enables the retrieval and presentation of data in a user-friendly manner, improving performance and user experience. Users can navigate through different pages or subsets of data using page navigation links or buttons. Pagination reduces the amount of data transferred and the processing time by fetching only the data corresponding to the requested page from the database or data source.

#### Ordering
The API allows clients to specify the order in which query results are returned. It supports the following features:

- Comma-separated fields: Clients can provide a list of fields separated by commas to indicate the order of sorting. Each field represents a criterion by which the results will be sorted.
- Ascending or Descending order: By default, the results are sorted in ascending order based on the specified fields. To sort the results in descending order, clients can prefix the field name with a hyphen (-). This indicates that the values should be arranged in decreasing order for that particular field. For example:
  - "-vote,price": Orders the products by vote from highest to lowest and then by price from lowest to highest.

### API Security

Ensuring API security is crucial for protecting sensitive data and preventing unauthorized access. The e-commerce API backend addresses security considerations by implementing JSON Web Token (JWT) for authentication and authorization.

#### Basic Authentication
Initially, the basic authentication approach was used, where the user's credentials (username and password) were transmitted through the Authorization Header of the HTTP request. The server would then verify the credentials, and if valid, process the request. This approach, however, has some potential drawbacks, including lack of encryption, lack of support for revocation, lack of fine-grained access control, and processing overhead.

#### JSON Web Token (JWT)
To overcome the drawbacks of basic authentication, the e-commerce API backend utilizes JSON Web Tokens (JWT) for both authentication and authorization. JWT is an open standard that securely transmits information between parties as a JSON object. The token contains a header, payload, and signature.

- Header: Specifies the type of the token (JWT) and the signing algorithm used. The header is Base64Url encoded.
- Payload: Contains custom claims that share information between parties. The payload is Base64Url encoded. In the e-commerce API, the "sub" claim represents the userId.
- Signature: Verifies the message integrity and authenticity. It ensures that the token hasn't been tampered with.

The JWT token generation process involves setting claims, defining a symmetric key, and generating the token.

JWT tokens offer advantages over basic authentication, including encryption of credentials, improved performance by reducing the need for credential verification with each request, support for revocation through token expiration, and the ability to enforce fine-grained access control policies.

### Other Project Related Components

The project implements several other components to enhance and facilitate the developing process.

### Migrations
Migrations refer to the controlled and organized process of modifying a database schema. It involves adding or modifying database tables, columns, data types, and related operations. Migrations are version-controlled artifacts that track changes to the database schema over time. Organizing migrations in a sequential order simplifies managing and tracking schema changes.

Migrations support both upward and downward changes to the database schema. Upward changes are applied when moving to a newer version, adding or modifying the schema. Downward changes are applied when rolling back or reverting to a previous version, undoing the modifications made by a specific migration.

By providing a standardized way to apply and track schema changes, migrations maintain consistency among developers working on the same project. They facilitate collaboration and allow multiple developers to work on the database schema simultaneously, ensuring everyone is working with the same version and applying changes in a controlled manner.

### Logging
Logging is an essential practice in software development that involves recording and storing information about the execution of an application or system. It helps in troubleshooting, monitoring, and analyzing the software by capturing important events, errors, warnings, and informational messages that occur during runtime.

Logs are valuable when investigating issues or bugs in an application. Reviewing log files provides insights into the sequence of events leading up to an issue, the context in which it occurred, and any relevant data or error messages. This helps in effectively isolating and resolving problems.

### Swagger Documentation
Swagger (OpenAPI Specification) is an open standard for describing and documenting RESTful APIs. It provides a language-agnostic way to define the structure, endpoints, input/output parameters, authentication requirements, and other details of an API. The documentation is not tied to any specific programming language or technology stack, making it universally applicable.

By utilizing Swagger, developers can design, build, and consume APIs with ease. It provides a representation of the API contract, allowing for better understanding and integration with the API.

## Conclusion

By employing the MVC pattern, repository pattern, and implementing various API features and security measures, the e-commerce RESTful Web API backend provides a robust and flexible foundation for managing an online store. The project structure and design choices prioritize modularity, maintainability, and testability, allowing for easy integration with other applications and scalability. The API features, such as record-based and field-based filtering, pagination, and ordering, enhance the usability and efficiency of the application. Additionally, the implementation of JSON Web Tokens (JWT) for authentication and authorization improves API security, ensuring that only authenticated and authorized users can access protected resources.
