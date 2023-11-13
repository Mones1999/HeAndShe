using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;

public class CategoriesViewComponent : ViewComponent
{
    private readonly ModelContext _context;

    public CategoriesViewComponent(ModelContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync(int id)
    {
        var categories = await _context.Categories.Where(c=> c.Genderid == id).ToListAsync();
        return View(categories);
    }

    
}
