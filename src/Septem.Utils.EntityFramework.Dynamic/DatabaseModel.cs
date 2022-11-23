
using System.Collections.Generic;

namespace Septem.Utils.EntityFramework.Dynamic;

public class DatabaseModel
{
    public string SchemaName { get; set; }

    public string DataContextName { get; set; }

    public string RootNameSpace { get; set; }

    public ICollection<TableModel> Tables { get; set; }

}