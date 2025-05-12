# SEO Tracker
A modern web application for tracking SEO rankings across multiple search engines.

## Overview
This project consists of two main components:

- A React TypeScript frontend with a clean, responsive UI
- A .NET Core backend API built with Clean Architecture principles
The application allows users to:

- Enter search keywords and a target URL
- Select between Google and Bing search engines
- View the ranking positions of the target URL in search results
- Get cached results for improved performance
## Frontend
The frontend is built with React and TypeScript, featuring:

- Modern UI : Clean, responsive design using Tailwind CSS
- Component-Based Architecture : Modular components for maintainability
- TypeScript : Type-safe code for better developer experience
- Search Engine Selection : Toggle between Google and Bing
### Key Components
- SearchBar : Input form for keywords and URL
- BrowserSelect : Dropdown to select search engine
- RankingResult : Display component for search results
- MainLayout : Consistent layout wrapper
### Getting Started with Frontend

yarn install
yarn start

The frontend will be available at http://localhost:3000

## Backend
The backend is a .NET Core API built with Clean Architecture principles:

- Clean Architecture : Separation of concerns with distinct layers
- SOLID Principles : Well-designed, maintainable code
- Dependency Injection : Loose coupling between components
- Caching : One-hour caching of search results for performance
- Multiple Search Providers : Extensible design for adding new search engines
### Architecture Layers
- Core : Contains interfaces, entities, and business rules
- Application : Contains business logic and use cases
- Infrastructure : Implements interfaces from Core
- API : Handles HTTP requests and responses
### Design Patterns
- Factory Pattern : For creating appropriate search service implementations
- Repository Pattern : For data access abstraction
- Mediator Pattern : For handling requests and commands
- CQRS : Separation of read and write operations
### Getting Started with Backend
cd backend

dotnet restore

dotnet build

dotnet run --project SympliSeoTracker.API

The API will be available at http://localhost:5184

## API Endpoints
- GET /api/Search : Search for keyword rankings
  - Parameters:
    - keywords : Search terms
    - url : Target URL to find in results
    - provider : Search engine (0 for Google, 1 for Bing)
## Development
### Prerequisites
- Node.js and yarn for frontend
- .NET 8 SDK for backend
- Visual Studio 2022 or VS Code
### Project Structure


├── frontend/                # React TypeScript

frontend

│   ├── src/

│   │   ├── components/      # UI components

│   │   ├── services/        # API services

│   │   └── types/           # TypeScript interfaces

│   └── package.json

│

└── backend/                 # .NET Core backend

├── SympliSeoTracker.API/            # API controllers

├── SympliSeoTracker.Application/    # Business logic

├── SympliSeoTracker.Core/           # Entities and interfaces

├── SympliSeoTracker.Infrastructure/ # External services

└── SympliSeoTracker.Tests/          # Unit tests


## Future Enhancements
- User authentication and history tracking
- Additional search engines
- Detailed analytics and reporting
- Scheduled monitoring of rankings