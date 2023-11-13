using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using ProjectMVC.Models;

namespace ProjectMVC.Controllers
{
    public class UserReviewController : Controller
    {
        // Assume you have a DbContext object to interact with your database
        private readonly ModelContext _context;

        public UserReviewController(ModelContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(ProductReviewModel viewModel)
        {

            viewModel.Review.Userid = HttpContext.Session.GetInt32("UserId");
            viewModel.Review.Username = HttpContext.Session.GetString("UserName");
            viewModel.Review.Reviewdate = DateTime.Now;

            _context.Reviews.Add(viewModel.Review);
            await _context.SaveChangesAsync();

            
            return RedirectToAction("ProductDetails", "Home", new { id = viewModel.Review.Productid });

        }

    }
}
