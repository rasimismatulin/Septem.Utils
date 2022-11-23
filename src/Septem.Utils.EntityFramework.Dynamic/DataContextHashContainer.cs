using System.Collections.Generic;
using System.Linq;

namespace Septem.Utils.EntityFramework.Dynamic;

public class DataContextHashContainer
{
    private IDictionary<string, string> _hash;

    public DataContextHashContainer()
    {
        _hash = new Dictionary<string, string>();
    }

    public bool ValidateHash(string tableName, ICollection<KeyValuePair<string, string>> columns)
    {
        if (!_hash.ContainsKey(tableName))
            return false;
        var currentHash = _hash[tableName];
        return columns.Select(column => $"{column.Key}: {column.Value}").All(columnData => currentHash.Contains(columnData));
    }

    public void SaveHash(string tableName, ICollection<KeyValuePair<string, string>> columns)
    {
        var hash = string.Join(",", columns.Select(x => $"{x.Key}: {x.Value}"));
        if (_hash.ContainsKey(tableName))
            _hash[tableName] = hash;
        else
            _hash.Add(tableName, hash);
    }

    public void SaveHash(DatabaseModel databaseModel)
    {
        _hash = new Dictionary<string, string>();
        foreach (var table in databaseModel.Tables)  
            SaveHash(table.Name, table.Columns.Select(x => new KeyValuePair<string,string>(x.Name, x.Type)).ToList());
    }
}