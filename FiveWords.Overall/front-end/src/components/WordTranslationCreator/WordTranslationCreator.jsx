import React from 'react';
import WordTranslation from '../WordTranslation/WordTranslation';
import FetchStateDisplay from '../FetchStateDisplay/FetchStateDisplay';
import { FETCH_STATUSES } from '../../hooks/useFetchStoreUpdater';
import styles from './WordTranslationCreator.module';

const WordTranslationCreator = ({ fetchCreate, elementCreatorIsActive, setElementCreatorIsActive, createdElementFetchState, setCreatedElementFetchState }) => {
    const handleUpdate = React.useCallback((newValue) => {
        if (newValue.id && newValue.translation) {
            fetchCreate(newValue);
        }
    }, []);

    const handleDelete = React.useCallback(() => {
        setCreatedElementFetchState(null);
        setElementCreatorIsActive(false)
    }, []);


    const handleClearFetchState = React.useCallback(() => {
        if (createdElementFetchState.status !== FETCH_STATUSES.OK)
            alert('Вот тут, наверное, надо скрыть этот компонент?!');
        setCreatedElementFetchState(null);
    }, []);

    return (
        <div className={styles.default}>
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
                        fetchState={createdElementFetchState}
                        handleClearFetchState={handleClearFetchState}
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