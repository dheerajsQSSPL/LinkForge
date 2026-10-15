export const MAX_ORIGINAL_URL_LENGTH = 2000;
export const MAX_CUSTOM_ALIAS_LENGTH = 20;
const CUSTOM_ALIAS_PATTERN = /^[a-zA-Z0-9_-]+$/;

export function validateOriginalUrl(value) {
  const url = value.trim();

  if (!url) return 'Enter the destination URL first.';
  if (url.length > MAX_ORIGINAL_URL_LENGTH) {
    return 'The destination URL must be 2,000 characters or fewer.';
  }

  try {
    const parsedUrl = new URL(url);
    if (!['http:', 'https:'].includes(parsedUrl.protocol)) {
      return 'Enter a valid URL that starts with http:// or https://.';
    }
  } catch {
    return 'Enter a valid URL that starts with http:// or https://.';
  }

  return '';
}

export function validateCustomAlias(value) {
  const alias = value.trim();
  if (!alias) return '';
  if (alias.length > MAX_CUSTOM_ALIAS_LENGTH) {
    return 'Alias must be 20 characters or fewer.';
  }
  if (!CUSTOM_ALIAS_PATTERN.test(alias)) {
    return 'Use only letters, numbers, hyphens, and underscores.';
  }
  return '';
}
