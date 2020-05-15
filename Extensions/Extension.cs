using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingWebApp.Extensions
{
    public static class Extension
    {
        public static void AddApplicationHeaders(this HttpResponse httpResponse,string errorMessage)
        {
            httpResponse.Headers.Add("Application-Error", errorMessage);
            httpResponse.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            httpResponse.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}
