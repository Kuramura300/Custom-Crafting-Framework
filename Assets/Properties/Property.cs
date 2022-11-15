using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Properties
{
    /// <summary>
    /// Class that holds the data of an individual property
    /// </summary>
    public class Property
    {
        /// <summary>
        /// The name of the property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The minimum value that the property can have
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// The maximum value that the property can have
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        /// The actual value of this property for the object it is on
        /// </summary>
        public int ActualValue { get; set; }
    }
}