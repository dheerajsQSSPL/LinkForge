import { Link2 } from 'lucide-react';

export default function Brand({ footer = false }) {
  const className = footer ? 'brand footer-brand' : 'brand';

  return (
    <a className={className} href="/" aria-label="LinkForge home">
      <span className="brand-mark"><Link2 size={footer ? 17 : 21} /></span>
      <span>Link<span className="brand-accent">Forge</span></span>
    </a>
  );
}
