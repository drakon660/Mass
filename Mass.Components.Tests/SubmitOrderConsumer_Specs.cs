using mass.components.Consumers;
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
    public class When_an_roder_request_is_consumed
    {
        [Fact]
        public async Task Should_respond_with_acceptance_if_ok()
        {
            var harness = new InMemoryTestHarness();
            var consumer = harness.Consumer<SubmitOrderConsumer>();

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();
                var requestClient = await harness.ConnectRequestClient<SubmitOrder>();
                var response = await requestClient.GetResponse<OrderSubmissionAccepted>(new {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "12345"
                });

                await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new { 
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "12345"
                });

                Assert.Equal(response.Message.OrderId, orderId);

                Assert.True(consumer.Consumed.Select<SubmitOrder>().Any());
                Assert.True(harness.Sent.Select<OrderSubmissionAccepted>().Any());
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task Should_respond_with_rejected_if_testk()
        {
            var harness = new InMemoryTestHarness();
            var consumer = harness.Consumer<SubmitOrderConsumer>();

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();
                var requestClient = await harness.ConnectRequestClient<SubmitOrder>();
                var response = await requestClient.GetResponse<OrderSubmissionRejected>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "TEST12345"
                });

                await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "12345"
                });

                Assert.Equal(response.Message.OrderId, orderId);

                Assert.True(consumer.Consumed.Select<SubmitOrder>().Any());
                Assert.True(harness.Sent.Select<OrderSubmissionRejected>().Any());
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task Should_consume_submit_order_commands()
        {
            var harness = new InMemoryTestHarness();
            harness.TestTimeout = TimeSpan.FromSeconds(5);
            var consumer = harness.Consumer<SubmitOrderConsumer>();

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();                

                await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "12345"
                });

                Assert.True(consumer.Consumed.Select<SubmitOrder>().Any());

                Assert.False(harness.Sent.Select<OrderSubmissionAccepted>().Any());
                Assert.False(harness.Sent.Select<OrderSubmissionRejected>().Any());
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task Should_not_publish_order_submited_event_when_rejected()
        {
            var harness = new InMemoryTestHarness();
            harness.TestTimeout = TimeSpan.FromSeconds(5);
            var consumer = harness.Consumer<SubmitOrderConsumer>();

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();

                await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "TEST12345"
                });

                Assert.False(harness.Published.Select<OrderSubmitted>().Any());


            }
            finally
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task Should_publish_order_submited_event()
        {
            var harness = new InMemoryTestHarness();
            harness.TestTimeout = TimeSpan.FromSeconds(5);
            var consumer = harness.Consumer<SubmitOrderConsumer>();

            await harness.Start();
            try
            {
                var orderId = NewId.NextGuid();

                await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
                {
                    OrderId = orderId,
                    InVar.Timestamp,
                    CustomerNumber = "12345"
                });

                Assert.True(harness.Published.Select<OrderSubmitted>().Any());

             
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}
