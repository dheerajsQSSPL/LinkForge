import { requestJson } from '../api/httpClient.js';

/** API operations for short links. Keep endpoint details out of UI components. */
export function createShortLink({ originalUrl, customAlias }) {
  const payload = { originalUrl };
  if (customAlias) payload.customAlias = customAlias;

  return requestJson('/api/links', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}
