using Microsoft.AspNetCore.Mvc;
using ProjectMVC.Models;

namespace ProjectMVC.Controllers
{
    public class TestimonialController : Controller
    {

        private readonly ModelContext _context;
        public TestimonialController(ModelContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Get user id from session
            var userId = HttpContext.Session.GetInt32("UserId");

            // Check user id is not null
            if (userId != null)
            {
                return View();
            }
            else 
            {
                return RedirectToAction("RegisterLoginView","Account");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> AddTestimonial(Testimonial testimonial) 
        {
            // Get user id from session 
            var userId = HttpContext.Session.GetInt32("UserId");
            if (ModelState.IsValid)
            {
                testimonial.Userid = userId;
                testimonial.Status = "Pending";
                _context.Add(testimonial);
                await _context.SaveChangesAsync();
                ViewData["Success"] = "Your Testimonial sent successfully";
            }
            else 
            {
                ViewData["Wrong"] = "Something is Wrong";  
            }

            return View(nameof(Index));
        }
    }
}
