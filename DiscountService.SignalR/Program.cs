

using DiscountService.CodeService;
using DiscountService.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.WriteIndented = true;
    options.PayloadSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
}); ;


// Storage + service singletons
builder.Services.AddSingleton<ICodeStore>(sp =>
{
    // Persist in the current folder; customize path as needed
    var dataPath = Path.Combine(AppContext.BaseDirectory, "discount_codes.json");
    return new FileCodeStore(dataPath);
});


builder.Services.AddSingleton<DiscountCodeService>();


var app = builder.Build();


app.MapHub<DiscountHub>("/discountHub");


app.Run();