using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;
using ProjectMVC.Services;
using System.Data.Odbc;
using System.Linq;
using System.Text;

namespace ProjectMVC.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly ModelContext _context;
		private IConverter _converter;
		private readonly IEmailService _emailService;
		private readonly CurrencyService _currencyService;
		public CheckoutController(ModelContext context, IConverter converter, IEmailService emailService,CurrencyService currencyService)
		{
			_context = context;
			_converter = converter;
			_emailService = emailService;
			_currencyService = currencyService;
		}

		
		public IActionResult Checkout()
		{
			// Get User Id
			var userId = HttpContext.Session.GetInt32("UserId");
			
			var carts = _context.Shoppingcarts.Include(c=> c.Product).Where(c=> c.Userid == userId).ToList();
			var totalPrice = carts.Sum(c => c.Quantity * c.Product.Price);
			totalPrice = _currencyService.ConvertToSelectedCurrency(totalPrice);
			ViewBag.TotalPrice = totalPrice;
			ViewBag.CurrencyCode = _currencyService.GetCurrentCurrencyCode();
			ViewBag.CartCount = _context.Shoppingcarts.Where(c => c.Userid == userId).Count();

			return View();
		}

		public async Task<IActionResult> CheckoutProcess(string address, string phoneNumber) {

			// Get user id from session 
			var userId = HttpContext.Session.GetInt32("UserId");

			// Get the user and update the email and phone number
			var user = _context.Users.Find((decimal)userId);

			user.Address = address;
			user.Phonenumber = phoneNumber;
			_context.Update(user);
			await _context.SaveChangesAsync();

			// Get user email by user Id (To send email notification)
			var userEmail = user.Email.ToString();

			// Get all from shopping carts 
			var carts = _context.Shoppingcarts.Where(s => s.Userid == userId).Include(s => s.Product).ToList();

			// Total price for cart
			var totalPrice = carts.Sum(s => s.Quantity * s.Product.Price);

			// Get wallet for current user
			var wallet = _context.Wallets.SingleOrDefault(s => s.Userid == userId);

			// Generate random number for Shipment tracking number
			Random rand = new Random();
			int randomNumber = rand.Next(1000000000, 2000000000);

			if (userId != null)
			{
				if (wallet.Balance >= totalPrice)
				{
					// Create new order
					var order = new Order
					{
						Userid = userId,
						Orderdate = DateTime.Now,
						Totalamount = totalPrice,
						Status = "Pending",
						Shipmenttrackingnumber = randomNumber.ToString()
					};
					_context.Add(order);
					await _context.SaveChangesAsync();

					// Create new order details 
					var orderDetails = new List<Orderdetail>();
					foreach (var cart in carts)
					{
						orderDetails.Add(new Orderdetail
						{
							Orderid = order.Orderid,
							Productid = cart.Productid,
							Quantity = cart.Quantity,
							Priceattimeofpurchase = cart.Product.Price
						});

						_context.Add(orderDetails.SingleOrDefault(o => o.Productid == cart.Productid));
						await _context.SaveChangesAsync();
					}

					// Create payment for order  
					if (order.Totalamount > 0)
					{

						var payment = new Payment
						{
							Orderid = order.Orderid,
							Walletid = wallet.Walletid,
							Paymentdate = DateTime.Now,
							Amount = order.Totalamount
						};

						_context.Add(payment);
						await _context.SaveChangesAsync();

						// Update the balance for wallet
						wallet.Balance -= order.Totalamount;
						_context.Update(wallet);
						await _context.SaveChangesAsync();

						var walletTransaction = new Wallettransaction
						{
							Walletid = wallet.Walletid,
							Transactiontype = "Withdraw",
							Amount = order.Totalamount,
							Transactiondate = DateTime.Now,
							Description = $"Withdraw for order [{order.Orderid}]"
						};
						_context.Update(walletTransaction);
						await _context.SaveChangesAsync();


						// Clear shopping cart after checkout
						foreach (var cart in carts)
						{
							_context.Remove(cart);
							await _context.SaveChangesAsync();
						}

						// Send Oder details to user email
						_emailService.SendEmail(order.Orderid, userEmail);

						return View("CheckoutSucess", orderDetails);
					}
					else
					{
						return RedirectToAction("Index", "Home");
					}
				}
				else {

					ViewData["BalancError"] = "You do not have enough credit";
					return View("Checkout");
				}
			}
			else { 
				return View("RegisterLoginView", "Account");
			}

			
		}

		public IActionResult ToPdf(decimal id)
		{
			var orderDetails = _context.Orderdetails.Include(o => o.Product).Where(o => o.Orderid == id);
			var htmlContent = GetOrderDetailsHtml(orderDetails);
			var globalSettings = new GlobalSettings
			{
				ColorMode = ColorMode.Color,
				Orientation = Orientation.Portrait,
				PaperSize = PaperKind.A4

			};

			var objectSettings = new ObjectSettings
			{
				HtmlContent = htmlContent
			};

			var pdfDoc = new HtmlToPdfDocument
			{
				GlobalSettings = globalSettings,
				Objects = { objectSettings }
			};

			var pdfData = _converter.Convert(pdfDoc);

			return File(pdfData, "application/pdf", "OrderDetails.pdf");
		}

		private string GetOrderDetailsHtml(IQueryable<Orderdetail> orderDetails)
		{
			StringBuilder htmlBuilder = new StringBuilder();
			var userId = HttpContext.Session.GetInt32("UserId");
			var user = _context.Users.SingleOrDefault(u => u.Userid == userId);
			var payment = _context.Payments.Where(p => p.Orderid == orderDetails.Select(o => o.Orderid).FirstOrDefault());
			// Include the styling
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


                <b>Order ID:</b> {orderDetails.Select(o => o.Orderid).FirstOrDefault().ToString()}<br>
                <b>Payment Due:</b> {payment.Select(p => p.Paymentdate).FirstOrDefault()}<br>
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

			foreach (var item in orderDetails)
			{
				htmlBuilder.Append($@"
                <tr>
                    <td>{item.Orderid}</td>
                    <td>{item.Product.Productname}</td>
                    <td>{item.Priceattimeofpurchase}</td>
                    <td>{item.Quantity}</td>
                </tr>");
			}

			var totalPrice = orderDetails.Sum(o => o.Quantity * o.Priceattimeofpurchase);
			htmlBuilder.Append($@"
                    
                <tr>
                    <th>Total Price: {totalPrice}</th>
                </tr>
                </tbody>
            </table>");

			return htmlBuilder.ToString();
		}


	}
}
