using System.Linq.Expressions;

namespace MovieApp.Core.Interfaces
{
    /// <summary>
    /// Generic Repository Interface
    /// SOLID: Interface Segregation & Dependency Inversion
    /// </summary>
    public interface IRepository<T> where T : class
    {
        // ═══ Read Operations ═══

        /// <summary>
        /// Get entity by ID
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Get all entities
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Find entities with condition
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Get single entity with condition
        /// </summary>
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);

        // ═══ Write Operations ═══

        /// <summary>
        /// Add new entity
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Add multiple entities
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Update entity
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Delete entity
        /// </summary>
        void Delete(T entity);

        /// <summary>
        /// Delete multiple entities
        /// </summary>
        void DeleteRange(IEnumerable<T> entities);

        // ═══ Query Operations ═══

        /// <summary>
        /// Count entities
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// Count with condition
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Check if exists
        /// </summary>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}