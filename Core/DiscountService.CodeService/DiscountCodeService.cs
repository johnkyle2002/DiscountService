using DiscountService.Common.Enumerator;
using DiscountService.Common.Extension;

namespace DiscountService.CodeService
{
    public class DiscountCodeService
    {
        private readonly ICodeStore _store;


        public DiscountCodeService(ICodeStore store)
        {
            _store = store;
            // Fire and forget initialization (safe in ASP.NET startup); alternatively call in Program
            _store.InitializeAsync().GetAwaiter().GetResult();
        }


        public async Task<List<string>> GenerateAsync(ushort count, byte length, CancellationToken ct = default)
        {
            if (count == 0) return new List<string>();
            if (count > 2000) throw new ArgumentOutOfRangeException(nameof(count), "Max 2000 per request");
            if (length < 7 || length > 8) throw new ArgumentOutOfRangeException(nameof(length), "Length must be 7–8");


            var result = new List<string>(count);


            // Generate until we have the requested unique codes
            while (result.Count < count)
            {
                var code = SecureCodeGenerator.Generate(length);


                // Ensure no duplicates in-batch or persisted
                if (result.Contains(code)) continue;


                var added = await _store.TryAddAsync(code, ct);
                if (added) result.Add(code);
            }


            // Persist happens lazily on MarkUsed; for safety, we can force a store persist by toggling a no-op
            // Here we keep file writes minimal; codes are already in-memory and will be persisted on next MarkUsed
            // If you want immediate durability on generate, you can extend ICodeStore with PersistAsync() and call it here.


            return result;
        }


        public async Task<ResultEnum.Result> UseAsync(string code, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(code)) return ResultEnum.Result.NotFound;


            var (found, used) = await _store.TryGetAsync(code, ct);
            if (!found) return ResultEnum.Result.NotFound; 
            if (used) return ResultEnum.Result.AlreadyUsed; 


            var ok = await _store.MarkUsedAsync(code, ct);
            return ok ? ResultEnum.Result.Success : ResultEnum.Result.AlreadyUsed; 
        }
    }
}
