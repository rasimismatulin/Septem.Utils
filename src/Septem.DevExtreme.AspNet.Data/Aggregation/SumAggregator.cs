﻿using System;
using Septem.DevExtreme.AspNet.Data.Aggregation.Accumulators;
using Septem.DevExtreme.AspNet.Data.Helpers;

namespace Septem.DevExtreme.AspNet.Data.Aggregation {

    class SumAggregator<T> : Aggregator<T> {
        IAccumulator _accumulator;

        public SumAggregator(IAccessor<T> accessor)
            : base(accessor) {
        }

        public override void Step(T container, string selector) {
            var value = Accessor.Read(container, selector);

            if(value != null) {
                if(_accumulator == null)
                    _accumulator = AccumulatorFactory.Create(value.GetType());

                try {
                    _accumulator.Add(value);
                } catch(FormatException) {
                } catch(InvalidCastException) {
                }
            }
        }

        public override object Finish() {
            return _accumulator?.GetValue();
        }

        public IAccumulator GetAccumulator() {
            return _accumulator;
        }

    }

}
