using System.Threading.Tasks;
using Task11;
using Xunit;

namespace Task11Tests
{
    public class CalculatorTests
    {
        private const string CalculatorCode = @"
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Minus(int a, int b) => a - b;
    public int Mul(int a, int b) => a * b;
    public int Div(int a, int b) => a / b;
}";

        [Fact]
        public async Task Calculator_Methods_WorkCorrectly()
        {
            var service = new CalculatorService(CalculatorCode);
            dynamic calc = await service.CreateCalculatorAsync();

            Assert.Equal(5, calc.Add(2, 3));
            Assert.Equal(-1, calc.Minus(2, 3));
            Assert.Equal(6, calc.Mul(2, 3));
            Assert.Equal(1, calc.Div(3, 2));
        }

        [Fact]
        public async Task Calculator_DivideByZero_ThrowsException()
        {
            var service = new CalculatorService(CalculatorCode);
            dynamic calc = await service.CreateCalculatorAsync();

            var exception = Assert.Throws<DivideByZeroException>(() =>
            {
                var result = calc.Div(5, 0);
                return result;
            });

            Assert.NotNull(exception);
        }

        [Fact]
        public async Task Invalid_Code_Throws_Exception()
        {
            var invalidCode = "public class Calculator { public int Add(int a, int b) => a ++ b; }";
            var service = new CalculatorService(invalidCode);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await service.CreateCalculatorAsync());
        }
    }
}
