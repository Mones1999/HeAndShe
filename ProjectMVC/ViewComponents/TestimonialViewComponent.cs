using Microsoft.AspNetCore.Mvc;
using ProjectMVC.Models;

namespace ProjectMVC.ViewComponents
{
    public class TestimonialViewComponent : ViewComponent
    {
        private readonly ModelContext _context;

        public TestimonialViewComponent(ModelContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync() 
        {
            var testimonials = _context.Testimonials.Where(t=> t.Status == "Approved").ToList();
            return View(testimonials);
        }
    }
}
