# coffee.co - Coffee Shop Review Site

A full-stack web application built with .NET for discovering and managing your favorite local coffee shops.

## Features

- View all coffee shops sorted by rating (highest first)
- Add new coffee shops
- Favorite/unfavorite shops
- Soft delete shops
- Persistent data storage in MySQL database

## Tech Stack

- **Frontend**: Vanilla HTML, CSS, JavaScript with Bootstrap 5.3.x
- **Backend**: .NET 8.0 Web API
- **Database**: MySQL
- **ORM**: Entity Framework Core with Pomelo MySQL provider

## Prerequisites

- .NET 8.0 SDK or later
- MySQL Server (v5.7 or higher) or MySQL-compatible database
- Your database credentials

## Setup Instructions

### 1. Configure Environment Variables

Copy `env.template` to `.env` and fill in your MySQL database credentials:

```
DB_HOST=your_host
DB_USER=your_username
DB_PASSWORD=your_password
DB_NAME=your_database_name
DB_PORT=3306
PORT=3000
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Set Up Database

Make sure your MySQL database exists and run the SQL from `dummy_data.sql` to create the table and insert sample data, or the application will create the table automatically on first run.

### 4. Run the Application

```bash
dotnet run
```

The application will start on `http://localhost:3000` (or the port specified in your `.env` file).

## Database Schema

The `Shop` table has the following structure:
- `ShopID` (INT, Primary Key, Auto Increment)
- `ShopName` (VARCHAR(255), Required)
- `Rating` (DECIMAL(3,2), Default: 0.00)
- `DateEntered` (DATETIME, Default: CURRENT_TIMESTAMP)
- `Favorited` (BOOLEAN, Default: FALSE)
- `Deleted` (BOOLEAN, Default: FALSE) - Used for soft deletes

## API Endpoints

- `GET /api/shops` - Get all shops (sorted by rating descending)
- `GET /api/shops/{id}` - Get a single shop by ID
- `POST /api/shops` - Create a new shop
- `PATCH /api/shops/{id}/favorite` - Toggle favorite status
- `DELETE /api/shops/{id}` - Soft delete a shop

## Project Structure

```
.
├── Controllers/
│   └── ShopsController.cs    # API endpoints
├── Data/
│   └── CoffeeDbContext.cs     # Entity Framework DbContext
├── Models/
│   └── Shop.cs                # Shop entity model
├── public/
│   ├── index.html             # Main frontend page
│   ├── scripts/
│   │   ├── api.js             # API interaction functions
│   │   └── index.js           # Main frontend logic
│   └── styles/
│       └── main.css           # Custom styles
├── CoffeeCo.csproj            # .NET project file
├── Program.cs                  # Application entry point
├── env.template                # Environment variables template
└── README.md                   # This file
```

## Notes

- The application uses soft deletes, so deleted shops are marked with `Deleted = TRUE` but not removed from the database
- Shops are automatically sorted by rating in descending order
- The database table will be created automatically if it doesn't exist when you first run the application
