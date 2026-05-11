public static class Task12
{
    private const int SETS_COUNT = 15;
    private const int NUMBERS_PER_SET = 100;
    private const int MAX_THREADS = 3;
    private const string DATA_FILE = "numbers.txt";

    private static List<string> _results = new List<string>();
    private static int _totalSum = 0;

    private static readonly object _logLock = new object();
    private static Mutex _totalMutex = new Mutex();
    private static Semaphore? _semaphore; // ? - can be nullable

    public static void Run()
    {
        Console.WriteLine("Обработка наборов чисел");

        List<int[]> numberSets = LoadOrGenerateData();

        _semaphore = new Semaphore(MAX_THREADS, MAX_THREADS);

        DateTime startTime = DateTime.Now;

        Thread[] threads = new Thread[SETS_COUNT];
        for (int i = 0; i < SETS_COUNT; i++)
        {
            int setIndex = i;
            int[] numbers = numberSets[i];

            threads[i] = new Thread(() => ProcessSet(setIndex + 1, numbers));
            threads[i].Start();
        }

        foreach (Thread t in threads)
        {
            t.Join();
        }

        DateTime endTime = DateTime.Now;

        Console.WriteLine("\n Результаты:");
        foreach (string result in _results)
        {
            Console.WriteLine(result);
        }

        Console.WriteLine($"\nОбщий итог: {_totalSum}");
        Console.WriteLine($"Время выполнения: {(endTime - startTime).TotalMilliseconds} мс");
    }

    private static void ProcessSet(int setNumber, int[] numbers)
    {
        _semaphore!.WaitOne();
        try
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            int sum = 0;
            foreach (int num in numbers)
            {
                sum += num;
            }

            lock (_logLock)
            {
                _results.Add($"Набор {setNumber}: сумма = {sum}, поток {threadId}");
            }

            _totalMutex.WaitOne();
            try
            {
                _totalSum += sum;
            }
            finally
            {
                _totalMutex.ReleaseMutex();
            }

            Console.WriteLine($"Поток {threadId} обработал набор {setNumber}, сумма = {sum}");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static List<int[]> LoadOrGenerateData()
    {
        if (File.Exists(DATA_FILE))
        {
            Console.WriteLine($"Загрузка данных из файла");
            List<int[]> sets = new List<int[]>();
            string[] lines = File.ReadAllLines(DATA_FILE);

            foreach (string line in lines)
            {
                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int[] numbers = Array.ConvertAll(parts, int.Parse);
                sets.Add(numbers);
            }

            if (sets.Count == SETS_COUNT)
            {
                Console.WriteLine("Успешно\n");
                return sets;
            }
        }

        Console.WriteLine("Генерация наборов чисел");
        Random rand = new Random();
        List<int[]> newSets = new List<int[]>();

        using (StreamWriter writer = new StreamWriter(DATA_FILE))
        {
            for (int i = 0; i < SETS_COUNT; i++)
            {
                int[] numbers = new int[NUMBERS_PER_SET];
                for (int j = 0; j < NUMBERS_PER_SET; j++)
                {
                    numbers[j] = rand.Next(1, 101);
                }
                newSets.Add(numbers);

                writer.WriteLine(string.Join(" ", numbers));
            }
        }

        Console.WriteLine($"Сгенерировано \n");
        return newSets;
    }
}
