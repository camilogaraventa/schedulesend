using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ScheduleSend
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                x.AddRabbitMqMessageScheduler();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "test", h =>
                    {
                        h.Username("test");
                        h.Password("test");
                    });

                    cfg.UseDelayedExchangeMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            });

            var provider = services.BuildServiceProvider();

            var scheduler = provider.GetRequiredService<IMessageScheduler>();

            await scheduler.ScheduleSend<Mensaje>(
                new Uri("queue:Mensaje"),
                DateTime.Now + TimeSpan.FromSeconds(30), new Mensaje
                {
                    Fecha = DateTime.Now,
                    Id = Guid.NewGuid()
                });
        }
    }

    public class Mensaje
    {
        public DateTime Fecha { get; set; }
        public Guid Id { get; set; }
    }
}
