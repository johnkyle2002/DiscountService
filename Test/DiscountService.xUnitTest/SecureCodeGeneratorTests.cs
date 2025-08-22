using DiscountService.Common.Extension;
using System.Collections.Concurrent;
using System.Diagnostics;
using Xunit.Abstractions;

namespace DiscountService.xUnitTest
{
    public class SecureCodeGeneratorTests
    {
        private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        public ITestOutputHelper _outputHelper { get; }

        public SecureCodeGeneratorTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }


        [Theory]
        [InlineData(6)]
        [InlineData(9)]
        [InlineData(0)]
        public void Generate_InvalidLength_ThrowsException(int invalidLength)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SecureCodeGenerator.Generate(invalidLength));
        }

        [Theory]
        [InlineData(7)]
        [InlineData(8)]
        public void Generate_ValidLength_ReturnsCorrectLength(int validLength)
        {
            var code = SecureCodeGenerator.Generate(validLength);

            Assert.Equal(validLength, code.Length);
        }

        [Theory]
        [InlineData(7)]
        [InlineData(8)]
        public void Generate_ValidLength_ReturnsOnlyAllowedCharacters(int validLength)
        {
            var code = SecureCodeGenerator.Generate(validLength);

            Assert.All(code, c => Assert.Contains(c, Alphabet));
        }

        [Fact]
        public void Generate_MultipleCalls_ProduceDifferentResults()
        {
            var code1 = SecureCodeGenerator.Generate(8);
            var code2 = SecureCodeGenerator.Generate(8);

            Assert.NotEqual(code1, code2); // Very low probability of collision
        }

        [Fact]
        public void Generate_2000Code_WithParallelism()
        {
            var sw = Stopwatch.StartNew();
            ConcurrentBag<string> codes = new ConcurrentBag<string>();
            Enumerable.Range(1, 2000).ToList().AsParallel().ForAll(_ =>
            {
                var code = SecureCodeGenerator.Generate(8);
                codes.Add(code);
            });
            sw.Stop();
            _outputHelper.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");
            var codeLenght = codes.Distinct().Count(); // Ensure uniqueness

            Assert.Equal(2000, codeLenght);
            foreach (var item in codes)
            {
                Assert.All(item, c => Assert.Contains(c, Alphabet));
            } 
        }

        [Fact]
        public void Generate_2000Code()
        {

            var sw = Stopwatch.StartNew();
            ConcurrentBag<string> codes = new ConcurrentBag<string>();
            Enumerable.Range(1, 2000).ToList().ForEach(_ =>
            {
                var code = SecureCodeGenerator.Generate(8);
                codes.Add(code);
            });
            sw.Stop();
            _outputHelper.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");

            Assert.Equal(2000, codes.Count);
            foreach (var item in codes)
            {
                Assert.All(item, c => Assert.Contains(c, Alphabet));
            } 
        }

        [Fact]
        public void Generate_2000Codes_AllValidAndUnique()
        {
            const int count = 2000;
            var codes = new List<string>(count);

            for (int i = 0; i < count; i++)
            {
                var code = SecureCodeGenerator.Generate(8);
                codes.Add(code);

                // Assert valid length
                Assert.Equal(8, code.Length);

                // Assert all chars are from alphabet
                Assert.All(code, c => Assert.Contains(c, Alphabet));
            }

            // Assert no duplicates (high probability, but not guaranteed mathematically)
            var uniqueCount = codes.Distinct().Count();
            Assert.Equal(count, uniqueCount);
        }
    }

}