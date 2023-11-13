namespace ProjectMVC.Services
{
	public interface IEmailService
	{
		public void SendEmail(decimal orderId, string to);

		public void SendEmailConfirmed(decimal orderId, string to, bool accepted);

    }
}
