using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using ProjectMVC.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ProjectMVC.Services
{
	public class EmailSrevice : IEmailService
	{
		private readonly IConfiguration _config;
		private readonly ModelContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EmailSrevice(IConfiguration config, ModelContext context, IHttpContextAccessor httpContextAccessor)
		{
			_config = config;
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}
		public void SendEmail(decimal orderId, string to)
		{
			// Get all order details
			var orderdetals = _context.Orderdetails.Include(o=> o.Product).Where(o=> o.Orderid == orderId).ToList();
            var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.SingleOrDefault(u => u.Userid == userId);
            var payment = _context.Payments.Where(p => p.Orderid == orderId).FirstOrDefault();

            StringBuilder htmlBuilder = new StringBuilder();

            htmlBuilder.Append(@"
            <style>
                h1
                {
                    text-align:center;
                }
                .col-md-4 {
                    width: 33.3333%;
                    float: left;
                }
                .table {
                    width: 100%;
                    margin-bottom: 1rem;
                    color: #212529;
                    border-collapse: collapse;
                    text-align:center;
                }
                .table th, .table td {
                    padding: 0.75rem;
                    vertical-align: top;
                    border-bottom: 1px solid #dee2e6;
                    text-align:center;
                }
                /* ... any other styles you want to include ... */
            </style>");

            // Include the content
            htmlBuilder.Append($@"
                <h1>He & She Store</h1>
                        <br>
                        <br>
            <div class='col-md-4'>
                From
                <div>
                    <strong>Admin, Inc.</strong><br>
                    795 Folsom Ave, Suite 600<br>
                    San Francisco, CA 94107<br>
                    Phone: (804) 123-5432<br>
                    Email: info@almasaeedstudio.com
                </div>
            </div>
            <!-- /.col -->
                <div class=""col-md-4"">
                    To
                    <div>
                    <strong>{user.Username}</strong><br>
                    {user.Address}<br>
                    Phone: {user.Phonenumber}<br>
                    Email: {user.Email}
                    </div>
                </div>
                <!-- /.col -->
                <div class=""col-sm-4"">


                <b>Order ID:</b> {orderdetals.Select(o => o.Orderid).FirstOrDefault().ToString()}<br>
                <b>Payment Due:</b> {payment.Paymentdate}<br>
                    <b>Account:</b> 968-34567
                </div>
                <!-- /.col -->
                <br>
                <br>
                <br>
            <table class='table'>
                <thead>
                    <tr>
                        <th>Order ID</th>
                        <th>Product Name</th>
                        <th>Price</th>
                        <th>Quantity</th>
                    </tr>
                </thead>
                     
                <tbody>");

            foreach (var item in orderdetals)
            {
                htmlBuilder.Append($@"
                <tr>
                    <td>{item.Orderid}</td>
                    <td>{item.Product.Productname}</td>
                    <td>{item.Priceattimeofpurchase}</td>
                    <td>{item.Quantity}</td>
                </tr>");
            }

            var totalPrice = orderdetals.Sum(o => o.Quantity * o.Priceattimeofpurchase);
            htmlBuilder.Append($@"
                    
                <tr>
                    <th>Total Price: {totalPrice}</th>
                </tr>
                </tbody>
            </table>");

            string message = htmlBuilder.ToString();

			

			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
			email.To.Add(MailboxAddress.Parse(to));
			email.Subject = $"Order Details {orderId}";
			email.Body = new TextPart(TextFormat.Html) { Text = message };

			using var client = new SmtpClient();
			client.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
			client.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
			client.Send(email);
			client.Disconnect(true);
			

		}

        public void SendEmailConfirmed(decimal orderId, string to, bool accepted) {
            // Get all order details
            var order = _context.Orders.Find(orderId);
            
            StringBuilder htmlBuilder = new StringBuilder();

            htmlBuilder.Append(@"
            <style>
                h1
                {
                    text-align:center;
                }
            </style>");

            // Include the content
            if (accepted)
            {
                htmlBuilder.Append($@"
                <h1>He & She Store</h1>
                        <br>
                        <br>
                       <h2>your Order with id : [{orderId}]</h2>
             has been delivered

             <h3>order details:</h3>
             <ul>Order ID: {orderId}</ul>
             <ul>Total Amount: {order.Totalamount}</ul>
             <ul>Shippment tracking number:{order.Shipmenttrackingnumber} </ul>

            ");
            }
            else {
                htmlBuilder.Append($@"
                <h1>He & She Store</h1>
                        <br>
                        <br>
                       <h2>your Order with id : [{orderId}]</h2>
             has been rejected

             <h3>order details:</h3>
             <ul>Order ID: {orderId}</ul>
             <ul>Total Amount: {order.Totalamount}</ul>
             <ul>Shippment tracking number:{order.Shipmenttrackingnumber} </ul>
                ");
            }

            string message = htmlBuilder.ToString();

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = $"Order Status Updated {orderId}";
            email.Body = new TextPart(TextFormat.Html) { Text = message };

            using var client = new SmtpClient();
            client.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            client.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
            client.Send(email);
            client.Disconnect(true);

        }

		
	}
}
