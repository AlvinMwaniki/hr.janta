// HR.Web.Admin.Models.ViewModels/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace HR.Web.Admin.Models.ViewModels
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "Email address is required")]
		[DataType(DataType.EmailAddress)]
		// Using Email to match the field in your User model for login purposes
		public string Email { get; set; } = default!;

		[Required(ErrorMessage = "Password is required")]
		[DataType(DataType.Password)]
		public string Password { get; set; } = default!;

		// This is necessary for Blazor Server/Web Apps that rely on anti-forgery tokens
		// and often involves external services or libraries.
		[Required(ErrorMessage = "Verification code is required")]
		public string CaptchaCode { get; set; } = default!;

		public string ReturnUrl { get; set; } = "/";
	}
}