using CarConsoleApp;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Введите марку автомобиля: Tesla, BMW или Toyota.");
Console.WriteLine("Для выхода введите done.");
Console.WriteLine();

while (true)
{
    Console.Write("Марка> ");
    string? line = Console.ReadLine();
    if (line is null)
    {
        Console.WriteLine("Ввод завершён.");
        break;
    }

    line = line.Trim();
    if (string.Equals(line, "done", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Завершение работы.");
        break;
    }

    if (!Enum.TryParse<CarType>(line, ignoreCase: true, out CarType type))
    {
        Console.WriteLine($"Неизвестная марка «{line}». Допустимы: Tesla, BMW, Toyota или done.");
        continue;
    }

    try
    {
        ICar car = CarFactory.CreateCar(type);
        Console.WriteLine(car.GetDescription());
    }
    catch (ArgumentOutOfRangeException ex)
    {
        Console.WriteLine($"Ошибка фабрики: {ex.Message}");
    }

    Console.WriteLine();
}
