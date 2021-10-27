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
//---------ClaimIdentity Cookie必要設定-----------
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
//---------ClaimIdentity Cookie必要設定-----------

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
				option.Cookie.HttpOnly = true;                              //HTTP認證，無法直接使用Web API方式直接存取頁面(預設已自動啟用)
				option.AccessDeniedPath = new PathString("/Home/Index");   //登入權限不足時，自動導向路徑
				option.LoginPath = new PathString("/Home/Index");          //未登入時，自動導向路徑
				option.ExpireTimeSpan = TimeSpan.FromMinutes(5);            //Cookie到期時間
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

			//---------ClaimIdentity Cookie必要設定-----------
			app.UseAuthentication();
			app.UseAuthorization();
			//---------ClaimIdentity Cookie必要設定-----------

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
