using DTO.MessageBroker.Messages.Users;
using EmailService.Config;
using EmailService.Models;
using EmailService.Services.Interfaces;
using MassTransit;
using Microsoft.Extensions.Options;

namespace EmailService.Consumers.Users;

public sealed class NoteFromCustomerMessageConsumer : IConsumer<NoteFromCustomerMessage>
{
    private readonly ILogger<NoteFromCustomerMessageConsumer> _logger;
    private readonly IEmailSender _emailSender;
    private readonly ITemplateProvider _templateProvider;
    private readonly AdminSettings _adminSettings;

    public NoteFromCustomerMessageConsumer(
        ILogger<NoteFromCustomerMessageConsumer> logger,
        IEmailSender emailSender,
        ITemplateProvider templateProvider,
        IOptions<AdminSettings> adminSettings)
    {
        _logger = logger;
        _emailSender = emailSender;
        _templateProvider = templateProvider;
        _adminSettings = adminSettings.Value;
    }
    public async Task Consume(ConsumeContext<NoteFromCustomerMessage> context)
    {
        try
        {
            _logger.LogInformation("Forwarding note from customer to admin: {adminEmail}", _adminSettings.AdminEmail);

            var parameters = new Dictionary<string, string>
                {
                    { "@type", context.Message.MessageType },
                    { "@firstName", context.Message.FirstName },
                    { "@email", context.Message.Email },
                    { "@message", context.Message.Note }
                };

            var htmlTemplateContent = await _templateProvider.GetTemplateAsync("User", "NoteFromCustomer", parameters);

            await _emailSender.SendAsync(new MailMessageRequest(
                _adminSettings.AdminEmail,
                $"Hiring - {context.Message.MessageType}",
                htmlTemplateContent,
                $"Hiring Administrator"));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error sending the note on {Email}: {Message}", _adminSettings.AdminEmail, ex.Message);
        }
    }
}
