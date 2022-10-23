function useForceUpdate() {
    const [_, setValue] = React.useState(0);
    return React.useCallback(() => setValue(value => value + 1), []);
}

const EditableText = ({ initialText, validate, handleEditAccept }) => {
    const style = { border: '1px #ccc solid', marginRight: '120px', fontSize: '18px' }
    const [isBeingEdited, setIsBeingEdited] = React.useState(false);
    const [currentText, setCurrentText] = React.useState(initialText ?? '');
    const [validationError, setValidationError] = React.useState(null);

    const focusAndSelect = React.useCallback(domElement => {
        domElement?.focus();
        domElement?.select();
    }, []);

    const startEdit = () => {
        setIsBeingEdited(true)
        setValidationError(null);
    }

    const acceptEdit = e => {
        const currentError = validate(currentText);
        setValidationError(currentError);
        //if (currentError) focusAndSelect(e.target);
        if (!currentError)
            if (handleEditAccept(currentText))
                setIsBeingEdited(false);
    }

    const cancelEdit = () => {
        setCurrentText(initialText);
        setIsBeingEdited(false);
        setValidationError(null);
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

    const handleIdEditAccept = newValue => {
        lastEditedState.current.id = newValue.trim();
        return handleUpdate(Object.assign({}, lastEditedState.current));
    }
    const handleTranslationEditAccept = newValue => {
        lastEditedState.current.translation = newValue.trim();
        return handleUpdate(Object.assign({}, lastEditedState.current));
    }

    const validateId = (id) => {
        if (!id) return 'Empty id';
        const regex = /^[\wА-Яа-яЁё\-\.\?!\)\(,: ]{1,30}$/;
        if (regex.test(id.trim()))
            return null;
        return regex;
    }

    const validateTranslation = (translation) => {
        if (!translation) return 'Empty translation';
        const regex = /^[\wА-Яа-яЁё\-\.\?!\)\(,: ]{1,30}$/;
        if (regex.test(translation.trim()))
            return null;
        return regex;
    }

    return (
        <span style={{ marginTop: '10px', display: 'inline-block' }}>
            Слово:
            <EditableText
                initialText={id}
                validate={validateId}
                handleEditAccept={handleIdEditAccept}
            />
            Перевод:
            <EditableText
                initialText={translation}
                validate={validateTranslation}
                handleEditAccept={handleTranslationEditAccept}
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
    const [currentContent, setCurrentContent] = React.useState(initialContent);
    const elementsFetchStatuses = React.useRef(new Map()); //Так, или всё же стейтом сделать , чтоб не форсапдейтить?
    const [addedElementFetchStatus, setAddedElementFetchStatus] = React.useState(null);
    const forceUpdate = useForceUpdate();

    const fetchCreate = (newValue) => {
        setAddedElementFetchStatus(FETCH_STATUSES.PENDING);
        new Promise((resolve, reject) => setTimeout(confirm('Успешный запрос к серверу?') ? resolve : reject, 3000))
            .then(() => {
                currentContent.push(newValue);
                setCurrentContent(currentContent => currentContent.slice(0));
                setAddedElementFetchStatus(null);
                elementsFetchStatuses.current.set(newValue.id, FETCH_STATUSES.OK);
            })
            .catch(() => {
                setAddedElementFetchStatus(FETCH_STATUSES.ERROR);
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

    return { currentContent, elementsFetchStatuses, fetchCreate, fetchUpdate, fetchDelete, addedElementFetchStatus, setAddedElementFetchStatus, fetchCreate };
}

const WordTranslationsContainer = ({ content }) => {
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
            return true;
        }
        fetchUpdate(id, newValue);
        return true;
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

const WordTranslationCreator = ({ fetchCreate, addedElementFetchStatus, setAddedElementFetchStatus }) => {
    const [isActive, setIsActive] = React.useState(false);

    const handleUpdate = (newValue) => {
        if (newValue.id && newValue.translation)
            fetchCreate(newValue);
        return true;
    };

    const handleDelete = () => {
        setIsActive(false);
        return true;
    };

    const handleClearFetchStatus = () => {
        if (addedElementFetchStatus === FETCH_STATUSES.ERROR)
            alert('Вот тут, наверное, надо скрыть этот компонент?!');
        setAddedElementFetchStatus(null);
    }

    return (
        <div>
            {isActive || <button onClick={() => { setIsActive(true) }}> Добавить слово...</button >}
            {isActive &&
                <FetchStateDisplay
                    fetchStatus={addedElementFetchStatus}
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