# WebScraper
FIPIran Data Scraper

A robust C# application for scraping and storing financial data from the FIPIran website (Investment Funds of Iran). This project automates the extraction of detailed fund-related information, including fund types, performance metrics, investment details, and transaction data, and persists them in a structured database using Entity Framework Core.

Features
Web Scraping via API:
Retrieves data from FIPIran's API endpoints (e.g., /api/v1/fund/fundcompare, /api/v1/chart/portfoliochart) using RestSharp.
Comprehensive Data Collection: Scrapes and processes:
Fund Types: Fund categories and their status.
Average Returns: Performance metrics (daily, weekly, monthly, etc.) for each fund type.
Fund Details: General information like fund name, manager, registration details, and website.
Fund Metrics: Key performance indicators such as NAV (Net Asset Value), fund size, and efficiency metrics.
Fund Investments: Ownership structure (e.g., individual vs. institutional investors).
Fund Ranks: Performance rankings over various periods (12, 24, 36 months, etc.).
Mutual Fund Licenses: License details including status and expiration.
Fund Risk: Risk metrics like Alpha and Beta.
Fund Units: Unit-level data for subscriptions and cancellations.
Fund Composition: Portfolio allocation (stocks, bonds, cash, etc.).
Net Assets: Historical net asset values and unit subscriptions/redemptions.
NAV Comparison: Comparison of issue, cancel, and statistical NAVs.
Fund Efficiency: Efficiency metrics over different timeframes.
Profit per Unit: Profit distribution data.
Instrument Data: Market instrument details, including best limits, client types, and transactions.
Data Persistence: Stores data in a relational database using Entity Framework Core, with support for updating existing records.
Error Handling: Robust handling of API errors, invalid data, and network issues.
Configurable HTTP Client: Uses RestSharp with a custom user agent and SSL bypass for reliable API communication.
Technologies
Language: C# (.NET Core/Framework)
Libraries:
RestSharp: For making HTTP requests to FIPIran APIs.
System.Text.Json: For JSON parsing and deserialization.
EntityFrameworkCore: For database operations.
Tools: Visual Studio, Git, SQL Server (or other compatible database)
Database: Relational database (configurable via SanayContext)
