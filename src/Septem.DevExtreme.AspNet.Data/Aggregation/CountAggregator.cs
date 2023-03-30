using DevExtreme.AspNet.Data.Helpers;

namespace DevExtreme.AspNet.Data.Aggregation {

    class CountAggregator<T> : Aggregator<T> {
        int _count;
        bool _skipNulls;

        public CountAggregator(IAccessor<T> accessor, bool skipNulls)
            : base(accessor) {
            _skipNulls = skipNulls;
        }

        public override void Step(T container, string selector) {
            if(!_skipNulls || Accessor.Read(container, selector) != null)
                _count++;
        }

        public override object Finish() {
            return _count;
        }

    }

}
