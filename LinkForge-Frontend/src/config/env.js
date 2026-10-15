const configuredApiBaseUrl = import.meta.env.VITE_API_BASE_URL;

/** Centralized frontend environment configuration. */
export const appConfig = Object.freeze({
  apiBaseUrl: (configuredApiBaseUrl || 'https://localhost:7118')
    .trim()
    .replace(/\/+$/, ''),
  frontendOrigin: window.location.origin,
});
