import { createRoot } from 'react-dom/client';
import TranslationChallengePage from "./components/TranslationChallengePage/TranslationChallengePage";

addEventListener('load', () => {
    let dictionaryName;
    const regexResult = location.pathname.match(/challenge-page\/(.+)/);
    if (regexResult && regexResult[1])
        dictionaryName = decodeURI(regexResult[1]);
    createRoot(document.querySelector("#reactRoot"))
        .render(<TranslationChallengePage dictionaryName={dictionaryName} />);
});