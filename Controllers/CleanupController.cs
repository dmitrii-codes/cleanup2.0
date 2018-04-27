using Microsoft.AspNetCore.Mvc;
using Cleanup.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Cleanup
{
    public class CleanupController : Controller //Controller for Cleanup CRUD
    {
        private readonly IHostingEnvironment HE; //create new 'HE' for hosting environment to store files
        private CleanupContext _context;
        public CleanupController(CleanupContext context, IHostingEnvironment he)
        {
            HE = he; //initiate the hosting environment to store files
            _context = context;
        }
        //message test
        [HttpGet]
        [Route("mboard/{id}")]
        public IActionResult MBoard(int id){
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {  
                //retrive the messages by event with id and INCLUDE boardmessages;
                ViewBag.messages = _context.boardmessages.Where(c => c.EventId == id).OrderBy(c => c.CreatedAt).Include(m => m.Sender).ToList();
                ViewBag.cleanup = _context.cleanups.Single(e => e.CleanupId == id);       
                return View("mboard");
            }
            return RedirectToAction("Index", "User");
        }

        [HttpGet]
        [Route("dashboard")] //Needs a legit Route
        public IActionResult Dashboard()
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                //getting all the events
                var events = _context.cleanups.Where(c => c.Pending == false).Include(c => c.CleaningUsers).Include(c => c.User).Include( c => c.Images).ToList();
                ViewBag.markers = events;
                ViewBag.Latitude = HttpContext.Session.GetString("latitude");
                ViewBag.Longitude = HttpContext.Session.GetString("longitude");
                List<Live> livemessages = _context.livemessages.OrderBy(c => c.CreatedAt).ToList();
                ViewBag.liveMsg = livemessages; 
                var active = _context.users.Where(u => u.UserId == activeId)
                    .Include(u => u.SentToUser)
                        .ThenInclude(m => m.Recipient)
                    .Include(u => u.Received)
                        .ThenInclude(m => m.Sender)
                    .ToList();
                ViewBag.active = active[0]; 
                //messages logic
                ViewBag.unread = _context.privatemessages.Where(m => m.RecipientId == activeId && m.ReadStatus == false).ToList().Count;
                var inboxMsg = active[0].SentToUser.Concat(active[0].Received);
                List<User> msgUsers = new List<User>();
                foreach(var each in inboxMsg){
                    if (each.RecipientId == activeId){
                        msgUsers.Add(each.Sender);
                    }
                    else{
                        msgUsers.Add(each.Recipient);
                    }
                }
                ViewBag.msgUsers = msgUsers.Distinct().ToList().OrderByDescending(u => u.SentToUser.Concat(u.Received).OrderByDescending(m => m.CreatedAt).First().CreatedAt);
                return View("Dashboard");
            }
            return RedirectToAction("Index", "User");
        }
        [HttpPost]
        [Route("postboardmessage/{id}")]
        public IActionResult PostBoardMessage(int id, string content){
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {   
                if (content == null){
                    ViewBag.error = "Content can't be empty";
                    ViewBag.messages = _context.boardmessages.Where(c => c.EventId == id).OrderBy(c => c.CreatedAt).Include(m => m.Sender).ToList();
                    ViewBag.cleanup = _context.cleanups.Single(e => e.CleanupId == id);      
                    return View("mboard");
                }
                BoardMessage bm = new BoardMessage{
                    SenderId = (int)HttpContext.Session.GetInt32("activeUser"),
                    EventId = id,
                    Content = content
                };
                _context.Add(bm);
                _context.SaveChanges();
                return RedirectToAction("MBoard");
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("add/cleanup")]
        public IActionResult NewCleanup()
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                return View();
            }
            return RedirectToAction("Index", "User");
        }
        [HttpPost]
        [Route("add/cleanup")]
        public IActionResult AddCleanup(CleanupViewModel model)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.Single( u => u.UserId == (int)activeId);
                if(activeUser.Token>0)
                {
                    if(ModelState.IsValid)
                    {
                        CleanupEvent newCleanup = new CleanupEvent{
                            Title = model.Title,
                            DescriptionOfArea = model.DescriptionOfArea,
                            DescriptionOfTrash = model.DescriptionOfTrash,
                            UserId = (int)activeId,
                            Pending = true,
                            Value = 0,
                            MaxCleaners = 0,
                            Address = model.Address,
                            Latitude = model.Latitude,
                            Longitude = model.Longitude
                        };
                        _context.Add(newCleanup);
                        activeUser.Token-=1;
                        _context.SaveChanges();
                        CleanupEvent freshCleanup = _context.cleanups.OrderByDescending( c => c.CreatedAt ).First();
                        return RedirectToAction("AddPhoto", new { id = freshCleanup.CleanupId});
                    }
                }
                else
                {
                    ViewBag.error = "Insufficient tokens to report trash, go and help out more!";
                }
                return View("NewCleanup");
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("cleanup/{id}")]
        public IActionResult ViewCleanup(int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).Include( c => c.User ).Include( c => c.Images ).Include( c => c.CleaningUsers).ToList();
                if(possibleCleanup.Count == 1)
                {
                    bool attending = false;
                    foreach(var user in possibleCleanup[0].CleaningUsers)
                    {
                        if(user.UserId == (int)activeId)
                        {
                            attending = true;
                            break;
                        }
                    }
                    ViewBag.cleanup = possibleCleanup[0];
                    ViewBag.attending = attending;
                    return View();
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("delete/cleanup/{id}")]
        public IActionResult DeleteCleanup(int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.Single( u => u.UserId == (int)activeId);
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).ToList();
                if(possibleCleanup.Count == 1)
                {
                    if(activeUser.UserLevel == 9 || (possibleCleanup[0].Pending = true && possibleCleanup[0].UserId == activeUser.UserId))
                    {
                        _context.cleanups.Remove(possibleCleanup[0]);
                        _context.SaveChanges();
                        return RedirectToAction("AdminPage");
                    }
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpPost]
        [Route("approve/cleanup/{id}")]
        public IActionResult ApproveCleanup(int id, int value, int max)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.Single( u => u.UserId == (int)activeId);
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).ToList();
                if(possibleCleanup.Count == 1 && activeUser.UserLevel == 9) //Confirm that event exists and that user is admin
                {
                    possibleCleanup[0].Pending = false;
                    possibleCleanup[0].MaxCleaners = max;
                    possibleCleanup[0].Value = value;
                    _context.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("close/cleanup/{id}")]
        public IActionResult CloseCleanup(int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.Single( u => u.UserId == (int)activeId);
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).Include( c => c.CleaningUsers ).ToList();
                if(possibleCleanup.Count == 1 && activeUser.UserLevel == 9) //Confirm that event exists and that user is admin
                {
                    foreach(User cleaninguser in possibleCleanup[0].CleaningUsers)
                    {
                        cleaninguser.Score += possibleCleanup[0].Value;
                        cleaninguser.Token += 1;
                        return RedirectToAction("DeleteCleanup", new { id = possibleCleanup[0].CleanupId});
                    }
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("add/photos/cleanup/{id}")]
        public IActionResult AddPhoto(int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).Include( c => c.CleaningUsers ).Include( c => c.Images ).ToList();
                if(possibleCleanup.Count == 1)
                {
                    bool ActiveUserAttending = false;
                    foreach(var user in possibleCleanup[0].CleaningUsers)
                    {
                        if ((int)activeId == user.UserId)
                        {
                            ActiveUserAttending = true;
                            break;
                        }
                    }
                    ViewBag.Attending = ActiveUserAttending;
                    ViewBag.Cleanup = possibleCleanup[0];
                    return View();
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpPost]
        [Route("add/photos/cleanup/{id}")]
        public IActionResult ProcessPhoto(int id, IFormFile pic)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).Include( c => c.CleaningUsers ).ToList();
                if(possibleCleanup.Count == 1)//Confirm that they went to an existing cleanup event and that they should be the one adding photos
                {
                    bool ActiveUserAttending = false;
                    foreach(var user in possibleCleanup[0].CleaningUsers)
                    {
                        if ((int)activeId == user.UserId)
                        {
                            ActiveUserAttending = true;
                            break;
                        }
                    }
                    if (pic != null && (ActiveUserAttending || (int)activeId == possibleCleanup[0].UserId)){ //aka if a picture was uploaded
                        var filename = Path.Combine(HE.WebRootPath + "/images", Path.GetFileName(pic.FileName)); //stores a string of where the new file root should be
                        String filestring = GetRandString(); //returns a string of numbers to randomize the file names
                        String[] newfile = filename.Split("."); //creates an array of the file string before the period and after so we can add the randomized string
                        String newFileString = newfile[0] + filestring + "." + newfile[1]; //puts the string back together including the random string
                        String[] splitrootfile = newFileString.Split("wwwroot"); //creates a string with the path necessary to store and retrieve the image from the images folder 
                        pic.CopyTo(new FileStream(newFileString, FileMode.Create));
                        Image newImage = new Image{
                            CleanupEventId = id,
                            FileName = splitrootfile[1]
                        };
                        _context.Add(newImage);
                        _context.SaveChanges();
                        return RedirectToAction("AddPhoto", new { id = id }); //After new photo added, redirect to photo add page so user can add more (up to 5 max)
                    }
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("admin/page")]
        public IActionResult AdminPage()
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null)
            {
                User activeUser = _context.users.Single( u => u.UserId == (int)activeId);
                if(activeUser.UserLevel == 9)
                {
                    ViewBag.allCleanups = _context.cleanups.Where( c => c.Pending == true ).Include( c => c.User ).Include( c => c.Images ).OrderBy( c => c.UpdatedAt ).ToList();
                    return View();
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("admin/cleanup/{id}")]
        public IActionResult AdminCleanupPage(int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null)
            {
                User activeUser = _context.users.Single( u => u.UserId == (int)activeId);
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).Include( c => c.Images ).Include( c => c.User ).ToList();
                if(activeUser.UserLevel == 9 && possibleCleanup.Count == 1 && possibleCleanup[0].Pending == true)
                {
                    ViewBag.cleanup = possibleCleanup[0];
                    return View();
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("decline/cleanup/{id}")]
        public IActionResult DeclineCleanupReport(int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.Single( u => u.UserId == (int)activeId);
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).ToList();
                if(possibleCleanup.Count == 1 && activeUser.UserLevel == 9 && possibleCleanup[0].Pending == true) //Confirm that event exists and that user is admin
                {
                    possibleCleanup[0].Pending = false;
                    _context.SaveChanges();
                    return RedirectToAction("AdminPage");
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("set/cleanup/status/{id}")]
        public IActionResult ChangeToPending(int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).Include( c => c.CleaningUsers ).ToList();
                if(possibleCleanup.Count == 1)
                {
                    bool ActiveUserAttending = false;
                    foreach(var user in possibleCleanup[0].CleaningUsers)
                    {
                        if ((int)activeId == user.UserId)
                        {
                            ActiveUserAttending = true;
                            break;
                        }
                    }
                    if((ActiveUserAttending || possibleCleanup[0].UserId == (int)activeId) && possibleCleanup[0].Pending == false)
                    {
                        possibleCleanup[0].Pending= true;
                        _context.SaveChanges();
                    }
                    return RedirectToAction("Dashboard");
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("profile/{id}")]
        public IActionResult viewprofile(int id){
            int? activeuser = HttpContext.Session.GetInt32("activeUser");
            if(activeuser != null){
                List<User> active = _context.users.Where(u => u.UserId == id).Include(c => c.CleanupEvent).Include(cr => cr.CreatedCleanups)
                    .Include(u => u.SentToUser)
                        .ThenInclude(m => m.Recipient)
                    .Include(u => u.Received)
                        .ThenInclude(m => m.Sender)
                    .ToList();
                if(active.Count < 1){
                    return RedirectToAction("Index", "User");
                }
                ViewBag.active = active[0];
                if(active[0].UserId == activeuser){
                    ViewBag.edit = true; 
                    ViewBag.unread = _context.privatemessages.Where(m => m.RecipientId == id && m.ReadStatus == false).ToList().Count;
                    var inboxMsg = active[0].SentToUser.Concat(active[0].Received);
                    List<User> msgUsers = new List<User>();
                    foreach(var each in inboxMsg){
                    if (each.RecipientId == active[0].UserId){
                        msgUsers.Add(each.Sender);
                    }
                    else{
                        msgUsers.Add(each.Recipient);
                    }
                }
                ViewBag.msgUsers = msgUsers.Distinct().ToList().OrderByDescending(u => u.SentToUser.Concat(u.Received).OrderByDescending(m => m.CreatedAt).First().CreatedAt);
                }
                else{
                    ViewBag.edit = false; 
                }
                return View();
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("leaderboard")]
        public IActionResult leaderboard(){
            int? activeuser = HttpContext.Session.GetInt32("activeUser");
            if(activeuser != null){
                List<User> toptokens = _context.users.OrderByDescending(t => t.Token).ToList();
                List<User> topscore = _context.users.OrderByDescending(s => s.Score).ToList();
                User active = _context.users.Single(u => u.UserId == activeuser);
                ViewBag.active = active;
                ViewBag.tokens = toptokens;
                ViewBag.score = topscore;
                if(topscore.Count < 3){
                    ViewBag.scorecount = topscore.Count; 
                }
                else{
                    ViewBag.scorecount = 3; 
                }
                if(toptokens.Count < 3){
                    ViewBag.tokencount = toptokens.Count;  
                }
                else{
                    ViewBag.tokencount = 3; 
                }
                return View();
            }
            return RedirectToAction("Index", "User");
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        [Route("join/{id}")]
        public IActionResult Join(int id)
        {
            int? activeuser = HttpContext.Session.GetInt32("activeUser");
            if(activeuser != null)
            {
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).Include( c => c.CleaningUsers ).ToList();
                if(possibleCleanup.Count == 1 && possibleCleanup[0].CleaningUsers.Count < possibleCleanup[0].MaxCleaners)
                {
                    User joiningUser = _context.users.Single( u => u.UserId == (int)activeuser);
                    joiningUser.CleanupEventId = possibleCleanup[0].CleanupId;
                    _context.SaveChanges();
                    return RedirectToAction("ViewCleanup", new { id = possibleCleanup[0].CleanupId });
                }
            }
            return RedirectToAction("Index", "User");
        }

        // [HttpPost]
        // [Route("live")]
        // public ActionResult live(string data){
        //     Live newmsg = new Live{
        //         Messages = data
        //     };
        //     _context.Add(newmsg);
        //     _context.SaveChanges();
        // }
        public String GetRandString(){ // create a random string for storing more randomized file names
            Random rand = new Random();
            String Str = "";
            for(var i = 0; i < 1; i++){
                Str += rand.Next(0, 1000);
            }
            return Str;
        }
    }
}