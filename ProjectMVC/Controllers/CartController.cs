using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;
using ProjectMVC.Services;

namespace ProjectMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly ModelContext _context;
        private readonly CurrencyService _currencyService;

        

        public CartController(ModelContext context, CurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }
        public IActionResult MyCart() {
            var CurrentUserId = HttpContext.Session.GetInt32("UserId");

            if (CurrentUserId != null)
            {

				var carts = _context.Shoppingcarts.Include(c => c.Product).Where(c => c.Userid == CurrentUserId).ToList();

				
				carts = _currencyService.ConvertToSelectedCurrency(carts);
                ViewBag.CurrencyCode = _currencyService.GetCurrentCurrencyCode();


                return View(carts);
            }
            else
            {
                return RedirectToAction("RegisterLoginView", "Account");
            }


        }

        public IActionResult AddToCart(decimal productId)
        {
            var product = _context.Products.Find(productId);

            if (product == null)
            {
                return NotFound();
            }

            var CurrentUserId = HttpContext.Session.GetInt32("UserId");
            if (CurrentUserId != null)
            {
                // Check if the specific product is already in the cart for the current user
                var cartItem = _context.Shoppingcarts.SingleOrDefault(c => c.Userid == CurrentUserId && c.Productid == productId);

                if (cartItem != null)
                {
                    // If the product is already in the cart, increase the quantity
                    cartItem.Quantity += 1;

                }
                else
                {
                    // If the product is not in the cart, add a new entry
                    var newCartItem = new Shoppingcart
                    {
                        Userid = CurrentUserId.Value,
                        Productid = productId,
                        Quantity = 1
                    };
                    _context.Shoppingcarts.Add(newCartItem);
                }

                _context.SaveChanges();

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
            else {
                return RedirectToAction("RegisterLoginView", "Account");
            }

        }

        public IActionResult RemoveFromCart(decimal cartId)
        {
            var cartItem = _context.Shoppingcarts.Find(cartId);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.Shoppingcarts.Remove(cartItem);
            _context.SaveChanges();

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

        [HttpPost]
        public IActionResult UpdateCart(decimal productId, int quantity)
        {
            // Get the product to ensure it exists
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }
            // Get the current user's ID from the session
            var CurrentUserId = HttpContext.Session.GetInt32("UserId");

            // If there's no user ID in the session, handle the error (e.g., user not logged in)
            if (CurrentUserId == null)
            {
                return RedirectToAction("RegisterLoginView", "Account");
            }

            // Check if the product is already in the cart for the current user
            var cartItem = _context.Shoppingcarts.SingleOrDefault(c => c.Userid == CurrentUserId && c.Productid == productId);

            if (cartItem != null)
            {
                // If the product is already in the cart, set its quantity to the provided value
                cartItem.Quantity += quantity;
            }
            else
            {
                // If the product is not in the cart, add a new entry with the provided quantity
                var newCartItem = new Shoppingcart
                {
                    Userid = CurrentUserId.Value,
                    Productid = productId,
                    Quantity = quantity
                };
                _context.Shoppingcarts.Add(newCartItem);
            }

            // Save changes to the database
            _context.SaveChanges();

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


        

	}
}
