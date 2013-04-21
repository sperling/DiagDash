using ICSharpCode.ILSpy.XmlDoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Routing;

namespace DiagDash
{
    // TODO:    lägg till ProcessModelInfo.
    internal static class RootObjectUtils
    {
        // OBS!!!   also the order in UI menu.
        public static readonly string[] RootObjectIds = new[] { "HttpRuntime", "HttpContext", "HostingEnviroment", "BuildManager", "Routes", "Enviroment" };

        private static readonly Dictionary<string, List<Expression<Func<HttpContext, object>>>> _rootObjectExpressions = new Dictionary<string, List<Expression<Func<HttpContext, object>>>>();
        private static readonly List<Expression<Func<HttpContext, object>>> _httpRuntimeExpressions = new List<Expression<Func<HttpContext, object>>>()
        {
            _ => HttpRuntime.AppDomainAppId,
            _ => HttpRuntime.AppDomainAppPath,
            _ => HttpRuntime.AppDomainAppVirtualPath,
            _ => HttpRuntime.AppDomainId,
            _ => HttpRuntime.AspClientScriptPhysicalPath,
            _ => HttpRuntime.AspClientScriptVirtualPath,
            _ => HttpRuntime.AspInstallDirectory,
            _ => HttpRuntime.BinDirectory,
            _ => HttpRuntime.Cache.Count,
            _ => HttpRuntime.Cache.EffectivePercentagePhysicalMemoryLimit,
            _ => HttpRuntime.Cache.EffectivePrivateBytesLimit,
            _ => HttpRuntime.ClrInstallDirectory,
            _ => HttpRuntime.CodegenDir,
            _ => HttpRuntime.IsOnUNCShare,
            _ => HttpRuntime.MachineConfigurationDirectory,
            _ => HttpRuntime.UsingIntegratedPipeline
        };
        private static readonly List<Expression<Func<HttpContext, object>>> _httpContextExpressions = new List<Expression<Func<HttpContext, object>>>()
        {
            x => x.Application.Count,
            x => x.Error,
            x => x.IsCustomErrorEnabled,
            x => x.IsDebuggingEnabled,
            x => x.IsPostNotification,
            x => x.Items.Count,
            x => x.Server.MachineName,
            x => x.Server.ScriptTimeout,
            x => x.Session.CodePage,
            x => x.Session.CookieMode,
            x => x.Session.Count,
            x => x.Session.IsCookieless,
            x => x.Session.IsNewSession,
            // x => x.Session.IsReadOnly, will always be readonly here
            x => x.Session.IsSynchronized,
            x => x.Session.LCID,
            x => x.Session.Mode,
            x => x.Session.SessionID,
            x => x.Session.Timeout,
            x => x.SkipAuthorization,
            x => x.Timestamp
        };
        
        private static readonly List<Expression<Func<HttpContext, object>>> _enviromentExpressions = new List<Expression<Func<HttpContext, object>>>()
        {
            _ => Environment.HasShutdownStarted,
            _ => Environment.Is64BitOperatingSystem,
            _ => Environment.Is64BitProcess,
            _ => Environment.MachineName,
            _ => Environment.OSVersion,
            _ => Environment.ProcessorCount,
            _ => Environment.SystemDirectory,
            _ => Environment.SystemPageSize,
            _ => Environment.TickCount,
            _ => Environment.UserDomainName,
            _ => Environment.UserInteractive,
            _ => Environment.UserName,
            _ => Environment.Version,
            _ => Environment.WorkingSet
        };
        private static readonly List<Expression<Func<HttpContext, object>>> _buildManagerExpressions = new List<Expression<Func<HttpContext, object>>>()
        {
            _ => BuildManager.BatchCompilationEnabled,
            _ => BuildManager.CodeAssemblies.Count,
#if FRAMEWORK_45
            // in 4.5
            _ => BuildManager.IsPrecompiledApp,
            _ => BuildManager.IsUpdatablePrecompiledApp,
#endif
            _ => BuildManager.TargetFramework
        };
        private static readonly List<Expression<Func<HttpContext, object>>> _hostingEnviromentExpressions = new List<Expression<Func<HttpContext, object>>>()
        {
            _ => HostingEnvironment.InClientBuildManager,
            _ => HostingEnvironment.InitializationException,
#if FRAMEWORK_45
            // in 4.5
            _ => HostingEnvironment.IsDevelopmentEnvironment,
#endif
            _ => HostingEnvironment.IsHosted,
            _ => HostingEnvironment.MaxConcurrentRequestsPerCPU,
            _ => HostingEnvironment.MaxConcurrentThreadsPerCPU,
            _ => HostingEnvironment.ShutdownReason,
            _ => HostingEnvironment.SiteName
        };
        /*private static readonly List<Expression<Func<HttpContext, object>>> _processStatusExpressions = new List<Expression<Func<HttpContext, object>>>()
        {
            _ => HostingEnvironment.InClientBuildManager,
            _ => HostingEnvironment.InitializationException,
            _ => HostingEnvironment.IsHosted,
            _ => HostingEnvironment.MaxConcurrentRequestsPerCPU,
            _ => HostingEnvironment.MaxConcurrentThreadsPerCPU,
            _ => HostingEnvironment.ShutdownReason,
            _ => HostingEnvironment.SiteName
        };*/
        private static readonly Dictionary<string, List<Tuple<Tuple<string, string>, Func<HttpContext, object>>>> _rootObjectFuncs = new Dictionary<string, List<Tuple<Tuple<string, string>, Func<HttpContext, object>>>>()
        {
            { "HttpRuntime", null },
            { "HttpContext", null },
            { "Enviroment", null },
            { "BuildManager", null },
            { "HostingEnviroment", null }
        };

        private static readonly object _compileFuncLock = new object();

        static RootObjectUtils()
        {
            _rootObjectExpressions.Add("HttpRuntime", _httpRuntimeExpressions);
            _rootObjectExpressions.Add("HttpContext", _httpContextExpressions);
            _rootObjectExpressions.Add("Enviroment", _enviromentExpressions);
            _rootObjectExpressions.Add("BuildManager", _buildManagerExpressions);
            _rootObjectExpressions.Add("HostingEnviroment", _hostingEnviromentExpressions);            
        }

        public static List<RootObjectRow> GetRootObjectRows(HttpContext context, string rootObjectId)
        {
            if (String.IsNullOrEmpty(rootObjectId))
            {
                return null;
            }

            if (rootObjectId == "Routes")
            {
                return GetRootObjectRowsForRoutes(context);
            }
            
            List<Tuple<Tuple<string, string>, Func<HttpContext, object>>> funcs;

            if (!_rootObjectFuncs.TryGetValue(rootObjectId, out funcs))
            {
                return null;
            }

            if (funcs == null)
            {
                lock (_compileFuncLock)
                {
                    // recheck, may be compiled by now.
                    funcs = _rootObjectFuncs[rootObjectId];
                    if (funcs == null)
                    {
                        funcs = CompileFuncs(_rootObjectExpressions[rootObjectId]);
                        _rootObjectFuncs[rootObjectId] = funcs;
                    }
                }
            }

            var list = new List<RootObjectRow>();

            foreach (var i in funcs)
            {
                try
                {
                    object value = i.Item2(context);
                    list.Add(new RootObjectRow() { Name = i.Item1.Item1, Doc = i.Item1.Item2, Value = value != null ? value.ToString() : "null" });
                }
                catch
                {
                }
            }

            return list;
        }

        private static FieldInfo _owinRoutePathBase = null;
        private static bool _triedOwinRoutePathBase = false;

        private static List<RootObjectRow> GetRootObjectRowsForRoutes(HttpContext context)
        {
            var list = new List<RootObjectRow>();

            using (RouteTable.Routes.GetReadLock())
            {
                var httpContext = new HttpContextWrapper(context);

                foreach (RouteBase routeBase in RouteTable.Routes)
                {
                    var routeData = routeBase.GetRouteData(httpContext);

                    var route = routeBase as Route;
                    
                    if (route != null)
                    {
                        // TODO:    we can add more stuff here.
                        list.Add(new RootObjectRow() { Name = route.Url, Doc = String.Empty, Value = String.Empty });
                    }
                    else
                    {
                        var routeBaseType = routeBase.GetType();

                        if (routeBaseType.FullName == "Microsoft.Owin.Host.SystemWeb.OwinRoute")
                        {
                            // TODO:    ok not to lock here?
                            if (!_triedOwinRoutePathBase)
                            {
                                _triedOwinRoutePathBase = true;
                                _owinRoutePathBase = routeBaseType.GetField("_pathBase", BindingFlags.Instance | BindingFlags.NonPublic);
                            }

                            if (_owinRoutePathBase != null)
                            {
                                string pathBase = (string)_owinRoutePathBase.GetValue(routeBase);
                                list.Add(new RootObjectRow() { Name = pathBase != null ? pathBase : "null", Doc = String.Empty, Value = String.Empty });
                            }
                            else
                            {
                                list.Add(new RootObjectRow() { Name = routeBaseType.FullName, Doc = String.Empty, Value = "missing path base field" });
                            }
                        }
                        else
                        {
                            list.Add(new RootObjectRow() { Name = routeBaseType.FullName, Doc = String.Empty, Value = "unknown type" });
                        }
                    }
                }
            }

            return list;
        }

        private static List<Tuple<Tuple<string, string>, Func<HttpContext, object>>> CompileFuncs(List<Expression<Func<HttpContext, object>>> expressions)
        {
            var list = new List<Tuple<Tuple<string, string>, Func<HttpContext, object>>>();
            
            foreach (var i in expressions)
            {
                try
                {
                    var nameAndDoc = GetMemberName(i.Body);
                    var compiledFunc = i.Compile();
                    list.Add(Tuple.Create(nameAndDoc, compiledFunc));
                }
                catch
                {
                }
            }

            return list;
        }

        private static string GetDoc(MemberInfo member)
        {
            try
            {
                var docProvider = XmlDocLoader.LoadDocumentation(member.Module);
                if (docProvider != null)
                {
                    string documentation = docProvider.GetDocumentation(XmlDocKeyProvider.GetKey(member));
                    if (documentation != null)
                    {
                        var renderer = new XmlDocRenderer();
                        renderer.AddXmlDocumentation(documentation);
                        return renderer.CreateTextBlock();
                    }
                }
            }
            catch
            {
                return "Exception for " + member.Name;
            }

            return String.Empty;
        }

        private static Tuple<string, string> GetMemberName(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    MemberExpression m = (MemberExpression)expression;
			        if (m.Expression != null && m.Expression.NodeType == ExpressionType.MemberAccess) 
			        {
                        var ret = GetMemberName(m.Expression);
				        return Tuple.Create(String.Format("{1}.{0}", m.Member.Name, ret.Item1), GetDoc(m.Member));
			        }

			        return Tuple.Create(m.Member.Name, GetDoc(m.Member));
                case ExpressionType.Convert:
                    return GetMemberName(((UnaryExpression)expression).Operand);
                default:
                    return Tuple.Create("FIXME: " + expression.NodeType, "");
            }
        }
    }
}
