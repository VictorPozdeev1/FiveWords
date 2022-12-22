import { useEffect, useState, useRef, useCallback } from 'react';
import { fetchWithAuth, removeToken, isAuthenticated } from "../../modules/Auth";

import TranslationChallengeUnit from '../TranslationChallengeUnit/TranslationChallengeUnit';
import TranslationChallengeAssessment from '../TranslationChallengeAssessment/TranslationChallengeAssessment';
import styles from './TranslationChallenge.module';

const TranslationChallenge = ({ dictionaryName }) => {
    const [currentUnitNumber, setCurrentUnitNumber] = useState(null);
    const [assessment, setAssessment] = useState(null);
    const [challenge, setChallenge] = useState(null);
    const selectedAnswerOptionIndices = useRef([]);

    useEffect(() => {
        async function fetchChallenge() {
            const unitsCount = 5;
            const answerOptionsCount = 4;

            const url = isAuthenticated() ?
                new URL(`WordTranslationsChallenge/${unitsCount}:${answerOptionsCount}/${dictionaryName}`, location.origin) :
                new URL(`WordTranslationsChallenge/${unitsCount}:${answerOptionsCount}`, location.origin);

            console.log('url:', url);

            const response = await (isAuthenticated() ? fetchWithAuth : fetch)(url);
            if (response.ok) {
                const responseJson = await response.json();
                setChallenge(responseJson);
                setCurrentUnitNumber(0);
            }
            else {
                alert("Не удалось загрузить ваши словари. Вы будете перенаправлены на гостевую страницу.");
            }
        };
        if (!challenge) {
            fetchChallenge();
        }
    }, [challenge, setChallenge]);

    function handleAnswerOptionSelect(selectedAnswerOptionIndex) {
        selectedAnswerOptionIndices.current.push(selectedAnswerOptionIndex);
        if (currentUnitNumber < challenge.units.length - 1) {
            setCurrentUnitNumber(currentUnitNumber => currentUnitNumber + 1);
        }
        else {
            setAssessment(getAssessment(challenge, selectedAnswerOptionIndices.current));
        }
    }

    function handleOneMoreTimeButtonClick() {
        setChallenge(null);
        selectedAnswerOptionIndices.current = [];
        setAssessment(null);
    }

    function getAssessment(challenge, selectedAnswerOptionIndices) {
        const rightAnswers = challenge.units.reduce(
            (aggregate, current, index) => aggregate + (current.rightOptionIndex === selectedAnswerOptionIndices[index] ? 1 : 0), 0
        );
        return `Правильных ответов: ${rightAnswers} из ${challenge.units.length}.`;
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