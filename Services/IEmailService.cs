namespace TraskManager.Services;

public interface IEmailService
{
    Task EnviarAsync(string destinatario, string assunto, string corpo);
}
