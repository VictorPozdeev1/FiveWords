import styles from './FetchStateDisplay.module';
import classnames from 'classnames';
import { FETCH_STATUSES } from '../../hooks/useFetchStoreUpdater';

const FetchStateDisplay = ({ fetchStatus, handleClearFetchStatus, children }) => {
    return (
        <span className={classnames({ [styles.pending]: fetchStatus === FETCH_STATUSES.PENDING })}>
            {children}
            {fetchStatus &&
                <span>
                    <span className={styles.statusValue}>
                        {fetchStatus}
                    </span>
                    <span onClick={handleClearFetchStatus} className={styles.clearStatusButton}>
                        (X)
                    </span>
                </span>
            }
        </span>
    )
}

export default FetchStateDisplay;