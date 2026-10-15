import CreateLinkForm from '../components/links/CreateLinkForm.jsx';
import LinkPreviewCard from '../components/links/LinkPreviewCard.jsx';
import LinkResultCard from '../components/links/LinkResultCard.jsx';
import Footer from '../components/layout/Footer.jsx';
import Header from '../components/layout/Header.jsx';
import HeroSection from '../components/layout/HeroSection.jsx';
import HowItWorks from '../components/layout/HowItWorks.jsx';
import { useCreateShortLink } from '../features/links/hooks/useCreateShortLink.js';

export default function App() {
  const {
    state,
    aliasError,
    updateField,
    submitForm,
    copyShortUrl,
    resetForm,
  } = useCreateShortLink();

  return (
    <div className="app-shell">
      <Header />
      <main>
        <HeroSection />
        <section className="workspace" aria-label="Create a short URL">
          <CreateLinkForm
            originalUrl={state.originalUrl}
            customAlias={state.customAlias}
            aliasError={aliasError}
            error={state.error}
            isSubmitting={state.isSubmitting}
            onFieldChange={updateField}
            onSubmit={submitForm}
          />
          <aside className="side-panel">
            {state.result ? (
              <LinkResultCard
                result={state.result}
                copied={state.copied}
                onCopy={copyShortUrl}
                onReset={resetForm}
              />
            ) : (
              <LinkPreviewCard />
            )}
          </aside>
        </section>
        <HowItWorks />
      </main>
      <Footer />
    </div>
  );
}
