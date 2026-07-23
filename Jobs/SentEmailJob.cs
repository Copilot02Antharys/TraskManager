using Hangfire;
using TraskManager.Services;

namespace TraskManager.Jobs;

public class SentEmailJob
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SentEmailJob> _logger;

    public SentEmailJob(IEmailService emailService, ILogger<SentEmailJob> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    // Continuation: executa após o job de relatório terminar
    [Queue("default")]
    [AutomaticRetry(Attempts = 2)]
    public async Task SentRelatorioEmail(string email, string tituloRelatorio)
    {
        _logger.LogInformation("Enviando relatório '{Titulo}' para {Email}", tituloRelatorio, email);

        await _emailService.EnviarAsync(
            destinatario: email,
            assunto: tituloRelatorio,
            corpo: $"<h2>{tituloRelatorio}</h2><p>Segue em anexo o relatório gerado automaticamente.</p>");

        _logger.LogInformation("Relatório enviado para {Email}", email);
    }
}
