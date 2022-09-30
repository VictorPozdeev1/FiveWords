import { setupInputValidationBehavior } from './InputValidation.js'

export function showPromptForm(title, defaultValue, pattern, patternMessage) {
    const modalTemplateHtml =
        `<div class="modal fade" id="exampleModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">${title}</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Закрыть"></button>
                    </div>
                    <div class="modal-body">
                        <input type="text" name="" value="${defaultValue}" id="input" required pattern="${pattern}">
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Закрыть</button>
                    <button type="button" class="btn btn-primary" id="save">Сохранить изменения</button>
                    </div>
                </div>
            </div>
        </div>`;

    document.body.insertAdjacentHTML('beforeend', modalTemplateHtml);
    const modalTemplate = document.querySelector('#exampleModal');
    const modal = new bootstrap.Modal(modalTemplate);
    modal.show();

    const input = modalTemplate.querySelector('#input');
    setupInputValidationBehavior(input, patternMessage)

    return new Promise((resolve, reject) => {
        modalTemplate.querySelector('#save').addEventListener('click', () => {
            if (input.validity.valid) {
                modal.hide();
                resolve(input.value);
            }
            else {
                input.reportValidity();
            }
        });
        modalTemplate.addEventListener('hidden.bs.modal', (event) => {
            modal.dispose();
            modalTemplate.remove();
            reject('cancelled by user');
        });
    });
}