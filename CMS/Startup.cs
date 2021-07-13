using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CMS
{
    public class Startup
    {
        private readonly IConfiguration Configuration;
        private readonly IWebHostEnvironment HostEnvironment;

        public Startup(IConfiguration configuration,
            IWebHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region BaseServices

            //add controller with views support
            var mvcBuilder = services.AddControllersWithViews();

            //add newtonsoft json serializer
            mvcBuilder.AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();

            });








            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver()
            };

            if (HostEnvironment.IsDevelopment())
                mvcBuilder.AddRazorRuntimeCompilation();




            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddDistributedMemoryCache();//To Store session in Memory, This is default implementation of IDistributedCache    
            services.AddSession(s => s.IdleTimeout = TimeSpan.FromMinutes(360));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();

            services.AddEntityFrameworkSqlServer().AddDbContext<CMSDBContext>(opt =>
            opt.UseSqlServer(Configuration.GetConnectionString("CMSDBContext"), b => b.MigrationsAssembly("CMSDBContext")));

            services.AddScoped(typeof(ISendMail), typeof(SendMail));

            services.AddScoped(typeof(IBaseSession), typeof(BaseSession));
            services.AddScoped(typeof(IGenericRepo<IBaseModel>), typeof(GenericRepo<CMSDBContext, IBaseModel>));
            #endregion


            var allprops = AppDomain.CurrentDomain.GetAssemblies();
            var props = allprops.Where(o => o.ManifestModule.Name == "DynamicSiteService.dll").FirstOrDefault().DefinedTypes;
            var servicesAll = props.Where(o => (!o.IsInterface && o.BaseType.Name.Contains("GenericRepo"))).ToList();
            servicesAll.ForEach(baseService =>
            {
                services.AddScoped(baseService.GetInterface("I" + baseService.Name), baseService);
            });


            services.AddAuthentication(IISDefaults.AuthenticationScheme);

            services.AddKendo();
           services.AddCors();
          //services.AddCors(options =>
          //{
          //    options.AddPolicy(name: "MyAllowSpecificOrigins",
          //                      builder =>
          //                      {
          //                          builder.WithOrigins("https://*.mydomain.com")
          //                      });
          //});



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IConfiguration config)
        {
            if (HostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
            }
            app.UseStaticFiles();
            //   app.UseCors(x => { x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithOrigins("https://*.interal.com.tr"); });
            app.UseCors(x => { x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });

            app.UseRouting();
            app.UseCookiePolicy();
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAuthenticationMiddleware();

            app.UseMiddleware<ErrorMid>();

            //app.InitializeDatabase();

            SessionRequest.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());



            var provider = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider.Mappings[".dwg"] = "application/acad";
            var provider2 = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider2.Mappings[".dwg"] = "application/acad";

            var provider3 = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider3.Mappings[".dwg"] = "application/acad";

            var provider4 = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider4.Mappings[".dwg"] = "application/acad";


            //TODO belirtilen folderlarda tanımlama
            //foreach (string d in Directory.GetDirectories("wwwroot"))
            //{
            //    Path.GetFileName(d);
            //
            //
            //}

            string[] pathArr = new string[] { "wwwroot", "fileupload", "UserFiles" };

            app.Use(async (context, next) =>
            {
                // if (!context.User.Identity.IsAuthenticated && context.Request.Path.StartsWithSegments("/CadFiles") )
                //HttpContext.Session.GetString("Test");

                if (SessionRequest._DocUser == null && context.Request.Path.StartsWithSegments("/CadFiles"))
                {
                    throw new Exception("Not authenticated");
                }
                await next.Invoke();
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fileupload", "UserFiles", "Folders", "CAD-Cephe Sistemleri-Mimarlar ve Uygulayıcılar")),
                RequestPath = "/CadFiles",
                ContentTypeProvider = provider
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
           Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fileupload", "UserFiles", "Folders", "CAD-Cephe Sistemleri-Sadece Uygulayıcılar")),
                RequestPath = "/CadFiles",
                ContentTypeProvider = provider2
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                   Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fileupload", "UserFiles", "Folders", "CAD-Doğrama-Mimarlar ve Uygulayıcılar")),
                RequestPath = "/CadFiles",
                ContentTypeProvider = provider3
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                   Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fileupload", "UserFiles", "Folders", "CAD-Doğrama-Sadece Uygulayıcılar")),
                RequestPath = "/CadFiles",
                ContentTypeProvider = provider4
            });


            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller=" + SessionRequest.StartPage + "}/{action=" + SessionRequest.StartAction + "}/{Id?}");
            });
        }
    }
}
