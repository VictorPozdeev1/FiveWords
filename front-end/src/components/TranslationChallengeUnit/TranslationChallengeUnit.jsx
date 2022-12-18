import styles from './TranslationChallengeUnit.module';
import classnames from 'classnames';

const TranslationChallengeUnit = ({ challengeUnit, unitNumber, unitsCount, handleAnswerOptionSelect }) => {
    return (
        <div className={styles.body}>
            <p className={styles.questionNumber}>Вопрос {unitNumber} из {unitsCount}:</p>
            <p className={styles.questionText}>{challengeUnit.question}</p>
            <div className={styles.buttonsContainer}>
                {challengeUnit.answerOptions.map((answerOption, index) =>
                    <button
                        key={index + answerOption}
                        className={classnames(styles.answerOptionButton, 'button__blue')}
                        onClick={() => { handleAnswerOptionSelect(index) }}
                    >
                        {answerOption}
                    </button>
                )}
            </div>
        </div>
    )
}

export default TranslationChallengeUnit;