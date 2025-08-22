using DiscountService.Common.Dto;
using System.Collections.Concurrent;
using System.Text.Json;

namespace DiscountService.CodeService
{
    public interface ICodeStore
    {
        Task InitializeAsync(CancellationToken ct = default);
        Task<bool> TryAddAsync(string code, CancellationToken ct = default);
        Task<bool> ExistsAsync(string code, CancellationToken ct = default);
        Task<(bool found, bool used)> TryGetAsync(string code, CancellationToken ct = default);
        Task<bool> MarkUsedAsync(string code, CancellationToken ct = default);
    }


    public sealed class FileCodeStore : ICodeStore
    {
        private readonly string _filePath;
        private readonly SemaphoreSlim _ioLock = new(1, 1);
        private readonly ConcurrentDictionary<string, DiscountCode> _codes = new(StringComparer.Ordinal);


        public FileCodeStore(string filePath)
        {
            _filePath = filePath;
        }


        public async Task InitializeAsync(CancellationToken ct = default)
        {
            if (!File.Exists(_filePath))
            {
                await PersistAsync(ct);
                return;
            }
            await _ioLock.WaitAsync(ct);
            try
            {
                using var fs = File.OpenRead(_filePath);
                var list = await JsonSerializer.DeserializeAsync<List<DiscountCode>>(fs, cancellationToken: ct) ?? new();
                foreach (var dc in list)
                    _codes[dc.Code] = dc;
            }
            finally { _ioLock.Release(); }
        }


        public Task<bool> TryAddAsync(string code, CancellationToken ct = default)
        {
            var added = _codes.TryAdd(code, new DiscountCode(code, Used: false));
            return Task.FromResult(added);
        }

        public Task<bool> ExistsAsync(string code, CancellationToken ct = default)
        => Task.FromResult(_codes.ContainsKey(code));

        public Task<(bool found, bool used)> TryGetAsync(string code, CancellationToken ct = default)
        {
            if (_codes.TryGetValue(code, out var dc))
                return Task.FromResult((true, dc.Used));
            return Task.FromResult((false, false));
        }

        public async Task<bool> MarkUsedAsync(string code, CancellationToken ct = default)
        {
            if (_codes.TryGetValue(code, out var dc))
            {
                if (dc.Used) return false;
                _codes[code] = dc with { Used = true };
                await PersistAsync(ct);
                return true;
            }
            return false;
        }
        
        private async Task PersistAsync(CancellationToken ct)
        {
            await _ioLock.WaitAsync(ct);
            try
            {
                var list = _codes.Values.OrderBy(c => c.Code).ToList();
                var tmp = _filePath + ".tmp";
                await using (var fs = File.Create(tmp))
                {
                    await JsonSerializer.SerializeAsync(fs, list, new JsonSerializerOptions { WriteIndented = false }, ct);
                }
                File.Copy(tmp, _filePath, overwrite: true);
                File.Delete(tmp);
            }
            finally { _ioLock.Release(); }
        }
    }
}
