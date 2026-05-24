using TaskHub.Models;
using TaskItemStatus = TaskHub.Models.TaskStatus;

namespace TaskHub.Services;

/// <summary>
/// Репозиторий задач с методами фильтрации и поиска.
/// </summary>
public class TaskRepository : Repository<TaskItem>
{
  /// <summary>
  /// Делегат для фильтрации — используется в меню просмотра.
  /// </summary>
  public delegate IEnumerable<TaskItem> TaskFilter(IEnumerable<TaskItem> source);

  private static readonly Dictionary<string, TaskFilter> ViewFilters = new()
  {
    ["all"] = source => source,
    ["done"] = source => source.Where(t => t.Status == TaskItemStatus.Done),
    ["pending"] = source => source.Where(t => t.Status != TaskItemStatus.Done),
    ["high"] = source => source.Where(t => t.Priority == Priority.High),
  };

  /// <summary>
  /// Возвращает следующий свободный Id.
  /// </summary>
  public int GetNextId()
  {
    if (Count == 0)
    {
      return 1;
    }

    return GetAll().Max(t => t.Id) + 1;
  }

  /// <summary>
  /// Заменяет все задачи (используется при загрузке из файла).
  /// </summary>
  public void ReplaceAll(IEnumerable<TaskItem> tasks)
  {
    ArgumentNullException.ThrowIfNull(tasks);

    var list = tasks.ToList();
    var duplicateIds = list.GroupBy(t => t.Id)
      .Where(g => g.Count() > 1)
      .Select(g => g.Key)
      .ToList();

    if (duplicateIds.Count > 0)
    {
      throw new InvalidOperationException(
        $"Обнаружены дубликаты Id: {string.Join(", ", duplicateIds)}");
    }

    ClearInternal();
    foreach (TaskItem task in list)
    {
      Add(task);
    }
  }

  /// <summary>
  /// Применяет именованный фильтр просмотра.
  /// </summary>
  public IEnumerable<TaskItem> ApplyViewFilter(string filterKey)
  {
    if (!ViewFilters.TryGetValue(filterKey, out TaskFilter? filter))
    {
      throw new ArgumentException($"Неизвестный фильтр: {filterKey}");
    }

    return filter(GetAll());
  }

  /// <summary>
  /// Поиск по названию (частичное совпадение, без учёта регистра).
  /// </summary>
  public IEnumerable<TaskItem> SearchByTitle(string titlePart) =>
    Find(t => t.Title.Contains(titlePart, StringComparison.OrdinalIgnoreCase));

  /// <summary>
  /// Поиск по статусу.
  /// </summary>
  public IEnumerable<TaskItem> SearchByStatus(TaskItemStatus status) =>
    Find(t => t.Status == status);

  /// <summary>
  /// Поиск по приоритету.
  /// </summary>
  public IEnumerable<TaskItem> SearchByPriority(Priority priority) =>
    Find(t => t.Priority == priority);

  private void ClearInternal()
  {
    var ids = GetAll().Select(t => t.Id).ToList();
    foreach (int id in ids)
    {
      Remove(id);
    }
  }
}
