import React from 'react';
import useForceUpdate from './useForceUpdate';
import { fetchWithAuth } from '../modules/Auth.js';

export const FETCH_STATUSES = {
    OK: 'ok!',
    ERROR: 'error :(',
    PENDING: 'pending...'
}

const useFetchStoreUpdater = ({ initialContent, urlPathBase }) => {
    const [currentContent, setCurrentContent] = React.useState(initialContent.slice(0));
    const elementsFetchStatuses = React.useRef(new Map()); //Так, или всё же стейтом сделать , чтоб не форсапдейтить?
    const [createdElementFetchStatus, setCreatedElementFetchStatus] = React.useState(null);
    const [elementCreatorIsActive, setElementCreatorIsActive] = React.useState(false);
    const forceUpdate = useForceUpdate();

    const fetchCreate = (newValue) => {
        setCreatedElementFetchStatus(FETCH_STATUSES.PENDING);
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
                    elementsFetchStatuses.current.set(newValue.id, FETCH_STATUSES.OK);
                    setCreatedElementFetchStatus(null);
                    setElementCreatorIsActive(false);
                }
                else {
                    response.json().then(json => {
                        const errorMessage = json.error?.message ?? json.errors[Object.keys(json.errors)[0]] ?? FETCH_STATUSES.ERROR;
                        setCreatedElementFetchStatus(errorMessage);
                        forceUpdate();
                    })
                }
            })
            .catch(() => {
                setCreatedElementFetchStatus(FETCH_STATUSES.ERROR);
            });
    };

    const fetchUpdate = (id, newValue) => {
        elementsFetchStatuses.current.set(id, FETCH_STATUSES.PENDING);
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
                        elementsFetchStatuses.current.delete(id);
                    elementsFetchStatuses.current.set(newValue.id, FETCH_STATUSES.OK);
                }
                else {
                    response.json().then(json => {
                        const errorMessage = json.error?.message ?? json.errors[Object.keys(json.errors)[0]] ?? FETCH_STATUSES.ERROR;
                        elementsFetchStatuses.current.set(id, errorMessage);
                        forceUpdate();
                    })
                }
            })
            .catch(() => {
                elementsFetchStatuses.current.set(id, FETCH_STATUSES.ERROR)
                forceUpdate();
            });
    };

    const fetchDelete = (id) => {
        elementsFetchStatuses.current.set(id, FETCH_STATUSES.PENDING);
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
                    elementsFetchStatuses.current.delete(id);
                }
                else {
                    response.json().then(json => {
                        const errorMessage = json.error?.message ?? json.errors[Object.keys(json.errors)[0]] ?? FETCH_STATUSES.ERROR;
                        elementsFetchStatuses.current.set(id, errorMessage)
                        forceUpdate();
                    })
                }
            })
            .catch(() => {
                elementsFetchStatuses.current.set(id, FETCH_STATUSES.ERROR)
                forceUpdate();
            });
    };

    return {
        fetchCreate, fetchUpdate, fetchDelete,
        currentContent,
        elementsFetchStatuses,
        createdElementFetchStatus, setCreatedElementFetchStatus,
        elementCreatorIsActive, setElementCreatorIsActive
    };
}

export default useFetchStoreUpdater;