using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace DiagDash.Model
{
    public class PerfCounter
    {
        public string CategoryName { get; set; }
        public string CounterHelp { get; set; }
        public string CounterName { get; set; }
        public PerformanceCounterType CounterType { get; set; }
        public string InstanceName { get; set; }

        public int Hash { get; set; }
    }
}
