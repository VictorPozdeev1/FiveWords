function WordTranslation(props) {
    const style = { border: '1px #ccc solid', margin: '15px' }
    return (
        <div>
            Слово:
            <span style={style}>{props.id}</span>
            Перевод:
            <span style={style}>{props.translation}</span>
        </div>
    )
}

function WordTranslationsContainer(props) {
    return (
        <div>
            {props.content.length > 0 && props.content.map(wordTranslation => WordTranslation(wordTranslation))}
        </div>
    )
}