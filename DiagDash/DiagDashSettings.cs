using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DiagDash
{
    internal static class DiagDashSettings
    {
        private static bool? _enable = null;
        public static bool Enable
        {
            get
            {
                if (_enable == null)
                {
                    bool v;

                    if (ConfigurationManager.AppSettings["DiagDash.Enable"] == null)
                    {
                        // default to on if missing.
                        v = true;
                    }
                    else
                    {
                        bool.TryParse(ConfigurationManager.AppSettings["DiagDash.Enable"], out v);
                    }

                    _enable = v;
                }

                return _enable.Value;
            }
        }

        private static string _rootUrl = null;
        public static string RootUrl
        {
            get
            {
                if (_rootUrl == null)
                {
                    _rootUrl = ConfigurationManager.AppSettings["DiagDash.RootUrl"];

                    if (String.IsNullOrEmpty(_rootUrl))
                    {
                        _rootUrl = "_diagdash";
                    }

                    _rootUrl = "/" + _rootUrl.Replace("/", "").Replace("\\", "");
                }

                return _rootUrl;
            }
        }

        private static string _cookieName = null;
        public static string CookieName
        {
            get
            {
                if (_cookieName == null)
                {
                    _cookieName = ConfigurationManager.AppSettings["DiagDash.CookieName"];

                    if (String.IsNullOrEmpty(_cookieName))
                    {
                        _cookieName = "diagdash";
                    }                    
                }

                return _cookieName;
            }
        }

        private static string _cookieSecret = null;
        public static string CookieSecret
        {
            get
            {
                if (_cookieSecret == null)
                {
                    _cookieSecret = ConfigurationManager.AppSettings["DiagDash.CookieSecret"];
#if DEBUG
                    // this should always be set in appsettings by user. 
                    // security issue if we are defaulting to something.
                    // so we are only doing that in debug build when testing.
                    if (String.IsNullOrEmpty(_cookieSecret))
                    {
                        _cookieSecret = "diagdash";
                    }
#endif
                    if (_cookieSecret == null)
                    {
                        _cookieSecret = "";
                    }
                }

                return _cookieSecret;
            }
        }
    }
}
