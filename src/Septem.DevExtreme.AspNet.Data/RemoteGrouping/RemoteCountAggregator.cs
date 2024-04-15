using Septem.DevExtreme.AspNet.Data.Aggregation;
using Septem.DevExtreme.AspNet.Data.Helpers;
using Septem.DevExtreme.AspNet.Data.Types;

namespace Septem.DevExtreme.AspNet.Data.RemoteGrouping {

    class RemoteCountAggregator<T> : Aggregator<T> {
        int _count = 0;

        public RemoteCountAggregator(IAccessor<T> accessor)
            : base(accessor) {
        }

        public override void Step(T dataitem, string selector) {
            var group = dataitem as AnonType;
            _count += (int)group[0];
        }

        public override object Finish() {
            return _count;
        }
    }

}
