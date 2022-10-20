function useForceUpdate() {
    const [_, setValue] = React.useState(0);
    return React.useCallback(() => setValue(value => value + 1), []);
}

const EditableText = ({ initialText, handleEditAccept }) => {
    const style = { border: '1px #ccc solid', marginRight: '120px' }
    const [isBeingEdited, setIsBeingEdited] = React.useState(false);
    const [currentText, setCurrentText] = React.useState(initialText);

    const focusAndSelect = React.useCallback(domElement => {
        domElement?.focus();
        domElement?.select();
    }, []);

    const startEdit = () => {
        setIsBeingEdited(true)
    }

    const acceptEdit = e => {
        if (handleEditAccept(currentText))
            setIsBeingEdited(false);
        //else
        //    focusAndSelect(e.target);
    }

    const cancelEdit = () => {
        setCurrentText(initialText);
        setIsBeingEdited(false);
    }

    return (
        isBeingEdited
            ? <input
                //style={style}
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
                {currentText}
            </span>
    )
}

const WordTranslation = ({ id, translation, processingStatus, handleUpdate, handleDelete }) => {
    console.log('render WordTranslation', id);
    const [currentId, setCurrentId] = React.useState(id);
    const [currentTranslation, setCurrentTranslation] = React.useState(translation);
    React.useEffect(() => {
        console.log('created', id, translation);
        return () => { console.log('destroyed', id, translation); }
    }, []);

    const handleIdEditAccept = newValue => {
        newValue = newValue.trim();
        if (newValue.length < 5) //заглушка нормальной проверки
            return false;
        if (newValue === id)
            return true;
        if (handleUpdate({ id: newValue, translation }))
            setCurrentId(newValue);
    }
    const handleTranslationEditAccept = newValue => {
        newValue = newValue.trim();
        if (newValue.length < 5) //заглушка нормальной проверки
            return false;
        if (newValue === translation)
            return true;
        if (handleUpdate({ id, translation: newValue }))
                setCurrentTranslation(newValue);
    }
    return (
        <div>
            Слово:
            <EditableText initialText={currentId} handleEditAccept={handleIdEditAccept} />
            Перевод:
            <EditableText initialText={currentTranslation} handleEditAccept={handleTranslationEditAccept} />
            <button onClick={handleDelete}>Удалить</button>
            {processingStatus && <span style={{ marginLeft: 20 }}>{processingStatus}</span>}
        </div>
    )
}

const FETCH_STATUSES = {
    OK: 'ok!',
    ERROR: 'error :(',
    PENDING: 'pending...'
}

const useFetchStoreUpdater = ({ initialContent, urlPathBase }) => {
    const [currentContent, setCurrentContent] = React.useState(initialContent);
    const elementsProcessingStatuses = React.useRef(new Map()); //Так, или всё же стейтом сделать , чтоб не форсапдейтить?
    const forceUpdate = useForceUpdate();

    const fetchCreate = () => { };

    const fetchUpdate = (id, newValue) => {
        dispatchFetchRequest(
            id,
            () => new Promise((resolve, reject) => setTimeout(confirm('Успешный запрос к серверу?') ? resolve : reject, 3000)),
            () => {
                setCurrentContent(currentContent => currentContent.map(element => element.id === id ? newValue : element));
                if (newValue.id !== id) {
                    console.log('statuses count:', elementsProcessingStatuses.current.size);
                    console.log('old old:', elementsProcessingStatuses.current.get(id));
                    console.log('old new:', elementsProcessingStatuses.current.get(newValue.id));
                    elementsProcessingStatuses.current.delete(id);
                    elementsProcessingStatuses.current.set(newValue.id, FETCH_STATUSES.OK);
                    console.log('new old:', elementsProcessingStatuses.current.get(id));
                    console.log('new new:', elementsProcessingStatuses.current.get(newValue.id));
                }
            }
        )
    };

    const fetchDelete = (id) => dispatchFetchRequest(
        id,
        () => new Promise((resolve, reject) => setTimeout(confirm('Успешный запрос к серверу?') ? resolve : reject, 3000)),
        () => {
            setCurrentContent(currentContent => currentContent.filter(element => element.id !== id));
            elementsProcessingStatuses.current.delete(id);
        }
    );

    const dispatchFetchRequest = (id, fetchCallbackAsync, updateStoreCallback) => {
        elementsProcessingStatuses.current.set(id, FETCH_STATUSES.PENDING);
        forceUpdate();
        fetchCallbackAsync()
            .then(() => {
                elementsProcessingStatuses.current.set(id, FETCH_STATUSES.OK);
                updateStoreCallback();
            })
            .catch(() => {
                elementsProcessingStatuses.current.set(id, FETCH_STATUSES.ERROR)
                forceUpdate();
            });
    };

    return { currentContent, elementsProcessingStatuses, fetchCreate, fetchUpdate, fetchDelete };
}

const WordTranslationsContainer = ({ content }) => {
    console.log('render WordTranslationContainer');
    const { currentContent, elementsProcessingStatuses, fetchCreate, fetchUpdate, fetchDelete }
        = useFetchStoreUpdater({ initialContent: content, urlPathBase: 'hzhzpokakudatut..' })

    const handleUpdate = React.useCallback((id, newValue) => {
        if (false) //написать проверку на дублирование id.
            return false;
        fetchUpdate(id, newValue);
        return true;
    });

    const handleDelete = React.useCallback((id) => fetchDelete(id));

    return currentContent.length > 0 ?
        <div>
            {currentContent.map(wordTranslation =>
                <WordTranslation
                    {...wordTranslation}
                    key={wordTranslation.id}
                    processingStatus={elementsProcessingStatuses.current.get(wordTranslation.id)}
                    handleDelete={() => handleDelete(wordTranslation.id)}
                    handleUpdate={(newValue) => handleUpdate(wordTranslation.id, newValue)}
                />
            )}
            <br />
            <button onClick={() => setCurrentContent(Array.from(currentContent))}>Test</button>
        </div>
        : null;
}