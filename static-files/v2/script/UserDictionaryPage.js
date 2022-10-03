import { fetchWithAuth, removeToken, isAuthenticated } from "../script/Auth.js";
//import { UserDictionaryCard } from "./UserDictionaryCard.js";
//import { showPromptForm } from "./PromptForm.js";

if (!isAuthenticated())
    location.assign('/');

addEventListener('load', async () => {
    //const dictionariesContainer = document.querySelector('#userDictionaries_Div');
    //const createNewDictionary_Div = document.querySelector('#createNewDictionary_Div');
    let responseJson = null;
    try {
        let sections = location.pathname.split('/').filter(x => x.length > 0);
        let dictionaryName = sections.pop();
        let url = new URL(`dictionaryContent/${dictionaryName}`, location.origin);
        const response = await fetchWithAuth(url);
        if (response.ok) {
            responseJson = await response.json();
            document.querySelector('#dictionaryName').innerHTML = responseJson.id;
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