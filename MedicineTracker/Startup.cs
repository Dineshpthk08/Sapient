﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MedicineTracker.Context;
using MedicineTracker.Repositories;
using MedicineTracker.Repositories.Interfaces;
using MedicineTracker.Services;
using MedicineTracker.Services.Interfaces;

namespace MedicineTracker
{
    public class Startup
    {
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            ConfigureTransientServices(services);
            ConfigureRepositories(services);
            ConfigureEntityFramework(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        private static void ConfigureTransientServices(IServiceCollection services)
        {
            services.AddTransient<IMedicineService, MedicineService>();
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddSingleton<IMedicineRepository, MedicineRepository>();
        }

        private static void ConfigureEntityFramework(IServiceCollection services)
        {
            var databaseName = Configuration["EntityFramework:DatabaseName"];

            services.AddDbContext<MedicineDatabaseContext>(options =>
                options.UseInMemoryDatabase(databaseName));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}