# Earnings Advance Platform

## 📋 Overview

Earnings Advance Platform is a full-stack web application for managing earnings advance requests for content creators, featuring a .NET 9 backend and a React/TypeScript frontend.


## 🏛️ Project Structure

The project is divided into two main parts:

```
earnings-advance-platform/
├── backend/                                           # .NET 9 API with Clean Architecture
│   ├── src/
│   │   ├── Earnings.Advance.Platform.Application      # Application logic (commands, handlers)
│   │   ├── Earnings.Advance.Platform.Domain           # Domain entities and business rules
│   │   └── Earnings.Advance.Platform.Infrastructure   # Infrastructure implementations
│   │   ├── Earnings.Advance.Platform.WebApi           # API controllers and configurations
│   └── tests/ 
│   │   └── Earnings.Advance.Platform.IntegrationTests # Integration tests for the API
│   │   ├── Earnings.Advance.Platform.UnitTest         # Unit tests for application logic
│
├── frontend/                                          # React application with TypeScript (IN CONSTRUCTION)
│   └── ...
│
├── README.md                                          # This file
└── ...
```

## 🚀 Technologies Used

### Backend:
- .NET 9
- Entity Framework Core (In-Memory Database)
- Swagger/OpenAPI
- XUnit & Moq
- Clean Architecture

### Prerequisites
- .NET 9 SDK
- Visual Studio 2022 or VS Code

## 🛠️ Manual Installation

If you prefer running without Docker, check the specific READMEs:
- [Backend README](backend/README.md)

## 📱 Key Features

- ✅ Create advance request  
- ✅ List requests by creator (with pagination)  
- ✅ Approve/Reject requests  
- ✅ Simulate advance  
- ✅ Business rule validations  
- ✅ Automated testing

## 🔄 Architecture

- **Backend**: Clean Architecture for clear separation of concerns
- **Frontend**: Component-based architecture with contextual state management
- **Communication**: RESTful API
- **Persistence**: In-Memory database managed via Entity Framework Core

## 📊 API Endpoints

- **POST** `/api/v1/advance` - Create a new advance request
- **GET** `/api/v1/advance/creator/{creatorId}?pageNumber=1&pageSize=10` - List requests by creator with pagination
- **PATCH** `/api/v1/advance/{id}/approve` - Approve an advance request
- **PATCH** `/api/v1/advance/{id}/reject` - Reject an advance request
- **GET** `/api/v1/advance/simulate?amount=1000` - Simulate an advance request

For detailed API documentation, visit `/swagger`.

## 🧪 Testing

The project includes:

- Unit Tests:
  - Services and Domain
  - DTOs and Validations
  - Constants and Formatting

### Run all backend tests 

```bash
cd backend
dotnet test
```

### With coverage report
```bash
cd backend
dotnet test --collect:"XPlat Code Coverage"
```


## 📜 License

This project is licensed under the MIT License.

## 👥 Contribution

Contributions are welcome! Please open an issue or pull request.

## 📞 Contact

- **Developer**: Leandro Veiga
- **LinkedIn**: [Leandro's Profile](https://www.linkedin.com/in/leandro-camargo-da-veiga/)
- **GitHub**: veiga-leandro

Developed with ❤️