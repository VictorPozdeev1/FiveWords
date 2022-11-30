import { useCallback } from 'react';
import styles from './WordTranslationsFile.module';
import { fetchWithAuth } from '../../modules/Auth.js';

const WordTranslationsFile = () => {
    const saveSampleFile = useCallback(() => {
        const link = document.createElement('a');
        link.setAttribute('href', '/sample-dictionary/words.csv');
        link.setAttribute('download', 'sample-dictionary.csv');
        link.click();
    }, []);

    const uploadFile = useCallback(() => {
        const fileInput = document.createElement('input');
        fileInput.setAttribute('type', 'file');
        fileInput.addEventListener("change", () => { handleFiles(fileInput.files) }, false);
        fileInput.click();
    }, []);

    const handleFiles = async files => {
        if (files && files.length && files[0].size < 50000) {
            const file = files[0];
            console.log('size:', file.size);
            const formData = new FormData();
            formData.append('uploadingFile', file, 'words-to-add.csv');
            const url = encodeURI('/DictionaryContentFile/Test1');
            const options = {
                method: 'POST',
                body: formData
            }
            const response = await fetchWithAuth(url, options);
            const json = await response.json();
            console.log(json);
        }
    }

    return (
        <div>
            <button
                onClick={uploadFile}
                className={styles.addFromFileButton}
            >
                Добавить слова из файла...
            </button>
            <button
                onClick={saveSampleFile}
                className={styles.loadSampleFileButton}
            >
                Скачать образец файла
            </button>
        </div>
    )
}

export default WordTranslationsFile;