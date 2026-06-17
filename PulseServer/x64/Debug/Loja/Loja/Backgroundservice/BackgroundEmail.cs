
using Loja.Classes;

namespace Loja.Backgroundservice
{
    public class BackgroundEmail : BackgroundService
    {
        private readonly TriggerEmail triggerEmail;
        private readonly ILogger<BackgroundEmail> logger;
        public BackgroundEmail(TriggerEmail trigger, ILogger<BackgroundEmail> logger)
        {
            triggerEmail = trigger;
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await triggerEmail.ReceberMensagem();
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
