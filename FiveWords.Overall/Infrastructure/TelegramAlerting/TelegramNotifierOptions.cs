namespace FiveWords.Infrastructure.TelegramAlerting;

public sealed class TelegramNotifierOptions
{
    public string BotToken { get; set; }
    public string ChatId { get; set; }
}
