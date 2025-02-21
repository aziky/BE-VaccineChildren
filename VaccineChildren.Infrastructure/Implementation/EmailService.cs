using Microsoft.Extensions.Configuration;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Extensions.Logging;
using VaccineChildren.Application.Services.Impl;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Infrastructure.Implementation;

public class EmailService : IEmailService
{
    private readonly IMailjetClient _mailjetClient;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmailService> _logger;
    private readonly string _senderEmail;
    private readonly string _senderName;

    public EmailService(
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        ILogger<EmailService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _senderEmail = configuration["Mailjet:SenderEmail"];
        _senderName = configuration["Mailjet:SenderName"];
        
        _mailjetClient = new MailjetClient(
            configuration["Mailjet:ApiKey"],
            configuration["Mailjet:ApiSecret"]
        );
    }

    public async Task<bool> SendEmailAsync(string recipientEmail, string recipientName, int templateId, Dictionary<string, string> templateData)
    {
        try
        {
            var templateRepository = _unitOfWork.GetRepository<Template>();
            var template = await templateRepository.FindByConditionAsync(t => t.TemplateId == templateId);

            if (template == null)
            {
                _logger.LogError("Email template not found: {TemplateId}", templateId);
                return false;
            }

            // Replace template variables
            string htmlContent = template.Temaplate;
            foreach (var data in templateData)
            {
                htmlContent = htmlContent.Replace($"{{{data.Key}}}", data.Value);
            }

            var email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact(_senderEmail, _senderName))
                .WithSubject(template.Subject)
                .WithHtmlPart(htmlContent)
                .WithTo(new SendContact(recipientEmail, recipientName))
                .Build();

            var response = await _mailjetClient.SendTransactionalEmailAsync(email);

            if (response.Messages != null && response.Messages.Length > 0)
            {
                var message = response.Messages[0];
                if (message.Status == "success")
                {
                    _logger.LogInformation("Email sent successfully to {RecipientEmail}", recipientEmail);
                    return true;
                }
            }

            _logger.LogError("Failed to send email to {RecipientEmail}", recipientEmail);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {RecipientEmail}", recipientEmail);
            return false;
        }
    }
}