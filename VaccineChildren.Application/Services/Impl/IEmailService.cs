namespace VaccineChildren.Application.Services.Impl;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string recipientEmail, string recipientName, int templateId, Dictionary<string, string> templateData);
}