using Xunit;

public class task14tests
{
    [Fact]
    public void Test_Linear_Function()
    {
        Func<double, double> linear = x => x;
        double result = task14.Solve(-1, 1, linear, 1e-4, 2);
        Assert.InRange(result, -1e-4, 1e-4);
    }

    [Fact]
    public void Test_Sin_Function()
    {
        Func<double, double> sin = x => Math.Sin(x);
        double result = task14.Solve(-1, 1, sin, 1e-5, 8);
        Assert.InRange(result, -1e-4, 1e-4);
    }

    [Fact]
    public void Test_Quadratic_Function()
    {
        Func<double, double> quadratic = x => x;
        double result = task14.Solve(0, 5, quadratic, 1e-6, 8);
        Assert.Equal(12.5, result, 5);
    }
}
