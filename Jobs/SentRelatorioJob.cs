using Hangfire;
using TraskManager.Services;

namespace TraskManager.Jobs;

public class SentRelatorioJob
{
    private readonly RelatorioService _relatorioService;
    private readonly ILogger<SentRelatorioJob> _logger;

    public SentRelatorioJob(RelatorioService relatorioService, ILogger<SentRelatorioJob> logger)
    {
        _relatorioService = relatorioService;
        _logger = logger;
    }

    // Recurring Job: toda segunda-feira às 8h
    [Queue("default")]
    [AutomaticRetry(Attempts = 1)]
    public async Task SentRelatorioSemanal()
    {
        _logger.LogInformation("Gerando relatório semanal...");

        var dados = await _relatorioService.ColetarDadosSemanaAsync();
        await _relatorioService.SalvarRelatorioAsync("Semanal", dados);

        _logger.LogInformation("Relatório semanal gerado. Total de registros: {Count}", dados.Count);
    }

    // Fire-and-Forget: pai do Continuation Job
    [Queue("default")]
    [AutomaticRetry(Attempts = 2)]
    public async Task SentRelatorioMensal(int mes)
    {
        _logger.LogInformation("Gerando relatório mensal do mês {Mes}...", mes);

        var dados = await _relatorioService.ColetarDadosMesAsync(mes);
        await _relatorioService.SalvarRelatorioAsync($"Mensal-{mes}", dados);

        _logger.LogInformation("Relatório mensal do mês {Mes} gerado. Total: {Count}", mes, dados.Count);
    }
}
