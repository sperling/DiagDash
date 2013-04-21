using DiagDash.Model;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace DiagDash
{
    internal static class PerformanceCounterUtils
    {
        public static Timer Timer { get; private set; }

        private static ConcurrentDictionary <string, List<int>> _clientPerformanceCounters = new ConcurrentDictionary<string, List<int>>();
        // TODO:    keep ref count by client and remove if no one left using it, except default counters.
        private static ConcurrentDictionary<int, PerformanceCounter> _perfomanceCounters = new ConcurrentDictionary<int, PerformanceCounter>();
        private static List<int> _defaultPerformanceCounters = null;

        // TODO:    from settings.
        public const int SNAPSHOT_MILLISECONDS = 2000;

        public static void Init()
        {
            // TODO:    lazy load this on first client connects.
            TryAddCounter("Processor", "% Processor Time", "_Total");
            TryAddCounter("System", "System Up Time", String.Empty);

            _defaultPerformanceCounters = _perfomanceCounters.Keys.ToList();

            Timer = new Timer(SNAPSHOT_MILLISECONDS);
            Timer.AutoReset = true;
            Timer.Elapsed += OnElapsed;

            // TOOD:    start lazy when first client connects and start/stop when clients/no clients.
            Timer.Start();
        }

        private static int HashForCounter(this PerformanceCounter counter)
        {
            return String.Format("{0}{1}{2}", counter.CategoryName, counter.CounterName, counter.InstanceName).GetHashCode();
        }

        private static float NormalizeValue(this float value, PerformanceCounterType counterType)
        {
            // TODO:    figure out all scaling here. should be 0-100 range.
            switch (counterType)
            {
                case PerformanceCounterType.ElapsedTime:
                    value *= 0.00001f;
                    break;
            }

            return value;
        }

        private static bool TryAddCounter(string categoryName, string counterName, string instanceName)
        {
            try
            {
                var c = new PerformanceCounter(categoryName, counterName, instanceName, true);
                _perfomanceCounters.TryAdd(c.HashForCounter(), c);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static PerfCounter[] AddDefaultCountersForClient(string clientId, PerfCounter[] existingCounters)
        {
            // TODO:    add existingCounters if they are valid counters and remove set from default.
            //          the hash for existingCounters may be stale, so need to recalc and then match.
            _clientPerformanceCounters.TryAdd(clientId, _defaultPerformanceCounters.ToList());
            // take snapshot of current counters. another client may be updating it.
            var allCounters = _perfomanceCounters.ToDictionary(x => x.Key, x => x.Value);
            // default counters should never be removed. so this is safe.
            return _defaultPerformanceCounters.Select(x => new { hash = x, counter = allCounters[x] }).Select(x => new PerfCounter() { CategoryName = x.counter.CategoryName, CounterHelp = x.counter.CounterHelp, CounterName = x.counter.CounterName, CounterType = x.counter.CounterType.ToString(), InstanceName = x.counter.InstanceName, Hash = x.hash }).ToArray();
        }

        public static void RemoveCountersForClient(string clientId)
        {
            List<int> dummy;

            _clientPerformanceCounters.TryRemove(clientId, out dummy);
        }

        private static DateTime _epoch = new DateTime(1970, 1, 1);

        private static void OnElapsed(object sender, ElapsedEventArgs e)
        {
            if (!DiagDashSettings.Enable || String.IsNullOrEmpty(DiagDashSettings.CookieSecret))
            {
                return;
            }

            if (_clientPerformanceCounters.Count < 1)
            {
                return;
            }

            var timestamp = (long)Math.Floor((DateTime.UtcNow - _epoch).TotalMilliseconds + 0.5);

            // TODO:    this may throw.
            //          check where ok to continue with next counter or stop reading anymore.
            var counterData = _perfomanceCounters.ToArray().ToDictionary(x => x.Key, x => new PerfCounterSnapShot 
            { 
                Sample = x.Value.NextSample(),
                Value = x.Value.NextValue(),
                CounterType = x.Value.CounterType
            });
            foreach (var i in counterData.Values)
            {
                i.NormalizedValue = i.Value.NormalizeValue(i.CounterType);
            }

            var context = GlobalHost.ConnectionManager.GetHubContext<DiagDashHub>();
            foreach (var clientPerformanceCounters in _clientPerformanceCounters.ToArray())
            {
                // TODO:    only send if client is viewing.
                var performanceCountersForClientToSend = new List<CounterSnapShot>();
                foreach (var counterHash in clientPerformanceCounters.Value)
                {
                    PerfCounterSnapShot snapshot;

                    if (counterData.TryGetValue(counterHash, out snapshot))
                    {
                        performanceCountersForClientToSend.Add(new CounterSnapShot() { Value = snapshot.Value, NormalizedValue = snapshot.NormalizedValue, Hash = counterHash });
                    }
                }

                if (performanceCountersForClientToSend.Count > 0)
                {
                    // TODO:    should only send value/sample here.
                    context.Clients.Client(clientPerformanceCounters.Key).updatePerformanceCounters(timestamp, performanceCountersForClientToSend);
                }
            }
        }

        private class PerfCounterSnapShot
        {
            public CounterSample Sample { get; set; }
            public float Value { get; set; }
            public float NormalizedValue { get; set; }
            public  PerformanceCounterType CounterType { get; set; }
        }
    }
}
