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
using MaximeRouiller.Azure.AppService.EasyAuth;
using ROWM.Controllers;

namespace ROWM
{
    public class StartupB2h
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

        public StartupB2h(IHostingEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            _env = env;
        }
        public IConfiguration Configuration { get; }
        private readonly IHostingEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("edit", policy => policy.RequireClaim("full-agent", "limited-edit"));
                opt.AddPolicy("financials", policy => policy.RequireClaim("full-agent"));
                opt.AddPolicy("contactlog", policy => policy.RequireClaim("full-agent", "limited-edit", "log"));               
            });

            services.AddAuthentication().AddEasyAuthAuthentication(opt => { });

            services.AddCors();

            // Add framework services.
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
            services.AddScoped<ROWM.Dal.StatisticsRepository>();
            services.AddScoped<ROWM.Dal.AppRepository>();
            services.AddScoped<ROWM.Dal.DocTypes>(fac => new Dal.DocTypes(new Dal.ROWM_Context(cs)));
            services.AddScoped<Controllers.ParcelStatusHelper>();
            services.AddScoped<B2hParcelHelper>();

            var feat = new B2hParcel("https://maps-stg.hdrgateway.com/arcgis/rest/services/Idaho/B2H_ROW_Parcels_FS/FeatureServer");
            services.AddSingleton<IFeatureUpdate>(feat);
            services.AddSingleton<IRenderer>(new B2hParcel("https://maps-stg.hdrgateway.com/arcgis/rest/services/Idaho/B2H_ROW_MapService/MapServer"));
            services.AddSingleton<B2hSymbology>();

            services.AddScoped<ISharePointCRUD, SharePointCRUD>();

            services.AddSingleton<SiteDecoration, B2H>();

            #region local files
            var filep = _env.ContentRootFileProvider;
            services.AddSingleton<Microsoft.Extensions.FileProviders.IFileProvider>(filep);
            #endregion

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

