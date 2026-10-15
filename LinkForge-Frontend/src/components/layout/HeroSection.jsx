import { CheckCircle2, Sparkles } from 'lucide-react';

export default function HeroSection() {
  return (
    <section className="hero">
      <div className="eyebrow"><Sparkles size={14} /> YOUR LINKS, REFINED</div>
      <h1>Long links.<br /><span>Short stories.</span></h1>
      <p className="hero-copy">
        Make every character count. Turn long URLs into clean, memorable links that are ready to share.
      </p>
      <div className="hero-points">
        <span><CheckCircle2 size={16} /> Free to create</span>
        <span><CheckCircle2 size={16} /> Expires in 7 days</span>
        <span><CheckCircle2 size={16} /> No account needed</span>
      </div>
    </section>
  );
}
