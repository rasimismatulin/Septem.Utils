using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Septem.Utils.Helpers.Dynamic;

namespace Septem.Utils.EntityFramework.Dynamic;

public class DynamicQueryProvider
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly DataContextGenerator _dataContextGenerator;

    public DynamicQueryProvider(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _dataContextGenerator = new DataContextGenerator(loggerFactory);
    }
    
    public async Task<IDynamicQueryable> GetQueryAsync(string tableName, ICollection<KeyValuePair<string, string>> columns, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return _dataContextGenerator.GetQuery(tableName);
    }

    public bool ValidateTable(string tableName, ICollection<KeyValuePair<string, string>> columns)
    {
        return _dataContextGenerator.ValidateHash(tableName, columns);
    }

    public void CreateDatabase(DatabaseModel dbModel)
    {
        _dataContextGenerator.Generate(dbModel);
    }

    public Type GetRuntimeType(string tableName)
    {
        return _dataContextGenerator.GetRuntimeType(tableName);
    }
}