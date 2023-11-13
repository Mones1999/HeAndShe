using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;
using ProjectMVC.Services;

namespace ProjectMVC.Controllers
{
    public class WishlistsController : Controller
    {
        private readonly ModelContext _context;
		private readonly CurrencyService _currencyService;

		public WishlistsController(ModelContext context, CurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }
        

        public IActionResult MyWishList()
        {

            var CurrentUserId = HttpContext.Session.GetInt32("UserId");
            if (CurrentUserId != null)
            {
                var wishList = _context.Wishlists.Where(w => w.Userid == CurrentUserId).Include(w => w.Product).ToList();
                _currencyService.ConvertToSelectedCurrency(wishList);
                ViewBag.CurrencyCode = _currencyService.GetCurrentCurrencyCode();
                return View(wishList);
            }
            else 
            {
                return RedirectToAction("RegisterLoginView", "Account");
            }
		}
        private decimal ConvertToSelectedCurrency(decimal basePrice, decimal exchangeRate)
        {
            return basePrice * exchangeRate;
        }
        public IActionResult AddToWishList(decimal productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            var CurrentUserId = HttpContext.Session.GetInt32("UserId");
            if (CurrentUserId != null)
            {
                // Check if the specific product is already in the list for the current user
                var ListItem = _context.Wishlists.SingleOrDefault(c => c.Userid == CurrentUserId && c.Productid == productId);
                if (ListItem == null)
                { // If null add new
                    var NewListItem = new Wishlist
                    {
                        Userid = CurrentUserId.Value,
                        Productid = productId,
                        Dateadded = DateTime.Now
                    };
                    _context.Wishlists.Add(NewListItem);
                    //NewListItemView = new WishListItemModel {
                    //    ProductId = productId,
                    //    ProductName = product.Productname,
                    //    Price = product.Price,
                    //    ImageUrl = product.Imageurl,
                    //    Dateadded = DateTime.Now
                    //};
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
            else
            {
                return RedirectToAction("RegisterLoginView", "Account");
            }

        }

        public IActionResult RemoveFromWishList(decimal wishListId)
        {

            var wishListItem = _context.Wishlists.Find(wishListId);

            if (wishListItem == null)
            {
                return NotFound();
            }

            _context.Wishlists.Remove(wishListItem);

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
