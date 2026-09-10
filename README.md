# FlightBooking ✈️

A flight booking and travel management application built with **ASP.NET Core MVC, MongoDB, OpenAI and ML.NET**.

## 🚀 Key Features

* **Flight & Booking Management** — Flight search, passenger management, booking creation, price calculation and unique PNR generation.
* **AI Travel Agent** — Natural-language travel assistance with **OpenAI**, intent detection, city extraction, weather information and Google Places integration.
* **Machine Learning** — **ML.NET** models for flight occupancy and passenger demand prediction.
* **No-show & Overbooking Analysis** — Historical data analysis and overbooking recommendations based on no-show rates.
* **Admin Panel** — Dedicated MVC Area for managing flights, bookings, check-ins and analytics.

## 🛠️ Tech Stack

**Backend:** C#, ASP.NET Core MVC, .NET 6
**Database:** MongoDB
**AI:** OpenAI API, Google Places API
**Machine Learning:** ML.NET, FastTree
**Other:** AutoMapper, FluentValidation, Dependency Injection, Razor Views

## 🏗️ Architecture

The application follows a service-based **ASP.NET Core MVC architecture** with **DTOs, ViewModels, Dependency Injection, AutoMapper and MVC Areas**.

### AI Flow

```text
User Request → Intent Detection → AI / External Tools → Response
```

### ML Flow

```text
Historical Flight Data → Feature Transformation → ML.NET → Prediction
```

## 📌 Project Highlights

This project combines **backend development, database management, external API integration, AI and machine learning** within a single ASP.NET Core application.
