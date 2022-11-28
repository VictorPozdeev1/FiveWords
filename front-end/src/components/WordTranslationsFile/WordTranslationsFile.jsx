import { useCallback } from 'react';
import styles from './WordTranslationsFile.module';

const WordTranslationsFile = () => {
    const saveSampleFile = useCallback(() => {
        const link = document.createElement('a');
        link.setAttribute('href', '/sample-dictionary/words.csv');
        link.setAttribute('download', 'sample-dictionary.csv');
        link.click();
    }, []);

    return (
        <div>
            <button
                //onClick={() => { setElementCreatorIsActive(true) }}
                className={styles.addFromFileButton}
            >
                Добавить слова из файла...
            </button>
            <button
                onClick={saveSampleFile}
                className={styles.loadSampleFileButton}
            >
                Скачать образец файла...
            </button>
        </div>
    )
}

export default WordTranslationsFile;