using AppointmentScheduling.Data;
using AppointmentScheduling.Models;
using AppointmentScheduling.Models.ViewModels;
using AppointmentScheduling.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentScheduling.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        //UserManager is used as the database class for AspNetUser (CRUD Operations)
        UserManager<ApplicationUser> _userManager;
        //SignInManager contains the build in functions for login, logout, registartion, etc. 
        SignInManager<ApplicationUser> _signInManager;
        //RoleManager is used as the database class for AspNetRoles (CRUD Operations)
        RoleManager<IdentityRole> _roleManager;

        public AccountController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;  
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                // _signInManager.PasswordSignInAsync matches credentials with database values
                var resut = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if(resut.Succeeded)
                {
                    return RedirectToAction("Index", "Appointment");
                }
                ModelState.AddModelError("", "Login Failed");
            }
            return View(model);
        }

        public async Task<IActionResult> Register()
        {
            // _roleManager.RoleExistsAsync checks whether the value (Helper.Admin) exists in database or not
            if (!_roleManager.RoleExistsAsync(Helper.Admin).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole(Helper.Admin));
                await _roleManager.CreateAsync(new IdentityRole(Helper.Patient));
                await _roleManager.CreateAsync(new IdentityRole(Helper.Doctor));
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name
                };

                // _userManager.CreateAsync creates the values in the database for AspNetUser
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // _userManager.AddToRoleAsync adds a role for the user i.e., as FK in separate Table 
                    await _userManager.AddToRoleAsync(user, model.RoleName);
                    // _signInManager.SignInAsync signs in the user
                    if (!User.IsInRole(Helper.Admin))
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                    }
                    else
                    {
                        TempData["newAdminSignUp"] = user.Name;
                    }
                    return RedirectToAction("Index", "Appointment");
                }

                foreach(var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> LogOff()
        {
            // _signInManager.SignOutAsync is used to logout the user from the database
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}