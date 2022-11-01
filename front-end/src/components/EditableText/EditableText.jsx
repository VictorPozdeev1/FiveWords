import React from 'react';
import styles from './EditableText.module';
import classnames from 'classnames';

const EditableText = ({ initialText, validateAndHandleEditAccept, handleEditCancel }) => {
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
            className={classnames(styles.underEdit, { [styles.validationError]: validationError })}
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
        : <span onClick={startEdit} className={styles.default}>
            {currentText ? currentText : 'Введите текст...'}
        </span>)
};

export default EditableText;