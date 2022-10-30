import styles from './FetchStateDisplay.module';
import { FETCH_STATUSES } from '../../hooks/useFetchStoreUpdater';

const FetchStateDisplay = ({ fetchStatus, handleClearFetchStatus, children }) => {
    return (
        <div style={fetchStatus === FETCH_STATUSES.PENDING ? { pointerEvents: 'none', opacity: '0.4' } : {}}>
            {children}
            {fetchStatus &&
                <span>
                    <span style={{ marginLeft: 100, color: 'deeppink' }}>
                        {fetchStatus}
                    </span>
                    <span onClick={handleClearFetchStatus} style={{ cursor: 'default', marginLeft: 20, border: '1px #ccc solid' }}>
                        (X)
                    </span>
                </span>
            }
        </div>
    )
}

export default FetchStateDisplay;