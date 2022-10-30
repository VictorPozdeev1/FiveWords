import React from 'react';
import useForceUpdate from '../../hooks/useForceUpdate';
import useFetchStoreUpdater from '../../hooks/useFetchStoreUpdater';
import { WordTranslation } from '../WordTranslation/WordTranslation';
import FetchStateDisplay from '../FetchStateDisplay/FetchStateDisplay';
import WordTranslationCreator from '../WordTranslationCreator/WordTranslationCreator';

const WordTranslationsContainer = ({ content }) => {
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

export default WordTranslationsContainer;