namespace TraskManager.Services;

public class RelatorioService
{
    private readonly ILogger<RelatorioService> _logger;

    public RelatorioService(ILogger<RelatorioService> logger)
    {
        _logger = logger;
    }

    public async Task<List<string>> ColetarDadosSemanaAsync()
    {
        await Task.Delay(500); // simula consulta ao banco
        return Enumerable.Range(1, 20).Select(i => $"Registro-Semana-{i}").ToList();
    }

    public async Task<List<string>> ColetarDadosMesAsync(int mes)
    {
        await Task.Delay(800); // simula consulta ao banco
        return Enumerable.Range(1, 100).Select(i => $"Registro-Mes{mes}-{i}").ToList();
    }

    public async Task SalvarRelatorioAsync(string tipo, List<string> dados)
    {
        await Task.Delay(300); // simula gravação
        _logger.LogInformation("📄 Relatório '{Tipo}' salvo com {Count} registros.", tipo, dados.Count);
    }
}
