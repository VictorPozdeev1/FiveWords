import styles from './FetchStateDisplay.module';
import classnames from 'classnames';
import { FETCH_STATUSES } from '../../hooks/useFetchStoreUpdater';

const FetchStateDisplay = ({ fetchState, handleClearFetchState, children }) => {
    const status = fetchState?.status;
    let message = fetchState?.message;

    //Возможно, это в итоге будет не нужно, т.к. никакие состояние, кроме состояния ошибки, не будут отображать сообщение.
    if (!message) {
        if (status === FETCH_STATUSES.PENDING)
            message = "Ожидание ответа сервера...";
        if (status === FETCH_STATUSES.OK)
            message = "Сохранено."
    }

    const statusValueClassName = classnames(
        styles.anyStatusValue,
        {
            [styles.okStatusValue]: status === FETCH_STATUSES.OK,
            [styles.errorStatusValue]: status === FETCH_STATUSES.ERROR,
            [styles.pendingStatusValue]: status === FETCH_STATUSES.PENDING,
        }
    );

    return (
        <div className={classnames({ [styles.pending]: status === FETCH_STATUSES.PENDING })}>
            {children}
            {message &&
                /*<div className={styles.statusDiv}>
                    <div>
                        <div onClick={handleClearFetchState} className={styles.clearStatusButton}>
                            X
                        </div>
                    </div>
                    <div className={statusValueClassName}>
                        {message}
                    </div>
                </div >*/
                <div className={statusValueClassName}>
                    {message}
                </div>
            }
        </div >
    )
}

export default FetchStateDisplay;