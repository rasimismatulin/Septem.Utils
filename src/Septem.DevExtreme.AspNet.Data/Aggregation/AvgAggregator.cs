using Septem.DevExtreme.AspNet.Data.Helpers;

namespace Septem.DevExtreme.AspNet.Data.Aggregation {

    class AvgAggregator<T> : Aggregator<T> {
        Aggregator<T> _counter;
        SumAggregator<T> _summator;

        public AvgAggregator(IAccessor<T> accessor)
            : base(accessor) {
            _counter = new CountAggregator<T>(accessor, true);
            _summator = new SumAggregator<T>(accessor);
        }

        public override void Step(T container, string selector) {
            _counter.Step(container, selector);
            _summator.Step(container, selector);
        }

        public override object Finish() {
            var count = (int)_counter.Finish();
            if(count == 0)
                return null;

            var accumulator = _summator.GetAccumulator();
            accumulator.Divide(count);
            return accumulator.GetValue();
        }
    }

}
