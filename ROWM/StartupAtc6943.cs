using geographia.ags;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ROWM.Dal;
using SharePointInterface;
using ROWM.Dal;

namespace ROWM
{
    public class StartupAtc6943
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

        public StartupAtc6943(IConfiguration configuration)
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
            services.AddScoped<ROWM.Dal.ROWM_Context>(fac =>
            {
                return new ROWM.Dal.ROWM_Context(cs);
            });

            services.AddScoped<ROWM.Dal.OwnerRepository>();
            services.AddScoped<ROWM.Dal.ContactInfoRepository>();
            services.AddScoped<ROWM.Dal.StatisticsRepository>();
            services.AddScoped<ROWM.Dal.AppRepository>();
            services.AddScoped<DeleteHelper>();
            services.AddScoped<ROWM.Dal.DocTypes>(fac => new Dal.DocTypes(fac.GetRequiredService<ROWM_Context>()));
            services.AddScoped<Controllers.ParcelStatusHelper>();
            services.AddScoped<IFeatureUpdate, AtcParcel>(fac =>
                new AtcParcel("https://gis05s.hdrgateway.com/arcgis/rest/services/California/ATC_Line6943_Parcel_FS/FeatureServer"));

            var sec = AtcSharePointConfig.SharePointAppSecret();
            services.AddScoped<ISharePointCRUD, SharePointCRUD>(fac => new SharePointCRUD(
               d: fac.GetRequiredService<DocTypes>(), __appId: sec.AppId, __appSecret: sec.AppSec, _url: "https://atcpmp.sharepoint.com/atcrow/test6943"));
            //services.AddScoped<ISharePointCRUD, SharePointCRUD>(fac => new SharePointCRUD("e8d38b84-11bb-43df-b07d-a549b05eab19", "/kzpHsp4A8NXWYyhGOGI8LmA8jdBwZCtKjqLrfN3W3A=", "https://atcpmp.sharepoint.com/line6943",
            //    d: fac.GetRequiredService<Dal.DocTypes>()));

            services.AddSingleton<SiteDecoration, Atc6943>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "ROW Manager", Version = "v1" });
            });
            services.ConfigureSwaggerGen(o =>
            {
                o.OperationFilter<FileOperation>();
            });
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ROW Manager V1");
            });
        }
    }
}

