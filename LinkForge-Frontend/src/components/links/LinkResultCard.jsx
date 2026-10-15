import { Check, CheckCircle2, Clipboard, ExternalLink, RotateCcw, Zap } from 'lucide-react';
import { formatDate } from '../../utils/dateFormatter.js';

export default function LinkResultCard({ result, copied, onCopy, onReset }) {
  return (
    <div className="result-panel">
      <div className="result-success"><CheckCircle2 size={17} /> LINK CREATED</div>
      <h3>Your link is ready.</h3>
      <p className="result-intro">Here’s your new short link. Copy it and share it anywhere.</p>
      <div className="short-url-box">
        <a href={result.shortUrl} target="_blank" rel="noreferrer">{result.shortUrl}</a>
        <button className="copy-button" type="button" onClick={onCopy} aria-label="Copy short URL">
          {copied ? <Check size={17} /> : <Clipboard size={17} />}
          <span>{copied ? 'Copied' : 'Copy'}</span>
        </button>
      </div>
      <div className="result-meta">
        <div className="meta-icon"><Zap size={16} /></div>
        <div><span>Expires on</span><strong>{formatDate(result.expiresAt)}</strong></div>
      </div>
      {result.message && <p className="server-message">{result.message}</p>}
      <a className="open-link" href={result.shortUrl} target="_blank" rel="noreferrer">
        Test your short link <ExternalLink size={15} />
      </a>
      <button className="text-button" type="button" onClick={onReset}>
        <RotateCcw size={14} /> Create another link
      </button>
    </div>
  );
}
