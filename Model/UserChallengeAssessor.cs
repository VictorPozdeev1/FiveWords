using FiveWords.DataObjects;

namespace FiveWords.Model;

public class GuessRightVariant_UserAnswerAssessor
{
    public string GetAssessment<TUnitQuestion, TUnitAnswerVariant>(UserChallenge<GuessRightVariant_UserChallengeUnit<TUnitQuestion, TUnitAnswerVariant>> userChallenge, int[] userAnswers)
    {
        byte rightAnswersCounter = 0;
        for (int i = 0; i < userAnswers.Length; i++)
            if (userAnswers[i] == userChallenge.Units[i].RightAnswer)
                rightAnswersCounter++;
        return $"Правильных ответов: {rightAnswersCounter}.".ToString();
    }
}