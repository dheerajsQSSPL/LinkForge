import { useEffect, useMemo, useReducer, useRef } from 'react';
import { createShortLink } from '../../../services/links/linksApi.js';
import { validateCustomAlias, validateOriginalUrl } from '../../../utils/urlValidation.js';

const initialState = {
  originalUrl: '',
  customAlias: '',
  result: null,
  error: '',
  isSubmitting: false,
  copied: false,
};

function reducer(state, action) {
  switch (action.type) {
    case 'fieldChanged':
      return { ...state, [action.field]: action.value, error: '' };
    case 'submitStarted':
      return { ...state, error: '', result: null, copied: false, isSubmitting: true };
    case 'submitSucceeded':
      return { ...state, result: action.result, isSubmitting: false, error: '' };
    case 'submitFailed':
      return { ...state, error: action.message, isSubmitting: false };
    case 'copySucceeded':
      return { ...state, copied: true, error: '' };
    case 'reset':
      return initialState;
    default:
      return state;
  }
}

/** Owns the create-link screen state and its user actions. */
export function useCreateShortLink() {
  const [state, dispatch] = useReducer(reducer, initialState);
  const copiedTimerRef = useRef(null);

  const aliasError = useMemo(
    () => validateCustomAlias(state.customAlias),
    [state.customAlias],
  );

  useEffect(() => () => window.clearTimeout(copiedTimerRef.current), []);

  function updateField(field, value) {
    dispatch({ type: 'fieldChanged', field, value });
  }

  async function submitForm(event) {
    event.preventDefault();

    const urlError = validateOriginalUrl(state.originalUrl);
    if (urlError) {
      dispatch({ type: 'submitFailed', message: urlError });
      return;
    }

    const currentAliasError = validateCustomAlias(state.customAlias);
    if (currentAliasError) {
      dispatch({ type: 'submitFailed', message: currentAliasError });
      return;
    }

    dispatch({ type: 'submitStarted' });

    try {
      const result = await createShortLink({
        originalUrl: state.originalUrl.trim(),
        customAlias: state.customAlias.trim(),
      });

      if (!result?.shortUrl) {
        throw new Error('The API response did not include a short URL.');
      }

      dispatch({ type: 'submitSucceeded', result });
    } catch (error) {
      dispatch({
        type: 'submitFailed',
        message: error instanceof Error ? error.message : 'An unexpected error occurred.',
      });
    }
  }

  async function copyShortUrl() {
    if (!state.result?.shortUrl) return;

    try {
      if (!navigator.clipboard?.writeText) {
        throw new Error('Clipboard access is unavailable in this browser. Select and copy the URL manually.');
      }

      await navigator.clipboard.writeText(state.result.shortUrl);
      dispatch({ type: 'copySucceeded' });
      window.clearTimeout(copiedTimerRef.current);
      copiedTimerRef.current = window.setTimeout(() => {
        dispatch({ type: 'fieldChanged', field: 'copied', value: false });
      }, 1800);
    } catch (error) {
      dispatch({
        type: 'submitFailed',
        message: error instanceof Error
          ? error.message
          : 'Could not copy the URL. Select and copy it manually.',
      });
    }
  }

  function resetForm() {
    window.clearTimeout(copiedTimerRef.current);
    dispatch({ type: 'reset' });
  }

  return {
    state,
    aliasError,
    updateField,
    submitForm,
    copyShortUrl,
    resetForm,
  };
}
