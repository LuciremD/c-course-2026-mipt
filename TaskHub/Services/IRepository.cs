using TaskHub.Models;

namespace TaskHub.Services;

/// <summary>
/// Generic-интерфейс репозитория для сущностей с Id.
/// </summary>
public interface IRepository<T> where T : IIdentifiable
{
  int Count { get; }

  void Add(T item);

  bool Update(T item);

  bool Remove(int id);

  T? GetById(int id);

  IEnumerable<T> GetAll();

  IEnumerable<T> Find(Predicate<T> predicate);

  bool Contains(int id);
}
