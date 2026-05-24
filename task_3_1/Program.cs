using System;
using System.Collections.Generic;

internal static class Program
{
    private static void Main()
    {
        TestProducts();
        TestUsers();
    }

    private static void TestProducts()
    {
        var productRepository = new Repository<Product>();
        productRepository.Add(new Product { Id = 1, Name = "Laptop", Price = 125000m });
        productRepository.Add(new Product { Id = 2, Name = "Mouse", Price = 1200m });
        productRepository.Add(new Product { Id = 3, Name = "Cable", Price = 500m });

        Product? productById = productRepository.GetById(2);
        Console.WriteLine(productById is null
            ? "Продукт с Id=2 не найден"
            : $"Найден продукт: {productById.Name}, цена {productById.Price}");

        IReadOnlyList<Product> expensiveProducts = productRepository.Find(p => p.Price > 1000m);
        Console.WriteLine("Продукты дороже 1000:");
        foreach (Product product in expensiveProducts)
        {
            Console.WriteLine($"- #{product.Id}: {product.Name} ({product.Price})");
        }

        try
        {
            productRepository.Add(new Product { Id = 2, Name = "Duplicate Mouse", Price = 1500m });
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Ошибка при добавлении дубликата: {ex.Message}");
        }

        bool removed = productRepository.Remove(3);
        Console.WriteLine($"Удаление продукта с Id=3: {(removed ? "успешно" : "не найден")}");
        Console.WriteLine($"Всего продуктов: {productRepository.Count}");
        Console.WriteLine();
    }

    private static void TestUsers()
    {
        var userRepository = new Repository<User>();
        userRepository.Add(new User { Id = 1, Name = "Alice", Email = "alice@example.com" });
        userRepository.Add(new User { Id = 2, Name = "Bob", Email = "bob@example.com" });

        IReadOnlyList<User> usersWithA = userRepository.Find(u => u.Name.Contains('a', StringComparison.OrdinalIgnoreCase));
        Console.WriteLine("Пользователи с буквой 'a' в имени:");
        foreach (User user in usersWithA)
        {
            Console.WriteLine($"- #{user.Id}: {user.Name} ({user.Email})");
        }

        Console.WriteLine($"Всего пользователей: {userRepository.Count}");
    }
}
