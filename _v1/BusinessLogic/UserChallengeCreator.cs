using FiveWords.DataObjects;
using FiveWords._v1.DataObjects;
using FiveWords._v1.Repository;
using FiveWords.Repository;
using System.Security.Cryptography;
using FiveWords.Utils;

namespace FiveWords._v1.BusinessLogic
{
    internal class TranslationUserChallengeCreator
    {
        // todo Разобраться уже, наконец, что если сливать данные из разных репозиториев.
        // Возможно, для этого придётся заморочиться с нормализацией и вспомнить первые 3 нф
        readonly IEnumerable<UserRepositoriesManager<IWordsRepository>> translationsRepoManagers;
        readonly RussianWordsUserRepositoriesManager russianWordsRepoManager;
        public TranslationUserChallengeCreator(IEnumerable<UserRepositoriesManager<IWordsRepository>> translationsRepoManagers, RussianWordsUserRepositoriesManager russianWordsRepoManager)
        {
            this.translationsRepoManagers = translationsRepoManagers;
            this.russianWordsRepoManager = russianWordsRepoManager;
        }

        public UserChallenge<ChoosingRightOption_UserChallengeUnit_v1<WordWithEnglishTranslationId, Word>> CreateGuessTranslateChallenge(User currentUser, byte unitsCount, byte answerVariantsCount)
        {
            var result = new UserChallenge<ChoosingRightOption_UserChallengeUnit_v1<WordWithEnglishTranslationId, Word>>()
            { Units = new ChoosingRightOption_UserChallengeUnit_v1<WordWithEnglishTranslationId, Word>[unitsCount] };

            var englishWordsRepo = translationsRepoManagers.First().GetRepository(currentUser);

            var suitablesForQuestion = russianWordsRepoManager.GetRepository(currentUser)
                .GetWordsHavingEnglishTranslationId().ToArray();
            var suitablesForQuestionAmount = suitablesForQuestion.Length;
            List<int> questionWordsIndices = GetDifferentRandomIndexes(unitsCount, suitablesForQuestionAmount);
            for (int i = 0; i < unitsCount; i++)
            {
                var question = suitablesForQuestion[questionWordsIndices[i]];
                var rightAnswer = englishWordsRepo.Get(question.DefaultEnglishTranslationId!.Value);

                var suitablesForOtherAnswerVariants = englishWordsRepo.GetByWritingFilter(w => w != rightAnswer.Writing).ToArray();
                var suitablesForOtherAnswerVariantsAmount = suitablesForOtherAnswerVariants.Length;
                List<int> answerVariantIndexes = GetDifferentRandomIndexes(answerVariantsCount, suitablesForOtherAnswerVariantsAmount);
                var answerVariants = suitablesForOtherAnswerVariants.GetElementsAt(answerVariantIndexes).ToArray();

                var rightAnswerIndex = RandomNumberGenerator.GetInt32(answerVariantsCount);
                answerVariants[rightAnswerIndex] = rightAnswer;

                result.Units[i] = new ChoosingRightOption_UserChallengeUnit_v1<WordWithEnglishTranslationId, Word>(question, answerVariants, rightAnswerIndex);
            }
            return result;
        }

        private static List<int> GetDifferentRandomIndexes(byte amount, int upperLimiter/*, Predicate<int>? filter = null*/)
        {
            List<int> result = new();
            while (result.Count < amount)
            {
                var testIndex = RandomNumberGenerator.GetInt32(upperLimiter);
                //if (filter != null && filter(testIndex))
                if (!result.Contains(testIndex))
                    result.Add(testIndex);
            }
            return result;
        }
    }
}
