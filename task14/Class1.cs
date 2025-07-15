using System;
using System.Linq;
using System.Threading;

public static class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (step <= 0 || a >= b) return 0;

        double length = b - a;
        int totalSteps = (int)Math.Ceiling(length / step);
        if (totalSteps == 0) return 0;

        double h = length / totalSteps;
        threadsNumber = Math.Min(threadsNumber, totalSteps);

        var partialResults = new double[threadsNumber];
        var barrier = new Barrier(threadsNumber + 1);

        Enumerable.Range(0, threadsNumber)
            .Select(threadIndex => new Thread(() =>
            {
                var (startStep, endStep) = GetStepsRange(threadIndex, threadsNumber, totalSteps);
                partialResults[threadIndex] = RecursiveSum(a, h, function, startStep, endStep);
                barrier.SignalAndWait();
            }))
            .ToList()
            .ForEach(thread => thread.Start());

        barrier.SignalAndWait();
        return partialResults.Sum();
    }

    private static (int startStep, int endStep) GetStepsRange(int threadIndex, int threadsNumber, int totalSteps)
    {
        int stepsPerThread = totalSteps / threadsNumber;
        int remainder = totalSteps % threadsNumber;
        int startStep = threadIndex * stepsPerThread + Math.Min(threadIndex, remainder);
        int endStep = startStep + stepsPerThread + (threadIndex < remainder ? 1 : 0);
        return (startStep, endStep);
    }

    private static double RecursiveSum(double a, double h, Func<double, double> function, int start, int end)
    {
        if (start + 1 >= end)
        {
            double x1 = a + start * h;
            double x2 = a + end * h;
            return (function(x1) + function(x2)) * h / 2;
        }

        int mid = (start + end) / 2;
        return RecursiveSum(a, h, function, start, mid) +
               RecursiveSum(a, h, function, mid, end);
    }
}
