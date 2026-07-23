using Hangfire;

namespace TraskManager.Jobs;

public class SentLimpezaJob
{
    private readonly ILogger<SentLimpezaJob> _logger;

    public SentLimpezaJob(ILogger<SentLimpezaJob> logger)
    {
        _logger = logger;
    }

    // Recurring Job: diário à meia-noite, com suporte a CancellationToken
    [Queue("low-priority")]
    [AutomaticRetry(Attempts = 0)]
    public async Task SentLimparRegistrosAntigos(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando limpeza de registros antigos...");

        int totalRemovidos = 0;

        for (int lote = 0; lote < 10; lote++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Delay(200, cancellationToken);
            totalRemovidos += 50;

            _logger.LogInformation("Lote {Lote}/10 processado. Removidos até agora: {Total}", lote + 1, totalRemovidos);
        }

        _logger.LogInformation("Limpeza concluída. Total de registros removidos: {Total}", totalRemovidos);
    }
}
