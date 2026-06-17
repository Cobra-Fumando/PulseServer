using MimeKit;
using MailKit.Net.Smtp;
using Loja.Tabelas;
using Loja.Interfaces;

namespace Loja.Classes
{
    public class EnviarEmail : IEnviarEmail
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<EnviarEmail> logger;
        public EnviarEmail(IConfiguration configuration, ILogger<EnviarEmail> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }
        public async Task EnviarEmailAsync(EmailInformation emailInformation)
        {
            try
            {
                logger.LogInformation("Email Chegou aqui");
                var email = new MimeMessage();

                email.From.Add(new MailboxAddress(emailInformation.Remetente, emailInformation.EmailRemetente));
                email.To.Add(new MailboxAddress(emailInformation.Destinatario, emailInformation.EmailDestinatario));

                email.Subject = emailInformation.Assunto;
                email.Body = new TextPart("html")
                {
                    Text = $@"
    <html>
    <head>
        <style>
            body {{
                font-family: Arial, Helvetica, sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 20px;
            }}

            .container {{
                max-width: 600px;
                margin: auto;
                background: #ffffff;
                border-radius: 10px;
                overflow: hidden;
                box-shadow: 0 0 10px rgba(0,0,0,0.1);
            }}

            .header {{
                background-color: #2e86de;
                color: white;
                text-align: center;
                padding: 25px;
                font-size: 24px;
                font-weight: bold;
            }}

            .content {{
                padding: 30px;
                color: #333;
                line-height: 1.6;
                font-size: 16px;
            }}

            .message {{
                background-color: #f8f9fa;
                padding: 15px;
                border-left: 5px solid #2e86de;
                border-radius: 5px;
                margin-top: 20px;
            }}

            .button {{
                display: inline-block;
                margin-top: 25px;
                padding: 12px 25px;
                background-color: #2e86de;
                color: white;
                text-decoration: none;
                border-radius: 5px;
                font-weight: bold;
            }}

            .footer {{
                text-align: center;
                padding: 15px;
                background-color: #eeeeee;
                color: #666;
                font-size: 12px;
            }}
        </style>
    </head>

    <body>
        <div class='container'>
            <div class='header'>
                Minha Loja
            </div>

            <div class='content'>
                <h2>Pedido realizado com sucesso!</h2>

                <p>
                    Obrigado pela sua compra. Abaixo está uma mensagem sobre o seu pedido:
                </p>

                <div class='message'>
                    {emailInformation.Mensagem}
                </div>

                <a href='#' class='button'>
                    Acompanhar Pedido
                </a>
            </div>

            <div class='footer'>
                © 2026 Minha Loja - Todos os direitos reservados
            </div>
        </div>
    </body>
    </html>"
                };

                using var smtp = new SmtpClient();
                smtp.CheckCertificateRevocation = false;

                await smtp.ConnectAsync(
                    "smtp.gmail.com",
                    587,
                    MailKit.Security.SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(
                    emailInformation.EmailRemetente,
                    configuration["Email:SenhaApp"]!
                );

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                logger.LogInformation("Email enviado com sucesso para {Destinatario}", emailInformation.EmailDestinatario);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao enviar email para {Destinatario}", emailInformation.EmailDestinatario);
            }
        }
    }
}
