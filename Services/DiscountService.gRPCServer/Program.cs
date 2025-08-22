using DiscountService.CodeService;
using DiscountService.gRPCServer.Service;

namespace DiscountService.gRPCServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();


            builder.Services.AddGrpc();

            // Register services
            builder.Services.AddSingleton<ICodeStore>(sp =>
            {
                var path = Path.Combine(AppContext.BaseDirectory, "discount_codes.json");
                return new FileCodeStore(path);
            });
            builder.Services.AddSingleton<DiscountCodeService>();
            builder.Services.AddGrpcReflection();

            var app = builder.Build();
            app.MapGrpcReflectionService();
            app.MapGrpcService<DiscountServiceImpl>();
            app.MapGet("/", () => "Discount gRPC Server running. Use a gRPC client.");

            app.Run();
        }
    }
}
