using TaskHub.Models;
using TaskHub.Services;
using TaskHub.Utils;
using TaskItemStatus = TaskHub.Models.TaskStatus;

const string DataFileName = "tasks.json";

TaskRepository repository = new();
StatisticsService statisticsService = new(repository);

using FileService fileService = new(DataFileName);
using DeadlineMonitor deadlineMonitor = new(() => repository.GetAll());

deadlineMonitor.OnOverdue += task =>
{
  Console.ForegroundColor = ConsoleColor.Yellow;
  Console.WriteLine();
  Console.WriteLine($"  ⚠ ПРОСРОЧЕНА задача [{task.Id}] «{task.Title}» — дедлайн был {task.Deadline:dd.MM.yyyy HH:mm}");
  Console.ResetColor();
};

SeedSampleTasks(repository);
deadlineMonitor.Start(intervalSeconds: 5);

ConsoleHelper.PrintHeader();
ConsoleHelper.WriteSuccess("Добавлены тестовые задачи. Мониторинг дедлайнов запущен.");
ConsoleHelper.Pause();

bool running = true;

while (running)
{
  try
  {
    ConsoleHelper.ShowMainMenu();
    string? choice = Console.ReadLine()?.Trim();

    switch (choice)
    {
      case "1":
        CreateTask(repository);
        break;
      case "2":
        ViewTasks(repository);
        break;
      case "3":
        EditTask(repository);
        break;
      case "4":
        DeleteTask(repository);
        break;
      case "5":
        SearchTasks(repository);
        break;
      case "6":
        statisticsService.PrintStatistics();
        ConsoleHelper.Pause();
        break;
      case "7":
        await SaveTasksAsync(fileService, repository);
        break;
      case "8":
        await LoadTasksAsync(fileService, repository);
        break;
      case "0":
        running = false;
        ConsoleHelper.WriteSuccess("До свидания!");
        break;
      default:
        ConsoleHelper.WriteError("Неверный пункт меню.");
        ConsoleHelper.Pause();
        break;
    }
  }
  catch (Exception ex)
  {
    ConsoleHelper.WriteError($"Неожиданная ошибка: {ex.Message}");
    ConsoleHelper.Pause();
  }
}

// ── Создание задачи ──────────────────────────────────────────────────────────

static void CreateTask(TaskRepository repository)
{
  ConsoleHelper.PrintHeader();
  Console.WriteLine("  ── Создание задачи ──");

  try
  {
    int id = repository.GetNextId();
    Console.WriteLine($"  Id будет присвоен автоматически: {id}");

    string title = ConsoleHelper.ReadRequiredString("Название");
    string description = ConsoleHelper.ReadRequiredString("Описание");
    Priority priority = ConsoleHelper.ReadPriority("Приоритет");
    DateTime deadline = ConsoleHelper.ReadDateTime("Дедлайн");
    TaskItemStatus status = ConsoleHelper.ReadTaskStatus("Статус");

    TaskItem task = new()
    {
      Id = id,
      Title = title,
      Description = description,
      Priority = priority,
      Deadline = deadline,
      Status = status,
    };

    repository.Add(task);
    ConsoleHelper.WriteSuccess($"Задача [{task.Id}] «{task.Title}» создана.");
  }
  catch (InvalidOperationException ex)
  {
    ConsoleHelper.WriteError(ex.Message);
  }

  ConsoleHelper.Pause();
}

// ── Просмотр задач ───────────────────────────────────────────────────────────

static void ViewTasks(TaskRepository repository)
{
  ConsoleHelper.PrintHeader();
  ConsoleHelper.ShowViewMenu();
  string? choice = Console.ReadLine()?.Trim();

  try
  {
    string filterKey = choice switch
    {
      "1" => "all",
      "2" => "done",
      "3" => "pending",
      "4" => "high",
      "0" => null!,
      _ => throw new ArgumentException("Неверный пункт меню."),
    };

    if (filterKey is null)
    {
      return;
    }

    string title = choice switch
    {
      "1" => "Все задачи",
      "2" => "Выполненные",
      "3" => "Невыполненные",
      "4" => "Высокий приоритет",
      _ => "Задачи",
    };

    IEnumerable<TaskItem> tasks = repository.ApplyViewFilter(filterKey);
    ConsoleHelper.PrintTasks(tasks, title);
  }
  catch (ArgumentException ex)
  {
    ConsoleHelper.WriteError(ex.Message);
  }

  ConsoleHelper.Pause();
}

// ── Редактирование ─────────────────────────────────────────────────────────────

static void EditTask(TaskRepository repository)
{
  ConsoleHelper.PrintHeader();
  Console.WriteLine("  ── Редактирование задачи ──");

  int id = ConsoleHelper.ReadInt("Id задачи");
  TaskItem? task = repository.GetById(id);

  if (task is null)
  {
    ConsoleHelper.WriteError($"Задача с Id={id} не найдена.");
    ConsoleHelper.Pause();
    return;
  }

  ConsoleHelper.PrintTaskDetails(task);
  Console.WriteLine();
  Console.WriteLine("  Оставьте поле пустым, чтобы не менять его.");

  task.Title = ConsoleHelper.ReadOptionalString("Название", task.Title);
  task.Description = ConsoleHelper.ReadOptionalString("Описание", task.Description);

  Console.Write("  Изменить приоритет? (y/n): ");
  if (Console.ReadLine()?.Trim().Equals("y", StringComparison.OrdinalIgnoreCase) == true)
  {
    task.Priority = ConsoleHelper.ReadPriority("Приоритет");
  }

  Console.Write("  Изменить статус? (y/n): ");
  if (Console.ReadLine()?.Trim().Equals("y", StringComparison.OrdinalIgnoreCase) == true)
  {
    task.Status = ConsoleHelper.ReadTaskStatus("Статус");
  }

  if (repository.Update(task))
  {
    ConsoleHelper.WriteSuccess($"Задача [{task.Id}] обновлена.");
  }
  else
  {
    ConsoleHelper.WriteError("Не удалось обновить задачу.");
  }

  ConsoleHelper.Pause();
}

// ── Удаление ─────────────────────────────────────────────────────────────────

static void DeleteTask(TaskRepository repository)
{
  ConsoleHelper.PrintHeader();
  Console.WriteLine("  ── Удаление задачи ──");

  int id = ConsoleHelper.ReadInt("Id задачи");

  if (repository.Remove(id))
  {
    ConsoleHelper.WriteSuccess($"Задача с Id={id} удалена.");
  }
  else
  {
    ConsoleHelper.WriteError($"Задача с Id={id} не найдена.");
  }

  ConsoleHelper.Pause();
}

// ── Поиск ────────────────────────────────────────────────────────────────────

static void SearchTasks(TaskRepository repository)
{
  ConsoleHelper.PrintHeader();
  ConsoleHelper.ShowSearchMenu();
  string? choice = Console.ReadLine()?.Trim();

  IEnumerable<TaskItem>? results = null;
  string title = "Результаты поиска";

  try
  {
    switch (choice)
    {
      case "1":
        string query = ConsoleHelper.ReadRequiredString("Часть названия");
        results = repository.SearchByTitle(query);
        title = $"Поиск по названию «{query}»";
        break;
      case "2":
        TaskItemStatus status = ConsoleHelper.ReadTaskStatus("Статус");
        results = repository.SearchByStatus(status);
        title = $"Поиск по статусу {status}";
        break;
      case "3":
        Priority priority = ConsoleHelper.ReadPriority("Приоритет");
        results = repository.SearchByPriority(priority);
        title = $"Поиск по приоритету {priority}";
        break;
      case "0":
        return;
      default:
        ConsoleHelper.WriteError("Неверный пункт меню.");
        ConsoleHelper.Pause();
        return;
    }

    ConsoleHelper.PrintTasks(results, title);
  }
  catch (Exception ex)
  {
    ConsoleHelper.WriteError(ex.Message);
  }

  ConsoleHelper.Pause();
}

// ── Сохранение / загрузка ────────────────────────────────────────────────────

static async Task SaveTasksAsync(FileService fileService, TaskRepository repository)
{
  ConsoleHelper.PrintHeader();
  Console.WriteLine("  ── Сохранение в файл ──");

  try
  {
    await fileService.SaveAsync(repository.GetAll());
    ConsoleHelper.WriteSuccess($"Задачи сохранены в {DataFileName}.");
  }
  catch (InvalidOperationException ex)
  {
    ConsoleHelper.WriteError(ex.Message);
  }

  ConsoleHelper.Pause();
}

static async Task LoadTasksAsync(FileService fileService, TaskRepository repository)
{
  ConsoleHelper.PrintHeader();
  Console.WriteLine("  ── Загрузка из файла ──");

  try
  {
    List<TaskItem> tasks = await fileService.LoadAsync();
    repository.ReplaceAll(tasks);
    ConsoleHelper.WriteSuccess($"Загружено задач: {tasks.Count}.");
  }
  catch (FileNotFoundException)
  {
    ConsoleHelper.WriteError($"Файл {DataFileName} не найден.");
  }
  catch (InvalidOperationException ex)
  {
    ConsoleHelper.WriteError(ex.Message);
  }

  ConsoleHelper.Pause();
}

// ── Тестовые данные ──────────────────────────────────────────────────────────

static void SeedSampleTasks(TaskRepository repository)
{
  try
  {
    TaskItem[] samples =
    [
      new()
      {
        Id = 1,
        Title = "Изучить async/await",
        Description = "Разобрать асинхронное сохранение и загрузку через FileStream.",
        Priority = Priority.High,
        Deadline = DateTime.Now.AddDays(2),
        Status = TaskItemStatus.InProgress,
      },
      new()
      {
        Id = 2,
        Title = "Написать unit-тесты",
        Description = "Покрыть Repository и FileService.",
        Priority = Priority.Medium,
        Deadline = DateTime.Now.AddDays(7),
        Status = TaskItemStatus.New,
      },
      new()
      {
        Id = 3,
        Title = "Сдать домашнее задание",
        Description = "Проверить компиляцию и отправить на проверку.",
        Priority = Priority.High,
        Deadline = DateTime.Now.AddHours(-1),
        Status = TaskItemStatus.New,
      },
      new()
      {
        Id = 4,
        Title = "Прочитать документацию по LINQ",
        Description = "Predicate, Where, GroupBy.",
        Priority = Priority.Low,
        Deadline = DateTime.Now.AddDays(-3),
        Status = TaskItemStatus.Done,
      },
    ];

    foreach (TaskItem task in samples)
    {
      if (!repository.Contains(task.Id))
      {
        repository.Add(task);
      }
    }
  }
  catch (InvalidOperationException)
  {
    // Задачи уже загружены — пропускаем.
  }
}
