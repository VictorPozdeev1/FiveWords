export function setupInputValidationBehavior(input, validityMessage) {
    input.addEventListener('input', () => {
        input.setCustomValidity('');
        input.checkValidity();
    });
    input.addEventListener('invalid', () => {
        if (!input.validity.valid && !input.validity.customError)
            input.setCustomValidity(validityMessage);
    });
}