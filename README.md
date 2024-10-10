# Monster Trading Card Game

## Overview

This project is an intermediate version of the Monster Trading Card Game, a server-based card trading and battling game implemented in C#. At this stage, the project focuses on implementing the HTTP server and basic user registration endpoints.

## Current Features

- Basic HTTP server implementation
- REST API for user registration and login
- In-memory database for temporary data storage
- Basic card types (Monster and Spell cards) 
- Fundamental battle logic implementation
- Simple battle service for player encounters

## Technical Stack

- C# (.NET 8.0)
- Custom HTTP server implementation
- Temporary in-memory database (to be replaced with PostgreSQL in future)

## Project Structure

### Overview



![overview](./MonsterTradingCardGame/ClassDiagram/png/Overview.png)

![layers](./MonsterTradingCardGame/ClassDiagram/png/MonsterTradingCardGameClassDiagram.png)

### API Layer
![apylayer](./MonsterTradingCardGame/ClassDiagram/png/API%20Layer%20Diagram.png)

## Business Layer
![businesslayer](./MonsterTradingCardGame/ClassDiagram/png/Business%20Layer%20Diagram.png)

## Data Layer and Domain
![datalayer](./MonsterTradingCardGame/ClassDiagram/png/Data%20Layer%20and%20Domain%20Models%20Diagram.png)

## Getting Started

1. Clone the repository.
2. Open the solution in your preferred IDE (e.g., Visual Studio, Rider).
3. Build the project.
4. Run the `Program.cs` file in the `Presentation.Console` namespace.

## Future Development


- Integrate PostgreSQL database.
- Develop trading and deck management features.
- Create user stats and scoreboard system
- Expand test coverage.

## Contributor
Aliz Jakus
