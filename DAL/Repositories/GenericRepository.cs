using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DAL.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DAL.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly StoreContext dbContext;
        private readonly DbSet<TEntity> dbSet;
        public GenericRepository(StoreContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<TEntity>();
        }
        public async Task DeleteAsync(object id)
        {
            var entity = await dbSet.FindAsync(id);
            dbSet.Remove(entity);
        }

        public IQueryable<TEntity> GetQuery()
            => dbSet.AsQueryable();

        public async Task<TEntity> GetByIdAsync(object id)
            => await dbSet.FindAsync(id);

        public async Task InsertAsync(TEntity entity)
            => await dbSet.AddAsync(entity);

        public void UpdateAsync(TEntity entity)
            => dbSet.Update(entity);

        public async Task<bool> CompleteAsync()
            => await dbContext.SaveChangesAsync() > 0;

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
                dbContext.Dispose();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync(
           IsolationLevel isolationLevel = IsolationLevel.Unspecified,
           CancellationToken cancellationToken = default)
        {
            IDbContextTransaction dbContextTransaction = await dbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
            return dbContextTransaction;
        }
    }

    public interface IGenericRepository<TEntity> where TEntity : class
    {
        public IQueryable<TEntity> GetQuery();
        public Task<TEntity> GetByIdAsync(object id);
        public Task InsertAsync(TEntity entity);
        public Task DeleteAsync(object id);
        public void UpdateAsync(TEntity entity);
        public Task<bool> CompleteAsync();
        public Task<IDbContextTransaction> BeginTransactionAsync(
           IsolationLevel isolationLevel = IsolationLevel.Unspecified,
           CancellationToken cancellationToken = default);
    }
}