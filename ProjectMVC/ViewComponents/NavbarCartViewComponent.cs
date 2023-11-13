using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;
using ProjectMVC.Services;

namespace ProjectMVC.ViewComponents
{
    
    public class NavbarCartViewComponent : ViewComponent
    {
        private readonly ModelContext _context;
        private readonly CurrencyService _currencyService;

        public NavbarCartViewComponent(ModelContext context, CurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var CurrentUserId = HttpContext.Session.GetInt32("UserId");
            var cartItems = new List<Shoppingcart>();

            
            cartItems = await _context.Shoppingcarts.Include(c => c.Product).Where(c => c.Userid == CurrentUserId).ToListAsync();
            
            ViewBag.CurrencyCode = _currencyService.GetCurrentCurrencyCode();

            decimal totalPrice = (decimal)cartItems.Sum(item => item.Product.Price * item.Quantity);
            ViewData["TotalPrice"] = totalPrice;
            int totalItem = (int)cartItems.Where(c => c.Userid == CurrentUserId).Sum(c => c.Quantity);
            ViewData["TotalItem"] = totalItem;
            
            

            return View(cartItems);
        }
    }
}
