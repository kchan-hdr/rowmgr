using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using geographia.ags;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ROWM.Dal;
using SharePointInterface;
using System.Threading.Tasks;

namespace ROWM
{
    public class StartupAtc862
    {
        //public Startup(IHostingEnvironment env)
        //{
        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(env.ContentRootPath)
        //        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        //        .AddEnvironmentVariables();
        //    if (env.IsDevelopment())
        //    {
        //        builder.AddUserSecrets<Startup>();
        //    }
        //    Configuration = builder.Build();
        //}

        public StartupAtc862(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            // Add framework services.
            services.AddApplicationInsightsTelemetry();
            services.AddMvc();

            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
            });

            var cs = Configuration.GetConnectionString("ROWM_Context");
            services.AddScoped<ROWM.Dal.ROWM_Context>(fac =>
            {
                return new ROWM.Dal.ROWM_Context(cs);
            });

            services.AddScoped<ROWM.Dal.OwnerRepository>();
            services.AddScoped<ROWM.Dal.ContactInfoRepository>();
            services.AddScoped<ROWM.Dal.StatisticsRepository>();
            services.AddScoped<ROWM.Dal.AppRepository>();
            services.AddScoped<DeleteHelper>();
            services.AddScoped<ROWM.Dal.DocTypes>(fac => new Dal.DocTypes(new Dal.ROWM_Context(cs)));
            services.AddScoped<Controllers.ParcelStatusHelper>();
            services.AddScoped<IFeatureUpdate, AtcParcel>(fac =>
                new AtcParcel("https://maps-stg.hdrgateway.com/arcgis/rest/services/California/ATC_Line862_Parcel_FS/FeatureServer"));

            var sec = AtcSharePointConfig.SharePointAppSecret();
            services.AddScoped<ISharePointCRUD, SharePointCRUD>(fac => new SharePointCRUD(
               d: fac.GetRequiredService<DocTypes>(), __appId: sec.AppId, __appSecret: sec.AppSec, _url: "https://atcpmp.sharepoint.com/atcrow/test862"));

            services.AddSingleton<SiteDecoration, Atc862>();

            services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

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

