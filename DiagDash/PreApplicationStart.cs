using Microsoft.AspNet.SignalR;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(DiagDash.PreApplicationStart), "PreStart")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(DiagDash.PreApplicationStart), "Shutdown")]

namespace DiagDash
{
    public static class PreApplicationStart
    {
        public static void PreStart()
        {
            // TODO:    ignore if not enable or missing cookie secret?

            // register our module, will validating request and creating handlers if ok.
            DynamicModuleUtility.RegisterModule(typeof(DiagDashModule));
            RouteTable.Routes.MapHubs(ResourceHelper.SignalrUrl().ToHtmlString(), new HubConfiguration());
            PerformanceCounterUtils.Init();
        }

        public static void Shutdown()
        {
            PerformanceCounterUtils.Timer.Close();
        }
    }
}
