using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;

namespace ProjectMVC.ViewComponents
{
    public class WishListView : ViewComponent
    {
        private readonly ModelContext _context;
        public WishListView(ModelContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var CurrentUserId = HttpContext.Session.GetInt32("UserId");
            var wishListCount = _context.Wishlists.Where(w=> w.Userid == CurrentUserId).Count();
            ViewBag.WishlistCount = wishListCount.ToString();
            return View();
        }
    }
}
