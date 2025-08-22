using DiscountService.CodeService;
using DiscountService.gRPCServer.Protos;
using Grpc.Core;

namespace DiscountService.gRPCServer.Service
{
    public class DiscountServiceImpl : Protos.DiscountService.DiscountServiceBase

    {
        private readonly DiscountCodeService _service;

        public DiscountServiceImpl(DiscountCodeService service)
        {
            _service = service;
        }

        public override async Task<GenerateResponse> Generate(
            GenerateRequest request, ServerCallContext context)
        {
            try
            {
                var codes = await _service.GenerateAsync((ushort)request.Count, (byte)request.Length);
                return new GenerateResponse
                {
                    Result = true,
                    Codes = { codes },
                };
            }
            catch (Exception ex)
            {
                return new GenerateResponse
                {
                    Result = false,
                    Error = ex.Message
                };
            }
        }

        public override async Task<UseCodeResponse> UseCode(
            UseCodeRequest request, ServerCallContext context)
        {
            var result = await _service.UseAsync(request.Code);
            return new UseCodeResponse { Result = (byte)result };
        }
    }
}
