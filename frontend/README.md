# Earnings Advance Platform - Frontend

## 🚧 Under Construction 🚧

This is the frontend application for the Earnings Advance Platform, designed to provide a user-friendly interface for content creators to manage their advance requests.

## 🎯 Planned Features

### Creator Dashboard
- View available amount for advance
- Simulation calculator with real-time fee calculation
- List of past requests with status tracking
- Create new advance requests

### Request Management
- Request status tracking
- Detailed view of each request
- Review of fees and net amounts
- Request history with pagination

### Simulation Tools
- Interactive calculator
- Fee preview
- Net amount calculation
- Minimum amount validation

## 🔧 Technical Stack

- **Core:**
  - React 18
  - TypeScript
  - Vite

- **UI/UX:**
  - Material UI
  - React Hook Form
  - Zod (validation)
  - Responsive design

- **State Management:**
  - React Query
  - Context API

- **Development:**
  - ESLint
  - Prettier
  - Husky
  - Jest
  - Testing Library

## 🏗️ Planned Structure

```
earnings-advance-platform/
├── frontend/ 
│   ├── src/ 
│   │   ├── api/              # API integration 
│   │   ├── components/       # Reusable UI components 
│   │   ├── contexts/         # React contexts 
│   │   ├── hooks/            # Custom hooks 
│   │   ├── pages/            # Main application pages 
│   │   ├── types/            # TypeScript definitions 
│   │   └── utils/            # Helper functions 
│   ├── tests/                # Test files 
│   └── README.md             # This file
└── ...
```


## 📋 Development Roadmap

### Phase 1: Foundation
- [ ] Project setup with Vite
- [ ] TypeScript configuration
- [ ] Material UI integration
- [ ] API client setup
- [ ] Basic routing

### Phase 2: Core Features
- [ ] Authentication flow
- [ ] Dashboard layout
- [ ] Request creation form
- [ ] Request listing with pagination
- [ ] Simulation calculator

### Phase 3: Enhanced Features
- [ ] Real-time validation
- [ ] Status tracking
- [ ] Advanced filtering
- [ ] Responsive design
- [ ] Error handling

### Phase 4: Testing & Polish
- [ ] Unit testing setup
- [ ] Integration testing
- [ ] E2E testing
- [ ] Performance optimization
- [ ] Accessibility improvements

## ⚡ Getting Started

> Note: These instructions will be updated as development progresses.

### Prerequisites
- Node.js 18+
- npm or yarn
- Backend API running

### Planned Setup Steps

1. Clone the repository:

	```bash
	git clone [repo-url]
	cd earnings-advance-platform
	```
	
2. Navigate to frontend directory:

	```bash
	cd frontend
	```

3. Install dependencies:

   ```bash
   npm install
   ```

4. Start development server:

   ```bash
   npm run dev
   ```

---
## 🔗 API Integration

The frontend will integrate with the following API endpoints:

- `POST /api/v1/advance` - Create advance request
- `GET /api/v1/advance/creator/{creatorId}` - List requests
- `PATCH /api/v1/advance/{id}/approve` - Approve request
- `PATCH /api/v1/advance/{id}/reject` - Reject request
- `GET /api/v1/advance/simulate` - Simulate advance

## 🧪 Testing Strategy

Planning to implement:
- Unit tests for components
- Integration tests for API calls
- E2E tests for critical flows
- Accessibility testing

## 📚 Documentation

Will include:
- Component documentation
- API integration details
- State management patterns
- Testing guidelines

## 🤝 Contributing

Guidelines for contribution will be added as the project develops.

## 📝 License

This project is licensed under the MIT License.
