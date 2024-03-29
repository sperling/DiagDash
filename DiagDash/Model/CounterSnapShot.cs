﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiagDash.Model
{
    internal class CounterSnapShot
    {
        /// <summary>
        /// Calculated value.
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Calculated value in 0-100 range.
        /// </summary>
        public float NormalizedValue { get; set; }

        public int Hash { get; set; }
    }
}
