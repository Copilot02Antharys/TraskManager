using Hangfire;
using Hangfire.SqlServer;
using TraskManager.Filters;
using TraskManager.Jobs;
using TraskManager.Dashboard;
using TraskManager.Services;

var builder = WebApplication.CreateBuilder(args);

// ─── Serviços da aplicação ───────────────────────────────────────────────────
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddScoped<SentEmailJob>();
builder.Services.AddScoped<SentRelatorioJob>();
builder.Services.AddScoped<SentLimpezaJob>();
builder.Services.AddScoped<SentNotificacaoJob>();
builder.Services.AddScoped<SentBoasVindasJob>();

// ─── Hangfire ────────────────────────────────────────────────────────────────
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(
        builder.Configuration.GetConnectionString("Hangfire"),
        new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout     = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval          = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true
        }));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 10;
    options.Queues = new[] { "critical", "default", "low-priority" };
});

// Filtro global de log para todos os jobs
GlobalJobFilters.Filters.Add(new LogJobFilter());

builder.Services.AddControllers();

var app = builder.Build();

// ─── Dashboard ───────────────────────────────────────────────────────────────
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() },
    DashboardTitle = "TraskManager - Monitoramento de Jobs"
});

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// ─── Registrar Jobs na inicialização ─────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    // 1. FIRE-AND-FORGET — Envia email de boas-vindas
    BackgroundJob.Enqueue<SentBoasVindasJob>(job =>
        job.SentBoasVindas("admin@empresa.com", "Administrador"));

    // 2. DELAYED JOB — Notificação push agendada para daqui 1 hora
    BackgroundJob.Schedule<SentNotificacaoJob>(
        methodCall: job => job.SentNotificacaoPush("Lembrete: reunião em 15 minutos!", 1),
        delay: TimeSpan.FromHours(1));

    // 3. RECURRING JOB — Limpeza diária à meia-noite
    RecurringJob.AddOrUpdate<SentLimpezaJob>(
        recurringJobId: "sent-limpeza-diaria",
        methodCall: job => job.SentLimparRegistrosAntigos(CancellationToken.None),
        cronExpression: Cron.Daily,
        queue: "low-priority");

    // 4. RECURRING JOB — Relatório semanal toda segunda-feira às 8h
    RecurringJob.AddOrUpdate<SentRelatorioJob>(
        recurringJobId: "sent-relatorio-semanal",
        methodCall: job => job.SentRelatorioSemanal(),
        cronExpression: "0 8 * * 1",
        queue: "default");

    // 5. CONTINUATION — Gera relatório mensal e depois envia por email
    var relatorioJobId = BackgroundJob.Enqueue<SentRelatorioJob>(
        job => job.SentRelatorioMensal(DateTime.Now.Month));

    BackgroundJob.ContinueJobWith<SentEmailJob>(
        parentId: relatorioJobId,
        methodCall: job => job.SentRelatorioEmail("gestor@empresa.com", "Relatório Mensal"));
}

app.Run();
