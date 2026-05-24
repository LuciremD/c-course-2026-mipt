using System;
using System.Collections.Generic;

public class Product
{
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
}

internal static class Program
{
    private static void Main()
    {
        TestDistinct();
        TestGroupBy();
        TestMerge();
        TestMaxBy();
    }

    private static void TestDistinct()
    {
        var numbers = new List<int> { 1, 2, 2, 3, 1, 4, 3, 5 };
        List<int> distinctNumbers = CollectionUtils.Distinct(numbers);
        Console.WriteLine("Distinct (int): " + string.Join(", ", distinctNumbers));

        var words = new List<string> { "apple", "banana", "apple", "cherry", "banana", "date" };
        List<string> distinctWords = CollectionUtils.Distinct(words);
        Console.WriteLine("Distinct (string): " + string.Join(", ", distinctWords));
        Console.WriteLine();
    }

    private static void TestGroupBy()
    {
        var words = new List<string>
        {
            "cat", "dog", "elephant", "ant", "bear", "ox", "tiger"
        };

        Dictionary<int, List<string>> byLength = CollectionUtils.GroupBy(words, w => w.Length);

        Console.WriteLine("GroupBy (длина слова):");
        foreach (KeyValuePair<int, List<string>> group in byLength)
        {
            Console.WriteLine($"  {group.Key}: {string.Join(", ", group.Value)}");
        }

        Console.WriteLine();
    }

    private static void TestMerge()
    {
        var text1Counts = new Dictionary<string, int>
        {
            ["hello"] = 2,
            ["world"] = 1,
            ["csharp"] = 3
        };

        var text2Counts = new Dictionary<string, int>
        {
            ["hello"] = 1,
            ["world"] = 2,
            ["linq"] = 1
        };

        Dictionary<string, int> merged = CollectionUtils.Merge(
            text1Counts,
            text2Counts,
            (a, b) => a + b);

        Console.WriteLine("Merge (сумма счётчиков):");
        foreach (KeyValuePair<string, int> pair in merged)
        {
            Console.WriteLine($"  {pair.Key}: {pair.Value}");
        }

        Console.WriteLine();
    }

    private static void TestMaxBy()
    {
        var products = new List<Product>
        {
            new() { Name = "Laptop", Price = 125000m },
            new() { Name = "Mouse", Price = 1200m },
            new() { Name = "Monitor", Price = 45000m },
            new() { Name = "Cable", Price = 500m }
        };

        Product mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
        Console.WriteLine($"MaxBy: {mostExpensive.Name} — {mostExpensive.Price}");
    }
}
