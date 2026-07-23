# TraskManager

Projeto de demonstração de **Hangfire** com ASP.NET Core 8, implementando 5 tipos diferentes de background jobs.

## 📁 Estrutura do Projeto

```
TraskManager/
├── Program.cs
├── appsettings.json
├── TraskManager.csproj
├── Jobs/
│   ├── SentEmailJob.cs          # Continuation Job
│   ├── SentBoasVindasJob.cs     # Fire-and-Forget Job
│   ├── SentRelatorioJob.cs      # Recurring Job
│   ├── SentLimpezaJob.cs        # Recurring Job (com CancellationToken)
│   └── SentNotificacaoJob.cs    # Delayed Job
├── Services/
│   ├── IEmailService.cs
│   ├── EmailService.cs
│   └── RelatorioService.cs
├── Filters/
│   └── LogJobFilter.cs
└── Dashboard/
    └── HangfireAuthorizationFilter.cs
```

## 🚀 Tipos de Jobs Implementados

| Job | Tipo | Fila | Retry |
|---|---|---|---|
| `SentBoasVindas` | 🔥 Fire-and-Forget | `default` | 3x |
| `SentNotificacaoPush` | ⏱️ Delayed (+1h) | `critical` | 5x |
| `SentRelatorioSemanal` | 🔁 Recurring (seg 8h) | `default` | 1x |
| `SentLimparRegistrosAntigos` | 🔁 Recurring (diário) | `low-priority` | 0x |
| `SentRelatorioMensal` + `SentRelatorioEmail` | 🔗 Continuation | `default` | 2x |

## ⚙️ Configuração

1. Atualize a connection string no `appsettings.json`:
```json
"ConnectionStrings": {
  "Hangfire": "Server=localhost;Database=TraskManagerDB;Trusted_Connection=True;"
}
```

2. Restaure os pacotes e execute:
```bash
dotnet restore
dotnet run
```

3. Acesse o Dashboard em: `http://localhost:5000/hangfire`

## 📦 Pacotes NuGet

- `Hangfire.AspNetCore`
- `Hangfire.SqlServer`
- `Hangfire.Core`
