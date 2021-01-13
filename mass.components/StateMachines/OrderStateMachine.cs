using Automatonymous;
using mass.components.StateMachines.OrderStateMachineActivities;
using mass.contracts;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mass.components.Consumers;

namespace mass.components.StateMachines
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderAccepted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => FulFillmentFaulted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => FulFillmentCompleted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => FulFillOrderFaulted, x => x.CorrelateById(m => m.Message.Message.OrderId));
            
            Event(() => OrderStatusRequested, x => {
                x.CorrelateById(m => m.Message.OrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {
                    if(context.RequestId.HasValue)
                    {
                        await context.RespondAsync<OrderNotFound>(new { context.Message.OrderId });
                    }
                }));
             });
            
            Event(() => AccountClosed, x 
                => x.CorrelateBy((saga,context)=>saga.CustomerNumber == context.Message.CustomerNumber));

            InstanceState(x => x.CurrentState);

            Initially(When(OrderSubmitted).Then(context =>
            {
                context.Instance.SubmitDate = context.Data.Timestamp;
                context.Instance.CustomerNumber = context.Data.CustomerNumber;
                context.Instance.PaymentCardNumber = context.Data.PaymentCardNumber;
                context.Instance.Updated = DateTime.UtcNow;
            }).TransitionTo(Submitted));

            During(Submitted, Ignore(OrderSubmitted), When(AccountClosed).TransitionTo(Canceled)
                ,When(OrderAccepted).Activity((x)=>x.OfType<AcceptOrderActivity>()).TransitionTo(Accepted)
                );
            
            During(Accepted,
                    When(FulFillOrderFaulted).Then(context=>Console.WriteLine("Fulfill Order Faulted: {0}", context.Data.Exceptions.FirstOrDefault()?.Message)).TransitionTo(Faulted),
                When(FulFillmentFaulted)
                    .TransitionTo(Faulted),
                When(FulFillmentCompleted)
                    .TransitionTo(Completed));

            DuringAny(When(OrderStatusRequested).RespondAsync(x=>x.Init<OrderStatus>(new {
                OrderId = x.Instance.CorrelationId,
                State=x.Instance.CurrentState,
                CustomerNumber = x.Instance.CustomerNumber
            })));


            DuringAny(When(OrderSubmitted).Then(context =>
            {
                context.Instance.SubmitDate ??= context.Data.Timestamp;
                context.Instance.CustomerNumber ??= context.Data.CustomerNumber;
            }));
        }
        

        public State Completed { get; private set; }

        public State Submitted { get; private set; }
        public State Canceled { get; private set; }
        public State Accepted { get; private set; }
        public State Faulted { get; private set; }

        public Event<OrderSubmitted> OrderSubmitted { get; private set; }
        public Event<CustomerAccountClosed> AccountClosed { get; private set; }
        public Event<OrderAccepted> OrderAccepted { get; private set; }
        
        public Event<OrderFulFillmentFaulted> FulFillmentFaulted { get; private set; }
        public Event<OrderFulFillmentCompleted> FulFillmentCompleted { get; private set; }
        public Event<Fault<FullfillOrder>> FulFillOrderFaulted { get; private set; }
        public Event<CheckOrder> OrderStatusRequested { get; private set; }
    }
}
