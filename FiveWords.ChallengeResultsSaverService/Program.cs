using FiveWords.ChallengeResultsSaverService;
using FiveWords.ChallengeResultsSaverService.PgSqlStoring;
using Microsoft.EntityFrameworkCore;
using Npgsql;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddDbContext<ChallengeResultsDbContext>(optionsBuilder =>
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(
                ctx.Configuration.GetConnectionString("PgSqlChallengeResults"));
            connectionStringBuilder.Password = ctx.Configuration["DbPasswords:PgSqlChallengeResults"];
            optionsBuilder.UseNpgsql(connectionStringBuilder.ConnectionString);
        });
        services.AddSingleton<IChallengeResultsSaver, ChallengeResultsToPgSqlSaver>();

        services.Configure<RabbitQueuesOptions>(ctx.Configuration.GetSection("RabbitMQ"));
        services.AddHostedService<RabbitMqListener>();
    })
    .Build()
    .RunAsync();
