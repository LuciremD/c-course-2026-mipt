namespace TaskHub.Models;

/// <summary>
/// Статус выполнения задачи.
/// Не путать с System.Threading.Tasks.TaskStatus — используем полное имя при необходимости.
/// </summary>
public enum TaskStatus
{
    New,
    InProgress,
    Done
}
