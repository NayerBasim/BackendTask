# Playlist API

A REST API for creating playlists and managing the songs inside them, built for the Academy Backend Test.

## Tech Stack

- **ASP.NET Core 10** (Web API, controller-based)
- **Entity Framework Core 10** with **SQLite**
- **xUnit** for unit and integration tests
- **Scalar** for interactive API documentation (development only)

### Why SQLite?

SQLite is a zero-configuration, file-based relational database (`playlist.db`). It was chosen because:

- The data is relational (a playlist has many songs) and benefits from foreign keys.
- It requires **no separate server to install or run** — the reviewer can clone and run the project on any machine immediately, which matches the "must run properly on any machine" deliverable.
- The same EF Core model can be swapped to SQL Server / PostgreSQL by changing one line in `AddPersistence` if production scale is needed.

## Scope & Assumptions

> **Note on users / ownership:** The business requirements mention *"a user's playlists."* User accounts, authentication, and per-user ownership of playlists are considered **out of scope** for this task and are **not implemented** — playlists are currently global rather than scoped to an owner. A `User` entity exists as a placeholder only. Adding ownership would mean introducing a `UserId` on `Playlist`, authentication, and filtering queries by the current user.

## Architecture

A simple layered (clean-ish) architecture:

| Project | Responsibility |
|---|---|
| `PlaylistApi.API` | HTTP layer — controllers, DI wiring, app startup |
| `PlaylistApi.Core` | Domain entities, DTOs, repository interfaces, mappers |
| `PlaylistApi.EF` | EF Core `AppDbContext`, migrations, repository implementations |
| `tests/PlaylistApi.UnitTests` | Repository unit tests (in-memory DB) |
| `tests/PlaylistApi.IntegrationTests` | End-to-end HTTP tests via `WebApplicationFactory` |

Controllers depend on **repository interfaces** (`IPlaylistRepository`, `ISongRepository`), so the data layer is decoupled and easily mockable.

## Data Model

- **Playlist** — `Id`, `Name`, `Description`, `Songs`, `CreatedAt`, `UpdatedAt`
- **Song** — `Id`, `Title`, `Artist`, `Album`, `Genre`, `Duration`, `CreatedAt`, `UpdatedAt`
- A playlist has many songs via an **optional** foreign key (`Song.PlaylistId` is nullable). Songs are treated as a **shared catalog**: deleting a playlist orphans its songs (sets their `PlaylistId` to null) rather than deleting them.
- Four songs are **seeded** on startup.

## Running the Project

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download)

```bash
# from the BackendTask folder
dotnet restore
dotnet run --project src/PlaylistApi.API
```

The database file and schema are created/migrated automatically on first run. The API listens on the URL printed in the console (e.g. `https://localhost:xxxx`).

Interactive docs (development): open `/scalar/v1` in the browser.

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

#### Example — create a playlist

```bash
curl -X POST https://localhost:xxxx/api/playlists \
  -H "Content-Type: application/json" \
  -d '{ "name": "Road Trip", "description": "Songs for the drive" }'
```

#### Example — add a song to a playlist

Songs are a shared catalog. Pick a `songId` from `GET /api/songs`, then link it:

```bash
curl -X POST https://localhost:xxxx/api/playlists/{playlistId}/songs \
  -H "Content-Type: application/json" \
  -d '{ "songId": "11111111-1111-1111-1111-111111111111" }'
```

## Tests

```bash
dotnet test                                                   # all tests
dotnet test tests/PlaylistApi.UnitTests                       # unit only
dotnet test tests/PlaylistApi.IntegrationTests                # integration only
```

- **Unit tests** exercise the repositories against an EF Core in-memory database.
- **Integration tests** spin up the full app with `WebApplicationFactory`, swapping SQLite for an in-memory provider, and drive the real HTTP endpoints.
