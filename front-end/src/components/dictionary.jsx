import styles from './styles.module.css';

function useForceUpdate() {
    const [_, setValue] = React.useState(0);
    return React.useCallback(() => setValue(value => value + 1), []);
}

const EditableText = ({ initialText, validateAndHandleEditAccept, handleEditCancel }) => {
    const style = { border: '1px #ccc solid', marginRight: '120px', fontSize: '18px' }
    const [isBeingEdited, setIsBeingEdited] = React.useState(false);
    const [currentText, setCurrentText] = React.useState(initialText ?? '');
    const [validationError, setValidationError] = React.useState(null);

    React.useEffect(() => setCurrentText(initialText), [initialText]);

    const focusAndSelect = React.useCallback(domElement => {
        domElement?.focus();
        domElement?.select();
    }, []);

    const startEdit = () => {
        setIsBeingEdited(true)
        setValidationError(null);
    }

    const acceptEdit = () => {
        const currentError = validateAndHandleEditAccept(currentText);
        setValidationError(currentError);
        //if (currentError) focusAndSelect(e.target);
        if (!currentError)
            setIsBeingEdited(false);
    }

    const cancelEdit = () => {
        setCurrentText(initialText);
        setIsBeingEdited(false);
        setValidationError(null);
        handleEditCancel();
    }

    return (isBeingEdited
        ? <input
            style={validationError ? { border: '2px solid red' } : {}}
            value={currentText}
            ref={focusAndSelect}
            onChange={e => { setCurrentText(e.target.value); }}
            onBlur={acceptEdit}
            onKeyDown={e => {
                if (e.key === "Escape")
                    cancelEdit();
                if (e.key === 'Enter')
                    acceptEdit();
            }}
        />
        : <span onClick={startEdit} style={style}>
            {currentText ? currentText : 'Введите текст...'}
        </span>)
}

const FetchStateDisplay = ({ fetchStatus, handleClearFetchStatus, children }) => {
    return (
        <div style={fetchStatus === FETCH_STATUSES.PENDING ? { pointerEvents: 'none', opacity: '0.4' } : {}}>
            {children}
            {fetchStatus &&
                <span>
                    <span style={{ marginLeft: 100, color: 'deeppink' }}>
                        {fetchStatus}
                    </span>
                    <span onClick={handleClearFetchStatus} style={{ cursor: 'default', marginLeft: 20, border: '1px #ccc solid' }}>
                        (X)
                    </span>
                </span>
            }
        </div>
    )
}

const WordTranslation = ({ id, translation, handleUpdate, handleDelete }) => {
    console.log('render WordTranslation', id);

    React.useEffect(() => {
        console.log('created', id, translation);
        return () => { console.log('destroyed', id, translation); }
    }, []);

    const lastEditedState = React.useRef({ id, translation });

    const handleIdEditCancel = React.useCallback(() => {
        lastEditedState.current.id = id;
    }, [lastEditedState.current, id]);

    const handleTranslationEditCancel = React.useCallback(() => {
        lastEditedState.current.translation = translation;
    }, [lastEditedState.current, translation]);

    const validateAndHandleIdEditAccept = React.useCallback(newValue => {
        lastEditedState.current.id = newValue.trim();
        const validateIdError = validateId(lastEditedState.current.id);
        if (validateIdError)
            return validateIdError;
        if (!validateTranslation(lastEditedState.current.translation))
            handleUpdate(Object.assign({}, lastEditedState.current));
        return null;
    }, [lastEditedState.current, lastEditedState.current.id, lastEditedState.current.translation]);

    const validateAndHandleTranslationEditAccept = React.useCallback(newValue => {
        lastEditedState.current.translation = newValue.trim();
        const validateTranslationError = validateTranslation(lastEditedState.current.translation);
        if (validateTranslationError)
            return validateTranslationError;
        if (!validateId(lastEditedState.current.id))
            handleUpdate(Object.assign({}, lastEditedState.current));
        return null;
    }, [lastEditedState.current, lastEditedState.current.id, lastEditedState.current.translation]);

    const validateId = React.useCallback(idToValidate => {
        if (!idToValidate)
            return 'Empty id';
        const regex = /^[\wА-Яа-яЁё\-\.\?!\)\(,: ]{1,30}$/;
        if (!regex.test(idToValidate))
            return regex;
        return null;
    }, []);

    const validateTranslation = React.useCallback(translationToValidate => {
        if (!translationToValidate)
            return 'Empty translation';
        const regex = /^[\wА-Яа-яЁё\-\.\?!\)\(,: ]{1,30}$/;
        if (!regex.test(translationToValidate))
            return regex;
        return null;
    }, []);

    return (
        <span style={{ marginTop: '10px', display: 'inline-block' }}>
            Слово:
            <EditableText
                initialText={id}
                validateAndHandleEditAccept={validateAndHandleIdEditAccept}
                handleEditCancel={handleIdEditCancel}
            />
            Перевод:
            <EditableText
                initialText={translation}
                validateAndHandleEditAccept={validateAndHandleTranslationEditAccept}
                handleEditCancel={handleTranslationEditCancel}
            />
            <button
                onClick={handleDelete}>
                Удалить
            </button>
        </span>
    )
}

const FETCH_STATUSES = {
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

export default WordTranslationsContainer = ({ content }) => {
    console.log('render WordTranslationContainer');
    const forceUpdate = useForceUpdate();
    const { currentContent, elementsFetchStatuses, fetchUpdate, fetchDelete, ...elementCreatorProps }
        = useFetchStoreUpdater({ initialContent: content, urlPathBase: 'hzhzpokakudatut..' })

    const handleClearFetchStatus = (id) => {
        if (elementsFetchStatuses.current.get(id) === FETCH_STATUSES.ERROR)
            alert('Вот тут, наверное, надо добавить сброс на initialState!');
        elementsFetchStatuses.current.delete(id);
        forceUpdate();
    }

    const handleUpdate = React.useCallback((id, newValue) => {
        const currentElementState = currentContent.find(element => element.id === id);
        if (newValue.id === currentElementState.id && newValue.translation === currentElementState.translation) {
            elementsFetchStatuses.current.delete(id);
            forceUpdate();
        }
        else
            fetchUpdate(id, newValue);
    });

    const handleDelete = React.useCallback((id) => fetchDelete(id));

    return currentContent.length > 0 ?
        <div>
            <React.StrictMode>
                {currentContent.map(wordTranslation =>
                    <FetchStateDisplay
                        key={wordTranslation.id}
                        fetchStatus={elementsFetchStatuses.current.get(wordTranslation.id)}
                        handleClearFetchStatus={() => handleClearFetchStatus(wordTranslation.id)}
                    >
                        <WordTranslation
                            {...wordTranslation}
                            handleDelete={() => handleDelete(wordTranslation.id)}
                            handleUpdate={(newValue) => handleUpdate(wordTranslation.id, newValue)}
                        />
                    </FetchStateDisplay>
                )}
                <hr></hr>
                <WordTranslationCreator {...elementCreatorProps} />
            </React.StrictMode>
        </div>
        : null;
}

const WordTranslationCreator = ({ fetchCreate, elementCreatorIsActive, setElementCreatorIsActive, createdElementFetchStatus, setCreatedElementFetchStatus }) => {
    const handleUpdate = React.useCallback((newValue) => {
        if (newValue.id && newValue.translation) {
            fetchCreate(newValue);
        }
    }, []);

    const handleDelete = React.useCallback(() => {
        setCreatedElementFetchStatus(null);
        setElementCreatorIsActive(false)
    }, []);


    const handleClearFetchStatus = React.useCallback(() => {
        if (createdElementFetchStatus === FETCH_STATUSES.ERROR)
            alert('Вот тут, наверное, надо скрыть этот компонент?!');
        setCreatedElementFetchStatus(null);
    }, []);

    return (
        <div>
            {elementCreatorIsActive || <button onClick={() => { setElementCreatorIsActive(true) }}> Добавить слово...</button >}
            {elementCreatorIsActive &&
                <FetchStateDisplay
                    fetchStatus={createdElementFetchStatus}
                    handleClearFetchStatus={handleClearFetchStatus}
                >
                    <WordTranslation
                        handleUpdate={handleUpdate}
                        handleDelete={handleDelete}
                    />
                </FetchStateDisplay>
            }
            {/*{isActive && <button onClick={() => { setIsActive(false) }}> Отмена</button>}*/}
        </div>
    )
}