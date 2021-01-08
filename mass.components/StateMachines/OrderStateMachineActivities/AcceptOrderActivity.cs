using Automatonymous;
using GreenPipes;
using mass.contracts;
using System;
using System.Threading.Tasks;
using mass.components.Consumers;
using MassTransit;

namespace mass.components.StateMachines.OrderStateMachineActivities
{
    public class AcceptOrderActivity : Automatonymous.Activity<OrderState, OrderAccepted>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("accept-order");            
        }

        public void Accept(StateMachineVisitor stateMachineVisitor)
        {
            stateMachineVisitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, OrderAccepted> context, Behavior<OrderState, OrderAccepted> next)
        {
            var consumeContext = context.GetPayload<ConsumeContext>();
            var sendEndpoint = await consumeContext.GetSendEndpoint(new Uri("queue:fullfill-order"));
            await sendEndpoint.Send<FullfillOrder>(new
            {
                OrderId = context.Data.OrderId
            });
            
            //TODO test what you have in arguments
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderAccepted, TException> context, Behavior<OrderState, OrderAccepted> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
