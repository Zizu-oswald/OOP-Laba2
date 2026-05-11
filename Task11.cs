public static class Task11
{
    public static void Run()
    {
        Console.WriteLine("\nВыбор:");
        Console.WriteLine("1) Monitor (lock)");
        Console.WriteLine("2) Mutex");
        Console.WriteLine("3) Semaphore");
        Console.WriteLine("0) Выход");
        Console.Write("\nВаш выбор: ");

        string choice = Console.ReadLine()!; // ! для уверенности что не null

        switch (choice)
        {
            case "1":
                Console.WriteLine("1) Monitor (lock)");
                RunMonitor();
                break;
            case "2":
                Console.WriteLine("2) Mutex");
                RunMutex();
                break;
            case "3":
                Console.WriteLine("3) Semaphore");
                RunSemaphore();
                break;
            case "0":
                Console.WriteLine("Adios amigo");
                return;
            default:
                Console.WriteLine("Error");
                break;
        }
    }

    static bool IsPrime(int number)
    {
        if (number < 2) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;

        for (int i = 3; i * i <= number; i += 2)
            if (number % i == 0) return false;

        return true;
    }

    static void RunMonitor()
    {
        int total = 0;
        object locker = new object();
        DateTime start = DateTime.Now;

        Thread[] threads = new Thread[4];
        for (int i = 0; i < 4; i++)
        {
            int threadNum = i;
            threads[i] = new Thread(() =>
            {
                int startNum = threadNum * 2500 + 1;
                int endNum = startNum + 2499;

                for (int num = startNum; num <= endNum; num++)
                {
                    Console.WriteLine($"Поток {threadNum + 1} проверяет {num}");

                    if (IsPrime(num))
                    {
                        Console.WriteLine($"  Поток {threadNum + 1} нашёл простое {num}");
                        lock (locker) { total++; }
                    }
                }
            });
            threads[i].Start();
        }

        foreach (var t in threads) t.Join();

        Console.WriteLine($"\nВсего простых чисел: {total}");
        Console.WriteLine($"Время: {(DateTime.Now - start).TotalMilliseconds} мс");
    }

    static void RunMutex()
    {
        int total = 0;
        Mutex mutex = new Mutex();
        DateTime start = DateTime.Now;

        Thread[] threads = new Thread[4];
        for (int i = 0; i < 4; i++)
        {
            int threadNum = i;
            threads[i] = new Thread(() =>
            {
                int startNum = threadNum * 2500 + 1;
                int endNum = startNum + 2499;

                for (int num = startNum; num <= endNum; num++)
                {
                    Console.WriteLine($"Поток {threadNum + 1} проверяет {num}");

                    if (IsPrime(num))
                    {
                        Console.WriteLine($"  Поток {threadNum + 1} нашёл простое {num}");
                        mutex.WaitOne();
                        total++;
                        mutex.ReleaseMutex();
                    }
                }
            });
            threads[i].Start();
        }

        foreach (var t in threads) t.Join();

        Console.WriteLine($"\nВсего простых чисел: {total}");
        Console.WriteLine($"Время: {(DateTime.Now - start).TotalMilliseconds} мс");
    }

    static void RunSemaphore()
    {
        int total = 0;
        Semaphore semaphore = new Semaphore(1, 1);
        DateTime start = DateTime.Now;

        Thread[] threads = new Thread[4];
        for (int i = 0; i < 4; i++)
        {
            int threadNum = i;
            threads[i] = new Thread(() =>
            {
                int startNum = threadNum * 2500 + 1;
                int endNum = startNum + 2499;

                for (int num = startNum; num <= endNum; num++)
                {
                    Console.WriteLine($"Поток {threadNum + 1} проверяет {num}");

                    if (IsPrime(num))
                    {
                        Console.WriteLine($"  Поток {threadNum + 1} нашёл простое {num}");
                        semaphore.WaitOne();
                        total++;
                        semaphore.Release();
                    }
                }
            });
            threads[i].Start();
        }

        foreach (var t in threads) t.Join();

        Console.WriteLine($"\nВсего простых чисел: {total}");
        Console.WriteLine($"Время: {(DateTime.Now - start).TotalMilliseconds} мс");
    }
}
