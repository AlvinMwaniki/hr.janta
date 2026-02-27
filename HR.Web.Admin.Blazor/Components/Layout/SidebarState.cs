namespace HR.Web.Admin.Blazor.Components.Layout
{
	public static class SidebarState
	{
		public static bool IsOpen { get; private set; }

		public static event Action? OnChange;

		public static void Toggle()
		{
			IsOpen = !IsOpen;
			OnChange?.Invoke();
		}
	}
}
