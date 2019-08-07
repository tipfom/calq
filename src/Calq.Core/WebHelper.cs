using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Calq.Core
{
    public static class WebHelper
    {
        public static string GetIntegral(string prefixExpression, List<string> variables, string variable)
        {
            string base64Expression = Convert.ToBase64String(Encoding.UTF8.GetBytes(prefixExpression));
            return Request($"method=int&function={base64Expression}&vars={string.Join("&vars=", variables)}&delta={variable}");
        }

        public static string GetIntegral(string prefixExpression, List<string> variables, string variable, string lowerLimit, string upperLimit)
        {
            string base64Expression = Convert.ToBase64String(Encoding.UTF8.GetBytes(prefixExpression));
            return Request($"method=int&function={base64Expression}&vars={string.Join("&vars=", variables)}&delta={variable}&lim1={lowerLimit}&lim2={upperLimit}");
        }

        public static string GetLimit(string prefixExpression, List<string> variables, string argument, string valueApproaching)
        {
            return GetLimit(prefixExpression, variables, argument, valueApproaching, "+-");
        }

        public static string GetLimit(string prefixExpression, List<string> variables, string argument, string valueApproaching, string direction)
        {
            string base64Expression = Convert.ToBase64String(Encoding.UTF8.GetBytes(prefixExpression));
            return Request($"method=lim&function={base64Expression}&vars={string.Join("&vars=", variables)}&arg={argument}&lim={valueApproaching}&dir={((direction == "+") ? "1" : (direction == "-" ? "2" : "3"))}");
        }

        private static string Request(string parameters)
        {
            WebRequest httpReq = WebRequest.CreateHttp($"http://localhost:8080/?" + parameters);
            httpReq.Method = "GET";
            WebResponse webResponse = httpReq.GetResponse();
            return new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
        }
    }
}
