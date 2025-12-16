using ProjeHavuzu.Data.Entites.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ProjectHavuzu.Business.Services.Abstracts
{
    public interface IManagerService<T> where T : BaseEntity 
    {
        //Read 
        Task<T?> GetAsync(
          Expression<Func<T, bool>> predicate,
          CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> ListAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default);

        //Commands
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        void Update(T entity);
        void Remove(T entity);

    }
}
