using ProjectHavuzu.Business.Services.Abstracts;
using ProjeHavuzu.Data.Entites.Common;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ProjectHavuzu.Business.Services.Concretes
{
    public class ManagerService<T> : IManagerService<T> where T : BaseEntity
    {
        private readonly IRepository<T> _repository;


        public ManagerService(
            IRepository<T> repository)
        {
            _repository = repository;
        }

        // READ
        public async Task<T?> GetAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _repository.GetAsync(predicate, cancellationToken);
        }

        public async Task<IReadOnlyList<T>> ListAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            return await _repository.ListAsync(predicate, cancellationToken);
        }

        // COMMANDS
        public async Task AddAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            // 🔹 Business rules burada olur
            // Validate(entity);

            await _repository.AddAsync(entity, cancellationToken);
            
        }

        public async Task AddRangeAsync(
            IEnumerable<T> entities,
            CancellationToken cancellationToken = default)
        {
            await _repository.AddRangeAsync(entities, cancellationToken);
            
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _repository.Update(entity);
            
        }

        public async Task RemoveAsync(T entity, CancellationToken cancellationToken = default)
        {
            _repository.Remove(entity);
            
        }

        public void Update(T entity)
        {
            _repository.Update(entity);
            

        }

        public void Remove(T entity)
        {
            _repository.Remove(entity);
            

        }
    }
}
