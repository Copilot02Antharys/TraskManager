namespace TraskManager.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task EnviarAsync(string destinatario, string assunto, string corpo)
    {
        // Aqui entraria a integração com SendGrid, SMTP, etc.
        await Task.Delay(300); // simula latência de envio
        _logger.LogInformation("📧 Email enviado → {Destinatario} | Assunto: {Assunto}", destinatario, assunto);
    }
}
