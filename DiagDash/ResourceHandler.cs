using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace DiagDash
{
    internal sealed class ResourceHandler : IHttpHandler
    {
        internal static void RemapHandlerForRequest(HttpApplication app)
        {
            HttpContextBase httpContextBase = new HttpContextWrapper(app.Context);
            httpContextBase.RemapHandler(new ResourceHandler());
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            context.Response.Clear();

            if (!String.IsNullOrEmpty(context.Request.Headers["If-Modified-Since"]))
            {
                // cached.
                context.Response.StatusCode = 304;
                return;
            }

            try
            {
                if (context.Request.Url.AbsolutePath.EndsWith("_png"))
                {
                    context.Response.ContentType = "image/png";
                    WriteImage(context);
                }
                else if (context.Request.Url.AbsolutePath.EndsWith("_gif"))
                {
                    context.Response.ContentType = "image/gif";
                    WriteImage(context);
                }
                else
                {
                    var result = ResourceHelper.Read(context.Request.Url.AbsolutePath);
                    if (String.IsNullOrEmpty(result))
                    {
                        context.Response.StatusCode = 500;
                        context.Response.SuppressContent = true;
                        context.Response.TrySkipIisCustomErrors = true;
                    }
                    else
                    {
                        if (context.Request.Url.AbsolutePath.EndsWith("_js"))
                        {
                            context.Response.ContentType = "text/javascript";
                        }
                        else if (context.Request.Url.AbsolutePath.EndsWith("_css"))
                        {
                            context.Response.ContentType = "text/css";
                        }
                        context.Response.Write(result);

                        SetCache(context);
                    }
                }
            }
            catch
            {
                context.Response.StatusCode = 500;
                context.Response.SuppressContent = true;
                context.Response.TrySkipIisCustomErrors = true;
            }
        }

        private void WriteImage(HttpContext context)
        {
            var stream = ResourceHelper.ReadBinary(context.Request.Url.AbsolutePath);
            if (stream == null)
            {
                context.Response.StatusCode = 500;
                context.Response.SuppressContent = true;
                context.Response.TrySkipIisCustomErrors = true;
            }
            else
            {
                stream.CopyTo(context.Response.OutputStream);
                SetCache(context);
                stream.Close();
            }
        }

        private void SetCache(HttpContext context)
        {
            HttpCachePolicy cache = context.Response.Cache;
            cache.SetCacheability(HttpCacheability.Public);
            cache.SetOmitVaryStar(true);
            cache.SetExpires(DateTime.Now.AddYears(1));
            cache.SetValidUntilExpires(true);
            cache.SetLastModified(DateTime.Now);
            cache.VaryByHeaders["User-Agent"] = true;
        }
    }
}
