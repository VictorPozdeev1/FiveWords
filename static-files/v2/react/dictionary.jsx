const EditableText = ({ initialText, onEditAccepted }) => {
    const style = { border: '1px #ccc solid', marginRight: '120px' }
    const [isBeingEdited, setIsBeingEdited] = React.useState(false);
    const [currentText, setCurrentText] = React.useState(initialText);
    const focusAndSelect = React.useCallback(domElement => { domElement?.focus(); domElement?.select(); }, []);
    const startEdit = () => {
        setIsBeingEdited(true)
    }
    const acceptEdit = () => {
        onEditAccepted(currentText);
        setIsBeingEdited(false);
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

const WordTranslation = ({ id, translation }) => {
    const [currentId, setCurrentId] = React.useState(id);
    const [currentTranslation, setCurrentTranslation] = React.useState(translation);
    const idEditAccepted = (newValue) => setCurrentId(newValue);
    const translationEditAccepted = (newValue) => setCurrentTranslation(newValue);
    return (
        <div>
            Слово:
            <EditableText initialText={currentId} onEditAccepted={idEditAccepted} />
            Перевод:
            <EditableText initialText={currentTranslation} onEditAccepted={translationEditAccepted} />
        </div>
    )
}

const WordTranslationsContainer = ({ content }) => {
    return content.length > 0 ?
        <div>
            {content.map(wordTranslation =>
                <WordTranslation {...wordTranslation} key={wordTranslation.id} />
            )}
            {/*<EditableTextContainer initialText="MyFirst" />*/}
        </div>
        : null;
}