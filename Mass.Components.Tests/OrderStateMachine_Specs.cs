using Automatonymous.Graphing;
using mass.components.Consumers;
using mass.components.StateMachines;
using mass.contracts;
using MassTransit;
using MassTransit.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mass.Components.Tests
{
    public class Submitting_an_order
    {
        [Fact]
        public async Task Should_create_a_state_instance()
        {
            var orderStateMachine = new OrderStateMachine();
            var harness = new InMemoryTestHarness();
            harness.TestTimeout = TimeSpan.FromSeconds(5);
            var saga = harness.StateMachineSaga<OrderState,OrderStateMachine>(new OrderStateMachine());

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();

                await harness.Bus.Publish<OrderSubmitted>(new { 
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "123456"
                });


                var instance = saga.Created.Contains(orderId);
                await saga.Exists(orderId, x => x.Submitted); //race condtion wait for state change

                Assert.True(saga.Created.Select(x => x.CorrelationId == orderId).Any());
                
                Assert.NotNull(instance);
                Assert.Equal(orderStateMachine.Submitted.Name,instance.CurrentState);
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task Should_respond_to_status_checks()
        {
            var orderStateMachine = new OrderStateMachine();
            var harness = new InMemoryTestHarness();
            harness.TestTimeout = TimeSpan.FromSeconds(5);
            var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(new OrderStateMachine());

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();

                await harness.Bus.Publish<OrderSubmitted>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "123456"
                });


                var instance = saga.Created.Contains(orderId);
                await saga.Exists(orderId, x => x.Submitted); //race condtion wait for state change

                var requestClient = await harness.ConnectRequestClient<CheckOrder>();
                var response = await requestClient.GetResponse<OrderStatus>(new { OrderId = orderId });
                Assert.Equal(orderStateMachine.Submitted.Name, response.Message.State);

            }
            finally
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task Should_cancel_when_customer_account_closed()
        {
            var orderStateMachine = new OrderStateMachine();
            var harness = new InMemoryTestHarness();
            harness.TestTimeout = TimeSpan.FromSeconds(5);
            var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(new OrderStateMachine());

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();

                await harness.Bus.Publish<OrderSubmitted>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "123456"
                });

                var instance = saga.Created.Contains(orderId);
                var instanceId = await saga.Exists(orderId, x => x.Submitted); //race condtion wait for state change
                Assert.NotNull(instanceId);

                await harness.Bus.Publish<CustomerAccountClosed>(new { 
                    CustomerId = InVar.Id,
                    CustomerNumber = "123456"
                });

                instanceId = await saga.Exists(orderId, x => x.Canceled);
                Assert.NotNull(instanceId);
            }
            finally
            {
                await harness.Stop();
            }
        }


        [Fact]
        public async Task Should_accept_when_order_is_accepted()
        {
            var orderStateMachine = new OrderStateMachine();
            var harness = new InMemoryTestHarness();
            harness.TestTimeout = TimeSpan.FromSeconds(5);
            var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(new OrderStateMachine());

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();

                await harness.Bus.Publish<OrderSubmitted>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "123456"
                });

                var instance = saga.Created.Contains(orderId);
                var instanceId = await saga.Exists(orderId, x => x.Submitted); //race condtion wait for state change
                Assert.NotNull(instanceId);

                await harness.Bus.Publish<OrderAccepted>(new
                {
                    OrderId = orderId
                });

                instanceId = await saga.Exists(orderId, x => x.Accepted);
                Assert.NotNull(instanceId);
            }
            finally
            {
                await harness.Stop();
            }
        }


        //TODO check visual for masstransit statemachine
        [Fact]
        public void Show_me_the_state_machine()
        {
            var orderStateMachine = new OrderStateMachine();
            var graph = orderStateMachine.GetGraph();

        }
    }
}
