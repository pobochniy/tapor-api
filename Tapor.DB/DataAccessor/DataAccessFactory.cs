using Dodo.Tools.DB.MySql;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo.Tools.DB.MySQL;

public class DataAccessFactory : IDataAccessFactory
{
	public virtual async Task<IDataAccess> GetDataAccess(string connectionString, bool beginTransaction, CancellationToken ct)
	{
		return await DataAccess.CreateAsync(connectionString, beginTransaction, cancellationToken: ct);
	}
}
