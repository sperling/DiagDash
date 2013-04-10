using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace DiagDash
{
    internal static class ResourceHelper
    {
        /// <summary>
        /// This may throw, so use in try/catch.
        /// </summary>
        /// <param name="fileName">Should be DiagSettings.RootPath/</param>
        /// <returns></returns>
        public static string Read(string fileName)
        {
            // can't find away to not let IIS native file handler take over request totally when using file extensions
            // without any config handler registration.
            // so workaround by replacing . with _
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
            // can't find away to not let IIS native file handler take over request totally when using file extensions
            // without any config handler registration.
            // so workaround by replacing . with _
            fileName = fileName.Replace(DiagDashSettings.RootUrl, "").Replace("_", ".");
            string path = Path.GetDirectoryName(fileName);
            string fname = Path.GetFileName(fileName);
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = "DiagDash" + path.Replace("\\", ".") + "." + fname;

            return assembly.GetManifestResourceStream(resourcePath);
        }

        public static IHtmlString Url(string url)
        {
            // can't find away to not let IIS native file handler take over request totally when using file extensions
            // without any config handler registration.
            // so workaround by replacing . with _
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
