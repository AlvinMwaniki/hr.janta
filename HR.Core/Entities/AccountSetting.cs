namespace HR.Core.Entities
{
	public class AccountSetting
	{
		public int Id { get; set; }

		public int PasswordExpiryDays { get; set; }
		public int InActiveLockoutDays { get; set; }
		public int MinimumPasswordLength { get; set; }
	}
}
