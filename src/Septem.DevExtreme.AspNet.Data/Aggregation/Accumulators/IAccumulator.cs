﻿namespace Septem.DevExtreme.AspNet.Data.Aggregation.Accumulators {

    interface IAccumulator {
        void Add(object value);
        void Divide(int divider);
        object GetValue();
    }

}
