using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Visma.DataAccess;
using Newtonsoft.Json;
using System.IO;
using Visma.Models;
using Microsoft.AspNetCore.Http;

namespace Visma.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    IExceptionHandlerFeature contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        dynamic appsettings = JsonConvert.DeserializeObject(File.ReadAllText(Path.GetFullPath("appsettings.json")));

                        string connectionString = appsettings["ConnectionStrings"]["Default"];

                        var optionsBuilder = new DbContextOptionsBuilder<VismaDBContext>();
                            optionsBuilder.UseSqlServer(connectionString);

                        using(VismaDBContext dbContext = new VismaDBContext(optionsBuilder.Options))
                        {
                            dbContext.ExceptionLog.Add(new ExceptionLog()
                            {
                                ExceptionID = 0,
                                ExceptionType = contextFeature.Error.GetType().Name,
                                Message = contextFeature.Error.Message,
                                StackTrace = contextFeature.Error.StackTrace,
                                Time = DateTime.Now
                            });

                            dbContext.SaveChanges();
                        }

                        await context.Response.WriteAsync(new Error()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal server error"
                        }.ToString());
                    }
                });
            });
        }

        internal class Error
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }
    }
}
