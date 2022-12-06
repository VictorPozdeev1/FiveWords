import React from 'react';
import WordTranslation from '../WordTranslation/WordTranslation';
import FetchStateDisplay from '../FetchStateDisplay/FetchStateDisplay';
import { FETCH_STATUSES } from '../../hooks/useFetchStoreUpdater';
import styles from './WordTranslationCreator.module';

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
            {elementCreatorIsActive ||
                <button
                    onClick={() => { setElementCreatorIsActive(true) }}
                    className={styles.addWordButton}
                >
                    Добавить слово...
                </button>
            }
            {elementCreatorIsActive &&
                <fieldset>
                    <legend>Добавление нового слова</legend>
                    <FetchStateDisplay
                        fetchStatus={createdElementFetchStatus}
                        handleClearFetchStatus={handleClearFetchStatus}
                    >
                        <WordTranslation
                            id=''
                            translation=''
                            handleUpdate={handleUpdate}
                            handleDelete={handleDelete}
                        />
                    </FetchStateDisplay>
                </fieldset>
            }
            {/*{isActive && <button onClick={() => { setIsActive(false) }}> Отмена</button>}*/}
        </div>
    )
}

export default WordTranslationCreator;