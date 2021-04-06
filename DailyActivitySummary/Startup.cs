using System;
using System.Collections.Generic;
using System.Text;
using DailyActivitySummary.Dal;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(DailyActivitySummary.Startup))]

namespace DailyActivitySummary
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDbContext<DailySummaryContext>( opt => opt
                .EnableSensitiveDataLogging()
                .UseSqlServer("data source=tcp:b2h.database.windows.net;initial catalog=b2h_prod_v2;persist security info=True;user id=rowm_app;password=SbhrDX6Cq5VPcR9z;MultipleActiveResultSets=True;App=Functions"));

            builder.Services.AddScoped<DailySummary>();
        }
    }
}
