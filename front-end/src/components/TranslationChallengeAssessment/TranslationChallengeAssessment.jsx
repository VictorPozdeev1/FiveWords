import styles from './TranslationChallengeAssessment.module';
import classnames from 'classnames';

const TranslationChallengeAssessment = ({ assessmentText, handleOneMoreTimeButtonClick }) => {
    return (
        <div className={styles.body}>
            <p className={styles.assessmentText}>{assessmentText}</p>
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