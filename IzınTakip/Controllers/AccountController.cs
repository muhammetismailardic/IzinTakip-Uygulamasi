using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IzinTakip.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IzinTakip.UI.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private RoleManager<Role> _roleManager;
        private SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager,
                                 RoleManager<Role> roleManager,
                                 SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        //[HttpGet]
        //public IActionResult Register()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Register(RegisterViewModel registerViewModel)
        //{
        //    if (ModelState.IsValid && (registerViewModel.Password == registerViewModel.ConfirmPassword))
        //    {
        //        User user = new User
        //        {
        //            UserName = registerViewModel.UserName,
        //            Email = registerViewModel.Email
        //        };

        //        IdentityResult result =
        //            _userManager.CreateAsync(user, registerViewModel.Password).Result;

        //        if (result.Succeeded)
        //        {
        //            if (!_roleManager.RoleExistsAsync("Subscriber").Result)
        //            {
        //                Role role = new Role
        //                {
        //                    Name = "Subscriber"
        //                };

        //                IdentityResult roleResult = _roleManager.CreateAsync(role).Result;

        //                if (!roleResult.Succeeded)
        //                {
        //                    ModelState.AddModelError("", "We can't add the role!");
        //                    return View(registerViewModel);
        //                }
        //            }
        //            _userManager.AddToRoleAsync(user, "Subscriber").Wait();
        //            return RedirectToAction("Index", "Home");
        //        }

        //        TempData.Put("modal", new ModalData
        //        {
        //            Type = "registration-modal",
        //            Error = "Please Insert Correct values!",
        //        });

        //        return RedirectToAction("Index", "Home");
        //    }

        //    return RedirectToAction("Index", "Home");
        //}

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel)
        {
            try
            {
                //TODO: This will be deleted later.
                var user = _userManager.FindByNameAsync(loginModel.UserName).Result;
                var role = _userManager.GetRolesAsync(user).Result.SingleOrDefault();

                if (ModelState.IsValid)
                {
                    //TODO: This condition will be deleted.
                    if (role == "Administrator")
                    {
                        var result = _signInManager.PasswordSignInAsync(loginModel.UserName,
                        loginModel.Password, loginModel.RememberMe, lockoutOnFailure: false).Result;

                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index", "Employee");
                        }

                        ModelState.AddModelError("", "Invalid login attempt!");
                    }
                    else { return RedirectToAction("Login", "Account"); }
                }

                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            _signInManager.SignOutAsync().Wait();
            return RedirectToAction("Login", "Account");
        }
    }
}
