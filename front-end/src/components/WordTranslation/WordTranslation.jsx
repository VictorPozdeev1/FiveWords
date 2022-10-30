import { useCallback, useRef } from 'react';
import { EditableText } from '../EditableText/EditableText.jsx';
import styles from './WordTranslation.module';

export const WordTranslation = ({ id, translation, handleUpdate, handleDelete }) => {

    const lastEditedState = useRef({ id, translation });

    const handleIdEditCancel = useCallback(() => {
        lastEditedState.current.id = id;
    }, [lastEditedState.current, id]);

    const handleTranslationEditCancel = useCallback(() => {
        lastEditedState.current.translation = translation;
    }, [lastEditedState.current, translation]);

    const validateAndHandleIdEditAccept = useCallback(newValue => {
        lastEditedState.current.id = newValue.trim();
        const validateIdError = validateId(lastEditedState.current.id);
        if (validateIdError)
            return validateIdError;
        if (!validateTranslation(lastEditedState.current.translation))
            handleUpdate(Object.assign({}, lastEditedState.current));
        return null;
    }, [lastEditedState.current, lastEditedState.current.id, lastEditedState.current.translation]);

    const validateAndHandleTranslationEditAccept = useCallback(newValue => {
        lastEditedState.current.translation = newValue.trim();
        const validateTranslationError = validateTranslation(lastEditedState.current.translation);
        if (validateTranslationError)
            return validateTranslationError;
        if (!validateId(lastEditedState.current.id))
            handleUpdate(Object.assign({}, lastEditedState.current));
        return null;
    }, [lastEditedState.current, lastEditedState.current.id, lastEditedState.current.translation]);

    const validateId = useCallback(idToValidate => {
        if (!idToValidate)
            return 'Empty id';
        const regex = /^[\wА-Яа-яЁё\-\.\?!\)\(,: ]{1,30}$/;
        if (!regex.test(idToValidate))
            return regex;
        return null;
    }, []);

    const validateTranslation = useCallback(translationToValidate => {
        if (!translationToValidate)
            return 'Empty translation';
        const regex = /^[\wА-Яа-яЁё\-\.\?!\)\(,: ]{1,30}$/;
        if (!regex.test(translationToValidate))
            return regex;
        return null;
    }, []);

    return (
        <span className={styles.WordTranslation}>
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