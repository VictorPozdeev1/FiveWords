import { useCallback, useState } from 'react';
import styles from './WordTranslationsFile.module';
import { fetchWithAuth } from '../../modules/Auth.js';

const WordTranslationsFile = ({ dictionaryName, addValuesToContent }) => {
    const [errorMessage, setErrorMessage] = useState(null);
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
        if (files && files.length) {
            const file = files[0];
            if (file.size > 50000) {
                setErrorMessage(`Разрешённый размер файла: не более 50 Кб. Размер выбранного файла: ${Math.ceil(file.size / 1024)} Кб.`);
                return;
            }
            const formData = new FormData();
            formData.append('uploadingFile', file, 'words-to-add.csv');
            const url = encodeURI(`/DictionaryContentFile/${dictionaryName}`);
            const options = {
                method: 'POST',
                body: formData
            }
            const response = await fetchWithAuth(url, options);
            if (response.ok) {
                const responseJson = await response.json();
                addValuesToContent(responseJson.wordsToAdd);
                setErrorMessage(null);
            }
            else {
                const responseJson = await response.json();
                const responseErrorMessage = responseJson.error?.message ?? responseJson.errors[Object.keys(responseJson.errors)[0]] ?? FETCH_STATUSES.ERROR;
                console.log(responseJson);
                setErrorMessage(responseErrorMessage);
            }
        }
    }

    return (
        <div className={styles.loadFiles}>
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
            {errorMessage &&
                <p className={styles.errorMessage}>
                    {errorMessage}
                </p>}
        </div>
    )
}

export default WordTranslationsFile;