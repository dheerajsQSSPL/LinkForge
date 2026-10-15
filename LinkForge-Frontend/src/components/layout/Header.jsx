import { LockKeyhole } from 'lucide-react';
import Brand from './Brand.jsx';

export default function Header() {
  return (
    <header className="topbar">
      <Brand />
      <div className="topbar-right">
        <span className="secure-label"><LockKeyhole size={14} /> Simple. Secure. Shareable.</span>
        <span className="api-status"><i /> API configured</span>
      </div>
    </header>
  );
}
