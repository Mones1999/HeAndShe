using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Attributes;
using ProjectMVC.Models;
using ProjectMVC.Services;
using System.Linq;

namespace ProjectMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminController(ModelContext context, IEmailService emailService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
        }
        [AdminAuthorize]
        public IActionResult Index()
        {


            ViewBag.Users = _context.Users.Count();
            ViewBag.Orders = _context.Orders.Count();
            ViewBag.Products = _context.Products.Count();
            ViewBag.Categories = _context.Categories.Count();

            // Lists
            ViewBag.ProductsList = _context.Products.OrderByDescending(p => p.Rating).Where(p => p.Rating != null).Take(3).ToList();
            ViewBag.TestimonialsList = _context.Testimonials.Take(3).ToList();

            return View();
        }
        [AdminAuthorize]
        public IActionResult Orders()
        {
            var orders = _context.Orders.Include(o=> o.Orderdetails).ThenInclude(o=> o.Product).Include(o => o.User).ToList();
            return View(orders);
        }
		[AdminAuthorize]
        public IActionResult OrdersByDate(DateTime? From = null, DateTime? To = null)
        {
            IQueryable<Order> query = _context.Orders.Include(o => o.Orderdetails).ThenInclude(o => o.Product).Include(o => o.User);

            if (From != null && To != null)
            {
                query = query.Where(o => o.Orderdate >= From && o.Orderdate <= To);
            }
            else if (From == null && To != null)
            {
                query = query.Where(o => o.Orderdate <= To);
            }
            else if (From != null && To == null)
            {
                query = query.Where(o => o.Orderdate >= From);
            }
            else
            {
                
                ViewData["ErrorMessage"] = "Please choose a correct date range.";
                return View(new List<Order>()); 
            }

            var orders = query.ToList();
            return View(orders);
        }

        [AdminAuthorize]
        public async Task<IActionResult> OrderStatus(decimal orderId, string newStatus)
        {
            var order = _context.Orders.Find(orderId);
            var userEmail = _context.Users.SingleOrDefault(u => u.Userid == order.Userid).Email.ToString();
            if (order != null)
            {
                if (newStatus != null && newStatus == "accept")
                {
                    order.Status = "Accepted";

                    _context.Update(order);
                    await _context.SaveChangesAsync();

                    // Send Email Confirmed to user
                    _emailService.SendEmailConfirmed(order.Orderid, userEmail, true);
                }
                else if (newStatus != null && newStatus == "reject")
                {
                    order.Status = "Rejected";

                    _context.Update(order);
                    _context.SaveChanges();

                    // Send Email Confirmed to user
                    _emailService.SendEmailConfirmed(order.Orderid, userEmail, false);

                    // Return Money if rejected
                    var Userid = _context.Users.SingleOrDefault(u => u.Userid == order.Userid).Userid;
                    ReturnMoney(Userid, order.Totalamount);


                }
            }
            else {
                return NotFound();
            }

            // Redirect to the same page the user was on
            var referrer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referrer))
            {
                return Redirect(referrer);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [AdminAuthorize]
        public void ReturnMoney(decimal Userid, decimal? amount = 0)
        {
            var wallet = _context.Wallets.SingleOrDefault(w => w.Userid == Userid);
            wallet.Balance += amount;

            _context.Update(wallet);
            _context.SaveChanges();

            var walletTransation = new Wallettransaction() {
                Walletid = wallet.Walletid,
                Transactiontype = "Deposit",
                Amount = amount,
                Transactiondate = DateTime.Now,
                Description = $"Return credit for order"
            };

            _context.Update(walletTransation);
            _context.SaveChanges();

        }
        [AdminAuthorize]
        public IActionResult OrderDetails(int orderId)
        {
            var orderDetails = _context.Orderdetails.Include(o => o.Product).Where(o => o.Orderid == orderId).ToList();

            var referer = Request.Headers["Referer"].ToString();
            ViewBag.Referer = referer;

            return View(orderDetails);
        }
        [AdminAuthorize]
        public IActionResult Users() {
            var users = _context.Users.Include(u => u.Role);
            var reviews = _context.Reviews;
            var testimonials = _context.Testimonials;
            var wallets = _context.Wallets;
            var orders = _context.Orders;

            var userDetails = new List<UserDetails>();
            foreach (var user in users) {
                userDetails.Add(new UserDetails {
                    User = user,
                    Review = reviews.Where(r => r.Userid == user.Userid).ToList(),
                    Testimonial = testimonials.Where(r => r.Userid == user.Userid).ToList(),
                    Wallet = wallets.SingleOrDefault(w => w.Userid == user.Userid),
                    Order = orders.Where(o => o.Userid == user.Userid).ToList()
                });
            }


            return View(userDetails);
        }
        [AdminAuthorize]
        public IActionResult AddUser() 
        {
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Rolename");
            return View();
        }

        [AdminAuthorize]
        [HttpPost]
        public IActionResult AddUser(User user, decimal roleId) 
        {
            user.Roleid = roleId;
            _context.Add(user);
            _context.SaveChanges();

            var wallet = new Wallet() {
                Userid = user.Userid,
                Balance = 0
            };
            _context.Add(wallet);
            _context.SaveChanges();
            return RedirectToAction("Users","Admin");
        }

        [AdminAuthorize]
        public IActionResult userProfile(decimal userId) {
            var user = _context.Users.Include(u => u.Role).SingleOrDefault(u => u.Userid == userId);
            var reviews = _context.Reviews.Include(r => r.Product).Where(r => r.Userid == userId).ToList();
            var testimonials = _context.Testimonials.Where(t => t.Userid == userId).ToList();
            var wallet = _context.Wallets.SingleOrDefault(w => w.Userid == userId);
            var orders = _context.Orders.Where(o => o.Userid == userId).ToList();

            var usersDetails = new UserDetails() {
                User = user,
                Review = reviews,
                Testimonial = testimonials,
                Order = orders,
                Wallet = wallet,
            };
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Rolename", user.Roleid);
            return View(usersDetails);
        }
        [AdminAuthorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(decimal userId,decimal? roleId, string? userName = null, string? email = null, string? newPassword = null, string? confirmNewPassword = null, string? address = null, string? phoneNumber = null)
        {
            var user = _context.Users.Find(userId);

            if (user == null) {
                return NotFound();
            }
            if (userName != null)
            {
                user.Username = userName;
                _context.Update(user);
            }
            if (email != null) {
                user.Email = email;
                _context.Update(user);
            }
            if (roleId != null) 
            { 
                user.Roleid = roleId;
                _context.Update(user);
            }
            if (newPassword != null && confirmNewPassword != null)
            {
                if (newPassword == confirmNewPassword)
                {
                    user.Password = newPassword;
                    _context.Update(user);
                }
                else
                {
                    TempData["ErrorMessage"] = "Password not correct";
                }
            }
            else if (newPassword != null && confirmNewPassword == null || newPassword == null && confirmNewPassword != null)
            {
                TempData["ErrorMessage"] = "Password not correct";

                // Redirect to the same page the user was on
                var referrer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referrer))
                {
                    return Redirect(referrer);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            if (address != null) {
                user.Address = address;
                _context.Update(user);
            }
            if (phoneNumber != null)
            {
                user.Phonenumber = phoneNumber;
                _context.Update(user);
            }
            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Changed Successfully";
            }
            catch {
                TempData["ErrorMessage2"] = "Something Wrong";
            }
            // Redirect to the same page the user was on
            var referrer2 = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referrer2))
            {
                return Redirect(referrer2);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [AdminAuthorize]
        [HttpPost]
        public async Task<IActionResult> DeleteProfile(decimal userId)
        {

            _context.Remove(_context.Users.Find(userId));
            await _context.SaveChangesAsync();

            // Return a JSON response for the AJAX call
            return Json(new { success = true, message = "User deleted successfully." });
        }
        [AdminAuthorize]
        public IActionResult OrdersByUser(decimal userId) {

            var orders = _context.Orders.Where(o => o.Userid == userId).ToList();

            ViewBag.UserName = _context.Users.SingleOrDefault(u => u.Userid == userId).Username;



            return View(orders);
        }
        [AdminAuthorize]
        public IActionResult Reviews()
        {

            var reviews = _context.Reviews.Include(r => r.Product).Include(r => r.User).ToList();

            if (reviews != null)
            {
                return View(reviews);
            }
            else
            {
                return RedirectToAction("Users");
            }
        }
        [AdminAuthorize]
        public IActionResult ReviewsByUser(decimal userId) {

            var reviews = _context.Reviews.Include(r => r.Product).Where(r => r.Userid == userId).ToList();

            if (reviews != null)
            {
                ViewBag.UserName = _context.Users.SingleOrDefault(u => u.Userid == userId).Username;
                ViewBag.UserId = userId;

                return View(reviews);
            }
            else {
                return RedirectToAction("Users");
            }
        }
        [AdminAuthorize]
        public async Task<IActionResult> DeleteReview(decimal id) {


            _context.Remove(_context.Reviews.Find(id));
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Deleted successfully";

            // Redirect to the same page the user was on
            var referrer2 = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referrer2))
            {
                return Redirect(referrer2);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }
        [AdminAuthorize]
        public IActionResult Testimonails()
        {

            var testimonials = _context.Testimonials.Include(t => t.User).ToList();

            return View(testimonials);
        }
        [AdminAuthorize]
        public IActionResult Contact()
        {
            var contacts = _context.Contacts.ToList();
            return View(contacts);
        }
        [AdminAuthorize]
        public IActionResult ContactMessage(decimal id)
        {
            var message = _context.Contacts.Find(id);
            return View(message);
        }
        [AdminAuthorize]
        public async Task<IActionResult> DeleteMessage(decimal id)
        {
            _context.Remove(_context.Contacts.Find(id));

            await _context.SaveChangesAsync();

            return RedirectToAction("Contact");

        }
        [AdminAuthorize]
        public IActionResult TestimonailsByUser(decimal userId) {

            var testimonials = _context.Testimonials.Include(t => t.User).Where(t => t.Userid == userId).ToList();
            ViewBag.UserId = userId;
            ViewBag.UserName = _context.Users.SingleOrDefault(u => u.Userid == userId).Username;
            return View(testimonials);
        }
        [AdminAuthorize]
        public async Task<IActionResult> UpdateTestimonailStatus(decimal id, bool approved) {

            var testimonial = _context.Testimonials.Find(id);
            if (testimonial != null)
            {
                if (approved)
                {
                    testimonial.Status = "Approved";
                    _context.Testimonials.Update(testimonial);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    testimonial.Status = "Rejected";
                    _context.Testimonials.Update(testimonial);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {

                return NotFound();
            }

            // Redirect to the same page the user was on
            var referrer2 = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referrer2))
            {
                return Redirect(referrer2);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [AdminAuthorize]
        public async Task<IActionResult> DeleteTestimonial(decimal id)
        {
            var testimonial = _context.Testimonials.Find(id);
            try
            {
                _context.Testimonials.Remove(testimonial);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Deleted successfully";
            }
            catch
            {
                TempData["ErrorMessage"] = "Something wrong";
            }
            // Redirect to the same page the user was on
            var referrer2 = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referrer2))
            {
                return Redirect(referrer2);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [AdminAuthorize]
        public IActionResult ProductsReport() {

            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Orderdetails)
                .Include(p => p.Reviews)
                .ToList();

            var productReportViewModel = products.Select(p => new ProductReportViewModel
            {
                ProductId = p.Productid,
                ProductName = p.Productname,
                ProductImage = p.Imageurl,
                CategoryName = p.Category?.Categoryname,
                Price = p.Price,
                StockQuantity = p.Stockquantity,
                Status = p.Status,
                Rating = p.Rating,
                NumberOfSales = (int)p.Orderdetails.Sum(od => od.Quantity),
                TotalSalesRevenue = (decimal)p.Orderdetails.Sum(od => od.Quantity * od.Priceattimeofpurchase),
                NumberOfReviews = p.Reviews.Count
            }).ToList();

            var totalSalesAmount = _context.Orderdetails
                                    .Where(od => od.Product != null)
                                    .Sum(od => od.Quantity * od.Priceattimeofpurchase);

            ViewBag.TotalSalesAmount = totalSalesAmount;

            ViewBag.TotalProducts = products.Count();
            ViewBag.TotalCategories = _context.Categories.Count();
            ViewBag.TotalOrders = _context.Orders.Count();


            var categories = _context.Categories.Include(c => c.Gender).Include(c => c.Products).ToList();

            var tuple = Tuple.Create(productReportViewModel, categories);
            ViewData["Categoryid"] = new SelectList(_context.Categories, "Categoryid", "Categoryname");
            return View(tuple);
        }
        [AdminAuthorize]
        public IActionResult FilterProductsReport(List<decimal>? category = null, decimal? price = null)  
        {
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Orderdetails)
                .Include(p => p.Reviews)
                .ToList();

            if (category != null) 
            {
                products = products.Where(p => category.Contains((decimal)p.Categoryid)).ToList();
            }
            if (price != null) 
            {
                products = products.Where(p=> p.Price <= price).ToList();
            }

            var productReportViewModel = products.Select(p => new ProductReportViewModel
            {
                ProductId = p.Productid,
                ProductName = p.Productname,
                ProductImage = p.Imageurl,
                CategoryName = p.Category?.Categoryname,
                Price = p.Price,
                StockQuantity = p.Stockquantity,
                Status = p.Status,
                Rating = p.Rating,
                NumberOfSales = (int)p.Orderdetails.Sum(od => od.Quantity),
                TotalSalesRevenue = (decimal)p.Orderdetails.Sum(od => od.Quantity * od.Priceattimeofpurchase),
                NumberOfReviews = p.Reviews.Count
            }).ToList();


            var totalSalesAmount = _context.Orderdetails
                                    .Where(od => od.Product != null)
                                    .Sum(od => od.Quantity * od.Priceattimeofpurchase);

            ViewBag.TotalSalesAmount = totalSalesAmount;

            ViewBag.TotalProducts = products.Count();
            ViewBag.TotalCategories = _context.Categories.Count();
            ViewBag.TotalOrders = _context.Orders.Count();


            var categories = _context.Categories.Include(c => c.Gender).Include(c => c.Products).ToList();

            var tuple = Tuple.Create(productReportViewModel, categories);
            ViewData["Categoryid"] = new SelectList(_context.Categories, "Categoryid", "Categoryname");

            return View(tuple);
        }

        [AdminAuthorize]
        public IActionResult ProductDetails(decimal id)
        {
            // Get Product By ID
            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Orderdetails)
                .Include(p => p.Reviews).ThenInclude(r => r.User).SingleOrDefault(p => p.Productid == id);

            // Create ProductReportViewModel obj
            var productReportViewModel = new ProductReportViewModel
            {
                ProductId = product.Productid,
                ProductName = product.Productname,
                CategoryName = product.Category?.Categoryname,
                Price = product.Price,
                StockQuantity = product.Stockquantity,
                Status = product.Status,
                Rating = product.Rating,
                NumberOfSales = (int)product.Orderdetails.Sum(od => od.Quantity),
                TotalSalesRevenue = (decimal)product.Orderdetails.Sum(od => od.Quantity * od.Priceattimeofpurchase),
                NumberOfReviews = product.Reviews.Count
            };

            // Create Tuple

            var tuple = Tuple.Create(product, productReportViewModel);

            return View(tuple);

        }
        [HttpPost]
        [AdminAuthorize]
        public async Task<IActionResult> EditProduct(decimal productId, string? productName = null, string? description = null, string? size = null, decimal? price = null, IFormFile? ImageFile = null) {

            var product = _context.Products.SingleOrDefault(p => p.Productid == productId);

            if (productName != null)
            {
                product.Productname = productName;
                _context.Products.Update(product);
            }

            if (description != null)
            {
                product.Description = description;
                _context.Update(product);
            }
            if (size != null) {
                product.Productsize = size;
                _context.Update(product);
            }
            if (price != null)
            {
                product.Price = price;
                _context.Update(product);
            }
            if (ImageFile != null)
            {

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }
                product.Imageurl = fileName;
                _context.Update(product);

            }
            await _context.SaveChangesAsync();
            // Redirect to the same page the user was on
            var referrer2 = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referrer2))
            {
                return Redirect(referrer2);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<IActionResult> DeleteProduct(decimal productId)
        {

            _context.Remove(_context.Products.Find(productId));
            await _context.SaveChangesAsync();

            // Return a JSON response for the AJAX call
            return Json(new { success = true, message = "Product deleted successfully." });
        }

        [AdminAuthorize]
        public IActionResult AddProduct()
        {
            ViewData["Categoryid"] = new SelectList(_context.Categories, "Categoryid", "Categoryname");
            return View();
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<IActionResult> AddProduct(string? productName = null, decimal? categoryId = null, decimal? price = null, IFormFile? ImageFile = null, decimal? quantity = null, string? size = null, string? description = null)
        {

            var product = new Product() {
                Productname = productName,
                Price = price,
                Categoryid = categoryId,
                Stockquantity = quantity,
                Productsize = size,
                Status = (quantity > 0 ? "In Stock" : "Out of stock"),
                Description = description
            };

            if (ImageFile != null)
            {

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }
                product.Imageurl = fileName;
            }

            _context.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("ProductsReport");
        }
        [AdminAuthorize]
        public IActionResult AddCategory()
        {
            ViewData["Genderid"] = new SelectList(_context.Genders, "Genderid", "Gendername");
            return View();
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<IActionResult> AddCategory(Category category)
        {
            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProductsReport");

        }

        [AdminAuthorize]
        public IActionResult CategoryDetails(decimal id) 
        {
            var category = _context.Categories.Find(id);
            ViewData["Genderid"] = new SelectList(_context.Genders, "Genderid", "Gendername", category.Genderid);
            return View(category);
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<IActionResult> EditCategory(Category category) 
        { 
            _context.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProductsReport");
        }
        [AdminAuthorize]
        public async Task<IActionResult> DeleteCategory(decimal id) 
        {
            var category = _context.Categories.Find(id);
            _context.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProductsReport");
        }


        [AdminAuthorize]
        public IActionResult Currencies()
        {
            var currencies = _context.Currencies.ToList();
            return View(currencies);

        }
        [AdminAuthorize]
        public IActionResult AddCurrency()
        {
            return View();
        }
        [AdminAuthorize]
        [HttpPost]
        public async Task<IActionResult> AddCurrency(Currency currency)
        {
            _context.Add(currency);
            await _context.SaveChangesAsync();
            return RedirectToAction("Currencies");
        }
        [AdminAuthorize]
        public IActionResult EditCurrency(decimal id) 
        {
            var currency = _context.Currencies.FirstOrDefault(c=> c.Currencyid == id); 
            return View(currency); 
        }
        [HttpPost]
        [AdminAuthorize]
        public async Task<IActionResult> EditCurrency(decimal id, string Currencyname, string Currencycode, decimal Exchangetobase) {

            var currency = new Currency() {
                Currencyid = id,
                Currencyname = Currencyname,
                Currencycode = Currencycode,
                Exchangetobase = Exchangetobase
            };

            _context.Currencies.Update(currency);
            await _context.SaveChangesAsync();

            return RedirectToAction("Currencies");
        }

        [AdminAuthorize]
        public async Task<IActionResult> DeleteCurrency(decimal id) 
        {
            _context.Remove(_context.Currencies.Find(id));
            await _context.SaveChangesAsync();
            return RedirectToAction("Currencies");

        }
    }
}
