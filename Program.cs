using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AirlineSendAgent.App;

namespace AirlineSendAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            var host= Host.CreateDefaultBuilder()
                .ConfigureServices((context,services) =>
                    services.AddSingleton<IntAppHost,AppHost>()
                ).Build();
            
            host.Services.GetService<IntAppHost>().Run();
        }
    }
}
