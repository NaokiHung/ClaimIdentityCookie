using ClaimIdentityCookie.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
//---------ClaimIdentity Cookie必要設定-----------
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
//---------ClaimIdentity Cookie必要設定-----------

namespace ClaimIdentityCookie.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(UserDatas _userdata)
		{
			if ((_userdata == null) && (!ModelState.IsValid))
			{
				return View();
			}

			if ((_userdata.UserName == "123") && (_userdata.UserPasswd == "123"))
			{
				var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, _userdata.UserName),
						new Claim(ClaimTypes.NameIdentifier, _userdata.UserFullName),
						new Claim("FullName", _userdata.UserFullName),
						new Claim(ClaimTypes.Role, "Admin"),
					};

				var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

				//瀏覽器Cookie有兩種類型，一種是Session Cookie(預設使用這種方式)(瀏覽器關閉後Cookie就會自動刪除)
				//一種是Persistent Cookies(瀏覽器關閉後Cookie還會留存一段時間)
				//如果想使用Persistent Cookies方式可以參考以下方法。
				var authProperties = new AuthenticationProperties
				{
					//AllowRefresh = <bool>,
					// Refreshing the authentication session should be allowed.

					//ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
					// The time at which the authentication ticket expires. A 
					// value set here overrides the ExpireTimeSpan option of 
					// CookieAuthenticationOptions set with AddCookie.

					//IsPersistent = true,
					// Whether the authentication session is persisted across 
					// multiple requests. When used with cookies, controls
					// whether the cookie's lifetime is absolute (matching the
					// lifetime of the authentication ticket) or session-based.

					//IssuedUtc = <DateTimeOffset>,
					// The time at which the authentication ticket was issued.

					//RedirectUri = <string>
					// The full path or absolute URI to be used as an http 
					// redirect response value.
				};

				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
					new ClaimsPrincipal(claimsIdentity),
					authProperties);

				return RedirectToAction(nameof(Index2));
			}

			if ((_userdata.UserName == "456") && (_userdata.UserPasswd == "456"))
			{
				var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, _userdata.UserName),
						new Claim("FullName", _userdata.UserFullName),
						new Claim(ClaimTypes.Role, "Member"),
					};

				var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

				/*
				var authProperties = new AuthenticationProperties
				{

				};

				
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
					new ClaimsPrincipal(claimsIdentity),
					authProperties);
				*/

				await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

				return RedirectToAction(nameof(Index3));
			}

			return View();
		}


		[Authorize(Roles = "Admin")]
		//如果無登入，且登入權限不足就無法查看頁面。
		public IActionResult Index2()
		{
			if (User.Identity.IsAuthenticated)
			{
				ViewData["FullName"] = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == System.Security.Claims.ClaimTypes.Name).Value;
			}
			return View();
		}

		[Authorize]
		//如果無登入就無法看見頁面。
		public IActionResult Index3()
		{
			if (User.Identity.IsAuthenticated)
			{
				ViewData["FullName"] = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "FullName").Value;
			}

			return View();
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
