using DiscountService.CodeService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscountService.TCP
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                })
                .ConfigureLogging((hostingContext, logging) =>
                {

                })
                .ConfigureServices((context, services) =>
                {
                    // Add other services as needed
                    // Register services
                    services.AddSingleton<ICodeStore>(sp =>
                    {
                        var path = Path.Combine(AppContext.BaseDirectory, "discount_codes.json");
                        return new FileCodeStore(path);
                    });
                    services.AddSingleton<DiscountCodeService>();
                    services.AddSingleton<TCPServer>();
                    services.AddHostedService<TCPWorker>();
                });
                
            await host.RunConsoleAsync(); 
        }
    }
}
