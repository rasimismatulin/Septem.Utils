﻿using System.Collections;

namespace Septem.DevExtreme.AspNet.Data.ResponseModel {

    /// <summary>
    /// Represents a group in the resulting dataset.
    /// </summary>
    public class Group {
        /// <summary>
        /// The group's key.
        /// </summary>
        public object key { get; set; }

        /// <summary>
        /// Subgroups or data objects.
        /// </summary>
        public IList items { get; set; }

        /// <summary>
        /// The count of items in the group.
        /// </summary>
        public int? count { get; set; }

        /// <summary>
        /// Group summary calculation results.
        /// </summary>
        public object[] summary { get; set; }
    }

}
