# Playlist API

A REST API for creating playlists and managing the songs inside them, built for the Academy Backend Test. I hosted this project. You can find the backend at [Google Cloud Deployment ](https://playlist-api-63827338652.me-central1.run.app). Note the database is just an sqlite instance in the backend so every session it resets . Exact code on the deploy branch. I also made and a React frontend that calls this backend at [Vercel Frontend](https://luftsong-playlists.vercel.app). There are already already 4 songs seeded into the DB on both local and hosted versions. You can create a playlist to test it out.

## Tech Stack

- **ASP.NET Core 10** (Web API, controller-based)
- **Entity Framework Core 10** with **SQLite**
- **xUnit** for unit and integration tests
- **Scalar** for interactive API documentation (development only)

### Why SQLite?

I would usually use MSSQL but I used because:

- The data is relational (a playlist has many songs) and benefits from foreign keys ( So any SQL database ).
- It requires **no separate server to install or run** — the reviewer can clone and run the project on any machine immediately( Only SQLite can do that)

## Scope & Assumptions

> **Note on users / ownership:** User accounts, authentication, and per-user ownership of playlists are considered **out of scope** for this task and are **not implemented** 

## Architecture

A simple layered (clean-ish) architecture:

| Project | Responsibility |
|---|---|
| `PlaylistApi.API` | HTTP layer — controllers, app startup |
| `PlaylistApi.Core` | Domain entities, DTOs, repository interfaces, mappers |
| `PlaylistApi.EF` | EF Core `AppDbContext`, migrations, repository implementations |
| `tests/PlaylistApi.UnitTests` | Repository unit tests (in-memory DB) |
| `tests/PlaylistApi.IntegrationTests` | End-to-end HTTP tests 


## Data Model

- **Playlist** — `Id`, `Name`, `Description`, `Songs`, `CreatedAt`, `UpdatedAt`
- **Song** — `Id`, `Title`, `Artist`, `Album`, `Genre`, `Duration`, `CreatedAt`, `UpdatedAt`
- A playlist has many songs via an **optional** foreign key (`Song.PlaylistId` is nullable). Songs are treated as a **shared catalog**: deleting a playlist orphans its songs (sets their `PlaylistId` to null) rather than deleting them.
- Four songs are **seeded** on startup.

## Running the Project

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download)

1. Clone the project locally onto your computer

2. Navigate into BackendTask folder

```bash
# from the BackendTask folder
dotnet restore
dotnet run --project src/PlaylistApi.API
```

3. The database file and schema are created/migrated automatically on first run. The API listens on the URL printed in the console (e.g. `https://localhost:xxxx`).

4. Interactive docs (development): open `/scalar/v1` in the browser.

## API Endpoints

### Playlists — `/api/playlists`

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/playlists` | List all playlists |
| `GET` | `/api/playlists/{id}` | Get one playlist (with its songs) |
| `POST` | `/api/playlists` | Create a playlist |
| `PUT` | `/api/playlists/{id}` | Update a playlist |
| `DELETE` | `/api/playlists/{id}` | Delete a playlist |
| `POST` | `/api/playlists/{playlistId}/songs` | Add an existing catalog song to a playlist (by `songId`) |
| `DELETE` | `/api/playlists/{playlistId}/songs/{songId}` | Remove a song from a playlist |

### Songs — `/api/songs`

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/songs` | List all songs in the catalog |

Request bodies are validated (`Name`/`Title`/`Artist` are required); invalid input returns `400 Bad Request`.


## Tests

```bash
dotnet test                                                   # all tests
dotnet test tests/PlaylistApi.UnitTests                       # unit only
dotnet test tests/PlaylistApi.IntegrationTests                # integration only
```

- **Unit tests** exercise the repositories against an EF Core in-memory database.
- **Integration tests** spin up the full app, swapping SQLite for an in-memory provider, and drive the real HTTP endpoints.
