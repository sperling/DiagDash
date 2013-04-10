using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace DiagDash
{
    public class DiagDashModule : IHttpModule
    {
        public void Dispose()
        {
            
        }

        public void Init(HttpApplication application)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            application.PostResolveRequestCache += OnPostResolveRequestCache;
            application.PostAuthorizeRequest += OnPostAuthorizeRequest;
        }

        private void OnPostAuthorizeRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;

            if (!ContinueRequest(app))
            {
                return;
            }

            var cookie = app.Request.Cookies[DiagDashSettings.CookieName];
            if (cookie == null || cookie.Value != DiagDashSettings.CookieSecret)
            {
                // not authorized. but give back 404, so will be seen as not found.
                app.Response.SuppressContent = true;
                app.Response.StatusCode = 404;
                app.Response.TrySkipIisCustomErrors = true;
                app.CompleteRequest();
            }
        }


        private void OnPostResolveRequestCache(object sender, EventArgs e)
        {
 	        HttpApplication app = (HttpApplication)sender;

            if (!ContinueRequest(app))
            {
                return;
            }

            if (app.Request.Url.AbsolutePath.Equals(DiagDashSettings.RootUrl, StringComparison.OrdinalIgnoreCase))
            {
                DiagDashHandler.RemapHandlerForRequest(app);
            }
            else if (!app.Request.Url.AbsolutePath.StartsWith(ResourceHelper.SignalrUrl().ToHtmlString(), StringComparison.OrdinalIgnoreCase))
            {
                ResourceHandler.RemapHandlerForRequest(app);
            }
        }

        /// <summary>
        /// False if not enable, missing cookie secret or url is not for us.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        private bool ContinueRequest(HttpApplication app)
        {
            if (!DiagDashSettings.Enable || String.IsNullOrEmpty(DiagDashSettings.CookieSecret) || !app.Request.Url.AbsolutePath.StartsWith(DiagDashSettings.RootUrl, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }
}
