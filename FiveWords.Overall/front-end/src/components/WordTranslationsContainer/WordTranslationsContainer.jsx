﻿import React from 'react';
import useForceUpdate from '../../hooks/useForceUpdate';
import useFetchStoreUpdater from '../../hooks/useFetchStoreUpdater';
import { FETCH_STATUSES } from '../../hooks/useFetchStoreUpdater';
import WordTranslation from '../WordTranslation/WordTranslation';
import FetchStateDisplay from '../FetchStateDisplay/FetchStateDisplay';
import WordTranslationCreator from '../WordTranslationCreator/WordTranslationCreator';
import WordTranslationsFile from '../WordTranslationsFile/WordTranslationsFile';
import styles from './WordTranslationsContainer.module';
import classnames from 'classnames';

const WordTranslationsContainer = ({ content, dictionaryName }) => {
    const forceUpdate = useForceUpdate();
    const { currentContent, setCurrentContent, elementFetchStates, fetchUpdate, fetchDelete, ...elementCreatorProps }
        = useFetchStoreUpdater({ initialContent: content, urlPathBase: `/DictionaryContentElements/${dictionaryName}` })

    const handleClearFetchState = (id) => {
        if (elementFetchStates.current.get(id).status !== FETCH_STATUSES.OK)
            alert('Вот тут, наверное, надо добавить сброс на initialState!');
        elementFetchStates.current.delete(id);
        forceUpdate();
    }

    const handleUpdate = React.useCallback((id, newValue) => {
        const currentElementState = currentContent.find(element => element.id === id);
        if (newValue.id === currentElementState.id && newValue.translation === currentElementState.translation) {
            elementFetchStates.current.delete(id);
            forceUpdate();
        }
        else
            fetchUpdate(id, newValue);
    });

    const handleDelete = React.useCallback((id) => fetchDelete(id));

    return (
        <div className={classnames(styles.default)} >
            {currentContent.length > 0 ?
                <React.StrictMode>
                    <fieldset>
                        <legend>Список слов (кликните по слову или переводу для его редактирования)</legend>
                        {currentContent.map(wordTranslation =>
                            <div className={styles.item} key={wordTranslation.id}>
                                <FetchStateDisplay
                                    fetchState={elementFetchStates.current.get(wordTranslation.id)}
                                    handleClearFetchState={() => handleClearFetchState(wordTranslation.id)}
                                >
                                    <WordTranslation
                                        {...wordTranslation}
                                        handleDelete={() => handleDelete(wordTranslation.id)}
                                        handleUpdate={(newValue) => handleUpdate(wordTranslation.id, newValue)}
                                    />
                                </FetchStateDisplay>
                            </div>
                        )}
                    </fieldset>
                </React.StrictMode>
                : <div>Словарь пуст. Добавьте слова вручную или загрузите из файла.</div>
            }
            <div className={styles.creatorSection}>
                <WordTranslationCreator {...elementCreatorProps} />
            </div>
            <div className={styles.translationsFileSection}>
                <WordTranslationsFile
                    addValuesToContent={valuesToAdd => { setCurrentContent(currentContent => [...currentContent, ...valuesToAdd]) }}
                    dictionaryName={dictionaryName}
                />
            </div>
        </div>
    );
}

export default WordTranslationsContainer;
