using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiagDash.Model;

namespace DiagDash
{
    public class DiagDashHub : Hub
    {
        public PerfCounter[] GetPerformanceCounters(PerfCounter[] existingCounters)
        {
            return PerformanceCounterUtils.AddDefaultCountersForClient(Context.ConnectionId, existingCounters);
        }

        // TODO:    add start/stop viewing api calls.

        /*public override Task OnConnected()
        {
            PerformanceCounterUtils.AddDefaultCountersForClient(Context.ConnectionId);            
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            PerformanceCounterUtils.RemoveCountersForClient(Context.ConnectionId);

            return base.OnDisconnected();
        }*/

        public override Task OnDisconnected()
        {
            PerformanceCounterUtils.RemoveCountersForClient(Context.ConnectionId);

            return base.OnDisconnected();
        }
        // TODO:    what about OnReconnect?
    }
}
