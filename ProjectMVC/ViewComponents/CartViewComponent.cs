using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;
using ProjectMVC.Services;

namespace ProjectMVC.ViewComponents
{
    
    public class CartViewComponent : ViewComponent
    {
        private readonly ModelContext _context;
        private readonly CurrencyService _currencyService;

        public CartViewComponent(ModelContext context, CurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var CurrentUserId = HttpContext.Session.GetInt32("UserId");
            var cart = await _context.Shoppingcarts.Where(c => c.Userid == CurrentUserId).Include(c => c.Product).ToListAsync();
            
            decimal totalPrice = (decimal)cart.Sum(item => item.Product.Price * item.Quantity);
            ViewData["TotalPrice"] = totalPrice;
            ViewBag.CurrencyCode = _currencyService.GetCurrentCurrencyCode();


            return View(cart);
        }

    }

}
