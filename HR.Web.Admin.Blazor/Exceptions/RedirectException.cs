namespace HR.Web.Admin.Blazor.Exceptions
{
	// This exception is used solely to abort the Blazor circuit rendering 
	// when an HTTP response (like a redirect) has already been started.
	public class RedirectException : Exception
	{
		public RedirectException() { }
	}
}