using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using userdb.Models;
using Microsoft.EntityFrameworkCore;

namespace userdb.Controllers
{
    public class UserController : Controller
    {
        private DBcontext _context;
        public UserController(DBcontext context)
        {
            _context = context;
        }

        private User ActiveUser 
        {
            get{ return _context.users.Where(u => u.user_id == HttpContext.Session.GetInt32("UserId")).FirstOrDefault();}
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            ViewBag.Current_User_Id = null;
            ViewBag.Is_Admin = false;
            return View();
        }
        
        [HttpGet("register")]
        public IActionResult Register()
        {
            ViewBag.Current_User_Id = null;
            ViewBag.Is_Admin = false;
            return View();
        }
        
        [HttpGet("adduser")]
        public IActionResult AddUser()
        {
            int? userlevel = HttpContext.Session.GetInt32("IsAdmin");
            if (userlevel == null || userlevel == 0)
            {
                return RedirectToAction("LogOut");
            }
            return View();
        }
        
        [HttpPost("newuser")]
        public IActionResult NewUser(UserView uv)
        {
            int? userlevel = HttpContext.Session.GetInt32("IsAdmin");
            if (userlevel == null || userlevel == 0)
            {
                ModelState.AddModelError("Authorization Error", "You do not have authority to add user");
                return RedirectToAction("LogOut");
            }
            if (uv.password != uv.confirmpwd) 
            {
                ModelState.AddModelError("password", "Confirmation Password must match the Password");
            }
            User userchk = _context.users.Where(e=> e.email == uv.email).FirstOrDefault();
            if (userchk != null)
            {
                ModelState.AddModelError("email", "User with this email already exists");
            }
            if (ModelState.IsValid)
            {
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                User user = new User();
                user.first_name = uv.first_name;
                user.last_name = uv.last_name;
                user.email = uv.email;
                user.password = hasher.HashPassword(user, uv.password);
                user.created_at = DateTime.Now;
                user.updated_at = DateTime.Now;
                user.user_level = 0;
                _context.users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Dashboard", "User");
            }
            return RedirectToAction("AddUser");
        }
        [HttpPost("registeruser")]
        public IActionResult RegisterUser(UserView uv)
        {
            if (uv.password != uv.confirmpwd) 
            {
                ModelState.AddModelError("password", "Confirmation Password must match the Password");
            }
            User userchk = _context.users.Where(e=> e.email == uv.email).FirstOrDefault();
            if (userchk != null)
            {
                ModelState.AddModelError("email", "User with this email already exists");
            }
            if (ModelState.IsValid)
            {
                int admincount = _context.users.Where(e=>e.user_level==1).Count();
                
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                User user = new User();
                user.first_name = uv.first_name;
                user.last_name = uv.last_name;
                user.email = uv.email;
                user.password = hasher.HashPassword(user, uv.password);
                user.created_at = DateTime.Now;
                user.updated_at = DateTime.Now;
                user.user_level = admincount > 0 ? 0 : 1;
                _context.users.Add(user);
                _context.SaveChanges();
                int UserId = _context.users.Last().user_id;
                HttpContext.Session.SetInt32("UserId", UserId);
                HttpContext.Session.SetInt32("IsAdmin", user.user_level);
                ViewBag.Current_User_Id = HttpContext.Session.GetInt32("UserId");
                ViewBag.Is_Admin = HttpContext.Session.GetInt32("IsAdmin");
                return RedirectToAction("Dashboard", "User");
            }
            return RedirectToAction("LoginRegister");
        }
        [HttpPost("loginuser")]
        public IActionResult LoginUser(User userInfo)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            User user = _context.users.Where(u => u.email == userInfo.email).SingleOrDefault();
            if(user == null)
                ModelState.AddModelError("email", "Invalid Email/Password");
            else if(hasher.VerifyHashedPassword(user, user.password, userInfo.password) != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("LogEmail", "Invalid Email/Password");
            }
            if(!ModelState.IsValid)
                return RedirectToAction("Login", "User");
            HttpContext.Session.SetInt32("UserId", user.user_id);
            HttpContext.Session.SetInt32("IsAdmin", user.user_level);
            return RedirectToAction("Dashboard","User");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            ViewBag.Current_User_Id = null;
            ViewBag.Is_Admin = false;
            return RedirectToAction("Index","Home");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            User user = ActiveUser;
            if(user == null)
                return RedirectToAction("LoginRegister", "User");
                
            DashboardView dashboardData = new DashboardView
            {
                SiteUsers = _context.users.ToList(),
                User = user
            };
            ViewBag.ActiveUser = user;
            ViewBag.Current_User_Id = HttpContext.Session.GetInt32("UserId");
            ViewBag.Is_Admin = HttpContext.Session.GetInt32("IsAdmin");
            return View(dashboardData);
        }
        
        [HttpGet("edit")]
        public IActionResult EditProfile()
        {
            int? Current_User_Id = HttpContext.Session.GetInt32("UserId");
            User u = _context.users.Where(e => e.user_id == (int) Current_User_Id).SingleOrDefault();
            ViewBag.Current_User_Id = HttpContext.Session.GetInt32("UserId");
            ViewBag.IsAdmin = HttpContext.Session.GetInt32("IsAdmin");
            return View(u);
        }
        
        [HttpGet("editbyadmin")]
        public IActionResult EditProfileByAdmin(int id)
        {
            int? Is_Admin = HttpContext.Session.GetInt32("IsAdmin");
            if (Is_Admin==null || Is_Admin == 0)
            {
                return RedirectToAction("LogOut");
            }
            User u = _context.users.Where(e => e.user_id ==  id).SingleOrDefault();
            ViewBag.Current_User_Id = HttpContext.Session.GetInt32("UserId");
            ViewBag.IsAdmin = HttpContext.Session.GetInt32("IsAdmin");
            return View("EditProfile", u);
        }
        
        [HttpPost("saveuser")]
        public IActionResult SaveUser(User uv)
        {
            User user = _context.users.Where(e=> e.email == uv.email).FirstOrDefault();
            if (uv.password != null)
            {
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                user.password = hasher.HashPassword(user, uv.password);
                user.updated_at = DateTime.Now;
                _context.users.Update(user);
                _context.SaveChanges();
            }
            else
            {
                user.first_name = uv.first_name;
                user.last_name = uv.last_name;
                user.email = uv.email;
                user.updated_at = DateTime.Now;
                _context.users.Update(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard", "User");
        }
        [HttpGet("removeuser")]
        public IActionResult RemoveUser(int id)
        {
            User user = _context.users.Where(e=> e.user_id == id).FirstOrDefault();
            if (user != null)
            {
                _context.users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard", "User");
        }
        [HttpGet("info/{id}")]
        public IActionResult UserInfo(int id)
        {
            ViewBag.Current_User_Id = HttpContext.Session.GetInt32("UserId");
            ViewBag.Is_Admin = HttpContext.Session.GetInt32("IsAdmin");
            User selectedUser = _context.users
                                    .Where(e=> e.user_id == id)
                                    .Include(e=>e.messages)
                                    .ThenInclude(e=>e.comments)
                                    .SingleOrDefault();
            return View(selectedUser);
        }
        
        [HttpPost("newmessage")]
        public IActionResult AddMessage(int id, string message_description)
        {
            int? Current_User_Id = HttpContext.Session.GetInt32("UserId");
            User u = _context.users.Where(e => e.user_id == (int) Current_User_Id).SingleOrDefault();
            if (message_description!=null)
            {
                Message msg = new Message()
                {
                    message_added_by = u.first_name + " " + u.last_name,
                    message_description = message_description,
                    user_id = id,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };
                _context.messages.Add(msg);
                _context.SaveChanges();
            }
            int intphrase = id;
            return RedirectToAction("UserInfo", new {id = intphrase});
        }        
        
        [HttpPost("newcomment")]
        public IActionResult AddComment(int id, string comment_description)
        {
            int? Current_User_Id = HttpContext.Session.GetInt32("UserId");
            User u = _context.users.Where(e => e.user_id == (int) Current_User_Id).SingleOrDefault();
            Message m = _context.messages.Where(e=>e.message_id == id).SingleOrDefault();
            if (comment_description!=null)
            {
                Comment cmt = new Comment()
                {
                    comment_added_by = u.first_name + " " + u.last_name,
                    comment_description = comment_description,
                    message_id = m.message_id,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };
                _context.comments.Add(cmt);
                _context.SaveChanges();
            }
            return RedirectToAction("UserInfo", new {id = m.user_id});
        }        
        
    }
}