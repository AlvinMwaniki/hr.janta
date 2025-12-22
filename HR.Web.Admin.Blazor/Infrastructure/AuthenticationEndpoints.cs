using System.Security.Claims;

using HR.Services.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.Admin.Blazor.Infrastructure // Adjust namespace if necessary
{
	public static class AuthenticationEndpoints
	{
		// This method contains ALL the login logic.
		public static async Task<IResult> HandleLoginPostAsync(
			[FromServices] IAuthService authService,
			[FromServices] CaptchaService captchaService,
			//[FromServices] ProtectedSessionStorage sessionStorage,
			HttpContext context,
			[FromForm] string Email,
			[FromForm] string Password,
			[FromForm] string CaptchaCode,
			[FromForm] string ReturnUrl)
		{
			// ⭐ CRITICAL FIX: Get the expected code directly from the HTTP Cookie Collection ⭐
			context.Request.Cookies.TryGetValue("CaptchaCode", out string? expectedCode);

			// 1. Clear the cookie immediately after reading
			context.Response.Cookies.Delete("CaptchaCode");

			// 2. Validate using the simple service method
			bool isCaptchaValid = captchaService.ValidateCaptcha(expectedCode ?? string.Empty, CaptchaCode);

			if (!isCaptchaValid)
			{
				return Results.Redirect($"/login?ErrorMessage=InvalidCaptcha&ReturnUrl={ReturnUrl}");
			}

			// --- ⭐ 2. USER AUTHENTICATION (The missing AWAIT) ⭐ ---
			var user = await authService.AuthenticateUserAsync(Email, Password);

			if (user == null)
			{
				return Results.Redirect($"/login?ErrorMessage=InvalidCredentials&ReturnUrl={ReturnUrl}");
			}

			// --- 3. CLAIMS SETUP ---
			var claims = new List<Claim>
	{
		new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
		new Claim(ClaimTypes.Email, user.Email),
		new Claim(ClaimTypes.Name, user.Username),
		new Claim(ClaimTypes.Role, "Admin")
	};
			var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomAuth"));

			// --- ⭐ 4. SIGN-IN (HI ILI WORK POA ) ⭐ ---
			await context.SignInAsync(
				"CustomAuth",
				principal,
				new AuthenticationProperties()
				{
					IsPersistent = true,
					ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
				}
			);
			return Results.Redirect(ReturnUrl ?? "/dashboard");
		}

		// ⭐ NEW LOGOUT ENDPOINT ⭐
		public static async Task<IResult> HandleLogoutPostAsync(HttpContext context)
		{
			// 1. Sign out the user, using AuthenticationProperties to define the redirect location
			await context.SignOutAsync(
				"CustomAuth"// Use the scheme name
				
			);

			// 2. IMPORTANT: Since SignOutAsync now handles sending the HTTP redirect response, 
			// we must return Results.Empty to prevent the endpoint from trying to send a second redirect.
return Results.Ok();		}
	}
}