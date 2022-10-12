import { fetchWithAuth, removeToken, isAuthenticated } from "../script/Auth.js";
//import { UserDictionaryCard } from "./UserDictionaryCard.js";
//import { showPromptForm } from "./PromptForm.js";

if (!isAuthenticated())
    location.assign('/');

addEventListener('load', async () => {
    let responseJson = null;
    try {
        const sections = location.pathname.split('/').filter(x => x.length > 0);
        const dictionaryName = sections.pop();
        const url = new URL(encodeURI(`dictionaries/${dictionaryName}`), location.origin);
        const response = await fetchWithAuth(url);
        if (response.ok) {
            responseJson = await response.json();
            document.querySelector('#dictionaryName').innerHTML = responseJson.header.id;
            responseJson.content.forEach(wordTranslation => {
                const wordTranslationElement = document.createElement('p');
                wordTranslationElement.innerHTML = `${wordTranslation.id} - ${wordTranslation.translation}`;
                wordTranslationElement.style.color = 'gray';
                document.querySelector('.dictionaries-block').append(wordTranslationElement);
            });
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

    //try {
    //    if (responseJson) {
    //        document.querySelector('#login_Label').innerHTML = responseJson.user;
    //        responseJson.dictionaries.forEach(dictionary => {
    //            dictionariesContainer.insertBefore(new UserDictionaryCard(dictionary), createNewDictionary_Div);
    //        });
    //    }
    //}
    //catch (error) {
    //    console.error(error);
    //}
});