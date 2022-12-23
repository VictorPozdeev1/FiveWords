import { useEffect, useState, useRef, useCallback } from 'react';
import TranslationChallenge from '../TranslationChallenge/TranslationChallenge';
import DivInsideLink_ButtonByArtem from '../DivInsideLink_ButtonByArtem/DivInsideLink_ButtonByArtem';
import styles from './TranslationChallengePage.module';
import classnames from 'classnames';

const TranslationChallengePage = ({ dictionaryName }) => {

    const handleClick = useCallback(() => history.back(), []);
    return (
        <div className={classnames(styles.body, 'main-block')}>
            <DivInsideLink_ButtonByArtem
                label='Назад на главную'
                width='160px'
                handleClick={handleClick}
                linkClassNames='container link__button-back'
                innerDivClassNames='main-block__button button-back button__blue'
            />
            <div className={classnames(styles.mainBlockContainer, 'container', 'dark-plate')}>
                <h1 className={classnames('title', 'dictionaries-block__title')}>
                    {dictionaryName ? `Тренируем словарь "${dictionaryName}"` : 'Пробное упражнение'}
                </h1>
                <TranslationChallenge dictionaryName={dictionaryName} />
            </div>
        </div>
    );
}

export default TranslationChallengePage;