import React from 'react';
import useForceUpdate from './useForceUpdate';

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
        new Promise((resolve, reject) => setTimeout(confirm('Успешный запрос к серверу?') ? resolve : reject, 3000))
            .then(() => {
                setCurrentContent(currentContent => [...currentContent, newValue]);
                elementsFetchStatuses.current.set(newValue.id, FETCH_STATUSES.OK);
                setCreatedElementFetchStatus(null);
                setElementCreatorIsActive(false);
            })
            .catch(() => {
                setCreatedElementFetchStatus(FETCH_STATUSES.ERROR);
            });
    };

    const fetchUpdate = (id, newValue) => {
        elementsFetchStatuses.current.set(id, FETCH_STATUSES.PENDING);
        forceUpdate();
        new Promise((resolve, reject) => setTimeout(confirm('Успешный запрос к серверу?') ? resolve : reject, 3000))
            .then(() => {
                setCurrentContent(currentContent => currentContent.map(element =>
                    element.id === id ? newValue : element));
                if (newValue.id !== id)
                    elementsFetchStatuses.current.delete(id);
                elementsFetchStatuses.current.set(newValue.id, FETCH_STATUSES.OK);
            })
            .catch(() => {
                elementsFetchStatuses.current.set(id, FETCH_STATUSES.ERROR)
                forceUpdate();
            });
    };

    const fetchDelete = (id) => {
        elementsFetchStatuses.current.set(id, FETCH_STATUSES.PENDING);
        forceUpdate();
        new Promise((resolve, reject) => setTimeout(confirm('Успешный запрос к серверу?') ? resolve : reject, 3000))
            .then(() => {
                setCurrentContent(currentContent => currentContent.filter(element => element.id !== id));
                elementsFetchStatuses.current.delete(id);
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