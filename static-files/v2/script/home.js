import { fetchWithAuth, removeToken, isAuthenticated } from "./Auth.js";
import { UserDictionary } from "./UserDictionary.js";
import { showPromptForm } from "./PromptForm.js";

if (!isAuthenticated())
    location.assign('/');

addEventListener('load', async () => {
    const dictionariesContainer = document.querySelector('#userDictionaries_Div');
    const createNewDictionary_Div = document.querySelector('#createNewDictionary_Div');

    let responseJson = null;
    try {
        const response = await fetchWithAuth('/dictionaries');
        if (response.ok)
            responseJson = await response.json();
        else {
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
            responseJson.dictionaries.forEach(dictionary => {
                dictionariesContainer.insertBefore(new UserDictionary(dictionary), createNewDictionary_Div);
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
        showPromptForm('Введите имя:', '', '^[\wа-яA-ЯЁё ]{4,20}$', '4-20 букв, цифр, пробелов или подчёркиваний')
            .then(async resolved => {
                const newData = { id: resolved };
                const response = await fetchWithAuth(`/dictionaries`, {
                    method: 'POST',
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(newData)
                });
                if (response.ok) {
                    const createdDictionary = await response.json();
                    dictionariesContainer.insertBefore(new UserDictionary(createdDictionary), createNewDictionary_Div);
                }
                else
                    alert("Что-то пошло не так. Попробуйте обновить страницу...");
            });
    });
});