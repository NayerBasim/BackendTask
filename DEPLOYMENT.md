# Deployment (Free Hosting)

The API is containerized ([`Dockerfile`](./Dockerfile)) and ready to deploy to **[Render](https://render.com)** on its free tier — no credit card required for a free web service.

## What's already set up

- **`Dockerfile`** — multi-stage build that publishes and runs the API.
- **`.dockerignore`** — keeps tests, build artifacts, and the local DB out of the image.
- **`render.yaml`** — a Render Blueprint describing a free Docker web service (health check at `/api/songs`).
- The app binds to the platform's `PORT`, exposes the Scalar API docs UI at `/scalar/v1` (with `/` redirecting there), and skips local HTTPS redirection (TLS is handled by Render's proxy).

## What you need to do

1. **Put the repo on GitHub** (Render deploys from a Git remote — there isn't one yet):
   ```bash
   # create an empty repo on github.com first (no README), then:
   git remote add origin https://github.com/<your-username>/<repo-name>.git
   git add .
   git commit -m "Add containerization and Render deployment config"
   git push -u origin HEAD
   ```

2. **Create a Render account** at https://render.com (sign in with GitHub is easiest).

3. **Deploy** — either option works:
   - **Blueprint (uses `render.yaml`):** Dashboard → **New +** → **Blueprint** → connect your repo → **Apply**.
   - **Manual:** Dashboard → **New +** → **Web Service** → connect your repo → Render detects the `Dockerfile` → choose the **Free** plan → **Create Web Service**.

4. **Wait for the first build** (a few minutes). Render gives you a public URL like
   `https://playlist-api-xxxx.onrender.com`.

5. **Verify** — open the URL in a browser; it redirects to the Scalar docs UI where you can try every endpoint. Or:
   ```bash
   curl https://playlist-api-xxxx.onrender.com/api/songs
   ```

## Running local + hosted side by side (with the frontend)

The React frontend (`../frontend`) picks its API target from `VITE_API_BASE_URL`, so the
same code runs in three configurations:

| Configuration | Frontend `VITE_API_BASE_URL` | Backend | CORS needed? |
|---|---|---|---|
| Local FE → Local BE | *unset* (Vite proxies `/api` → `localhost:5164`) | `dotnet run` | No (same origin via proxy) |
| Local FE → Hosted BE | `https://playlist-api-xxxx.onrender.com` | Render | **Yes** — allow `http://localhost:5173` |
| Hosted FE → Hosted BE | set in Vercel project env | Render | **Yes** — allow the Vercel origin |

**Backend CORS** is configured via `Cors:AllowedOrigins` (comma-separated). It defaults to
`http://localhost:5173` in `appsettings.json`. On Render, add an env var to allow your
other origins:

```
Cors__AllowedOrigins = https://your-frontend.vercel.app,http://localhost:5173
```

**Frontend env files** (in `../frontend`):
- Local → local: no file needed (proxy handles it).
- Local → hosted: create `.env.local` with `VITE_API_BASE_URL=https://playlist-api-xxxx.onrender.com`.
- Hosted (Vercel): set `VITE_API_BASE_URL` in the Vercel project's Environment Variables.

## Good to know (free tier)

- **Cold starts:** free instances sleep after ~15 minutes of inactivity; the first request after idle takes ~30–50 seconds while it wakes up.
- **Data is ephemeral:** SQLite lives inside the container, so data resets on every deploy/restart. The four seeded songs are always recreated on startup. (For persistent data you'd attach a paid disk or use a managed database — out of scope here.)

## Other free options

The same `Dockerfile` also works on **Fly.io**, **Google Cloud Run**, or **Koyeb** if you prefer — they read the `PORT` env var the app already honors.
