import { appConfig } from '../../config/env.js';
import { ApiError } from './ApiError.js';

function getResponseMessage(payload, status) {
  if (payload && typeof payload === 'object') {
    if (typeof payload.message === 'string' && payload.message.trim()) {
      return payload.message;
    }
    if (typeof payload.title === 'string' && payload.title.trim()) {
      return payload.title;
    }
  }

  if (status === 409) return 'That URL or alias already exists. Try a different alias.';
  if (status === 400) return 'Please check the submitted information and try again.';
  return `The request failed with status ${status}.`;
}

async function readResponseBody(response) {
  if (response.status === 204) return null;

  const responseText = await response.text();
  if (!responseText) return null;

  try {
    return JSON.parse(responseText);
  } catch {
    return { message: responseText };
  }
}

/** Shared HTTP client: centralizes request headers, network errors and API errors. */
export async function requestJson(path, options = {}) {
  const controller = new AbortController();
  const timeoutId = window.setTimeout(() => controller.abort(), 15000);

  try {
    const response = await fetch(`${appConfig.apiBaseUrl}${path}`, {
      ...options,
      signal: controller.signal,
      headers: {
        Accept: 'application/json',
        ...(options.body ? { 'Content-Type': 'application/json' } : {}),
        ...options.headers,
      },
    });

    const payload = await readResponseBody(response);
    if (!response.ok) {
      throw new ApiError(getResponseMessage(payload, response.status), {
        status: response.status,
        payload,
      });
    }

    return payload;
  } catch (error) {
    if (error instanceof ApiError) throw error;

    if (error?.name === 'AbortError') {
      throw new ApiError('The request timed out. Please try again.', { cause: error });
    }

    throw new ApiError(
      `Could not reach the LinkForge API at ${appConfig.apiBaseUrl}. Check that the API is running, its HTTPS certificate is trusted, and CORS allows ${appConfig.frontendOrigin}.`,
      { cause: error },
    );
  } finally {
    window.clearTimeout(timeoutId);
  }
}
