import { useEffect, useState, useRef, useCallback } from 'react';
import { fetchWithAuth, removeToken, isAuthenticated } from "../../modules/Auth";
import classnames from 'classnames';

import TranslationChallengeUnit from '../TranslationChallengeUnit/TranslationChallengeUnit';
import TranslationChallengeAssessment from '../TranslationChallengeAssessment/TranslationChallengeAssessment';
import styles from './TranslationChallenge.module';
import * as React from 'react';

const TranslationChallenge = ({ dictionaryName }) => {
    const [currentUnitNumber, setCurrentUnitNumber] = useState(null);
    const [needsAssessment, setNeedsAssessment] = useState(false);
    const [challenge, setChallenge] = useState(null);
    const [challengeFetchError, setChallengeFetchError] = useState(null);
    const userAnswers = useRef([]);

    useEffect(() => {
        async function fetchChallenge() {
            const unitsCount = 5;
            const answerOptionsCount = 4;

            const url = dictionaryName ?
                new URL(`WordTranslationsChallenge/${unitsCount}:${answerOptionsCount}/${dictionaryName}`, location.origin) :
                new URL(`WordTranslationsChallenge/${unitsCount}:${answerOptionsCount}`, location.origin);

            const response = await (isAuthenticated() ? fetchWithAuth : fetch)(url);
            if (response.ok) {
                const responseJson = await response.json();
                setChallengeFetchError(null);
                setChallenge(responseJson);
                setCurrentUnitNumber(0);
            }
            else {
                try {
                    setChallenge(null);
                    const responseJson = await response.json();
                    setChallengeFetchError(responseJson.error.message);
                }
                catch {
                    alert("Что-то пошло не так.. если бы мы знали что это такое, мы не знаем что это такое!");
                }
            }
        };
        if (!challenge) {
            fetchChallenge();
        }
    }, [challenge, setChallenge]);

    const handleAnswerOptionSelect = useCallback((selectedAnswerOptionIndex, answerTimeInMilliseconds) => {
        userAnswers.current.push({ selectedAnswerOptionIndex, answerTimeInMilliseconds });
        if (currentUnitNumber < challenge.units.length - 1) {
            setCurrentUnitNumber(currentUnitNumber => currentUnitNumber + 1);
        }
        else {
            setNeedsAssessment(true);
        }
    }, [userAnswers.current, currentUnitNumber, challenge]);

    const handleOneMoreTimeButtonClick = useCallback(() => {
        setChallenge(null);
        userAnswers.current = [];
        setNeedsAssessment(false);
    }, []);

    const goRegister = useCallback(() => { location.assign(new URL('?register=true', location.origin)) }, []);

    const getComponentBody = useCallback(() => {
        if (!challenge) return (
            <div style={{ 'color': 'orange' }}>
                {challengeFetchError ?? 'Загрузка...'}
            </div>
        );
        if (!needsAssessment) return (
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
                    challenge={challenge}
                    userAnswers={userAnswers.current}
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

    }, [challenge, needsAssessment, currentUnitNumber, challengeFetchError]);

    return (
        <div className={styles.body}>
            {getComponentBody()}
        </div >)
}

export default TranslationChallenge;