namespace FiveWords.View;

public abstract class ChallengeResult_HtmlViewCreator<TChallengeResult> : HtmlViewCreator<TChallengeResult>
{
    protected override string HtmlTemplateFilename => "challenge-result.html";
}
