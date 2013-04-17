using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace DiagDash
{
    /// <summary>
    /// It's tricky to bypass IIS native module serving request for url's ending with what looks like js/css/image 
    /// static files. We need our module to get the request, but runAllManagedModulesForAllRequests can be false in web.config
    /// and using ExtensionLessUrlHandler. Either way, no need to make it work for .js/.css/.png/.gif This would only
    /// force our module to be run for each request anyway. So workaround it by replacing . with _
    /// See http://www.west-wind.com/weblog/posts/2012/Oct/25/Caveats-with-the-runAllManagedModulesForAllRequests-in-IIS-78
    /// </summary>
    internal static class ResourceHelper
    {
        /// <summary>
        /// This may throw, so use in try/catch.
        /// </summary>
        /// <param name="fileName">Should be DiagSettings.RootPath/</param>
        /// <returns></returns>
        public static string Read(string fileName)
        {
            fileName = fileName.Replace(DiagDashSettings.RootUrl, "").Replace("_", ".");
            string path = Path.GetDirectoryName(fileName);
            string fname = Path.GetFileName(fileName);
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = "DiagDash" + path.Replace("\\", ".") + "." + fname;

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// This may throw, so use in try/catch.
        /// </summary>
        /// <param name="fileName">Should be DiagSettings.RootPath/</param>
        /// <returns></returns>
        public static Stream ReadBinary(string fileName)
        {
            fileName = fileName.Replace(DiagDashSettings.RootUrl, "").Replace("_", ".");
            string path = Path.GetDirectoryName(fileName);
            string fname = Path.GetFileName(fileName);
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = "DiagDash" + path.Replace("\\", ".") + "." + fname;

            return assembly.GetManifestResourceStream(resourcePath);
        }

        public static IHtmlString Url(string url, bool dontMinWhenDebug = false)
        {
#if DEBUG
            if (dontMinWhenDebug)
            {
                return new HtmlString(String.Format("{0}/{1}", DiagDashSettings.RootUrl, url.Replace(".min.js", ".js").Replace(".", "_")));
            }
#endif
            return new HtmlString(String.Format("{0}/{1}", DiagDashSettings.RootUrl, url.Replace(".", "_")));
        }

        public static IHtmlString SignalrUrl(bool includeHubs = false)
        {
            if (includeHubs)
            {
                return new HtmlString(String.Format("{0}/signalr/hubs", DiagDashSettings.RootUrl));
            }

            return new HtmlString(String.Format("{0}/signalr", DiagDashSettings.RootUrl));
        }
    }
}
