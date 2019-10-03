using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cleanup.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Cleanup
{
    // Controller for User Login/Registation
    public class UserController: Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private CleanupContext _context;
        public UserController(CleanupContext context, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index() // Welcome page
        {
            int? active = HttpContext.Session.GetInt32("activeUser");
            if (active != null)
            {
                return RedirectToAction("Dashboard", "Cleanup");
            }
            return View();
        }
        [HttpGet]
        [Route("signup")]
        public IActionResult IndexReg(){
            ViewBag.reg = true;
            return View("Index");
        }
        [HttpGet]
        [Route("signin")]
        public IActionResult IndexLog(){
            ViewBag.log = true;
            return View("Index");
        }
        [HttpPost]
        [Route("register")]
        public IActionResult Register(UserRegisterViewModel model, IFormFile ProfilePic, double Latitude, double Longitude)
        {
            if (ModelState.IsValid)
            {
                if (ProfilePic != null) // if picture was uploaded
                {
                    Random rand = new Random();
                    // stores a string of where the new file root should be
                    var filename = Path.Combine(_hostingEnvironment.WebRootPath + "/images/profile", Path.GetFileName(ProfilePic.FileName));
                    String[] newfile = { "", "" };
                    newfile[0] = filename.Substring(0,filename.Length-4);
                    newfile[1] = filename.Substring(filename.Length-3);
                    String newFileString = $"{newfile[0]}{rand.Next(0, 1000)}.{newfile[1]}";
                    // creates a string with the path necessary to store and retrieve the image from the images folder
                    String[] splitRootFile = newFileString.Split("wwwroot");
                    ProfilePic.CopyTo(new FileStream(newFileString, FileMode.Create));
                    User newUser = new User {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        UserName = model.UserName,
                        Password = model.Password,
                        // store only the second half of the split path into database. We only need this part of the path to access the images.
                        ProfilePic = splitRootFile[2],
                        Token = 1,
                        Score = 0,
                        UserLevel = 0
                    };
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    // Hash password
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    _context.Add(newUser);
                    _context.SaveChanges();
                    // re-obtain newly created User for Id information
                    User activeUser = _context.users.Single( u => (string)u.Email == (string)model.Email);
                    if (activeUser.UserId == 1)
                    {
                        // First user ever to admin
                        activeUser.UserLevel = 9;
                        _context.SaveChanges();
                    }
                    HttpContext.Session.SetString("latitude", Latitude.ToString());
                    HttpContext.Session.SetString("longitude", Longitude.ToString());
                    HttpContext.Session.SetString("userName", activeUser.UserName);
                    HttpContext.Session.SetInt32("activeUser", activeUser.UserId);
                    // for testing only: to display the image path
                    TempData["pic"] = splitRootFile[1];
                    // Go to actual site
                    return RedirectToAction("Dashboard", "Cleanup");
                }
            }
            //F ailed registration attempt goes here
            return View("RegistrationPartial");
        }
        [HttpPost]
        [Route("login")]
        public IActionResult Login(UserLoginViewModel model, double Latitude, double Longitude)
        {
            // we only check the db if user inputs the right information
            if (ModelState.IsValid)
            {
                // Check for existing username
                List<User> possibleLogin = _context.users.Where(u => (string)u.UserName == (string)model.UserNameLogin).ToList();
                // Due to unique validation, if username exists, only 1 item should be returned
                if (possibleLogin.Count == 1)
                {
                    var Hasher = new PasswordHasher<User>();
                    // Confirm hashed passsword
                    if (0!= Hasher.VerifyHashedPassword(possibleLogin[0], possibleLogin[0].Password, model.PasswordLogin))
                    {
                        HttpContext.Session.SetString("latitude", Latitude.ToString());
                        HttpContext.Session.SetString("longitude", Longitude.ToString());
                        HttpContext.Session.SetInt32("activeUser", possibleLogin[0].UserId);
                        return RedirectToAction("Dashboard", "Cleanup");
                    }
                }
            }
            // Failed login attempt error message
            ViewBag.error = "Incorrect Login Information";
            // Failed login attempt goes here
            return View("LoginPartial");
        }
        [HttpGet]
        [Route("delete/user/{id}")]
        public IActionResult DeleteUser(int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if (activeId != null)
            {
                User activeUser = _context.users.Single(u => u.UserId == (int)activeId);
                // If user is deleting himself or active user is an admin
                if(id == activeUser.UserId || activeUser.UserLevel == 9)
                {
                    User doomedUser;
                    if(id == activeUser.UserId)
                    {
                        doomedUser = activeUser;
                    }
                    else
                    {
                        // Obtain user info if being deleted by Admin
                        doomedUser = _context.users.Single( u => u.UserId == id);
                    }
                    _context.users.Remove(doomedUser);
                    _context.SaveChanges();
                    // If User is admin....
                    if(activeUser.UserLevel == 9)
                    {
                        // TODO: need to redirect to somewhere that makes sense
                        RedirectToAction("");
                    }
                }
            }
            // Return to login page if failed attempt or user deletes himself
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("sendprivatemessage/{id}")]
        public IActionResult PrivateMessages(int id)
        {
            // ViewBag.messages
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if (activeId != null)
            {
                ViewBag.recipient = _context.users.Where(u => u.UserId == id).Single();
                var messages = _context.privatemessages.Where(m => (m.SenderId == activeId && m.RecipientId == id) || (m.SenderId == id && m.RecipientId == activeId))
                    .OrderBy(m => m.CreatedAt).Include(m => m.Sender).Include(m => m.Recipient).ToList();
                var unread = messages.Where(m => m.RecipientId == activeId && m.ReadStatus == false).ToList();
                foreach (var msg in unread)
                {
                    msg.ReadStatus = true;
                }
                ViewBag.messages = messages;
                _context.SaveChanges();
                return View();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("postprivatemessage/{id}")]
        public IActionResult PostPrivateMessage(int id, string content)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if (activeId != null)
            {
                if (content == null)
                {
                    ViewBag.error = "Content can't be empty";
                    ViewBag.messages = _context.privatemessages.Where(m => m.SenderId == activeId && m.RecipientId == id)
                        .OrderBy(m => m.CreatedAt).Include(m => m.Sender).Include(m => m.Recipient).ToList();
                    ViewBag.recipient = _context.users.Where(u => u.UserId == id).Single();
                    return View("privatemessages");
                }
                PrivateMessage pm = new PrivateMessage{
                    SenderId = (int)HttpContext.Session.GetInt32("activeUser"),
                    RecipientId = id,
                    Content = content,
                    ReadStatus = false
                };
                _context.Add(pm);
                _context.SaveChanges();
                return RedirectToAction("PrivateMessages");
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("update/user/{id}")]
        public IActionResult UpdateUser(int id) // Loads page with edit user form, needs to get user infomation first
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if (activeId != null)
            {
                User activeUser = _context.users.SingleOrDefault( u => u.UserId == (int)activeId);
                // User can only edit profile if own or user is admin
                if(id == (int)activeId || activeUser.UserLevel == 9)
                {
                    // Place user to be edited into ViewBag
                    ViewBag.user = _context.users.SingleOrDefault(u => u.UserId == id);
                    // Store active user level in ViewBag to impact what can be edited.
                    ViewBag.userLevel = activeUser.UserLevel;
                    // Load Page with edit user form
                    return View("Update");
                }
                // TODO: redirect to somewhere that makes sense
                return RedirectToAction("");
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("process/update/user/{id}")]
        public IActionResult ProcessUpdateUser(UserUpdateViewModel model, int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if (activeId != null)
            {
                User activeUser = _context.users.Single( u => u.UserId == (int)activeId);
                // User can only edit profile if own or user is admin
                if (id == (int)activeId || activeUser.UserLevel == 9)
                {
                    // Note -> user is unable to edit password!! This needs to be fixed, but not a priority
                    User updatedUser = _context.users.Single( u => u.UserId == id);
                    if(ModelState.IsValid)
                    {
                        updatedUser.FirstName = model.FirstName;
                        updatedUser.LastName = model.LastName;
                        updatedUser.UserName = model.UserName;
                        updatedUser.Email = model.Email;
                        if (activeUser.UserLevel == 9)
                        {
                            updatedUser.Score = model.Score;
                            updatedUser.Token = model.Token;
                            updatedUser.UserLevel = model.UserLevel;
                        }
                        _context.SaveChanges();
                    }
                }
                // TODO: redirect to somewhere that makes sense
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}