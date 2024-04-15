using System.Linq.Expressions;

namespace Septem.DevExtreme.AspNet.Data.Helpers {

    public interface IBinaryExpressionInfo {
        Expression DataItemExpression { get; }
        string AccessorText { get; }
        string Operation { get; }
        object Value { get; }
        bool StringToLower { get; }
    }

}
