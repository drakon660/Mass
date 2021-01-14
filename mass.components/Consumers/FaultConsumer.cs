using System;
using System.Threading.Tasks;
using MassTransit;

namespace mass.components.Consumers
{
    public class FaultConsumer : IConsumer<Fault<FullfillOrder>>
    {
        public Task Consume(ConsumeContext<Fault<FullfillOrder>> context)
        {
            return Task.CompletedTask;
        }
    }
}