import { ArrowRight, Link2, LoaderCircle, LockKeyhole, WandSparkles } from 'lucide-react';

export default function CreateLinkForm({
  originalUrl,
  customAlias,
  aliasError,
  error,
  isSubmitting,
  onFieldChange,
  onSubmit,
}) {
  return (
    <div className="form-card">
      <div className="card-heading">
        <div className="heading-icon"><WandSparkles size={20} /></div>
        <div>
          <h2>Create a short link</h2>
          <p>Paste your destination and we’ll handle the rest.</p>
        </div>
      </div>

      <form onSubmit={onSubmit} noValidate>
        <label htmlFor="originalUrl">Destination URL <span className="required">*</span></label>
        <div className="input-wrap">
          <Link2 className="field-icon" size={18} />
          <input
            id="originalUrl"
            type="url"
            inputMode="url"
            autoComplete="url"
            maxLength={2000}
            placeholder="https://example.com/my/very/long/link"
            value={originalUrl}
            onChange={(event) => onFieldChange('originalUrl', event.target.value)}
            aria-describedby="url-help"
            required
          />
        </div>
        <div className="field-hint" id="url-help">
          Must be a valid HTTP or HTTPS URL · Up to 2,000 characters
        </div>

        <div className="label-row">
          <label htmlFor="customAlias">Custom alias <span className="optional">OPTIONAL</span></label>
          <span className="character-count">{customAlias.length}/20</span>
        </div>
        <div className={`alias-input ${aliasError ? 'has-error' : ''}`}>
          <span className="alias-prefix">…/</span>
          <input
            id="customAlias"
            type="text"
            maxLength={20}
            placeholder="your-memorable-name"
            value={customAlias}
            onChange={(event) => onFieldChange('customAlias', event.target.value)}
            aria-describedby="alias-help"
          />
        </div>
        <div className={`field-hint ${aliasError ? 'inline-error' : ''}`} id="alias-help">
          {aliasError || 'Letters, numbers, hyphens, and underscores only.'}
        </div>

        {error && (
          <div className="error-message" role="alert">
            <span className="error-symbol">!</span>
            <span>{error}</span>
          </div>
        )}

        <button className="submit-button" type="submit" disabled={isSubmitting}>
          {isSubmitting ? (
            <><LoaderCircle className="spin" size={18} /> Creating your link…</>
          ) : (
            <>Forge my link <ArrowRight size={18} /></>
          )}
        </button>
        <p className="privacy-note"><LockKeyhole size={13} /> Your links are stored in your LinkForge database.</p>
      </form>
    </div>
  );
}
