import { createRoot } from 'react-dom/client';
import TranslationChallenge from "./components/TranslationChallenge/TranslationChallenge";

addEventListener('load', () => {
    const sections = location.pathname.split('/').filter(x => x.length > 0);
    const dictionaryName = sections.pop();

    document.querySelector('#dictionaryName').innerHTML = dictionaryName;
    createRoot(document.querySelector("#reactRoot"))
        .render(<TranslationChallenge dictionaryName={dictionaryName} />);
});