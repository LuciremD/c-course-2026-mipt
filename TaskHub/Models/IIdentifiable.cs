namespace TaskHub.Models;

/// <summary>
/// Интерфейс для сущностей с числовым идентификатором.
/// Используется generic-репозиторием.
/// </summary>
public interface IIdentifiable
{
    int Id { get; set; }
}
