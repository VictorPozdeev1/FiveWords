import { useEffect, useState } from 'react';
import styles from './TranslationChallengeUnit.module';

const TranslationChallengeUnit = ({ challengeUnit, unitNumber, unitsCount, handleAnswerOptionSelect }) => {
    const [componentWasShownTime, setComponentWasShownTime] = useState();
    useEffect(() => {
        setComponentWasShownTime(new Date());
    }, [unitNumber]);

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
                            onClick={() => { handleAnswerOptionSelect(index, new Date() - componentWasShownTime) }}
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