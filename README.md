# ServerApp-msapps

A .NET application that handles user management with a SQL database backend.

## Description

ServerApp-msapps is a server application built with .NET that provides user management functionality. It manages user data in a SQL database and offers endpoints for authentication, user creation, retrieval, and management.

## Features

- User authentication and authorization
- User registration and account management
- SQL database integration for persistent storage
- RESTful API endpoints for user operations

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)
- SQL Server (or compatible database)
- Visual Studio 2019/2022 or Visual Studio Code

## Installation

1. Clone the repository:
   ```
   git clone https://github.com/nirReich/ServerApp-msapps.git
   ```

2. Navigate to the project directory:
   ```
   cd ServerApp-msapps
   ```

3. Restore NuGet packages:
   ```
   dotnet restore
   ```

4. Update the database connection string in `appsettings.json` with your SQL Server details:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YourServerName;Database=YourDatabaseName;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

5. Apply database migrations:
   ```
   dotnet ef database update
   ```

## Running the Application

To run the application locally:

```
dotnet run
```

The application will start and be available at `https://localhost:5001` and `http://localhost:5000`.

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration

### User Management
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

## Configuration

The application can be configured through the `appsettings.json` file. Key configuration points include:

- Database connection string
- JWT authentication settings
- Logging preferences

## Project Structure

- `Controllers/` - API controllers defining endpoints
- `Models/` - Data models and DTOs
- `Services/` - Business logic implementation
- `Data/` - Database context and repository implementations

## Technologies

- .NET 6.0+
- Entity Framework Core
- SQL Server
- JWT Authentication

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact

Nir Reich - [GitHub Profile](https://github.com/nirReich)

Project Link: [https://github.com/nirReich/ServerApp-msapps](https://github.com/nirReich/ServerApp-msapps)
