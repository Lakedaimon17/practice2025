using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ScottPlot;

class Program
{
    static void Main()
    {
        Func<double, double> function = Math.Sin;
        double a = -100, b = 100;
        double expectedIntegral = 0; 

        Console.WriteLine("=== Шаг 1: Поиск минимального шага ===");
        var steps = new[] { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
        foreach (var step in steps)
        {
            var result = DefiniteIntegral.SolveSingleThreaded(a, b, function, step);
            double error = Math.Abs(result - expectedIntegral);
            Console.WriteLine($"Step: {step:E}, Result: {result:F8}, Error: {error:F8}");
        }

        double chosenStep = 1e-4;
        Console.WriteLine($"\n=== Шаг 2–4: Тестирование многопоточной версии с шагом {chosenStep:E} ===");

        int maxThreads = Environment.ProcessorCount * 2;
        var threadCounts = Enumerable.Range(1, maxThreads).ToArray();
        double[] times = new double[maxThreads];

        DefiniteIntegral.Solve(a, b, function, chosenStep, 1);
        DefiniteIntegral.SolveSingleThreaded(a, b, function, chosenStep);

        for (int i = 0; i < maxThreads; i++)
        {
            int threads = threadCounts[i];
            times[i] = MeasureTime(() => DefiniteIntegral.Solve(a, b, function, chosenStep, threads));
            Console.WriteLine($"Threads: {threads}, Time: {times[i]:F2} мс");
        }

        double singleResult = DefiniteIntegral.SolveSingleThreaded(a, b, function, chosenStep);
        double multiResult = DefiniteIntegral.Solve(a, b, function, chosenStep, 1); // 1 поток для сравнения

        if (Math.Abs(singleResult - multiResult) > 1e-4)
        {
            Console.WriteLine("Результаты различаются между однопоточной и многопоточной версией!");
        }

        double timeSingle = MeasureTime(() => DefiniteIntegral.SolveSingleThreaded(a, b, function, chosenStep));
        double bestTime = times.Min();
        double speedup = (timeSingle - bestTime) / timeSingle * 100;
        int optimalThreads = threadCounts[Array.IndexOf(times, bestTime)];

        Console.WriteLine($"\n=== Результаты ===");
        Console.WriteLine($"Однопоточное время: {timeSingle:F2} мс");
        Console.WriteLine($"Лучшее многопоточное время: {bestTime:F2} мс");
        Console.WriteLine($"Ускорение: {speedup:F2}%");
        Console.WriteLine($"Оптимальное число потоков: {optimalThreads}");

        File.WriteAllLines("results.txt", new[]
        {
            $"Выбранный шаг: {chosenStep:E}",
            $"Оптимальное число потоков: {optimalThreads}",
            $"Время однопоточной версии: {timeSingle:F2} мс",
            $"Лучшее время многопоточной версии: {bestTime:F2} мс",
            $"Ускорение: {speedup:F2}%"
        });

        Console.WriteLine("\n=== Построение графика ===");
        var plt = new ScottPlot.Plot();
        plt.Add.Scatter(threadCounts, times);
        plt.Title("Execution Time vs Thread Count");
        plt.XLabel("Number of Threads");
        plt.YLabel("Time (ms)");
        plt.SavePng("performance.png", 600, 400);
        Console.WriteLine("График сохранён как 'performance.png'");
    }

    private static double MeasureTime(Func<double> action, int iterations = 5)
    {
        double totalTime = 0;
        var watch = new Stopwatch();

        action();

        for (int i = 0; i < iterations; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            watch.Restart();
            double result = action();
            watch.Stop();

            if (double.IsNaN(result))
                Console.WriteLine("Получено значение NaN");

            totalTime += watch.Elapsed.TotalMilliseconds;
        }

        return totalTime / iterations;
    }
}
