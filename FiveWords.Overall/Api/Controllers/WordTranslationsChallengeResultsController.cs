using FiveWords.CommonModels;
using FiveWords.CommonModels.SavingChallengeResults;
using FiveWords.Overall.Infrastructure.RabbitMQ;
using FiveWords.Overall.Utils;
using FiveWords.Repository.Interfaces;
using FiveWords.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FiveWords.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WordTranslationsChallengeResultsController : ControllerBase
{
    [HttpPost]
    [Authorize]
    public IActionResult PostChallengeResult(
        [FromBody] ChoosingRightOptionChallengeResult challengeResult,
        [FromServices] IDateTimeProvider dateTimeProvider,
        [FromServices] IUsersRepository usersRepository,
        [FromServices] IOptionsSnapshot<RabbitQueuesOptions> rabbitQueuesOptions
        )
    {
        var serializingOptionsProvider = HttpContext.RequestServices.GetRequiredService<IOptionsSnapshot<JsonSerializerOptions>>();
        var userChallenge = HttpContext.Session.GetUserChallenge<ChoosingTranslationUserChallenge>(challengeResult.ChallengeId, serializingOptionsProvider);

        if (userChallenge is null)
            return BadRequest(new { Error = new ActionError($"Не найден тест с Id {challengeResult.ChallengeId}", challengeResult.ChallengeId) });

        var currentUserName = User.Identity!.Name!;
        var currentUser = usersRepository.Get(currentUserName)!;

        //todo Вынести в сервис из контроллера
        ConnectionFactory factory = new ConnectionFactory();
        using IConnection connection = factory.CreateConnection();
        using IModel channel = connection.CreateModel();
        var queue = channel.QueueDeclare(
            rabbitQueuesOptions.Value.ChallengeResultsSavingQueueName,
            autoDelete: false,
            exclusive: false
            );

        var queueMessageData = new ChoosingRightOptionChallengeCompletedByUser<string, string>(
            userChallenge.Id,
            userChallenge.Units
                .Select((unit, unitIndex) => new ChoosingRightOptionChallengeUnitCompletedByUser<string, string>
                (
                    unit.Question,
                    unit.AnswerOptions,
                    unit.RightOptionIndex,
                    challengeResult.UserAnswers[unitIndex]
                ))
                .ToArray(),
            currentUser.Guid,
            dateTimeProvider.UtcNow
            );
        var queueMessageString = JsonSerializer.Serialize(queueMessageData, serializingOptionsProvider?.Get("Internal"));
        channel.BasicPublish("", queue.QueueName, body: Encoding.UTF8.GetBytes(queueMessageString));

        return Ok(userChallenge);
    }
}