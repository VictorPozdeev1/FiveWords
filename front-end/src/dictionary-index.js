import { fetchWithAuth, removeToken, isAuthenticated } from "./modules/Auth";
import { createRoot } from 'react-dom/client'
import WordTranslationsContainer from './components/WordTranslationsContainer/WordTranslationsContainer';

if (!isAuthenticated())
    location.assign('/');

addEventListener('load', async () => {
    let responseJson = null;
    try {
        const sections = location.pathname.split('/').filter(x => x.length > 0);
        const dictionaryName = sections.pop();
        const url = new URL(`dictionaries/${dictionaryName}`, location.origin);
        const response = await fetchWithAuth(url);
        if (response.ok) {
            responseJson = await response.json();
            document.querySelector('#dictionaryName').innerHTML = responseJson.header.id;
            createRoot(document.querySelector("#reactRoot"))
                .render(<WordTranslationsContainer content={responseJson.content} dictionaryName={dictionaryName} />);
        }
        else {
            alert("Не удалось загрузить ваши словари. Вы будете перенаправлены на гостевую страницу.");
            removeToken();
            location.assign('/');
        }
    }
    catch (error) {
        console.error(error);
    }
});