using FiveWords.ChallengeResultsSaverService;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddSingleton<IChallengeResultsSaver, ChallengeResultsToPgSqlSaver>();
        services.Configure<RabbitQueuesOptions>(ctx.Configuration.GetSection("RabbitMQ"));
        services.AddHostedService<RabbitMqListener>();
    })
    .Build()
    .RunAsync();
