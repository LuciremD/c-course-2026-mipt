using TaskHub.Models;

namespace TaskHub.Services;

/// <summary>
/// Generic-репозиторий на основе Dictionary&lt;int, T&gt;.
/// </summary>
public class Repository<T> : IRepository<T> where T : IIdentifiable
{
  private readonly Dictionary<int, T> _items = new();

  public int Count => _items.Count;

  /// <summary>
  /// Добавляет новую сущность. Выбрасывает исключение при дубликате Id.
  /// </summary>
  public void Add(T item)
  {
    ArgumentNullException.ThrowIfNull(item);

    if (_items.ContainsKey(item.Id))
    {
      throw new InvalidOperationException($"Сущность с Id={item.Id} уже существует.");
    }

    _items[item.Id] = item;
  }

  /// <summary>
  /// Обновляет существующую сущность по Id.
  /// </summary>
  public bool Update(T item)
  {
    ArgumentNullException.ThrowIfNull(item);

    if (!_items.ContainsKey(item.Id))
    {
      return false;
    }

    _items[item.Id] = item;
    return true;
  }

  public bool Remove(int id) => _items.Remove(id);

  public T? GetById(int id) =>
    _items.TryGetValue(id, out T? item) ? item : default;

  public IEnumerable<T> GetAll() => _items.Values;

  /// <summary>
  /// Поиск сущностей по делегату-предикату.
  /// </summary>
  public IEnumerable<T> Find(Predicate<T> predicate)
  {
    ArgumentNullException.ThrowIfNull(predicate);
    return _items.Values.Where(item => predicate(item));
  }

  public bool Contains(int id) => _items.ContainsKey(id);
}
