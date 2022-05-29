using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nivara.Core.ChatModule;
using Nivara.Core.Common;
using Nivara.Core.CompanyDetail;
using Nivara.Core.CompanyRole;
using Nivara.Core.Employee;
using Nivara.Core.EndUser;
using Nivara.Core.SideBar;
using Nivara.Core.UserAssignTask;
using Nivara.Core.UsersTask;
using Nivara.Data.Models.NivaraDbContext;
using Nivara.Web.ChatHub;
using Nivara.Web.ChatHub.UserConnection;
using System;
using Microsoft.AspNetCore.Authentication;
using Nivara.Models;
using Nivara.Core.PreDefinedTask;

namespace Nivara.Web
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
            string connectionString = Configuration.GetConnectionString("default");
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<NivaraDbContext>().AddDefaultTokenProviders();

            services.AddDbContext<NivaraDbContext>(c => c.UseSqlServer(connectionString));
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);

            });
            services.AddSignalR();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICompaniesServices, CompaniesServices>();
            services.AddScoped<ICompanyRolesServices, CompanyRolesServices>();
            services.AddScoped<ICommonServices, CommonServices>();
            services.AddScoped<IEmployeeServices, EmployeeServices>();
            services.AddScoped<IUsersTaskService, UsersTaskService>();
            services.AddScoped<IEndUserService, EndUserService>();
            services.AddScoped<IUserAssignTaskService, UserAssignTaskService>();
            services.AddScoped<IUserConnectionManager, UserConnectionManager>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<ISideBar, SideBar>();
            services.AddScoped<IPreDefinedTaskService, PreDefinedTaskService>();
            services.AddAuthentication()
                .AddGoogle("google", opt =>
                {
                    var googleAuth = Configuration.GetSection("Authentication:Google");
                    opt.ClientId = googleAuth["ClientId"];
                    opt.ClientSecret = googleAuth["ClientSecret"];
                    opt.SignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddFacebook(options =>
                {
                    var facebookAuth = Configuration.GetSection("Authentication:Facebook");
                    options.AppId = facebookAuth["AppId"];
                    options.AppSecret = facebookAuth["AppSecret"];
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                })
            .AddTwitter(options =>
             {
                 var twitterAuth = Configuration.GetSection("Authentication:Twitter");
                 options.ConsumerKey = twitterAuth["ConsumerKey"];
                 options.ConsumerSecret = twitterAuth["ConsumerSecret"];
                 options.SignInScheme = IdentityConstants.ExternalScheme;
             });

            services.Configure<AppSettingsModel>(Configuration.GetSection("SmtpMailInfo"));
        }

        //public void ConfigureContainer(ContainerBuilder builder)
        //{
        //    builder.RegisterModule(new BehaviourModule());
        //}
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<MessengerHub>("/MessengerHub");
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapAreaControllerRoute(
                //  name: "Areas",
                //  areaName: "EUsers",
                //  pattern: "EUsers/{controller=Home}/{action=Index}");

                endpoints.MapControllerRoute(
                    name: "area",
                    pattern: "{area:exists}/{controller=Account}/{action=Login}/{id?}");

                //     endpoints.MapAreaControllerRoute(
                //name: "MyAreaServices",
                //areaName: "EndUsers",
                //pattern: "EndUsers/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
