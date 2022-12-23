import { useEffect, useState, useRef, useCallback } from 'react';
import { fetchWithAuth, removeToken, isAuthenticated } from "../../modules/Auth";
import classnames from 'classnames';

import TranslationChallengeUnit from '../TranslationChallengeUnit/TranslationChallengeUnit';
import TranslationChallengeAssessment from '../TranslationChallengeAssessment/TranslationChallengeAssessment';
import styles from './TranslationChallenge.module';
import * as React from 'react';

const TranslationChallenge = ({ dictionaryName }) => {
    const [currentUnitNumber, setCurrentUnitNumber] = useState(null);
    const [assessmentText, setAssessmentText] = useState(null);
    const [challenge, setChallenge] = useState(null);
    const selectedAnswerOptionIndices = useRef([]);

    useEffect(() => {
        async function fetchChallenge() {
            const unitsCount = 5;
            const answerOptionsCount = 4;

            const url = dictionaryName ?
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
                alert("Что-то пошло не так.. если бы мы знали что это такое, мы не знаем что это такое!");
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
            setAssessmentText(createAssessmentText(challenge, selectedAnswerOptionIndices.current));
        }
    }

    function handleOneMoreTimeButtonClick() {
        setChallenge(null);
        selectedAnswerOptionIndices.current = [];
        setAssessmentText(null);
    }

    function createAssessmentText(challenge, selectedAnswerOptionIndices) {
        const rightAnswers = challenge.units.reduce(
            (aggregate, current, index) => aggregate + (current.rightOptionIndex === selectedAnswerOptionIndices[index] ? 1 : 0), 0
        );
        return `Правильных ответов: ${rightAnswers} из ${challenge.units.length}.`;
    }

    const goRegister = useCallback(() => { location.assign(new URL('?register=true', location.origin)) }, []);

    const getComponentBody = useCallback(() => {
        if (!challenge) return 'Загрузка...';
        if (!assessmentText) return (
            <TranslationChallengeUnit
                challengeUnit={challenge.units[currentUnitNumber]}
                unitNumber={currentUnitNumber + 1}
                unitsCount={challenge.units.length}
                handleAnswerOptionSelect={handleAnswerOptionSelect}
            />
        );
        return (
            <React.Fragment>
                <TranslationChallengeAssessment
                    assessmentText={assessmentText}
                    handleOneMoreTimeButtonClick={handleOneMoreTimeButtonClick}
                />
                {!isAuthenticated() &&
                    <div className={styles.registerSection}>
                        <div>А ещё можно...</div>
                        <button
                            className={classnames(styles.registerButton, 'button__green')}
                            onClick={goRegister}
                        >
                            Зарегистрироваться!
                        </button>
                        <div>(Это даст возможность создавать собственные словари для тренировок. А может быть, и что-то ещё...)</div>
                    </div>
                }
            </React.Fragment>
        );

    }, [challenge, assessmentText, currentUnitNumber]);

    return (
        <div className={styles.body}>
            {getComponentBody()}
        </div >)
}

export default TranslationChallenge;