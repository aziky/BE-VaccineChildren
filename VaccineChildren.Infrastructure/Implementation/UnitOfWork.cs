using Microsoft.EntityFrameworkCore;
using VaccineChildren.Domain.Abstraction;

namespace VaccineChildren.Infrastructure.Implementation;

public class UnitOfWork : IUnitOfWork
{
    private readonly VaccineSystemDbContext _dbContext;
    private bool _disposed = false;
    private Dictionary<Type, object> _repositories;

    public UnitOfWork(VaccineSystemDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _repositories = new Dictionary<Type, object>();
    }

    public IGenericRepository<T> GetRepository<T>() where T : class
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new GenericRepository<T>(_dbContext);
        }
        return (IGenericRepository<T>)_repositories[type];
    }

    public void BeginTransaction()
    {
        _dbContext.Database.BeginTransaction();
    }

    public void CommitTransaction()
    {
        _dbContext.Database.CommitTransaction();
    }

    public void RollBack()
    {
        _dbContext.Database.RollbackTransaction();
    }
    

    public void Save()
    {
        _dbContext.SaveChanges();
    }

    public async Task SaveChangeAsync()
    {
        await _dbContext.SaveChangesAsync();
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
                _dbContext.Dispose();
            }
            _disposed = true;
        }
    }

    // Thêm phương thức để kiểm tra trạng thái transaction
    public bool HasActiveTransaction()
    {
        return _dbContext.Database.CurrentTransaction != null;
    }
}