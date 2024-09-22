using System.Data;
using Tapor.DB.Session;

namespace Dodo.Data.Factories;

public interface IDbSessionFactory
{
    IDbSession OpenTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

    Task<IDbSession> OpenTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken ct = default);

    IDbSession Open();
    Task<IDbSession> OpenAsync(CancellationToken ct = default);
}