import { fetchWithAuth, removeToken, isAuthenticated } from "./modules/Auth";
import ReactDOM from 'react-dom';
import WordTranslationsContainer from './components/dictionary';

if (!isAuthenticated())
    location.assign('/');

addEventListener('load', async () => {
    console.log('hello im here! React=', React);
    let responseJson = null;
    try {
        const sections = location.pathname.split('/').filter(x => x.length > 0);
        const dictionaryName = sections.pop();
        const url = new URL(`dictionaries/${dictionaryName}`, location.origin);
        const response = await fetchWithAuth(url);
        if (response.ok) {
            responseJson = await response.json();
            document.querySelector('#dictionaryName').innerHTML = responseJson.header.id;
            ReactDOM.createRoot(document.querySelector("#reactRoot"))
                .render(React.createElement(WordTranslationsContainer, { content: responseJson.content }));
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