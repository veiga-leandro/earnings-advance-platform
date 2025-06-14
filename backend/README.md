# Earnings Advance Platform API

A REST API for managing earnings advance requests for content creators.

---

## 🚀 Features

✅ Create advance request  
✅ List requests by creator  
✅ Approve/Reject requests  
✅ Simulate advance  
✅ Business rule validations  
✅ Automated testing

---

## 🏗️ Architecture

The project follows Clean Architecture principles:

- **Domain**: Entities, enums, interfaces, and business rules  
- **Application**: DTOs, services, and use cases  
- **Infrastructure**: Repositories and data access  
- **API**: Controllers and web configuration

---

## 🔧 Technologies

- .NET 8.0  
- Entity Framework Core (In-Memory)  
- Swagger/OpenAPI  
- XUnit (Testing)  
- Moq (Mocks)

---

## ⚡ How to Run

### Prerequisites

- .NET 9 SDK  
- Visual Studio or VS Code

### Steps

1. Clone the repository:

	```bash
	git clone [repo-url]
	cd Earnings.Advance.Platform
	```

2. Restore dependencies:

   ```bash
   dotnet restore
   ```

3. Run the application:

   ```bash
   dotnet run --project src/Earnings.Advance.Platform.WebApi
   ```

4. Access Swagger at:
   `https://localhost:7071`

---

## ✅ Run Tests

```bash
dotnet test
```

---

## 📚 API Endpoints

### Create an advance request

**POST** `/api/v1/advance`

#### Request Body:

```json
{
  "creatorId": "creator123",
  "requestedAmount": 1000.00
}
```

#### Response (201):

```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "creatorId": "creator123",
  "requestedAmount": 1000.00,
  "fee": 50.00,
  "netAmount": 950.00,
  "status": "pending",
  "requestDate": "2025-06-14T18:25:00Z",
  "processedDate": null
}
```

---

### List requests by creator

**GET** `/api/v1/advance/creator/{creatorId}`

---

### Approve a request

**PATCH** `/api/v1/advance/{id}/approve`

---

### Reject a request

**PATCH** `/api/v1/advance/{id}/reject`

---

### Simulate an advance

**GET** `/api/v1/advance/simulate?amount=1000`

---

## 🎯 Business Rules

✅ Minimum amount: R\$ 100.00 <br/>
✅ Fixed fee: 5% <br/>
✅ Only one pending request per creator <br/>
✅ Initial status: `"pending"`

---

## 🧪 Tests

The project includes:

* Unit Tests: Services and Domain
* Mocks: For dependency isolation
* Coverage: Core business scenarios

```bash
# Run all tests
dotnet test

# With coverage report
dotnet test --collect:"XPlat Code Coverage"
```
