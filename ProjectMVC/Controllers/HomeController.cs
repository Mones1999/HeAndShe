using DinkToPdf;
using DinkToPdf.Contracts;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;
using ProjectMVC.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMVC.Controllers
{
    public class HomeController : Controller
    {
         
        private readonly ModelContext _context;
        private readonly CurrencyService _currencyService;
		private readonly IConverter _converter;
		public HomeController(ModelContext context, CurrencyService currencyService, IConverter converter)
        {
            _context = context;
            _currencyService = currencyService;
			_converter = converter;
        }
        public IActionResult Index()
        {
            ViewData["ShowSpecificStyles"] = true;

            var products = (IQueryable<Product>)_context.Products;

            products = _currencyService.ConvertToSelectedCurrency(products);

            ViewBag.CurrebcyCode = _currencyService.GetCurrentCurrencyCode();
            ViewBag.ProductAll = products.Include(p=> p.Category).Take(20).ToList();
            ViewBag.ProductMale = products.Include(p => p.Category).ThenInclude(c => c.Gender).Where(p => p.Category.Gender.Gendername == "Male").ToList();
            ViewBag.ProductFemale = products.Include(p => p.Category).ThenInclude(c => c.Gender).Where(p => p.Category.Gender.Gendername == "Female").ToList();
            return View();
        }

        public IActionResult AboutUs() 
        {
            var testimonials = _context.Testimonials.Where(t => t.Status == "Approved").ToList();
            return View(testimonials);
        }

        [HttpGet]
        public IActionResult ToPdf(decimal id)
		{
			var orderDetails = _context.Orderdetails.Include(o=> o.Product).Where(o=> o.Orderid == id); 
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


                <b>Order ID:</b> {orderDetails.Select(o=> o.Orderid).FirstOrDefault().ToString()}<br>
                <b>Payment Due:</b> {payment.Select(p=> p.Paymentdate).FirstOrDefault()}<br>
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

        [HttpPost]
        public async Task<IActionResult> ChargeWallet() 
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var wallet = _context.Wallets.SingleOrDefault(w=> w.Userid == userId);

			// Generate random number to store in balance
			Random rand = new Random();
			int randomNumber = rand.Next(50, 301);
            wallet.Balance += randomNumber;
            _context.Update(wallet);
            await _context.SaveChangesAsync();

            // New Transaction with Deposit type
            var tranaction = new Wallettransaction { 
                Walletid = wallet.Walletid,
				Transactiontype = "Deposit",
                Amount = randomNumber,
				Transactiondate = DateTime.Now,
				Description = "Deposit a balance to the wallet"
			};
			_context.Add(tranaction);
			await _context.SaveChangesAsync();

            TempData["ChargeSuccess"] = "The wallet has been charged successfully";
			return RedirectToAction("UserProfile");
        }


		public IActionResult UserProfile() 
        {
            // Get user id from session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
                // Get all user data by user id
                var userData = _context.Users.Include(u => u.Currency).SingleOrDefault(u => u.Userid == userId);
                var wallet = _context.Wallets.SingleOrDefault(w => w.Userid == userId);
                var orders = _context.Orders.OrderByDescending(o=> o.Orderdate).Where(o => o.Userid == userId).ToList();
                var transactions = _context.Wallettransactions.OrderByDescending(w => w.Transactiondate).Where(t=> t.Walletid == wallet.Walletid).ToList();
                // Add all data in new UserProfile model
                var userProfile = new UserProfile
                {
                    User = userData,
                    Wallet = wallet,
                    Wallettransaction = transactions, // List of transactions
					Order = orders // List of order
                };
				if (TempData["ErrorMessage"] != null)
				{
					ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
				}
				return View(userProfile);
            }
            else {
                return RedirectToAction("RegisterLoginView","Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfile
            (string? username = null,string? email = null, string? currentPassword = null, string? newPassword = null, string? newPasswordConfirm = null, string? address = null, string? phoneNumber = null)
        {
			var userId =  HttpContext.Session.GetInt32("UserId");

            // Get the user and modify
            if (userId == null) 
            {
				return RedirectToAction("RegisterLoginView", "Account");
			}
			var user = _context.Users.Find((decimal)userId.Value);
			
            if (username != null)
			{
				user.Username = username;
				_context.Update(user);
				await _context.SaveChangesAsync();
			}
            if (email != null) {
                // Chcek if email exist 

                var userEmail = _context.Users.AnyAsync(u => u.Email == email && u.Userid != userId);
                if (await userEmail)
                {
                    TempData["ErrorMessageEmail"] = "Email is already exist";
                    return RedirectToAction("UserProfile");
                }
                else 
                {
					user.Email = email;
					_context.Update(user);
					await _context.SaveChangesAsync();
				}
                
            }
			if (address != null)
			{
				user.Address = address;
				_context.Update(user);
				await _context.SaveChangesAsync();
			}
			if (phoneNumber != null)
			{
				user.Phonenumber = phoneNumber;
				_context.Update(user);
				await _context.SaveChangesAsync();
			}
			if (currentPassword != null) 
            {
                var ActualPassword = user.Password;

                // Check if the current password from user is correct 
                if (ActualPassword == currentPassword)
                {
                    if (newPassword != null && newPasswordConfirm != null)
                    {

                        // Check if new password and password confirm is equals
                        if (newPassword == newPasswordConfirm)
                        {
                            user.Password = newPassword;
                            _context.Update(user);
                            await _context.SaveChangesAsync();
                        }
                        else {
							TempData["ErrorMessage"] = "The new password and confirm new password not equals";
							return RedirectToAction("UserProfile");
						}
                    }
                }
                else {
					TempData["ErrorMessage"] = "The current password is incorrect.";
					return RedirectToAction("UserProfile");
				}
            }

			TempData["SuccessMessage"] = "Your information updated.";
			return RedirectToAction("UserProfile");
		}


        public IActionResult OrderDetails(decimal id) { 

            // Get Order details by order id
            var orderDetails = _context.Orderdetails.Include(o=> o.Order).Include(o=> o.Product).Where(o => o.Orderid == id).ToList();

            // Get payment date
            ViewBag.PaymentDate = _context.Payments.FirstOrDefault(p=> p.Orderid == id).Paymentdate;

            // Get user details
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.SingleOrDefault(u => u.Userid == userId);
            ViewBag.UserName = user.Username;
            ViewBag.UserAddress = user.Address;
            ViewBag.UserPhone = user.Phonenumber;
            ViewBag.UserEmail = user.Email;
            return View(orderDetails);
            
        }

        
        public IActionResult ProductsIn(int? id = null, int pg=1)
        {

            var products = _context.Products.Where(p => p.Categoryid == id);

            // Get Reviews 
            var reviews = _context.Reviews.Include(r=> r.Product).Where(r=> r.Product.Categoryid == id).ToList();


            // Assign # of Reviews and Rating for each product
            foreach (var product in products) 
            {
                product.Numberofreviews = reviews.Where(r => r.Productid == product.Productid).Count();
                product.Rating = reviews.Where(r=> r.Productid == product.Productid).Average(r => r.Rating);

                _context.Update(product);
                _context.SaveChanges();
            }

            

            // Pagination
            const int pageSize = 9;
            if (pg < 1)
                pg = 1;

            int proCount = products.Count();
			var pager = new Pager(proCount, pg, pageSize);
            int proSkip = (pg - 1) * pageSize;
            var data = products.Skip(proSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            
            // Convert To Selected Currency Service
            products = _currencyService.ConvertToSelectedCurrency(products);

            // View Bag For Category title
           // ViewBag.Title = _context.Categories.SingleOrDefault(c => c.Categoryid == id).Categoryname;
            
            // View Bag For Category title
            ViewBag.CurrencyCode = _currencyService.GetCurrentCurrencyCode();
            
            // View Bag For Category ID (For Filters Forms)
            ViewBag.CatId = id;
            
            return View(data);
        }


        
        public IActionResult ProductDetails(decimal id)
        {
            // Get Product By ID
            var product = _context.Products.Find(id);

            // Convert To Selected Currency Service
            product = _currencyService.ConvertToSelectedCurrency(product);

            var ProductReview = new ProductReviewModel
            {
                product = product,
                Review = new Review()
            };

            // Get All Reviews By Product ID
            var reviews = _context.Reviews.Where(r => r.Productid == id).ToList();

            // View Bag For Rating AVG
            ViewBag.ReviewAvg = reviews.Average(r => r.Rating);

            // View Bag For Review Count
            ViewBag.ReviewsCount = reviews.Count().ToString();

            // View Bag For Reviews
            ViewBag.Reviews = reviews;

            // View Bag For Currency Code
            ViewBag.CurrencyCode = _currencyService.GetCurrentCurrencyCode();
            
            return View(ProductReview);
        }
        
        [HttpGet]
        public IActionResult FilterProducts(int categoryId, decimal? priceTo = 500, List<string>? sizes = null, int pg = 1)
        {
            var Products = _context.Products.Where(p => p.Categoryid == categoryId);

            Products = _currencyService.ConvertToSelectedCurrency(Products);
            ViewBag.CurrencyCode = _currencyService.GetCurrentCurrencyCode();

            // Pagination
            const int pageSize = 9;
            if (pg < 1)
                pg = 1;

            int proCount = Products.Count();
            var pager = new Pager(proCount, pg, pageSize);
            int proSkip = (pg - 1) * pageSize;
            var data = Products.Skip(proSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;

            // Filter by selected sizes if any
            if (sizes != null && sizes.Any())
            {
                data = data.Where(p => sizes.Contains(p.Productsize)).ToList(); 
            }

            // Filter by price
            if (priceTo.HasValue)
            {
                data = data.Where(p => p.Price <= priceTo.Value).ToList();
            }

           
            ViewBag.CatId = categoryId;
            return View(data);
        }



        public IActionResult Details(int id)
        {

            return View();
        }

        


    }
}
