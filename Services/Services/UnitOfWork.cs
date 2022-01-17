using System;
using System.Threading.Tasks;

namespace Services
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        public Task<bool> Complete()
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    public interface IUnitOfWork : IDisposable
    {
        public Task<bool> Complete();
    }
}