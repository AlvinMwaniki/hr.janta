// HR.Web.Admin.Blazor.Models/SetPasswordModel.cs

using System.ComponentModel.DataAnnotations;

// Namespace is now simplified to just 'Models'
namespace HR.Web.Admin.Blazor.Models
{
	public class SetPasswordModel // Renamed from SetPasswordViewModel
	{
		// This will be populated from the URL query string by the component
		public string Token { get; set; } = string.Empty;

		[Required(ErrorMessage = "New password is required")]
		[DataType(DataType.Password)]
		[MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
		public string NewPassword { get; set; } = default!;

		[Required(ErrorMessage = "Password confirmation is required")]
		[DataType(DataType.Password)]
		[Compare(nameof(NewPassword), ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; } = default!;
	}
}