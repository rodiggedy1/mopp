using DTO.MessageBroker.Messages.System;
using DTO.MessageBroker.Messages.Users;
using EmailService.Config;
using EmailService.Models;
using EmailService.Services.Interfaces;
using MassTransit;
using Microsoft.Extensions.Options;

namespace EmailService.Consumers.System;

public sealed class RestartRedisMessageConsumer : IConsumer<RestartRedisMessage>
{
    private readonly ILogger<UserCreatedMessage> _logger;
    private readonly IEmailSender _emailSender;
    private readonly ITemplateProvider _templateProivder;
    private readonly AdminSettings _adminSettings;

    public RestartRedisMessageConsumer(
        ILogger<UserCreatedMessage> logger,
        IEmailSender emailSender,
        ITemplateProvider templateProivder,
        IOptions<AdminSettings> adminSettings)
    {
        _logger = logger;
        _emailSender = emailSender;
        _templateProivder = templateProivder;
        _adminSettings = adminSettings.Value;
    }
    public async Task Consume(ConsumeContext<RestartRedisMessage> context)
    {
        var parameters = new Dictionary<string, string>
        {
            { "@errorMessage", context.Message.ErrorCodeMessage }
        };

        var htmlTemplateContent = await _templateProivder.GetTemplateAsync("System", "RestartRedis", parameters!);

        if(_adminSettings.SystemEmails != null)
        {
            foreach (var email in _adminSettings.SystemEmails)
            {
                _logger.LogInformation("Sending redis needs to get restarted message to {email}", email);

                await _emailSender.SendAsync(new MailMessageRequest(
                    email,
                    "Hiring - Restart Redis Container Alert",
                    htmlTemplateContent,
                    $"Hiring System"));

            }
        }
    }
}
