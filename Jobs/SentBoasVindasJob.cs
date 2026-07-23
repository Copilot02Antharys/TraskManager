using Hangfire;
using TraskManager.Services;

namespace TraskManager.Jobs;

public class SentBoasVindasJob
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SentBoasVindasJob> _logger;

    public SentBoasVindasJob(IEmailService emailService, ILogger<SentBoasVindasJob> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    // Fire-and-Forget: executa imediatamente em background
    [Queue("default")]
    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 30, 60, 120 })]
    public async Task SentBoasVindas(string email, string nome)
    {
        _logger.LogInformation("Enviando email de boas-vindas para {Email}", email);

        await _emailService.EnviarAsync(
            destinatario: email,
            assunto: $"Bem-vindo, {nome}!",
            corpo: $"<h1>Olá, {nome}!</h1><p>Seja bem-vindo à nossa plataforma.</p>");

        _logger.LogInformation("Email de boas-vindas enviado com sucesso para {Email}", email);
    }
}
