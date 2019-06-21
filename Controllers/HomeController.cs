using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BeltExam.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace BeltExam.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
     
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        public int? user_id {
            get{ return HttpContext.Session.GetInt32("userid"); }
            set{ HttpContext.Session.SetInt32("userid", (int)value); }
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("home")]
        public IActionResult Home()
        {
            if(user_id == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.User = (int)user_id;
            User current = dbContext.Users.FirstOrDefault(u => u.UserId == (int)user_id);
            ViewBag.Name = current.FirstName;
            DateTime CurrentTime = DateTime.Now;
            List<Activity> AllActivities = dbContext.Activities
                .Include(a => a.Participants)
                .ThenInclude(ua => ua.Activity)
                .Include(a => a.Coordinator)
                .OrderBy(a => a.Date)
                .Where(a => a.Date > CurrentTime)
                .ToList();

            return View(AllActivities);

        }

        [HttpGet("add/activity")]
        public IActionResult AddActivity()
        {
            if(user_id == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.User = (int)user_id;
            return View();
        }

        [HttpPost("create/activity")]
        public IActionResult CreateActivity(Activity newActivity)
        {
            if(ModelState.IsValid)
            {
                Activity thisActivity = newActivity;
                thisActivity.Date = thisActivity.Date.Add(thisActivity.Time.TimeOfDay);
                dbContext.Add(thisActivity);
                dbContext.SaveChanges();
                return RedirectToAction("ShowActivity", new {num = thisActivity.ActivityId});
            }
            ViewBag.User = (int)user_id;
            return View("AddActivity");
        }

        [HttpGet("activity/{num}")]
        public IActionResult ShowActivity(int num)
        {
            if(user_id == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.User = (int)user_id;
            Activity thisActivity = dbContext.Activities
                .Include(a => a.Participants)
                .ThenInclude(ua => ua.Activity)
                .Include(a => a.Participants)
                .ThenInclude(ua => ua.User)
                .Include(a => a.Coordinator)
                .FirstOrDefault(a => a.ActivityId == num);
            return View(thisActivity);
        }

        [HttpGet("delete/{num}")]
        public IActionResult DeleteActivity(int num)
        {
            if(user_id == null)
            {
                return RedirectToAction("Index");
            }
            Activity thisActivity = dbContext.Activities.FirstOrDefault(a => a.ActivityId == num);
            if(thisActivity.UserId != (int)user_id)
            {
                return RedirectToAction("Index");
            }
            dbContext.Activities.Remove(thisActivity);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }

        [HttpGet("leave/{num}")]
        public IActionResult LeaveActivity(int num)
        {
            if(user_id == null)
            {
                return RedirectToAction("Index");
            }
            UserActivity thisUserActivity = dbContext.UserActivities
                .Where(a => a.ActivityId == num) 
                .FirstOrDefault(u => u.UserId == (int)user_id);
            if(thisUserActivity.UserId != (int)user_id)
            {
                return RedirectToAction("Index");
            }
            dbContext.UserActivities.Remove(thisUserActivity);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }

        [HttpPost("join/activity")]
        public IActionResult JoinActivity(int UserId, int ActivityId)
        {
            UserActivity thisUserActivity = new UserActivity();
            thisUserActivity.ActivityId = ActivityId;
            thisUserActivity.UserId = UserId;
            dbContext.UserActivities.Add(thisUserActivity);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            if(user_id == null)
            {
                return RedirectToAction("Index");
            }
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LogRegUser returningUser)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == returningUser.formUser.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(returningUser.formUser, userInDb.Password, returningUser.formUser.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                user_id = userInDb.UserId;
                return RedirectToAction("Home");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("create")]
        public IActionResult Create(LogRegUser newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newUser.regUser.Email))
                {
                    ModelState.AddModelError("Email", "Email is taken!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.regUser.Password = Hasher.HashPassword(newUser.regUser, newUser.regUser.Password);
                dbContext.Add(newUser.regUser);
                dbContext.SaveChanges();
                user_id = newUser.regUser.UserId;
                return RedirectToAction("Home");
            }
            else
            {
                return View("Index");
            }
        }
    }
}

