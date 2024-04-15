using System;
using Septem.DevExtreme.AspNet.Data.Aggregation;
using Septem.DevExtreme.AspNet.Data.Helpers;
using Septem.DevExtreme.AspNet.Data.Types;

namespace Septem.DevExtreme.AspNet.Data.RemoteGrouping {

    class RemoteAvgAggregator<T> : Aggregator<T> {
        Aggregator<T> _countAggregator;
        SumAggregator<T> _valueAggregator;

        public RemoteAvgAggregator(IAccessor<T> accessor)
            : base(accessor) {
            _countAggregator = new SumAggregator<T>(accessor);
            _valueAggregator = new SumAggregator<T>(accessor);
        }

        public override void Step(T container, string selector) {
            _countAggregator.Step(container, AnonType.IndexToField(1 + AnonType.FieldToIndex(selector)));
            _valueAggregator.Step(container, selector);
        }

        public override object Finish() {
            var count = Convert.ToInt32(_countAggregator.Finish());
            if(count == 0)
                return null;

            var valueAccumulator = _valueAggregator.GetAccumulator();
            valueAccumulator.Divide(count);
            return valueAccumulator.GetValue();
        }
    }

}
