import styles from './TranslationChallenge.module';

//import { fetchWithAuth, removeToken, isAuthenticated } from "./modules/Auth";

//if (!isAuthenticated())
//    location.assign('/');


const TranslationChallenge = ({ dictionaryName }) => {

    const url = new URL(`WordTranslationsChallenge/${dictionaryName}`, location.origin);
    //const response = await fetchWithAuth(url);
    //if (response.ok) {
    //    responseJson = await response.json();
    //        //}
    //else {
    //    alert("Не удалось загрузить ваши словари. Вы будете перенаправлены на гостевую страницу.");
    //    removeToken();
    //    location.assign('/');
    //}

    return (
        <div className={styles.body}>
            Hurray!
        </div>)
}

export default TranslationChallenge;