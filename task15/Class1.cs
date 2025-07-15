using System;
using System.Threading;

public static class DefiniteIntegral
{
    private const int SEQUENTIAL_THRESHOLD = 1000;

    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (step <= 0 || a >= b) return 0;
        double length = b - a;
        int totalSteps = (int)Math.Ceiling(length / step);
        if (totalSteps == 0) return 0;
        double h = length / totalSteps;

        threadsNumber = Math.Min(threadsNumber, totalSteps);

        if (threadsNumber == 1)
        {
            return RecursiveSum(a, h, function, 0, totalSteps);
        }

        var partialResults = new double[threadsNumber];
        var barrier = new Barrier(threadsNumber);
        object lockObj = new object();

        ManualResetEvent[] waitHandles = new ManualResetEvent[threadsNumber];

        for (int i = 0; i < threadsNumber; i++)
        {
            int threadIndex = i;
            waitHandles[i] = new ManualResetEvent(false);

            new Thread(() =>
            {
                try
                {
                    var (startStep, endStep) = GetStepsRange(threadIndex, threadsNumber, totalSteps);
                    double localResult = RecursiveSum(a, h, function, startStep, endStep);

                    lock (lockObj)
                    {
                        partialResults[threadIndex] = localResult;
                    }
                }
                finally
                {
                    barrier.SignalAndWait();
                    waitHandles[threadIndex].Set();
                }
            }).Start();
        }

        WaitHandle.WaitAll(waitHandles);

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

    public static double SolveSingleThreaded(double a, double b, Func<double, double> function, double step)
    {
        if (step <= 0 || a >= b) return 0;
        double length = b - a;
        int totalSteps = (int)Math.Ceiling(length / step);
        if (totalSteps == 0) return 0;
        double h = length / totalSteps;
        return RecursiveSum(a, h, function, 0, totalSteps);
    }

    private static double RecursiveSum(double a, double h, Func<double, double> function, int start, int end)
    {
        int span = end - start;

        if (span <= SEQUENTIAL_THRESHOLD)
        {
            double sum = 0;
            for (int i = start; i < end; i++)
            {
                double x1 = a + i * h;
                double x2 = a + (i + 1) * h;
                sum += (function(x1) + function(x2)) * h / 2;
            }
            return sum;
        }

        int mid = (start + end) / 2;
        double left = RecursiveSum(a, h, function, start, mid);
        double right = RecursiveSum(a, h, function, mid, end);
        return left + right;
    }
}
