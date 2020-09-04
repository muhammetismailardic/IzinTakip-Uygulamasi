using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IzinTakip.Business.Abstract;
using IzinTakip.Business.Concrete;
using IzinTakip.DataAccess.Abstract;
using IzinTakip.DataAccess.Concrete.EntityFramework;
using IzinTakip.Entities.Concrete;
using IzinTakip.UI.Models;
using IzinTakip.UI.Shared;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IzinTakip
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static object HttpClient { get; internal set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();

            services.AddDirectoryBrowser();

            //Adding Services..
            services.AddScoped<IEmployeeService, EmployeeManager>();
            services.AddScoped<IEmployeeDal, EfEmployeeDal>();
            services.AddScoped<IEmployeeAnnualDetailsService, EmployeeAnnualDetailsManager>();
            services.AddScoped<IEmployeeAnnualDetailsDal, EfEmployeeAnnualDetailsDal>();
            services.AddScoped<IEmployeeSpecialLeaveDal, EfEmployeeSpecialLeaveDal>();
            services.AddScoped<IEmployeeSpecialLeaveService, EmployeeSpecialLeaveManager>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDbContext<IzinTakipContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")),
                    ServiceLifetime.Scoped);

            //Adding Identity 
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<IzinTakipContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie();

            services.AddAuthorization();

            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Default2Days",
                    new CacheProfile()
                    {
                        Duration = 172800
                    });
            });

            //services.AddHostedService<TimedHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Employee}/{action=Index}/{id?}");

                    endpoints.MapRazorPages();
                    //endpoints.MapControllerRoute(
                    //name: "default",
                    //pattern: "{controller=Employee}/{action=Details}/{id?}");

                    //endpoints.MapRazorPages();
                });
            });

            SeedData.CreateRolesAndAdminUser(serviceProvider);
        }
    }
}
