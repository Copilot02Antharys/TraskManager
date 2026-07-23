using Hangfire;

namespace TraskManager.Jobs;

public class SentNotificacaoJob
{
    private readonly ILogger<SentNotificacaoJob> _logger;

    public SentNotificacaoJob(ILogger<SentNotificacaoJob> logger)
    {
        _logger = logger;
    }

    // Delayed Job: agendado para executar no futuro
    [Queue("critical")]
    [AutomaticRetry(Attempts = 5, DelaysInSeconds = new[] { 10, 30, 60, 300, 600 })]
    public async Task SentNotificacaoPush(string mensagem, int usuarioId)
    {
        _logger.LogInformation("Enviando push para usuário {Id}: {Msg}", usuarioId, mensagem);

        // Simula envio para serviço externo de push
        await Task.Delay(500);

        _logger.LogInformation("Push enviado com sucesso para usuário {Id}", usuarioId);
    }
}
