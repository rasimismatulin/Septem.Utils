using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Data.Types;

namespace DevExtreme.AspNet.Data.RemoteGrouping {

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
