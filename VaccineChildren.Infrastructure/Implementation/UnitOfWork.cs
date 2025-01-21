using VaccineChildren.Domain.Abstraction;

namespace VaccineChildren.Infrastructure.Implementation;

public class UnitOfWork(VaccineSystemDbContext dbContext) : IUnitOfWork
{
    private bool _disposed = false;

    public void BeginTransaction()
    {
        dbContext.Database.BeginTransaction();
    }

    public void CommitTransaction()
    {
        dbContext.Database.CommitTransaction();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
        }
        _disposed = true;
    }

    public IGenericRepository<T> GetRepository<T>() where T : class
    {
        return new GenericRepository<T>(dbContext);
    }

    public void RollBack()
    {
        dbContext.Database.RollbackTransaction();
    }

    public void Save()
    {
        dbContext.SaveChanges();
    }
    public async Task SaveChangeAsync()
    {
        await dbContext.SaveChangesAsync();	
    }
}