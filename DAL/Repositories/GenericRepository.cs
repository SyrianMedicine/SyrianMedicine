using System.Linq;
using System.Threading.Tasks;
using DAL.DataContext;
using Microsoft.EntityFrameworkCore;

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
        public async Task Delete(object id)
        {
            var entity = await dbSet.FindAsync(id);
            dbSet.Remove(entity);
        }

        public IQueryable<TEntity> GetQuery()
            => dbSet.AsQueryable();

        public async Task<TEntity> GetById(object id)
            => await dbSet.FindAsync(id);

        public async Task Insert(TEntity entity)
            => await dbSet.AddAsync(entity);

        public void Update(TEntity entity)
            => dbSet.Update(entity);
    }

    public interface IGenericRepository<TEntity> where TEntity : class
    {
        public IQueryable<TEntity> GetQuery();
        public Task<TEntity> GetById(object id);
        public Task Insert(TEntity entity);
        public Task Delete(object id);
        public void Update(TEntity entity);
    }
}