using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Task11
{
    public class CalculatorService
    {
        private readonly string _calculatorCode;

        public CalculatorService(string calculatorCode)
        {
            _calculatorCode = calculatorCode;
        }

        public async Task<dynamic> CreateCalculatorAsync()
        {
            var code = $@"
{_calculatorCode}

public class Wrapper {{
    private readonly Calculator _calculator = new Calculator();

    public int Add(int a, int b) => _calculator.Add(a, b);
    public int Minus(int a, int b) => _calculator.Minus(a, b);
    public int Mul(int a, int b) => _calculator.Mul(a, b);
    public int Div(int a, int b) => _calculator.Div(a, b);
}}
return new Wrapper();";

            var script = CSharpScript.Create<object>(
                code,
                options: ScriptOptions.Default.AddReferences(typeof(object).Assembly));

            try
            {
                var result = await script.RunAsync();
                return result.ReturnValue;
            }
            catch (CompilationErrorException ex)
            {
                throw new InvalidOperationException("Ошибка компиляции скрипта", new Exception(string.Join("\n", ex.Diagnostics)));
            }
        }
    }
}
