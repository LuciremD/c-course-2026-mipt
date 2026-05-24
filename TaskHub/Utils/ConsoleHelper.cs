using System.Globalization;
using TaskHub.Models;
using TaskItemStatus = TaskHub.Models.TaskStatus;

namespace TaskHub.Utils;

/// <summary>
/// Статический класс с утилитами для работы с консолью.
/// </summary>
public static class ConsoleHelper
{
  public static void PrintHeader()
  {
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("  ╔══════════════════════════════════════════════╗");
    Console.WriteLine("  ║              T A S K H U B                   ║");
    Console.WriteLine("  ║         Менеджер задач (.NET 8)              ║");
    Console.WriteLine("  ╚══════════════════════════════════════════════╝");
    Console.ResetColor();
    Console.WriteLine();
  }

  public static void ShowMainMenu()
  {
    PrintHeader();
    Console.WriteLine("  ┌──────────────────────────────────────────────┐");
    Console.WriteLine("  │  ГЛАВНОЕ МЕНЮ                                │");
    Console.WriteLine("  ├──────────────────────────────────────────────┤");
    Console.WriteLine("  │  1. Создать задачу                           │");
    Console.WriteLine("  │  2. Просмотреть задачи                       │");
    Console.WriteLine("  │  3. Редактировать задачу                     │");
    Console.WriteLine("  │  4. Удалить задачу                           │");
    Console.WriteLine("  │  5. Поиск задач                              │");
    Console.WriteLine("  │  6. Статистика                               │");
    Console.WriteLine("  │  7. Сохранить в файл                         │");
    Console.WriteLine("  │  8. Загрузить из файла                       │");
    Console.WriteLine("  │  0. Выход                                    │");
    Console.WriteLine("  └──────────────────────────────────────────────┘");
    Console.Write("  Выберите пункт: ");
  }

  public static void ShowViewMenu()
  {
    Console.WriteLine();
    Console.WriteLine("  ┌─ Просмотр задач ─────────────────────────────┐");
    Console.WriteLine("  │  1. Все задачи                               │");
    Console.WriteLine("  │  2. Выполненные                              │");
    Console.WriteLine("  │  3. Невыполненные                            │");
    Console.WriteLine("  │  4. С высоким приоритетом                    │");
    Console.WriteLine("  │  0. Назад                                    │");
    Console.WriteLine("  └──────────────────────────────────────────────┘");
    Console.Write("  Выберите фильтр: ");
  }

  public static void ShowSearchMenu()
  {
    Console.WriteLine();
    Console.WriteLine("  ┌─ Поиск ──────────────────────────────────────┐");
    Console.WriteLine("  │  1. По названию                              │");
    Console.WriteLine("  │  2. По статусу                               │");
    Console.WriteLine("  │  3. По приоритету                          │");
    Console.WriteLine("  │  0. Назад                                    │");
    Console.WriteLine("  └──────────────────────────────────────────────┘");
    Console.Write("  Выберите тип поиска: ");
  }

  public static void PrintTasks(IEnumerable<TaskItem> tasks, string title)
  {
    var list = tasks.ToList();
    Console.WriteLine();
    Console.WriteLine($"  ── {title} ({list.Count}) ──");

    if (list.Count == 0)
    {
      Console.WriteLine("  (список пуст)");
      return;
    }

    foreach (TaskItem task in list)
    {
      ConsoleColor color = task.IsOverdue ? ConsoleColor.Red
        : task.Status == TaskItemStatus.Done ? ConsoleColor.Green
        : ConsoleColor.White;

      Console.ForegroundColor = color;
      Console.WriteLine($"  {task}");
      Console.WriteLine($"      {task.Description}");
      Console.ResetColor();
    }
  }

  public static void PrintTaskDetails(TaskItem task)
  {
    Console.WriteLine();
    Console.WriteLine("  ── Детали задачи ──");
    Console.WriteLine($"  Id:          {task.Id}");
    Console.WriteLine($"  Название:    {task.Title}");
    Console.WriteLine($"  Описание:    {task.Description}");
    Console.WriteLine($"  Приоритет:   {task.Priority}");
    Console.WriteLine($"  Статус:      {task.Status}");
    Console.WriteLine($"  Дедлайн:     {task.Deadline:dd.MM.yyyy HH:mm}");
  }

  public static string ReadRequiredString(string prompt)
  {
    while (true)
    {
      Console.Write($"  {prompt}: ");
      string? input = Console.ReadLine()?.Trim();

      if (!string.IsNullOrWhiteSpace(input))
      {
        return input;
      }

      WriteError("Поле не может быть пустым. Попробуйте снова.");
    }
  }

  public static string ReadOptionalString(string prompt, string currentValue)
  {
    Console.Write($"  {prompt} [{currentValue}]: ");
    string? input = Console.ReadLine()?.Trim();
    return string.IsNullOrWhiteSpace(input) ? currentValue : input;
  }

  public static int ReadInt(string prompt, int? min = null, int? max = null)
  {
    while (true)
    {
      Console.Write($"  {prompt}: ");
      string? input = Console.ReadLine();

      if (int.TryParse(input, out int value))
      {
        if (min.HasValue && value < min.Value)
        {
          WriteError($"Значение должно быть не меньше {min.Value}.");
          continue;
        }

        if (max.HasValue && value > max.Value)
        {
          WriteError($"Значение должно быть не больше {max.Value}.");
          continue;
        }

        return value;
      }

      WriteError("Введите корректное целое число.");
    }
  }

  public static DateTime ReadDateTime(string prompt)
  {
    // Явный формат — не зависит от локали системы (en-US не понимает 25.05.2026).
    string[] formats = ["dd.MM.yyyy HH:mm", "dd.MM.yyyy H:mm"];

    while (true)
    {
      Console.Write($"  {prompt} (дд.мм.гггг чч:мм): ");
      string? input = Console.ReadLine()?.Trim();

      if (DateTime.TryParseExact(
            input,
            formats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime value))
      {
        return value;
      }

      WriteError("Неверный формат даты. Пример: 25.12.2026 18:00");
    }
  }

  public static Priority ReadPriority(string prompt)
  {
    Console.WriteLine($"  {prompt}: 0=Low, 1=Medium, 2=High");
    int value = ReadInt("  Введите число", 0, 2);
    return (Priority)value;
  }

  public static TaskItemStatus ReadTaskStatus(string prompt)
  {
    Console.WriteLine($"  {prompt}: 0=New, 1=InProgress, 2=Done");
    int value = ReadInt("  Введите число", 0, 2);
    return (TaskItemStatus)value;
  }

  public static void WriteSuccess(string message)
  {
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"  ✓ {message}");
    Console.ResetColor();
  }

  public static void WriteError(string message)
  {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"  ✗ {message}");
    Console.ResetColor();
  }

  public static void WriteWarning(string message)
  {
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"  ! {message}");
    Console.ResetColor();
  }

  public static void Pause()
  {
    Console.WriteLine();
    Console.Write("  Нажмите Enter для продолжения...");
    Console.ReadLine();
  }
}
