using Calq.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Calq.Core
{
    public class PythonWebProvider : IPythonProvider
    {
        const string VERSION = "1.0";
#if LOCAL
        const string SERVER_URL = "http://localhost:8080";
#else
        const string SERVER_URL = "http://timpokart.de:8080";
#endif

        public event Action IsOnlineChanged;

        public PythonWebProvider()
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

        private bool _IsOnline;
        public bool IsOnline {
            get { return _IsOnline; }
            private set {
                if (_IsOnline != value)
                {
                    _IsOnline = value;
                    IsOnlineChanged.Invoke();
                }
            }
        }

        private IEnumerable<string> GetBase64(IEnumerable<string> text)
        {
            foreach (string t in text)
                yield return Convert.ToBase64String(Encoding.UTF8.GetBytes(t));
        }
        private string GetBase64(string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }

        public bool GetIntegral(string prefixExpression, IEnumerable<string> variables, string variable, out string result)
        {
            string base64Expression = GetBase64(prefixExpression);
            return Request($"/math?method=int&expr={base64Expression}&var={string.Join("&var=", variables)}&d={variable}", out result);
        }

        public bool GetIntegral(string prefixExpression, IEnumerable<string> variables, string variable, string lowerLimit, string upperLimit, out string result)
        {
            string base64Expression = GetBase64(prefixExpression);
            string base64LowerLimit = GetBase64(lowerLimit);
            string base64UpperLimit = GetBase64(upperLimit);

            return Request($"/math?method=int&expr={base64Expression}&var={string.Join("&var=", variables)}&d={variable}&ulim={base64UpperLimit}&llim={base64LowerLimit}", out result);
        }

        public bool GetLimit(string prefixExpression, IEnumerable<string> variables, string argument, string valueApproaching, out string result)
        {
            return GetLimit(prefixExpression, variables, argument, valueApproaching, "+-", out result);
        }

        public bool GetLimit(string prefixExpression, IEnumerable<string> variables, string argument, string valueApproaching, string direction, out string result)
        {
            string base64Expression = GetBase64(prefixExpression);
            return Request($"/math?method=lim&expr={base64Expression}&var={string.Join("&var=", variables)}&arg={argument}&val={valueApproaching}&dir={((direction == "l") ? "1" : (direction == "r" ? "2" : "3"))}", out result);
        }

        public bool GetSolve(IEnumerable<string> prefixExpressions, IEnumerable<string> variables, IEnumerable<string> solveFors, out string result)
        {
            return Request($"/math?method=sol&expr={string.Join("&expr=", GetBase64(prefixExpressions))}&var={string.Join("&var=", variables)}&solve={string.Join("&solve=", solveFors)}", out result);
        }

        private bool Request(string path, out string value)
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
            catch (WebException e)
            {
                if (e.Response != null)
                    value = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                else
                    value = "general errer";
                return false;
            }
            catch
            {
                value = "general error";
                return false;
            }
        }

        public Term Integrate(Term expr, IEnumerable<string> usedSymbols, Term var)
        {
            if (IsOnline)
            {
                string term;
                GetIntegral(var.ToPrefix(), usedSymbols, var.ToString(), out term);
                return Term.Parse(term.Replace("**", "^").Replace("-oo", "ninf").Replace("oo", "pinf"));
            }
            return null;
        }

        public Term Integrate(Term expr, IEnumerable<string> usedSymbols, Term var, Term upperLimit, Term lowerLimit)
        {
            if (IsOnline)
            {
                string term;
                GetIntegral(var.ToPrefix(), usedSymbols, var.ToString(), upperLimit.ToPrefix(), lowerLimit.ToPrefix(), out term);
                return Term.Parse(term.Replace("**", "^").Replace("-oo", "ninf").Replace("oo", "pinf"));
            }
            return null;
        }

        public Term Limit(Term expr, IEnumerable<string> usedSymbols, Term var, Term limit)
        {
            throw new NotImplementedException();
        }
    }
}
