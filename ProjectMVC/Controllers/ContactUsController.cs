using Microsoft.AspNetCore.Mvc;
using ProjectMVC.Models;

namespace ProjectMVC.Controllers
{
	public class ContactUsController : Controller
	{
		private readonly ModelContext _context; 
        public ContactUsController(ModelContext context)
        {
            _context = context;
        }
        public IActionResult Index()
		{
			return View();
		}

		[HttpPost]

		public async Task<IActionResult> SendMessage(Contact contact) {

			if (ModelState.IsValid)
			{
				if (contact.Subject == null) 
				{
					contact.Subject = "(No subject)";
				}
				_context.Add(contact);
				await _context.SaveChangesAsync();

				ViewData["Success"] = "Your Message Sent Successfully";
			}
			else 
			{
                ViewData["Wrong"] = "Something Wrong Happened";
            }

			return View("Index");
			
		}

    }
}
