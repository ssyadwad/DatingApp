using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingWebApp.Contract;
using DatingWebApp.Data;
using DatingWebApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DatingWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionName = Configuration["ConnectionStrings:DefaultConnection"];
            services.AddDbContext<AppDbContext>(x=>x.UseSqlServer(connectionName));
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.Configure<IdentityOptions>(options=>{
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 10;
                options.Lockout.DefaultLockoutTimeSpan = new TimeSpan(0, 1, 0);
                options.Lockout.MaxFailedAccessAttempts = 5;
               

            });
            services.AddCors();
            services.AddScoped<IAuthRepository, AuthServiceLogin>();
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
