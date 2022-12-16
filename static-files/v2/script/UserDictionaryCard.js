import { fetchWithAuth } from "./Auth.js";
import { showPromptForm } from "./PromptForm.js";

export class UserDictionaryCard extends HTMLElement {
    constructor(userDictionaryHeader) {
        super();
        if (userDictionaryHeader) {
            this.data = userDictionaryHeader

            this.nameSpan = document.createElement('SPAN');
            this.nameSpan.setAttribute('slot', 'name');
            this.nameSpan.innerHTML = this.data.id;

            this.wordsQuantitySpan = document.createElement('SPAN');
            this.wordsQuantitySpan.setAttribute('slot', 'wordsQuantity');
            this.wordsQuantitySpan.innerHTML = this.data.wordsQuantity;

            this.append(this.nameSpan, this.wordsQuantitySpan);

            /*if (userDictionary.includeInChallenges)
            this.setAttribute('data-include-in-challenges', null );*/
        }
    }
    connectedCallback() {
        this.attachShadow({ mode: 'open' });
        this.shadowRoot.append(document.querySelector('#userDictionaryTemplate').content.cloneNode(true));
        //this.shadowRoot.querySelector('#includeInChallenges').checked = 'includeInChallenges' in this.dataset;

        this.shadowRoot.querySelector('#rename').addEventListener('click', event => {
            event.preventDefault();
            showPromptForm('Введите новое название:', this.data.id, '^[\\wА-Яа-яЁё ]{4,20}$', '4-20 букв, цифр, пробелов или подчёркиваний')
                .then(async resolved => {
                    let newData = {};
                    Object.assign(newData, this.data, { id: resolved.trim() });
                    const response = await fetchWithAuth(encodeURI(`/dictionaryHeaders/${this.data.id}`), {
                        method: 'PUT',
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify(newData)
                    });
                    if (response.ok) {
                        const responseJson = await response.json();
                        this.data = responseJson;
                        this.nameSpan.innerHTML = this.data.id;
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
                })
            //.catch((rej) => alert(rej));
        });

        this.shadowRoot.querySelector('#delete').addEventListener('click', async event => {
            event.preventDefault();
            if (confirm('Удалить словарь? Это действие нельзя будет отменить!')) {
                try {
                    const response = await fetchWithAuth(encodeURI(`/dictionaries/${this.data.id}`), { method: 'DELETE' });
                    if (response.ok) {
                        this.remove();
                    }
                    else {
                        const responseJson = await response.json();
                        alert(responseJson ?? "Что-то пошло не так. Попробуйте обновить страницу...");
                    }
                }
                catch (error) { console.error(error) }
            }
        });

        this.shadowRoot.querySelector('#editContent').addEventListener('click', () => {
            location.assign(`dictionary-page/${this.data.id}`);
        });

        this.shadowRoot.querySelector('#startChallenge').addEventListener('click', () =>
            location.assign(`challenge-page/${this.data.id}`));
    }
}
customElements.define("user-dictionary", UserDictionaryCard);
