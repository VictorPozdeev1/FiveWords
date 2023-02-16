import React from 'react';
import useForceUpdate from './useForceUpdate';
import { fetchWithAuth } from '../modules/Auth.js';

export const FETCH_STATUSES = {
    OK: 'ok!',
    ERROR: 'error :(',
    PENDING: 'pending...'
}

const FETCH_FAILED_DEFAULT_MESSAGE = "Не удалось отправить данные на сервер.";
const REQUEST_FAILED_DEFAULT_MESSAGE = "Произошла неизвестная ошибка при обработке запроса.";

const useFetchStoreUpdater = ({ initialContent, urlPathBase }) => {
    const [currentContent, setCurrentContent] = React.useState(initialContent.slice(0));
    const elementFetchStates = React.useRef(new Map()); //Так, или всё же стейтом сделать , чтоб не форсапдейтить?
    const [createdElementFetchState, setCreatedElementFetchState] = React.useState(null);
    const [elementCreatorIsActive, setElementCreatorIsActive] = React.useState(false);
    const forceUpdate = useForceUpdate();

    const fetchCreate = (newValue) => {
        setCreatedElementFetchState({ status: FETCH_STATUSES.PENDING });
        const url = encodeURI(urlPathBase);
        const options = {
            method: 'POST',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newValue)
        }
        //new Promise((resolve, reject) => setTimeout(confirm('Успешный запрос к серверу?') ? resolve : reject, 3000))
        fetchWithAuth(url, options)
            .then((response) => {
                if (response.ok) {
                    setCurrentContent(currentContent => [...currentContent, newValue]);
                    elementFetchStates.current.set(newValue.id, { status: FETCH_STATUSES.OK });
                    setCreatedElementFetchState(null);
                    setElementCreatorIsActive(false);
                }
                else {
                    response.json().then(json => {
                        const errorMessage = json.error?.message ?? json.errors[Object.keys(json.errors)[0]] ?? REQUEST_FAILED_DEFAULT_MESSAGE;
                        setCreatedElementFetchState({ status: FETCH_STATUSES.ERROR, message: errorMessage });
                        forceUpdate();
                    })
                }
            })
            .catch(() => {
                setCreatedElementFetchState({ status: FETCH_STATUSES.ERROR, message: FETCH_FAILED_DEFAULT_MESSAGE });
            });
    };

    const fetchUpdate = (id, newValue) => {
        elementFetchStates.current.set(id, { status: FETCH_STATUSES.PENDING });
        forceUpdate();

        const url = encodeURI(`${urlPathBase}/${id}`);
        const options = {
            method: 'PUT',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newValue)
        };
        //new Promise((resolve, reject) => setTimeout(confirm('Успешный запрос к серверу?') ? resolve : reject, 3000))
        fetchWithAuth(url, options)
            .then((response) => {
                if (response.ok) {
                    setCurrentContent(currentContent => currentContent.map(element =>
                        element.id === id ? newValue : element));
                    if (newValue.id !== id)
                        elementFetchStates.current.delete(id);
                    elementFetchStates.current.set(newValue.id, { status: FETCH_STATUSES.OK });
                }
                else {
                    response.json().then(json => {
                        const errorMessage = json.error?.message ?? json.errors[Object.keys(json.errors)[0]] ?? REQUEST_FAILED_DEFAULT_MESSAGE;
                        elementFetchStates.current.set(id, { status: FETCH_STATUSES.ERROR, message: errorMessage });
                        forceUpdate();
                    });
                }
            })
            .catch(() => {
                elementFetchStates.current.set(id, { status: FETCH_STATUSES.ERROR, message: FETCH_FAILED_DEFAULT_MESSAGE })
                forceUpdate();
            });
    };

    const fetchDelete = (id) => {
        elementFetchStates.current.set(id, { status: FETCH_STATUSES.PENDING });
        forceUpdate();

        const url = encodeURI(`${urlPathBase}/${id}`);
        const options = {
            method: 'DELETE',
            headers: { "Content-Type": "application/json" }
        };

        //new Promise((resolve, reject) => setTimeout(confirm('Успешный запрос к серверу?') ? resolve : reject, 3000))
        fetchWithAuth(url, options)
            .then((response) => {
                if (response.ok) {
                    setCurrentContent(currentContent => currentContent.filter(element => element.id !== id));
                    elementFetchStates.current.delete(id);
                }
                else {
                    response.json().then(json => {
                        const errorMessage = json.error?.message ?? json.errors[Object.keys(json.errors)[0]] ?? REQUEST_FAILED_DEFAULT_MESSAGE;
                        elementFetchStates.current.set(id, { status: FETCH_STATUSES.ERROR, message: errorMessage });
                        forceUpdate();
                    })
                }
            })
            .catch(() => {
                elementFetchStates.current.set(id, { status: FETCH_STATUSES.ERROR, message: FETCH_FAILED_DEFAULT_MESSAGE });
                forceUpdate();
            });
    };

    return {
        fetchCreate, fetchUpdate, fetchDelete,
        currentContent, setCurrentContent,
        elementFetchStates,
        createdElementFetchState, setCreatedElementFetchState,
        elementCreatorIsActive, setElementCreatorIsActive
    };
}

export default useFetchStoreUpdater;