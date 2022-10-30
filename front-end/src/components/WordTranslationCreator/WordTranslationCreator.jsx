import React from 'react';
import { WordTranslation } from '../WordTranslation/WordTranslation';

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

export default WordTranslationCreator;