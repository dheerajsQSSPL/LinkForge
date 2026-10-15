import { ArrowRight, Link2, WandSparkles } from 'lucide-react';

const steps = [
  {
    number: '01',
    title: 'Paste your URL',
    description: 'Enter the full destination you want people to visit.',
    Icon: Link2,
  },
  {
    number: '02',
    title: 'Make it yours',
    description: 'Use a custom alias or let LinkForge generate one.',
    Icon: WandSparkles,
  },
  {
    number: '03',
    title: 'Share it anywhere',
    description: 'Copy your short URL and send it wherever you like.',
    Icon: ArrowRight,
  },
];

export default function HowItWorks() {
  return (
    <section className="how-it-works">
      <div className="section-kicker">THREE SIMPLE STEPS</div>
      <h2>From long to linkable.</h2>
      <div className="steps-grid">
        {steps.map(({ number, title, description, Icon }) => (
          <article className="step-card" key={number}>
            <span className="step-number">{number}</span>
            <div className="step-icon"><Icon size={20} /></div>
            <h3>{title}</h3>
            <p>{description}</p>
          </article>
        ))}
      </div>
    </section>
  );
}
