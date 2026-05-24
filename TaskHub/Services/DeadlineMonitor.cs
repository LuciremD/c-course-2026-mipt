using TaskHub.Models;
using TaskItemStatus = TaskHub.Models.TaskStatus;

namespace TaskHub.Services;

/// <summary>
/// Фоновый мониторинг дедлайнов.
/// Периодически проверяет задачи и выводит предупреждения о просрочке.
/// </summary>
public class DeadlineMonitor : IDisposable
{
  private readonly Func<IEnumerable<TaskItem>> _getTasks;
  private readonly HashSet<int> _warnedTaskIds = new();
  private CancellationTokenSource? _cts;
  private Task? _monitorTask;
  private bool _disposed;

  /// <summary>
  /// Делегат, вызываемый при обнаружении просроченной задачи.
  /// </summary>
  public delegate void OverdueHandler(TaskItem task);

  public event OverdueHandler? OnOverdue;

  public DeadlineMonitor(Func<IEnumerable<TaskItem>> getTasks)
  {
    _getTasks = getTasks ?? throw new ArgumentNullException(nameof(getTasks));
  }

  /// <summary>
  /// Запускает фоновую проверку дедлайнов.
  /// </summary>
  public void Start(int intervalSeconds = 5)
  {
    ThrowIfDisposed();

    if (_monitorTask is { IsCompleted: false })
    {
      return;
    }

    _cts = new CancellationTokenSource();
    _monitorTask = Task.Run(() => MonitorLoopAsync(intervalSeconds, _cts.Token));
  }

  /// <summary>
  /// Останавливает фоновую проверку.
  /// </summary>
  public void Stop()
  {
    _cts?.Cancel();
    _monitorTask?.Wait(TimeSpan.FromSeconds(2));
    _cts?.Dispose();
    _cts = null;
  }

  private async Task MonitorLoopAsync(int intervalSeconds, CancellationToken token)
  {
    while (!token.IsCancellationRequested)
    {
      try
      {
        CheckDeadlines();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[DeadlineMonitor] Ошибка проверки: {ex.Message}");
      }

      try
      {
        await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), token);
      }
      catch (TaskCanceledException)
      {
        break;
      }
    }
  }

  private void CheckDeadlines()
  {
    IEnumerable<TaskItem> tasks = _getTasks();

    foreach (TaskItem task in tasks)
    {
      if (task.Status == TaskItemStatus.Done)
      {
        _warnedTaskIds.Remove(task.Id);
        continue;
      }

      if (task.IsOverdue && _warnedTaskIds.Add(task.Id))
      {
        OnOverdue?.Invoke(task);
      }
    }
  }

  public void Dispose()
  {
    if (_disposed)
    {
      return;
    }

    Stop();
    _disposed = true;
    GC.SuppressFinalize(this);
  }

  private void ThrowIfDisposed()
  {
    ObjectDisposedException.ThrowIf(_disposed, nameof(DeadlineMonitor));
  }
}
