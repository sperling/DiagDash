using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace DiagDash
{
    internal sealed class DiagDashHandler : IHttpHandler, IReadOnlySessionState
    {
        internal static void RemapHandlerForRequest(HttpApplication app)
        {
            HttpContextBase httpContextBase = new HttpContextWrapper(app.Context);
            httpContextBase.RemapHandler(new DiagDashHandler());
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

            try
            {
                if (context.Request.IsAjaxRequest())
                {
                    switch (context.Request.QueryString["m"])
                    {
                        case "loadRootObject":
                            LoadRootObject(context, context.Request.QueryString["id"]);
                            break;
                        default:
                            context.Response.StatusCode = 404;
                            break;
                    }
                }
                else
                {
                    // TODO:    set TransformText() to static. 
                    // render razor page.
                    var index = new Index();
                    context.Response.Write(index.TransformText());
                }
            }
            catch
            {
                context.Response.StatusCode = 500;
            }
        }

        private void LoadRootObject(HttpContext context, string id)
        {
            context.Response.ContentType = "application/json";

            // TODO:    use Newton here.
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            
            var rows = RootObjectUtils.GetRootObjectRows(context, id);

            if (rows == null)
            {
                context.Response.StatusCode = 500;
                return;
            }

            context.Response.Write(serializer.Serialize(rows));
        }
    }
}
