import styles from './TranslationChallengeAssessment.module';
import classnames from 'classnames';

const TranslationChallengeAssessment = ({ assessment, handleOneMoreTimeButtonClick }) => {
    return (
        <div className={styles.body}>
            <p className={styles.assessmentText}>{assessment}</p>
            <button
                className={classnames(styles.oneMoreTimeButton, 'button__blue')}
                onClick={handleOneMoreTimeButtonClick}
            >
                Ещё разок?
            </button>
        </div>
    )
};

export default TranslationChallengeAssessment;