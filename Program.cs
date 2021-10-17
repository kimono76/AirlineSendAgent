using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AirlineSendAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            var host= Host.CreateDefaultHostBuilder()
                .ConfigureServices((context,services) =>
                    services.AddSingleton<IntAppHost,AppHost>();
                ).Build();
            
            host.services.GetService<IntAppHost>().Run();
        }
    }
}
