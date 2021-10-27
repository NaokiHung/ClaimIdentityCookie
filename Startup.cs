using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//---------ClaimIdentity Cookie���n�]�w-----------
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
//---------ClaimIdentity Cookie���n�]�w-----------

namespace ClaimIdentityCookie
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
			services.AddControllersWithViews();

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
			{
				option.Cookie.HttpOnly = true;                              //HTTP�{�ҡA�L�k�����ϥ�Web API�覡�����s������(�w�]�w�۰ʱҥ�)
				option.AccessDeniedPath = new PathString("/Home/Index");   //�n�J�v�������ɡA�۰ʾɦV���|
				option.LoginPath = new PathString("/Home/Index");          //���n�J�ɡA�۰ʾɦV���|
				option.ExpireTimeSpan = TimeSpan.FromMinutes(5);            //Cookie����ɶ�
			});
		}

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
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			//---------ClaimIdentity Cookie���n�]�w-----------
			app.UseAuthentication();
			app.UseAuthorization();
			//---------ClaimIdentity Cookie���n�]�w-----------

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
