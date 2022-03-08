using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DAL.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using AutoMapper;
namespace DAL.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly StoreContext dbContext;
        private readonly DbSet<TEntity> dbSet;
        protected readonly IMapper _mapper;
        public GenericRepository(StoreContext dbContext, IMapper _mapper)
        {
            this._mapper = _mapper;
            this.dbContext = dbContext;
            dbSet = dbContext.Set<TEntity>();
        }
        public async Task DeleteAsync(object id)
        {
            var entity = await dbSet.FindAsync(id);
            dbSet.Remove(entity);
        }
        public async Task<IReadOnlyList<TEntity>> GetAllAsync()
            => await dbSet.ToListAsync();


        public IQueryable<TEntity> GetQuery()
            => dbSet.AsQueryable();

        public async Task<TEntity> GetByIdAsync(object id)
            => await dbSet.FindAsync(id);

        public async Task InsertAsync(TEntity entity)
            => await dbSet.AddAsync(entity);

        public void Update(TEntity entity)
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
        public Task<IReadOnlyList<TEntity>> GetAllAsync();
        public IQueryable<TEntity> GetQuery();
        public Task<TEntity> GetByIdAsync(object id);
        public Task InsertAsync(TEntity entity);
        public Task DeleteAsync(object id);
        public void Update(TEntity entity);
        public Task<bool> CompleteAsync();
        public Task<IDbContextTransaction> BeginTransactionAsync(
           IsolationLevel isolationLevel = IsolationLevel.Unspecified,
           CancellationToken cancellationToken = default);
    }
}