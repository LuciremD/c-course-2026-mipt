using System.Text.Json;
using TaskHub.Models;

namespace TaskHub.Services;

/// <summary>
/// Асинхронное сохранение и загрузка задач в JSON-файл через FileStream.
/// Реализует IDisposable для корректного освобождения ресурсов.
/// </summary>
public class FileService : IDisposable
{
  private readonly string _filePath;
  private readonly JsonSerializerOptions _jsonOptions;
  private FileStream? _stream;
  private bool _disposed;

  public FileService(string filePath)
  {
    if (string.IsNullOrWhiteSpace(filePath))
    {
      throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));
    }

    _filePath = filePath;
    _jsonOptions = new JsonSerializerOptions
    {
      WriteIndented = true,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
  }

  /// <summary>
  /// Асинхронно сохраняет список задач в JSON-файл.
  /// </summary>
  public async Task SaveAsync(IEnumerable<TaskItem> tasks)
  {
    ThrowIfDisposed();
    ArgumentNullException.ThrowIfNull(tasks);

    try
    {
      _stream?.Dispose();
      _stream = new FileStream(
        _filePath,
        FileMode.Create,
        FileAccess.Write,
        FileShare.None,
        bufferSize: 4096,
        useAsync: true);

      await JsonSerializer.SerializeAsync(_stream, tasks.ToList(), _jsonOptions);
      await _stream.FlushAsync();
    }
    catch (JsonException ex)
    {
      throw new InvalidOperationException("Ошибка сериализации задач в JSON.", ex);
    }
    catch (IOException ex)
    {
      throw new InvalidOperationException($"Не удалось записать файл: {_filePath}", ex);
    }
    finally
    {
      _stream?.Dispose();
      _stream = null;
    }
  }

  /// <summary>
  /// Асинхронно загружает задачи из JSON-файла.
  /// Если файл не найден — возвращает пустой список.
  /// </summary>
  public async Task<List<TaskItem>> LoadAsync()
  {
    ThrowIfDisposed();

    if (!File.Exists(_filePath))
    {
      throw new FileNotFoundException($"Файл не найден: {_filePath}", _filePath);
    }

    try
    {
      _stream?.Dispose();
      _stream = new FileStream(
        _filePath,
        FileMode.Open,
        FileAccess.Read,
        FileShare.Read,
        bufferSize: 4096,
        useAsync: true);

      List<TaskItem>? tasks = await JsonSerializer.DeserializeAsync<List<TaskItem>>(_stream, _jsonOptions);

      return tasks ?? [];
    }
    catch (JsonException ex)
    {
      throw new InvalidOperationException("Ошибка десериализации JSON. Файл повреждён или имеет неверный формат.", ex);
    }
    catch (IOException ex)
    {
      throw new InvalidOperationException($"Не удалось прочитать файл: {_filePath}", ex);
    }
    finally
    {
      _stream?.Dispose();
      _stream = null;
    }
  }

  public void Dispose()
  {
    if (_disposed)
    {
      return;
    }

    _stream?.Dispose();
    _stream = null;
    _disposed = true;
    GC.SuppressFinalize(this);
  }

  private void ThrowIfDisposed()
  {
    ObjectDisposedException.ThrowIf(_disposed, nameof(FileService));
  }
}
