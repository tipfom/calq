﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Calq.Core
{
    public static class WebHelper
    {
        const string VERSION = "1.0";
#if LOCAL
        const string SERVER_URL = "http://localhost:8080";
#else
        const string SERVER_URL = "http://timpokart.de:8080";
#endif

        public static event Action IsOnlineChanged;

        static WebHelper()
        {
            new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    string version;
                    if (Request("/handshake", out version) && version == VERSION)
                    {
                        IsOnline = true;
                    }
                    else
                    {
                        IsOnline = false;
                    }
                    Thread.Sleep(5000);
                }
            })).Start();
        }

        private static bool _IsOnline;
        public static bool IsOnline {
            get { return _IsOnline; }
            private set {
                if (_IsOnline != value)
                {
                    _IsOnline = value;
                    IsOnlineChanged.Invoke();
                }
            }
        }

        public static bool GetIntegral(string prefixExpression, IEnumerable<string> variables, string variable, out string result)
        {
            string base64Expression = Convert.ToBase64String(Encoding.UTF8.GetBytes(prefixExpression));
            return Request($"/math?method=int&expr={base64Expression}&var={string.Join("&var=", variables)}&d={variable}", out result);
        }

        public static bool GetIntegral(string prefixExpression, IEnumerable<string> variables, string variable, string lowerLimit, string upperLimit, out string result)
        {
            string base64Expression = Convert.ToBase64String(Encoding.UTF8.GetBytes(prefixExpression));
            return Request($"/math?method=int&expr={base64Expression}&var={string.Join("&var=", variables)}&d={variable}&ulim={upperLimit}&llim={lowerLimit}", out result);
        }

        public static bool GetLimit(string prefixExpression, IEnumerable<string> variables, string argument, string valueApproaching, out string result)
        {
            return GetLimit(prefixExpression, variables, argument, valueApproaching, "+-", out result);
        }

        public static bool GetLimit(string prefixExpression, IEnumerable<string> variables, string argument, string valueApproaching, string direction, out string result)
        {
            string base64Expression = Convert.ToBase64String(Encoding.UTF8.GetBytes(prefixExpression));
            return Request($"/math?method=lim&expr={base64Expression}&var={string.Join("&var=", variables)}&arg={argument}&val={valueApproaching}&dir={((direction == "r") ? "1" : (direction == "l" ? "2" : "3"))}", out result);
        }

        private static bool Request(string path, out string value)
        {
            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.CreateHttp(SERVER_URL + path);
                httpReq.Method = "GET";
                HttpWebResponse webResponse = (HttpWebResponse)httpReq.GetResponse();
                value = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                value = "error connecting";
                return false;
            }
        }
    }
}
