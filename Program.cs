using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AirlineSendAgent.App;
using AirlineSendAgent.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AirlineSendAgent.Client;

namespace AirlineSendAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            var host= Host.CreateDefaultBuilder()
                .ConfigureServices((context,services) =>{
                    services.AddSingleton<IntAppHost,AppHost>();
                    services.AddSingleton<IntWebhookClient,WebhookClient>();
                    services.AddDbContext<SendAgentDbContext>(opt => opt.UseSqlServer(context.Configuration.GetConnectionString("AirlineConnection")));
                    services.AddHttpClient();
                }).Build();
            
            host.Services.GetService<IntAppHost>().Run();

        }
    }
}
