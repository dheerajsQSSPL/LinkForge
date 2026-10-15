import { Link2, LockKeyhole, Sparkles, Zap } from 'lucide-react';

export default function LinkPreviewCard() {
  return (
    <div className="preview-panel">
      <div className="preview-top">
        <div className="preview-badge"><Sparkles size={13} /> LINK PREVIEW</div>
        <span className="preview-dots"><i /><i /><i /></span>
      </div>
      <div className="preview-graphic">
        <div className="orbit orbit-one" />
        <div className="orbit orbit-two" />
        <div className="link-tile tile-back"><Link2 size={24} /></div>
        <div className="link-tile tile-front"><Link2 size={29} /></div>
        <div className="spark spark-one">✦</div>
        <div className="spark spark-two">✧</div>
      </div>
      <h3>Small link.<br /><span>Big possibilities.</span></h3>
      <p>Your generated link will appear here, ready to copy and share.</p>
      <div className="preview-divider" />
      <div className="preview-feature">
        <span className="feature-icon"><Zap size={15} /></span>
        <div><strong>Quick redirects</strong><small>Jump straight to your destination</small></div>
      </div>
      <div className="preview-feature">
        <span className="feature-icon"><LockKeyhole size={15} /></span>
        <div><strong>Built with care</strong><small>Validated links and unique aliases</small></div>
      </div>
    </div>
  );
}
