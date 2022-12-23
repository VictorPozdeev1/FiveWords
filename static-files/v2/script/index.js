import { saveToken, isAuthenticated } from "./Auth.js";
import { setupInputValidationBehavior } from './InputValidation.js'

if (isAuthenticated())
    location.assign('/home');

const mainBlock = document.querySelector('.main-block');

const possiblePopups = Array.from(document.querySelectorAll('.form'));

const openSignupForm_Button = document.getElementById('openSignupForm_Button');
const openSigninForm_Button = document.getElementById('openSigninForm_Button');

const signupForm = document.getElementById('signupForm');
const signupFormLogin_Input = document.getElementById('signupFormLogin_Input');
const signupFormPassword_Input = document.getElementById('signupFormPassword_Input');
const signupFormPasswordRepeat_Input = document.getElementById('signupFormPasswordRepeat_Input');
const signupFormClose_Button = document.getElementById('signupFormClose_Button');

const signinForm = document.getElementById('signinForm');
const signinFormLogin_Input = document.getElementById('signinFormLogin_Input');
const signinFormPassword_Input = document.getElementById('signinFormPassword_Input');
const signinFormClose_Button = document.getElementById('signinFormClose_Button');

openSignupForm_Button.addEventListener('click', openSignupForm);
openSigninForm_Button.addEventListener('click', openSigninForm);
signupFormClose_Button.addEventListener('click', closeSignupForm);
signinFormClose_Button.addEventListener('click', closeSigninForm);

document.querySelector('#startChallenge').addEventListener('click', () =>
    location.assign('guest-challenge-page'));

setupInputValidationBehavior(signupFormLogin_Input, '4-20 латинских букв, цифр, пробелов или подчёркиваний');
setupInputValidationBehavior(signupFormPassword_Input, '4-20 любых символов');
setupInputValidationBehavior(signupFormPasswordRepeat_Input, '~~~(This message will never be shown probably. Let\'s check it..)');

signupForm.addEventListener('submit', (event) => {
    event.preventDefault();
    if (signupFormPasswordRepeat_Input.value !== signupFormPassword_Input.value) {
        signupFormPasswordRepeat_Input.setCustomValidity('Пароли не совпадают');
        signupFormPasswordRepeat_Input.reportValidity();
        return;
    }
    const loginString = signupFormLogin_Input.value;
    const passwordString = signupFormPasswordRepeat_Input.value;
    userSignupFetch(loginString, passwordString)
        .then(async response => {
            if (response.ok) {
                const signinResponse = await userSigninFetch(loginString, passwordString);
                if (signinResponse.ok) {
                    const token = await signinResponse.text();
                    saveToken(token);
                    location.assign('/home');
                }
                else {
                    response.json().then(json => {
                        const errorMessage = json.error.message;
                        console.error(errorMessage);
                        alert(errorMessage);
                        closeSignupForm();
                    });
                }
                return Promise.resolve();
            }
            else {
                try {
                    switch (response.status) {
                        case 400:
                            response.json().then(json => {
                                const responseErrors = json.errors;
                                for (let key in responseErrors) {
                                    console.error(`${key} : ${responseErrors[key]}`);
                                    const firstResponseError = responseErrors[0];
                                    signupFormLogin_Input.setCustomValidity(firstResponseError);
                                    signupFormLogin_Input.reportValidity(firstResponseError);
                                }
                            });
                            break;
                        case 409:
                            response.json().then(json => {
                                const responseError = json.error.message;
                                signupFormLogin_Input.setCustomValidity(responseError);
                                signupFormLogin_Input.reportValidity(responseError);
                            });
                            break;
                        default:
                            //?
                            console.log(response.status, response.statusText);
                    }
                }
                catch (error) {
                    console.log(error);
                }
                return Promise.reject();
            }
        })
});

signinForm.addEventListener('submit', async event => {
    event.preventDefault();
    const loginString = signinFormLogin_Input.value;
    const passwordString = signinFormPassword_Input.value;
    const response = await userSigninFetch(loginString, passwordString);
    if (response.ok) {
        const token = await response.text();
        saveToken(token);
        location.assign('/home');
    }
    else {
        response.json().then(json => {
            const errorMessage = json.error.message;
            signinFormLogin_Input.setCustomValidity(errorMessage);
            signinFormLogin_Input.reportValidity(errorMessage);
        });
    }
});

async function userSignupFetch(loginString, passwordString) {
    return hash(passwordString).then(passwordHash =>
        fetch("/registration", {
            method: "POST",
            headers: { "Accept": "text/html", "Content-Type": "application/json" },
            body: JSON.stringify({ login: loginString, passwordHash: passwordHash })
        }))
}

async function userSigninFetch(loginString, passwordString) {
    return hash(passwordString).then(passwordHash =>
        fetch("/login", {
            method: "POST",
            headers: { "Accept": "text/html", "Content-Type": "application/json" },
            body: JSON.stringify({ login: loginString, passwordHash: passwordHash })
        }))
}

async function hash(string) {
    const utf8 = new TextEncoder().encode(string);
    const hashBuffer = await crypto.subtle.digest('SHA-256', utf8);
    const hashArray = Array.from(new Uint8Array(hashBuffer));
    //hashArray[3] = 42342;
    return hashArray; // .map((bytes) => bytes.toString(16).padStart(2, '0')).join('');
}

mainBlock.addEventListener('click', event => {
    const openPopups = getOpenPopups();
    if (openPopups.length === 0)
        return;
    const composedPath = event.composedPath();
    if (openPopups.every(el => !composedPath.includes(el))) {
        event.stopPropagation();
        closePopups(openPopups);
    }
}, true);

document.addEventListener('keydown', function (e) {
    if (e.key == 'Escape')
        closePopups(getOpenPopups());
});

function getOpenPopups() {
    return possiblePopups.filter(el => getComputedStyle(el).display !== 'none');
}

function openSignupForm() {
    openForm(signupForm);
}

function openSigninForm() {
    openForm(signinForm);
}

function closeSignupForm() {
    closeForm(signupForm);
}

function closeSigninForm() {
    closeForm(signinForm);
}

function openForm(form) {
    form.style.display = 'block';
    refreshBlurState();
}

function closeForm(form) {
    form.style.display = 'none';
    refreshBlurState();
}

function closePopups(popups) {
    popups.forEach(el => el.style.display = 'none');
    refreshBlurState();
}

function refreshBlurState() {
    mainBlock.style.filter =
        possiblePopups.filter(el => getComputedStyle(el).display !== 'none').length > 0
            ? 'blur(5px)'
            : '';
}

if (location.search.match(/register=true/)) openSignupForm();