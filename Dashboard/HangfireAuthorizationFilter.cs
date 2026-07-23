using Hangfire.Dashboard;

namespace TraskManager.Dashboard;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var http = context.GetHttpContext();

        // Em desenvolvimento, libera acesso sem autenticação
        if (http.RequestServices
                .GetRequiredService<IWebHostEnvironment>()
                .IsDevelopment())
        {
            return true;
        }

        // Em produção, exige autenticação e role de Admin
        return http.User.Identity?.IsAuthenticated == true
            && http.User.IsInRole("Admin");
    }
}
