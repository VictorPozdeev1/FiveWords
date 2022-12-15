using FiveWords._v1.DataObjects;
using FiveWords.DataObjects;

namespace FiveWords._v1.BusinessLogic;

public class GuessRightVariant_UserAnswerAssessor
{
    public string GetAssessment<TUnitQuestion, TUnitAnswerVariant>(UserChallenge<ChoosingRightOption_UserChallengeUnit_v1<TUnitQuestion, TUnitAnswerVariant>> userChallenge, int[] userAnswers)
    {
        byte rightAnswersCounter = 0;
        for (int i = 0; i < userAnswers.Length; i++)
            if (userAnswers[i] == userChallenge.Units[i].RightAnswer)
                rightAnswersCounter++;
        return $"Правильных ответов: {rightAnswersCounter}.".ToString();
    }
}