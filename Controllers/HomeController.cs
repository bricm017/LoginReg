using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LoginReg.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LoginReg.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        // This is the registration page
        [HttpPost("users/create")]
        public IActionResult CreateUser(User newUser)
        {
            if(ModelState.IsValid)
            {
                //Will verify that email belongs to one person and gives an error if it does
                if(_context.Users.Any(s => s.Email == newUser.Email))
                {
                    ModelState.AddModelError("LogEmail", "Email already in use!");
                    return View("Index");
                }
                //This helps hash a password before it enters the DB but after validations
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                _context.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                return RedirectToAction("Success");
            } else {
                return View("Index");
            }
        }
        //This will be the login page
        [HttpGet("signin")]
        public IActionResult signin(LoginUser userSubmission)
        {
            HttpContext.Session.Clear();
            return View();
        }
        //This is the post request for logging in 
        [HttpPost("signin/login")]
        public IActionResult Login(LoginUser loginUser)
        {
            if(ModelState.IsValid)
            {
            User userInDb = _context.Users.FirstOrDefault(d => d.Email == loginUser.LogEmail);
            if(userInDb == null)
            {
                ModelState.AddModelError("Email", "Invaild Email/Password");
                return View("signin");
            }

            PasswordHasher<LoginUser> Hasher = new PasswordHasher<LoginUser>();
            var result = Hasher.VerifyHashedPassword(loginUser, userInDb.Password, loginUser.LogPassword);

            if(result == 0)
            {
                ModelState.AddModelError("Email", "Invaild Email/Password");
                return View("signin");
            }
            HttpContext.Session.SetInt32("UserId", userInDb.UserId);
            return RedirectToAction("Success");
            } else {
                return View("signin");
            }
        }

        //This page means you were able to register
        [HttpGet("Success")]
        public IActionResult Success()
        {
            //Lines 96-98 use session to only allow people that are logged in to view the success page
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            User LoggedInUser = _context.Users.FirstOrDefault(a => a.UserId == (int)HttpContext.Session.GetInt32("UserId"));
            return View(LoggedInUser);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
