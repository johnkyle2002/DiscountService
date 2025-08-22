namespace DiscountService.Common.Enumerator
{
    public class ResultEnum
    {
        public enum Result : byte
        {
            Success = 0,
            NotFound = 1,
            AlreadyUsed = 2
        }
    }
}
