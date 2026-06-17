using Loja.Backgroundservice;
using Loja.Interfaces;
using Loja.Tabelas;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Loja.Classes
{
    public class TriggerEmail
    {
        private readonly IConnection connection;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<TriggerEmail> logger;
        public TriggerEmail(IConnection connection, IServiceScopeFactory serviceScopeFactory, ILogger<TriggerEmail> logger)
        {
            this.connection = connection;
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        public async Task Enviar(EmailInformation emailInformation)
        {
            try
            {
                if (emailInformation == null)
                {
                    return;
                }

                var json = JsonSerializer.Serialize(emailInformation);

                var channel = await connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(queue: "email_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                var body = Encoding.UTF8.GetBytes($"{json}");
                await channel.BasicPublishAsync(exchange: "", routingKey: "email_queue", body: body);

                await channel.CloseAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao enviar mensagem para a fila de email.");
            }
        }

        public async Task ReceberMensagem()
        {
            try
            {
                var channel = await connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(queue: "email_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    logger.LogInformation("EmailEnviado");
                    using var scope = serviceScopeFactory.CreateScope();
                    var enviarEmail = scope.ServiceProvider.GetRequiredService<IEnviarEmail>();

                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<EmailInformation>(json);
                    if (message == null)
                    {
                        logger.LogInformation("Message é null");
                        return;
                    }

                    await enviarEmail.EnviarEmailAsync(message);
                };

                await channel.BasicConsumeAsync(queue: "email_queue", autoAck: true, consumer: consumer);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao receber mensagem da fila de email.");
            }
        }
    }
}
