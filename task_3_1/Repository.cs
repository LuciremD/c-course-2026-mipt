using System;
using System.Collections.Generic;
using System.Linq;

public interface IEntity
{
    int Id { get; }
}

public class Repository<T> where T : IEntity
{
    private readonly Dictionary<int, T> _itemsById = new();

    public int Count => _itemsById.Count;

    public void Add(T item)
    {
        if (!_itemsById.TryAdd(item.Id, item))
        {
            throw new InvalidOperationException($"Элемент с Id={item.Id} уже существует.");
        }
    }

    public bool Remove(int id)
    {
        return _itemsById.Remove(id);
    }

    public T? GetById(int id)
    {
        return _itemsById.TryGetValue(id, out T? item) ? item : default;
    }

    public IReadOnlyList<T> GetAll()
    {
        return _itemsById.Values.ToList();
    }

    public IReadOnlyList<T> Find(Predicate<T> predicate)
    {
        return _itemsById.Values.Where(item => predicate(item)).ToList();
    }
}

public class Product : IEntity
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
}

public class User : IEntity
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
