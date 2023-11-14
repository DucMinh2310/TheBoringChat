namespace TheBoringChat.Services;

public class EmailSetting : INotification
{
    public string From { get; set; } = null!;
    public string Pw { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string To { get; set; } = null!;
    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public int TimeOut { get; set; } = 60000;
    public bool SSL { get; set; } = true;
}

public class MailService(ILogger<MailService> logger, IConfiguration configuration) : INotificationHandler<EmailSetting>
{
    private readonly ILogger<MailService> _logger = logger;
    private readonly IConfiguration _configuration = configuration;

    public async Task Handle(EmailSetting request, CancellationToken cancellationToken)
    {
        // Setting
        _configuration.GetSection("EmailSetting").Bind(request);

        // Handle
        MailMessage message = new()
        {
            From = new MailAddress(request.From),
            Body = request.Body,
            Subject = request.Subject
        };
        var arrEmail = request.To.Split(";");
        foreach (var item in arrEmail) { message.To.Add(item); }

        SmtpClient smtp = new()
        {
            Host = request.Host,
            Port = request.Port,
            EnableSsl = request.SSL,
            UseDefaultCredentials = false,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Credentials = new NetworkCredential(request.From, request.Pw),
            Timeout = request.TimeOut
        };

        _logger.LogInformation("Confirmation email sent with message: {message}", message.DoSerialize());
        await smtp.SendMailAsync(message, cancellationToken);
    }
}

