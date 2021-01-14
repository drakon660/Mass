using GreenPipes;
using mass.components.Consumers;
using mass.contracts;
using MassTransit;
using MassTransit.Util;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Mass.Terminal
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.Delay(1000);
            
            var serviceProvider = ConfigureServiceProvider();

            var bus = serviceProvider.GetRequiredService<IBusControl>();

            try
            {
                bus.Start();
                try
                {
                    Console.WriteLine("Bus started, type 'exit' to exit.");

                    bool running = true;
                    while (running)
                    {
                        var input = Console.ReadLine();
                        switch (input)
                        {
                            case "exit":
                            case "quit":
                                running = false;
                                break;

                            case "submit":
                                TaskUtil.Await(() => Submit(serviceProvider));
                                break;
                        }
                    }
                }
                finally
                {
                    bus.Stop();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        //TODO nie dziala ?
        static async Task Submit(IServiceProvider provider)
        {
            //IBus bus = provider.GetRequiredService<IBus>();
            var _submitOrderRequestClient = provider.GetRequiredService<IRequestClient<SubmitOrder>>();
            var orderId = NewId.NextGuid();

            var (accepted, rejected) = await _submitOrderRequestClient.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new
            {
                OrderId = "1212",
                InVar.Timestamp,
                CustomerNumber="1212",
                PaymentCardNumber = "12`3123"                
            });

            //await bus.Send<SubmitOrder>(new
            //{
            //    OrderId = orderId,
            //    OrderDateTime = DateTimeOffset.Now
            //}, Pipe.Execute<SendContext>(sendContext => sendContext.ConversationId = sendContext.CorrelationId = orderId));
        }

        static IServiceProvider ConfigureServiceProvider()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<SubmitOrderConsumer>();
                cfg.AddRequestClient<SubmitOrder>();
                cfg.UsingInMemory((context, cfg)=> cfg.ConfigureEndpoints(context));
            });
            
            return collection.BuildServiceProvider();
        }

        static IBusControl BusFactory(IBusRegistrationContext provider)
        {
            return Bus.Factory.CreateUsingInMemory(cfg => cfg.ConfigureEndpoints(provider));
        }
    }
}
