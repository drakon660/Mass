using System.Linq;
using System.Threading.Tasks;
using mass.components.Consumers;
using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Logging;

namespace mass.components.BatchConsumers
{
    public class RoutingSlipBatchEventConsumer : IConsumer<Batch<RoutingSlipCompleted>>
    {
        private readonly ILogger<RoutingSlipEventConsumer> _logger;
        public RoutingSlipBatchEventConsumer(ILogger<RoutingSlipEventConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<Batch<RoutingSlipCompleted>> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.Log(LogLevel.Information, "Routing Slips Completed: {TrackingNumbers}",
                    string.Join(", ", context.Message.Select(x => x.Message.TrackingNumber)));
            }

            return Task.CompletedTask;
        }
    }
}