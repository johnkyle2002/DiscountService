using DiscountService.Common.Dto.Response;
using Microsoft.AspNetCore.SignalR;
using DiscountService.CodeService;

namespace DiscountService.SignalR.Hubs
{
    public class DiscountHub : Hub
    {
        private readonly ILogger<DiscountCodeService> _logger;
        private readonly DiscountCodeService _service;

        public DiscountHub(ILogger<DiscountCodeService> logger, DiscountCodeService service)
        {
            _logger = logger;
            _service = service;
        }

        // Generate request: fields Count (ushort) and Length (byte)
        public async Task<GenerateResponse> Generate(ushort count, byte length)
        {
            try
            {
                var codes = await _service.GenerateAsync(count, length);
                return new GenerateResponse { Result = true, Codes = codes };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating discount codes");
                return new GenerateResponse { Result = false, Codes = new List<string>(), ErrorMessage = ex.Message };
            }
        }

        // UseCode request: field Code (string length 7–8)
        public async Task<UseCodeResponse> UseCode(string code)
        {
            var result = await _service.UseAsync(code);
            return new UseCodeResponse { Result = result };
        }
    }
}
