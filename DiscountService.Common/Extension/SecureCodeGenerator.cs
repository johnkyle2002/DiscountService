using System.Security.Cryptography;
using System.Text;

namespace DiscountService.Common.Extension
{
    public static class SecureCodeGenerator
    {
        private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // omit confusing chars: I, O, 0, 1


        public static string Generate(int length)
        {
            if (length < 7 || length > 8) throw new ArgumentOutOfRangeException(nameof(length), "Length must be 7–8");


            var bytes = new byte[length];
            var sb = new StringBuilder(length);

            // Use rejection sampling to avoid modulo bias
            var max = byte.MaxValue - ((byte.MaxValue + 1) % Alphabet.Length);
            using var rng = RandomNumberGenerator.Create();
            var buf = new byte[1];
            for (int i = 0; i < length; i++)
            {
                byte b;
                do { rng.GetBytes(buf); b = buf[0]; } while (b > max);
                sb.Append(Alphabet[b % Alphabet.Length]);
            }
            return sb.ToString();
        }
    }
}
