using geographia.ags;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ROWM.Dal;
using SharePointInterface;

namespace ROWM
{
    public class StartupUinta
    {
        readonly IHostingEnvironment _env;

        public StartupUinta(IHostingEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            _env = env;
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
            services.AddScoped<ROWM.Dal.DocTypes>(fac => new DocTypes(fac.GetRequiredService<ROWM_Context>()));
            services.AddScoped<Controllers.ParcelStatusHelper>();
            services.AddScoped<Models.UpdateParcelStatus2>();

            var feat = new UbrrParcel("https://maps-stg.hdrgateway.com/arcgis/rest/services/Utah/UBRY_Parcel_FS/FeatureServer");
            services.AddSingleton<IFeatureUpdate>(feat);
            services.AddSingleton<IRenderer>(feat);

            services.AddScoped<ISharePointCRUD, DenverNoOp>();

            services.AddSingleton<SiteDecoration, Uinta>();

            #region local files
            var filep = _env.ContentRootFileProvider;
            services.AddSingleton<Microsoft.Extensions.FileProviders.IFileProvider>(filep);
            #endregion

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

