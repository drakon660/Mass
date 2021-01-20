using mass.components.Consumers;
using mass.contracts;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit.RabbitMqTransport;
using MassTransit.Definition;
using MassTransit.MessageData;
using MassTransit.MongoDbIntegration.MessageData;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mass.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

            //services.AddMediator(md => {
            //    md.AddConsumer<SubmitOrderConsumer>();
            //    md.AddRequestClient<SubmitOrder>();
            //});

            //services.AddMassTransit(mt =>
            //{
            //    mt.AddConsumer<SubmitOrderConsumer>();
            //    mt.AddRequestClient<SubmitOrder>();
            //    mt.UsingInMemory((context,cfg)=> {
            //        cfg.ConfigureEndpoints(context);
            //    });                
            //});

            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<SubmitOrderConsumer>();
                cfg.UsingRabbitMq((context, configurator) =>
                {
                    configurator.UseMessageData(new MongoDbMessageDataRepository("mongodb://127.0.0.1", "attachments"));
                    
                });
                // cfg.AddBus(x => { return Bus.Factory.CreateUsingRabbitMq(b =>
                //     {
                //         //MessageDataDefaults.
                //         b.UseMessageData(new MongoDbMessageDataRepository("mongodb://127.0.0.1", "attachments"));
                //     });
                // });
                cfg.AddRequestClient<SubmitOrder>();
                cfg.AddRequestClient<CheckOrder>();
                
                //cfg.AddRequestClient<SubmitOrder>(new Uri($"queue :{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));
                //cfg.AddRequestClient<SubmitOrder>(new Uri($"exchange :{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));
                //cfg.AddRequestClient<AllocateInventory>();
            });
            
            services.AddMassTransitHostedService();

            services.AddControllers();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            
            app.UseRouting();
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            //app.UseHttpsRedirection();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        //static void ConfigureBus(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator configurator)
        //{
        //    Bus.Factory.CreateUsingRabbitMq(cfg =>
        //    {
        //        cfg.ConfigureEndpoints(context);
        //    });
        //}
    }
}
