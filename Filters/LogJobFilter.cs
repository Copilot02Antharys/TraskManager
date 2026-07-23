using Hangfire.Common;
using Hangfire.States;

namespace TraskManager.Filters;

public class LogJobFilter : JobFilterAttribute, IElectStateFilter
{
    private static readonly ILogger Logger =
        LoggerFactory.Create(b => b.AddConsole()).CreateLogger<LogJobFilter>();

    public void OnStateElection(ElectStateContext context)
    {
        if (context.CandidateState is FailedState failedState)
        {
            Logger.LogError(
                failedState.Exception,
                "❌ Job {JobId} FALHOU após todas as tentativas. Método: {Method}",
                context.BackgroundJob.Id,
                context.BackgroundJob.Job.Method.Name);
        }

        if (context.CandidateState is SucceededState)
        {
            Logger.LogInformation(
                "✅ Job {JobId} concluído com sucesso. Método: {Method}",
                context.BackgroundJob.Id,
                context.BackgroundJob.Job.Method.Name);
        }
    }
}
