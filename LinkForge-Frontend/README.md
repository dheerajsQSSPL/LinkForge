# LinkForge React Frontend — Refactored

A reorganized React + Vite frontend for the existing anonymous LinkForge URL shortener API. This refactor preserves the existing user-facing flow while separating UI components, link feature state, API access, environment configuration, and validation utilities.

## Requirements

- Node.js 20 or newer
- Existing LinkForge ASP.NET Core API

## Run locally

```bash
npm install
npm run dev
```

Open <http://localhost:5173>.

The `.env` file configures the API origin:

```env
VITE_API_BASE_URL=https://localhost:7118
```

`VITE_API_BASE_URL` must point to the backend API, not the React frontend. For another environment, copy `.env.example` to `.env` and set the appropriate API origin. Restart Vite after changing environment variables.

## Build and formatting

```bash
npm run build
npm run format
npm run format:check
```

`npm run format` uses Prettier to consistently format JavaScript, JSX, CSS, JSON, HTML, and Markdown files included by the configured script.

## Project structure

```text
src/
├── app/
│   └── App.jsx                       # Screen composition
├── components/
│   ├── layout/                       # Shared brand, header, footer, hero, how-it-works
│   └── links/                        # Create form, preview and result cards
├── config/
│   └── env.js                        # Centralized Vite environment settings
├── features/
│   └── links/hooks/
│       └── useCreateShortLink.js     # Link screen state and actions (useReducer)
├── services/
│   ├── api/                          # Shared HTTP client and API error type
│   └── links/                        # Link API functions
├── styles/
│   └── global.css                    # Global design system and responsive styles
├── utils/
│   ├── dateFormatter.js              # Expiry date formatting
│   └── urlValidation.js              # URL and custom-alias validation
└── main.jsx                           # React entry point
```

## Separation of responsibilities

- **UI components:** render the layout and receive values/callbacks through props.
- **Feature hook:** owns the create-link screen state using `useReducer`; no external state library is needed for this small application.
- **Link API module:** contains the `POST /api/links` operation.
- **Shared HTTP client:** centralizes JSON parsing, timeout handling, network errors, and HTTP error messages.
- **Validation utilities:** keep URL and alias rules outside JSX.
- **Environment configuration:** keeps API origin outside the application components.

## API contract

Request to `POST /api/links`:

```json
{
  "originalUrl": "https://example.com/page",
  "customAlias": "optional-alias"
}
```

When no alias is provided, `customAlias` is omitted. The frontend expects a successful response with `message`, `shortUrl`, and `expiresAt` fields.

## CORS for local development

Since the React origin is `http://localhost:5173` and the API origin is `https://localhost:7118`, enable a development CORS policy in the ASP.NET Core API allowing exactly `http://localhost:5173`. Keep the policy restricted to trusted frontend origins in production.

## Error handling

- Client-side validation happens before the API request.
- HTTP errors use the API's `message` or `title` where available.
- Network failures and request timeouts show actionable messages.
- Unexpected/malformed API response shapes are handled rather than silently rendering an empty link.
- Clipboard failures provide a message so the URL can be copied manually.

## Scope

LinkForge remains an anonymous URL shortener. This refactor does not add authentication, user ownership, analytics endpoints, or change the backend API contract. It removes the original monolithic `App.jsx` by extracting responsibilities into modules; the visual design is retained.
