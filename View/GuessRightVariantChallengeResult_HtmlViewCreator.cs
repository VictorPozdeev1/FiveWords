using HtmlAgilityPack;

namespace FiveWords.View;

public class GuessRightVariantChallengeResult_HtmlViewCreator : ChallengeResult_HtmlViewCreator<string>
{
    protected override void WriteData(HtmlDocument htmlDocument, string assessment)
    {
        var mainLabel = htmlDocument.GetElementbyId("result-text");//htmlDocument.DocumentNode.SelectSingleNode("//*[@id='result-text']");
        mainLabel.InnerHtml = assessment;
        var whatsMoreLabel = htmlDocument.GetElementbyId("whats-more");
        whatsMoreLabel.InnerHtml = "(Тут когда-нибудь будет возможность зарегистрироваться, чтобы создавать собственные словари и программы тренировок, вести статистику, и т. д.)";
    }
}