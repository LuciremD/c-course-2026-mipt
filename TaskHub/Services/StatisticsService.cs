using TaskHub.Models;
using TaskItemStatus = TaskHub.Models.TaskStatus;

namespace TaskHub.Services;

/// <summary>
/// Сервис статистики по задачам.
/// </summary>
public class StatisticsService
{
  private readonly TaskRepository _repository;

  public StatisticsService(TaskRepository repository)
  {
    _repository = repository ?? throw new ArgumentNullException(nameof(repository));
  }

  /// <summary>
  /// Общее количество задач.
  /// </summary>
  public int TotalCount => _repository.Count;

  /// <summary>
  /// Количество выполненных задач.
  /// </summary>
  public int CompletedCount =>
    _repository.Find(t => t.Status == TaskItemStatus.Done).Count();

  /// <summary>
  /// Количество просроченных задач (не Done и дедлайн прошёл).
  /// </summary>
  public int OverdueCount =>
    _repository.Find(t => t.IsOverdue).Count();

  /// <summary>
  /// Статистика по приоритетам: ключ — приоритет, значение — количество.
  /// </summary>
  public Dictionary<Priority, int> GetPriorityStatistics()
  {
    var stats = new Dictionary<Priority, int>
    {
      [Priority.Low] = 0,
      [Priority.Medium] = 0,
      [Priority.High] = 0,
    };

    foreach (TaskItem task in _repository.GetAll())
    {
      stats[task.Priority]++;
    }

    return stats;
  }

  /// <summary>
  /// Выводит сводную статистику в консоль.
  /// </summary>
  public void PrintStatistics()
  {
    const int boxWidth = 36;

    Console.WriteLine();
    PrintBoxLine('╔', '═', '╗', boxWidth);
    PrintBoxContent("СТАТИСТИКА ЗАДАЧ", boxWidth, centered: true);
    PrintBoxLine('╠', '═', '╣', boxWidth);
    PrintBoxContent($"Всего задач:  {TotalCount}", boxWidth);
    PrintBoxContent($"Выполнено:    {CompletedCount}", boxWidth);
    PrintBoxContent($"Просрочено:   {OverdueCount}", boxWidth);
    PrintBoxLine('╠', '═', '╣', boxWidth);
    PrintBoxContent("По приоритетам:", boxWidth);

    foreach (KeyValuePair<Priority, int> pair in GetPriorityStatistics())
    {
      PrintBoxContent($"  {pair.Key,-8} {pair.Value}", boxWidth);
    }

    PrintBoxLine('╚', '═', '╝', boxWidth);
    Console.WriteLine();
  }

  private static void PrintBoxLine(char left, char fill, char right, int width)
  {
    Console.WriteLine($"  {left}{new string(fill, width)}{right}");
  }

  private static void PrintBoxContent(string text, int width, bool centered = false)
  {
    if (centered)
    {
      int padding = Math.Max(0, width - text.Length);
      int left = padding / 2;
      text = new string(' ', left) + text + new string(' ', padding - left);
    }
    else
    {
      text = text.Length > width ? text[..width] : text.PadRight(width);
    }

    Console.WriteLine($"  ║{text}║");
  }
}
