import { useEffect, useState, useRef } from 'react';
import { fetchWithAuth, removeToken, isAuthenticated } from "../../modules/Auth";

import TranslationChallengeUnit from '../TranslationChallengeUnit/TranslationChallengeUnit';
import TranslationChallengeAssessment from '../TranslationChallengeAssessment/TranslationChallengeAssessment';
import styles from './TranslationChallenge.module';

const TranslationChallenge = ({ dictionaryName }) => {
    const [currentUnitNumber, setCurrentUnitNumber] = useState(null);
    const [assessment, setAssessment] = useState(null);
    const [challenge, setChallenge] = useState(null);

    useEffect(() => {
        async function fetchChallenge() {
            const unitsCount = 5;
            const answerOptionsCount = 5;

            const url = isAuthenticated() ?
                new URL(`WordTranslationsChallenge/${unitsCount}:${answerOptionsCount}/${dictionaryName}`, location.origin) :
                new URL(`WordTranslationsChallenge/${unitsCount}:${answerOptionsCount}`, location.origin);

            console.log('url:', url);

            const response = await (isAuthenticated() ? fetchWithAuth : fetch)(url);
            if (response.ok) {
                debugger;
                const responseJson = await response.json();
                setChallenge(responseJson);
                setCurrentUnitNumber(0);
            }
            else {
                debugger;
                alert("Не удалось загрузить ваши словари. Вы будете перенаправлены на гостевую страницу.");
            }
        };
        debugger;
        if (!challenge) {
            fetchChallenge();
        }
    }, [challenge, setChallenge]);

    function handleAnswerOptionSelect(answerOptionIndex) {
        setAnswer(answerOptionIndex);
        if (currentUnitNumber < challenge.units.length - 1) {
            setCurrentUnitNumber(questionNumber => questionNumber + 1);
        }
        else {
            setAssessment(getAssessment(challenge));
        }
    }

    function handleOneMoreTimeButtonClick() {
        setChallenge(null);
        setAssessment(null);
    }

    function getAssessment(challenge) {
        const rightAnswers = 'N';
        return `Правильных ответов: ${rightAnswers} из ${challenge.units.length}.`;
    }

    function setAnswer() {
    }

    function getComponentBody() { }

    return (
        <div className={styles.body}>
            {challenge ?
                (
                    assessment ?
                        <TranslationChallengeAssessment assessment={assessment} handleOneMoreTimeButtonClick={handleOneMoreTimeButtonClick} /> :
                        //<p>{challenge.units.map(u => u.question).join(', ')}</p>
                        <TranslationChallengeUnit
                            challengeUnit={challenge.units[currentUnitNumber]}
                            unitNumber={currentUnitNumber + 1}
                            unitsCount={challenge.units.length}
                            handleAnswerOptionSelect={handleAnswerOptionSelect}
                        />
                ) :
                'Загрузка...'
            }
        </div >)
}

export default TranslationChallenge;