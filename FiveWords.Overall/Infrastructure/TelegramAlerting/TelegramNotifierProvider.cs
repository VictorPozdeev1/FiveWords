using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;

namespace FiveWords.Infrastructure.TelegramAlerting
{
    public sealed class TelegramNotifierProvider// : IDisposable
    {
        readonly TelegramNotifierOptions options;
        public TelegramNotifierProvider(IOptionsSnapshot<TelegramNotifierOptions> optionsSnapshot)
        {
            options = optionsSnapshot.Value;
            if (!IsConfigured)
                Log.Warning("Telegram notifier configuration is not set.");
        }

        public bool IsConfigured => !(string.IsNullOrWhiteSpace(options?.BotToken) || string.IsNullOrWhiteSpace(options?.ChatId));

        public Logger? TryCreateNotifier() =>
            IsConfigured
            ? new LoggerConfiguration()
                .WriteTo.Telegram(options.BotToken, options.ChatId)
                .CreateLogger()
            : null;

        //public void Dispose() => telegramNotifier.Dispose();
    }
}
