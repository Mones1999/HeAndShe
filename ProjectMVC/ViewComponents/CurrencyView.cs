using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;

namespace ProjectMVC.ViewComponents
{
	public class CurrencyView : ViewComponent
	{
		private readonly ModelContext _context;

		public CurrencyView(ModelContext context)
		{
			_context = context;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var currencies = await _context.Currencies.ToListAsync();
			ViewBag.Currencies = currencies;
			return View(currencies);
		}
	}
}
