using System;
using System.Linq;
using System.Threading;

public static class task14
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (step <= 0 || a >= b) return 0;

        double length = b - a;
        int totalSteps = (int)Math.Ceiling(length / step);
        if (totalSteps == 0) return 0;

        double h = length / totalSteps;
        threadsNumber = Math.Clamp(threadsNumber, 1, totalSteps);

        long resultBits = 0;

        Enumerable.Range(0, threadsNumber)
            .AsParallel()
            .WithDegreeOfParallelism(threadsNumber)
            .Select(localId =>
            {
                int stepsPerThread = totalSteps / threadsNumber;
                int remainder = totalSteps % threadsNumber;
                int startStep = localId * stepsPerThread + Math.Min(localId, remainder);
                int endStep = startStep + stepsPerThread + (localId < remainder ? 1 : 0);

                return Enumerable.Range(startStep, endStep - startStep)
                    .Select(j => (function(a + j * h) + function(a + (j + 1) * h)) * h / 2)
                    .Sum();
            })
            .Aggregate(0.0, (acc, localSum) =>
            {
                long newValue = BitConverter.DoubleToInt64Bits(acc + localSum);
                Interlocked.Exchange(ref resultBits, newValue);
                return BitConverter.Int64BitsToDouble(resultBits);
            });

        return BitConverter.Int64BitsToDouble(resultBits);
    }
}
