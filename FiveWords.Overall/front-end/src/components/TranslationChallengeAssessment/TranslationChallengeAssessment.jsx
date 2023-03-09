import styles from './TranslationChallengeAssessment.module';
import classnames from 'classnames';
import { useCallback } from 'react';

const TranslationChallengeAssessment = ({ challenge, userAnswers, handleOneMoreTimeButtonClick }) => {

    //Тут мемо можно
    const createAssessment = useCallback(() => {
        const mistakes = [];
        let rightAnswers = 0;
        challenge.units.forEach(
            (element, index) => {
                if (element.rightOptionIndex === userAnswers[index].selectedAnswerOptionIndex) rightAnswers++; else {
                    mistakes.push({
                        question: element.question,
                        wrongAnswer: element.answerOptions[userAnswers[index].selectedAnswerOptionIndex],
                        rightAnswer: element.answerOptions[element.rightOptionIndex]
                    });
                }
            });
        return { text: `Правильных ответов: ${rightAnswers} из ${challenge.units.length}.`, mistakes };
    }, [challenge, userAnswers]);

    const assessment = createAssessment();

    return (
        <div className={styles.body}>
            <p className={styles.assessmentText}>{assessment.text}</p>
            {assessment.mistakes && assessment.mistakes.length > 0 &&
                < div style={{ fontSize: '1.2em' }}>
                    <div>Ошибки:</div>
                    {assessment.mistakes.map(mistake =>
                        <div key={mistake.question}>
                            <span>{mistake.question}</span>
                            <span> – </span>
                            <span style={{ color: 'red', textDecorationLine: 'line-through' }}>{mistake.wrongAnswer}</span>
                            <span> </span>
                            <span style={{ color: 'green' }}> {mistake.rightAnswer}</span>
                        </div>
                    )}
                </div>
            }
            <div className={styles.whatsNextButtonsContainer}>
                <button
                    className={classnames(styles.oneMoreTimeButton, 'button__blue', 'main-block__button')}
                    onClick={handleOneMoreTimeButtonClick}
                >
                    Ещё разок?
                </button>
            </div>
        </div>
    )
};

export default TranslationChallengeAssessment;