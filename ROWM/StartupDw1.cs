using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Http.Features;
using geographia.ags;
using SharePointInterface;
using ROWM.Dal;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.AspNetCore.Mvc;
using ROWM.Models;

namespace ROWM
{
    public class StartupDw1
    {
        public StartupDw1(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            // Add framework services.
            services.AddApplicationInsightsTelemetry();
            services.AddMvc()
                .AddJsonOptions(o =>
                {
                    o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
            });

            var cs = Configuration.GetConnectionString("ROWM_Context");
            //services.AddScoped<ROWM.Dal.ROWM_Context>();
            services.AddScoped<ROWM.Dal.ROWM_Context>(fac =>
            {
               return new Dal.ROWM_Context(cs);
            });

            services.AddScoped<ROWM.Dal.OwnerRepository>();
            services.AddScoped<ROWM.Dal.ContactInfoRepository>();
            services.AddScoped<ROWM.Dal.StatisticsRepository>();
            services.AddScoped<ROWM.Dal.AppRepository>();
            services.AddScoped<DeleteHelper>();
            services.AddScoped<ROWM.Dal.DocTypes>(fac => new DocTypes(fac.GetRequiredService<ROWM_Context>()));
            services.AddScoped<Controllers.ParcelStatusHelper>();
            services.AddScoped<UpdateParcelStatus2>();
            services.AddScoped<IFeatureUpdate, DenverParcel>( fac => 
                new DenverParcel("https://gis05s.hdrgateway.com/arcgis/rest/services/California/DW_Parcel_FS/FeatureServer") 
            );

            var msi = new AzureServiceTokenProvider();
            var vaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(msi.KeyVaultTokenCallback));
            var appid = vaultClient.GetSecretAsync("https://denver-dev-keys.vault.azure.net/", "sharepoint-client-id").GetAwaiter().GetResult();
            var apps = vaultClient.GetSecretAsync("https://denver-dev-keys.vault.azure.net/", "sharepoint-api").GetAwaiter().GetResult();
            services.AddScoped<ISharePointCRUD, DenverNoOp>();

            services.AddSingleton<SiteDecoration, Dw>();

            services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseExceptionHandler("/Home/Error");
 
            app.UseStaticFiles();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
