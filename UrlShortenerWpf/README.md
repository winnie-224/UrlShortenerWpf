# URL Shortener – System Design Simulator (WPF)

This project is a **desktop-based system design simulator** that demonstrates how a real-world URL shortener works internally.

It visualizes core backend concepts such as:
- Load balancing
- Stateless services
- Sharded databases
- In-memory caching with LRU eviction
- Cache observability
- Request distribution across service nodes

## System Design Concepts Implemented

- **Load Balancer**
  - Round-robin routing
  - Health-aware service nodes

- **Stateless Services**
  - Multiple identical backend instances
  - Horizontally scalable design

- **Sharded Storage**
  - Hash-based sharding
  - Multiple SQLite shard databases
  - Deterministic routing

- **Cache Layer**
  - In-memory LRU cache
  - Cache hits/misses tracking
  - Eviction visualization

- **Observability**
  - Per-node request counts
  - Cache state visualization

## Tech Stack

- C#
- WPF (.NET 8)
- SQLite
- Microsoft.Data.Sqlite

## Screenshots


## How to Run

1. Clone the repository
2. Open the solution in Visual Studio
3. Build & run the WPF project
4. Interact with the UI to observe system behavior

## Purpose

This project was built for:
- Learning system design concepts
- Interview preparation
- Visualizing backend architecture decisions

---
