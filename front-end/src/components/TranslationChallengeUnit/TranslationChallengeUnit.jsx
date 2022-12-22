import styles from './TranslationChallengeUnit.module';

const TranslationChallengeUnit = ({ challengeUnit, unitNumber, unitsCount, handleAnswerOptionSelect }) => {
    return (
        <div className={styles.body}>
            <fieldset>
                <legend className={styles.questionNumber}>Вопрос {unitNumber} из {unitsCount}</legend>
                <p className={styles.questionText}>{challengeUnit.question}</p>
                <div className={styles.buttonsContainer}>
                    {challengeUnit.answerOptions.map((answerOption, index) =>
                        <button
                            key={index + answerOption}
                            className={styles.answerOptionButton}
                            onClick={() => { handleAnswerOptionSelect(index) }}
                        >
                            {answerOption}
                        </button>
                    )}
                </div>
            </fieldset>
        </div>
    )
}

export default TranslationChallengeUnit;