using FiveWords.ChallengeResultsSaverService;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.Configure<RabbitQueuesOptions>(ctx.Configuration.GetSection("RabbitMQ"));
        services.AddHostedService<ChallengeResultsSaver>();
    })
    .Build()
    .RunAsync();
