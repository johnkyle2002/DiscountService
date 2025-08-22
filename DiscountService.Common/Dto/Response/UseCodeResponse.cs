using DiscountService.Common.Enumerator;

namespace DiscountService.Common.Dto.Response
{
    public class UseCodeResponse
    {
        // 0=Success, 1=NotFound, 2=AlreadyUsed
        public ResultEnum.Result Result { get; set; }
    }
}
