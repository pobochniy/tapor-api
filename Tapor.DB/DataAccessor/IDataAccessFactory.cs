using Dodo.Tools.DB.MySql;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo.Tools.DB.MySQL;

public interface IDataAccessFactory
{
	Task<IDataAccess> GetDataAccess(string connectionString, bool beginTransaction, CancellationToken ct);
}