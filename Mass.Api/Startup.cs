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
                cfg.AddBus(x => Bus.Factory.CreateUsingRabbitMq());
                cfg.AddRequestClient<SubmitOrder>();
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
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

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
