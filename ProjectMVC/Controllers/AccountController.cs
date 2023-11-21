using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;
using System.Runtime.Intrinsics.X86;

namespace ProjectMVC.Controllers
{
	public class AccountController : Controller
	{
 // Some Edit
		private readonly ModelContext _context;
		public AccountController(ModelContext context)
		{
			_context = context;
		}
		public IActionResult RegisterLoginView()
		{
			ViewBag.RefererUrl = Request.Headers["Referer"].ToString();

			var CurrentUserId = HttpContext.Session.GetInt32("UserId");
			if (CurrentUserId != null)
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register([Bind("Username,Email,Password")] User model)
		{
			if (ModelState.IsValid)
			{
				/*
				 * Roles : 
				 * User	 => 1
				 * Admin => 2
				 */

				// Check if email exist 
				var usersEmail = _context.Users.AnyAsync(u=> u.Email == model.Email);
				var usersName = _context.Users.AnyAsync(u=> u.Username == model.Username);

				if (await usersEmail)
				{
					ViewData["EmailWrong"] = "Email Address already exist, please choose another email.";
					return View("RegisterLoginView", model);
				}
				else if (await usersName)
				{
                    ViewData["UserNameWrong"] = "Username already exist, please choose another username.";
                    return View("RegisterLoginView", model);

                }
				else 
				{
                    // Add new user 
                    var user = new User
                    {
                        Username = model.Username,
                        Email = model.Email,
                        Password = model.Password,
                        Registrationdate = DateTime.Now,
                        Roleid = 1
                    };

                    _context.Add(user);
                    await _context.SaveChangesAsync();

                    // Add new wallet for this user
                    var wallet = new Wallet
                    {
                        Userid = user.Userid,
                        Balance = 0
                    };

                    _context.Add(wallet);
                    await _context.SaveChangesAsync();
                }

				return RedirectToAction("Index", "Home");
			}

			return View("RegisterLoginView", model);
		}


        public IActionResult Login()
        {
			
			return View();
        }

        [HttpPost]
		public IActionResult Login([Bind("Username , Password")] User userLogin, string RefererUrl)
		{
			if (userLogin.Username != null && userLogin.Password != null)
			{
				var auth = _context.Users.SingleOrDefault(x => x.Username == userLogin.Username && x.Password == userLogin.Password);
				if (auth != null)
				{
					switch (auth.Roleid)
					{
						case 1:
							HttpContext.Session.SetInt32("UserId", (int)auth.Userid);
							HttpContext.Session.SetString("UserName", auth.Username.ToString());
							HttpContext.Session.SetInt32("UserRole", (int)auth.Roleid);
							break;

						case 2:
							HttpContext.Session.SetInt32("UserId", (int)auth.Userid);
							HttpContext.Session.SetString("UserName", auth.Username.ToString());
							HttpContext.Session.SetInt32("UserRole", (int)auth.Roleid);
							break;

					}

					if (RefererUrl != null)
					{

						return Redirect(RefererUrl); // Redirect to the previous page

					}
					else
					{
						return RedirectToAction("Index", "Home");
					}
				}
				else 
				{
					ViewData["ErrorMessage"] = "Incorrect information";
					return View("RegisterLoginView");
				}
			}
			return View("RegisterLoginView", userLogin);
		}

        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Redirect to the home page 
            return RedirectToAction("Index", "Home");
        }



    }
}
