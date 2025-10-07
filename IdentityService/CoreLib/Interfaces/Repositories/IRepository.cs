namespace CoreLib.Interfaces;

/// <summary>
/// Базовый интерфейс для CRUD-операций над сущностями.
/// </summary>
/// <typeparam name="T">Тип сущности.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Получить сущность по идентификатору.</summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>Получить все сущности данного типа.</summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>Добавить новую сущность.</summary>
    Task AddAsync(T entity);

    /// <summary>Обновить данные сущности.</summary>
    Task UpdateAsync(T entity);

    /// <summary>Удалить сущность по идентификатору.</summary>
    Task DeleteAsync(Guid id);
}