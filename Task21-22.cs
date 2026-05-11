public static class Task21
{
    private static string[] urls = new string[]
    {
        "https://jsonplaceholder.typicode.com/posts/1",
        "https://jsonplaceholder.typicode.com/users/1",
        "https://jsonplaceholder.typicode.com/todos/1"
    };

    public static void Run()
    {
        Console.WriteLine("Синхронные запросы");

        DateTime start = DateTime.Now;

        using (HttpClient client = new HttpClient())
        {
            foreach (string url in urls)
            {
                try
                {
                    Console.WriteLine($"Запрос к: {url}");

                    // Синхронный запрос (блокирует поток)
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    string json = response.Content.ReadAsStringAsync().Result;

                    Console.WriteLine($"Ответ: {json}\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}\n");
                }
            }
        }

        DateTime end = DateTime.Now;
        Console.WriteLine($"Время выполнения: {(end - start).TotalMilliseconds} мс");
    }
}

public static class Task22
{
    private static string[] urls = new string[]
    {
        "https://jsonplaceholder.typicode.com/posts/1",
        "https://jsonplaceholder.typicode.com/users/1",
        "https://jsonplaceholder.typicode.com/todos/1"
    };

    public static void Run()
    {
        Console.WriteLine("Асинхронные запросы");

        DateTime start = DateTime.Now;

        // Запускаем асинхронный метод и ждём
        Task.Run(async () => await MakeRequests()).Wait();

        DateTime end = DateTime.Now;
        Console.WriteLine($"Время выполнения: {(end - start).TotalMilliseconds} мс");
    }

    private static async Task MakeRequests()
    {
        using (HttpClient client = new HttpClient())
        {
            // Запускаем все запросы одновременно
            Task task1 = ProcessUrl(client, urls[0]);
            Task task2 = ProcessUrl(client, urls[1]);
            Task task3 = ProcessUrl(client, urls[2]);

            // Ждём все запросы
            await Task.WhenAll(task1, task2, task3);
        }
    }

    private static async Task ProcessUrl(HttpClient client, string url)
    {
        try
        {
            Console.WriteLine($"Запрос к: {url}");

            // Асинхронный запрос (не блокирует поток)
            HttpResponseMessage response = await client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Ответ: {json}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}\n");
        }
    }
}
