using Xunit;

public class task14tests
{
    [Fact]
    public void Test_Linear_Function_Symmetric_Interval()
    {
        var linear = (double x) => x;
        double result = DefiniteIntegral.Solve(-1, 1, linear, 1e-4, 2);
        Assert.Equal(0, result, 1e-4);
    }

    [Fact]
    public void Test_Sin_Function_Symmetric_Interval()
    {
        var sin = (double x) => Math.Sin(x);
        double result = DefiniteIntegral.Solve(-1, 1, sin, 1e-5, 8);
        Assert.Equal(0, result, 1e-4);
    }

    [Fact]
    public void Test_Linear_Function_From_0_To_5()
    {
        var linear = (double x) => x;
        double result = DefiniteIntegral.Solve(0, 5, linear, 1e-6, 8);
        Assert.Equal(12.5, result, 1e-5);
    }
}
