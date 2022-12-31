using FiveWords.DataObjects;
using FiveWords.Utils;
using System.Security.Cryptography;

namespace FiveWords.BusinessLogic;

public class ChoosingTranslationChallengeCreator
{
    public ChoosingTranslationUserChallenge? CreateChoosingTranslationChallenge(IEnumerable<WordTranslation> wordTranslationsSource, byte unitsCount, byte answerVariantsCount, out ActionError? actionError)
    {
        actionError = null;
        var insufficientWordTranslationsAmount = unitsCount + answerVariantsCount - wordTranslationsSource.DistinctBy(wt => wt.Translation).Count();
        if (insufficientWordTranslationsAmount > 0)
        {
            actionError = new ActionError($"Недостаточно слов в словаре. Необходимо ещё хотя бы {insufficientWordTranslationsAmount} различных переводов.", insufficientWordTranslationsAmount);
            return null;
        }

        WordTranslation[] wordTranslations = wordTranslationsSource.ToArray();
        ChoosingTranslationUserChallenge result = new()
        {
            Id = Guid.NewGuid(),
            Units = new ChoosingRightOption_UserChallengeUnit<string, string>[unitsCount],
        };

        // можно же тоже через ShuffleAndTake() переписать?
        List<int> questionWordIndices = Utils.Utils.GetDifferentRandomIndices(unitsCount, wordTranslations.Length);
        for (int i = 0; i < unitsCount; i++)
        {
            var wordTranslationForQuestion = wordTranslations[questionWordIndices[i]];
            var question = wordTranslationForQuestion.Id;
            var rightAnswerOption = wordTranslationForQuestion.Translation;
            var answerOptions = wordTranslations
                .ShuffleAndTake(answerVariantsCount - 1, it => it.Translation != rightAnswerOption)
                .Select(it => it.Translation)
                .ToList();
            var rightAnswerOptionIndex = RandomNumberGenerator.GetInt32(answerVariantsCount);
            answerOptions.Insert(rightAnswerOptionIndex, rightAnswerOption);
            result.Units[i] = new ChoosingRightOption_UserChallengeUnit<string, string>(question, answerOptions.ToArray(), rightAnswerOptionIndex);
        }

        return result;
    }
}