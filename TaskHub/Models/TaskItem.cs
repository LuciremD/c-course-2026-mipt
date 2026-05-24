namespace TaskHub.Models;

/// <summary>
/// Модель задачи в TaskHub.
/// </summary>
public class TaskItem : IIdentifiable
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Priority Priority { get; set; } = Priority.Medium;

    public DateTime Deadline { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.New;

    /// <summary>
    /// Задача просрочена, если дедлайн прошёл и она ещё не выполнена.
    /// </summary>
    public bool IsOverdue =>
        Status != TaskStatus.Done && Deadline < DateTime.Now;

    public override string ToString()
    {
        string overdueMark = IsOverdue ? " [ПРОСРОЧЕНА]" : string.Empty;
        return $"[{Id}] {Title} | {Priority} | {Status} | до {Deadline:dd.MM.yyyy HH:mm}{overdueMark}";
    }
}
