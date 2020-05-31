using Cw3.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cw3.Middleware
{
    public class LoggingMiddleware
    {

        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IStudentsDbService service)
        {
            context.Request.EnableBuffering();
            if (context.Request != null)
            {
                string path = context.Request.Path;
                string method = context.Request.Method;
                string queryString = context.Request.QueryString.ToString();
                string bodyString = "";

                
                using (var reader = new StreamReader(context.Request.Body,
                    Encoding.UTF8, true, 1024, true))
                {
                    bodyString = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;

                    string fPath = "Logs/LOGS.txt";

                    using (StreamWriter writer = File.AppendText(fPath))
                    {

                        await writer.WriteLineAsync("Path: " + path + ", Method: " + method +
                            ", Querry: " + queryString + ".");
                        await writer.WriteLineAsync("Body: " + reader);

                    }
                }

            }

            if (_next != null) await _next(context);
        }

    }
}