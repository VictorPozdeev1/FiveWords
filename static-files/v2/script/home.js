import { fetchWithAuth, removeToken, isAuthenticated } from "./Auth.js";
import { UserDictionaryCard } from "./UserDictionaryCard.js";
import { showPromptForm } from "./PromptForm.js";

if (!isAuthenticated())
    location.assign('/');

addEventListener('load', async () => {
    const dictionariesContainer = document.querySelector('#userDictionaries_Div');
    const createNewDictionary_Div = document.querySelector('#createNewDictionary_Div');

    let responseJson = null;
    try {
        const response = await fetchWithAuth('/dictionaryHeaders');
        if (response.ok)
            responseJson = await response.json();
        else {
            alert("Не удалось загрузить ваши словари. Вы будете перенаправлены на гостевую страницу.");
            removeToken();
            location.assign('/');
        }
    }
    catch (error) {
        console.error(error);
    }

    try {
        if (responseJson) {
            document.querySelector('#login_Label').innerHTML = responseJson.user;
            responseJson.dictionaryHeaders.forEach(dictionaryHeader => {
                dictionariesContainer.insertBefore(new UserDictionaryCard(dictionaryHeader), createNewDictionary_Div);
            });
        }
    } catch (error) {
        console.error(error);
    }

    document.querySelector('#logout_Button').addEventListener('click', (event) => {
        event.preventDefault();
        removeToken();
        location.assign('/');
    });

    document.querySelector('#createNewDictionary_Button').addEventListener('click', (event) => {
        //event.preventDefault();
        showPromptForm('Введите название:', '', '^[\\wА-Яа-яЁё ]{4,20}$', '4-20 букв, цифр, пробелов или подчёркиваний')
            .then(async resolved => {
                const newData = { Header: { id: resolved } };
                newData.Content = [{ "translation": "English1", "id": "Русское1" }, { "translation": "English2", "id": "Русское2" }, { "translation": "English3", "id": "Русское15" }];
                const response = await fetchWithAuth(`/dictionaries`, {
                    method: 'POST',
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(newData)
                });
                if (response.ok) {
                    const createdDictionary = await response.json();
                    const createdDictionaryHeader = {};
                    Object.assign(createdDictionaryHeader, createdDictionary.header, { wordsQuantity: createdDictionary.content.length })
                    dictionariesContainer.insertBefore(new UserDictionaryCard(createdDictionaryHeader), createNewDictionary_Div);
                    location.assign(`dictionary-page/${createdDictionaryHeader.id}`);
                }
                else {
                    if (response.status == 400) {
                        const responseJson = await response.json();
                        alert(responseJson.errors["Id"][0] ?? "Что-то пошло не так. Попробуйте обновить страницу...");
                    }
                    else {
                        const responseJson = await response.json();
                        alert(responseJson.error.message ?? "Что-то пошло не так. Попробуйте обновить страницу...");
                    }
                }
            });
    });
});