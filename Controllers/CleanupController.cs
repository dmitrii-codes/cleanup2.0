using Microsoft.AspNetCore.Mvc;
using Cleanup.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Cleanup
{
    public class CleanupController : Controller //Controller for Cleanup CRUD
    {
        private CleanupContext _context;
        public CleanupController(CleanupContext context)
        {
            _context = context;
        }
        //message test
        [HttpGet]
        [Route("mboard/{id}")]
        public IActionResult Test2(int id){
            //retrive the messages by event with id and INCLUDE boardmessages;
            ViewBag.messages = _context.boardmessages.Where(c => c.EventId == id).Include(m => m.Sender).ToList();
            ViewBag.cleanup = _context.cleanups.Single(e => e.CleanupId == id);       
            return View("mboard");
        }

        [HttpGet]
        [Route("dashboard")] //Needs a legit Route
        public IActionResult Dashboard()
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                //getting all the events
                var events = _context.cleanups.Where(c => c.Pending == false).Include(c => c.CleaningUsers).Include(c => c.User).ToList();
                ViewBag.markers = events;
                User active = _context.users.Single(u => u.UserId == activeId);
                ViewBag.active = active; 
                return View("Dashboard");
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
                            Latitude = model.Latitude,
                            Longitude = model.Longitude
                        };
                        _context.Add(newCleanup);
                        activeUser.Token-=1;
                        _context.SaveChanges();
                        CleanupEvent freshCleanup = _context.cleanups.OrderBy( c => c.CreatedAt ).Reverse().First();
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
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).Include( c => c.Images ).Include( c => c.CleaningUsers).ToList();
                if(possibleCleanup.Count == 1)
                {
                    ViewBag.viewedCleanup = possibleCleanup[0];
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
                        return RedirectToAction("Dashboard");
                    }
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpPost]
        [Route("approve/cleanup/{id}")]
        public IActionResult ApproveCleanup(int id, int value)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                User activeUser = _context.users.Single( u => u.UserId == (int)activeId);
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).ToList();
                if(possibleCleanup.Count == 1 && activeUser.UserLevel == 9) //Confirm that event exists and that user is admin
                {
                    possibleCleanup[0].Pending = false;
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
                    int scoreEarned = (possibleCleanup[0].Value/possibleCleanup[0].CleaningUsers.Count);
                    foreach(User cleaninguser in possibleCleanup[0].CleaningUsers)
                    {
                        cleaninguser.Score = scoreEarned;
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
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).Include( c => c.Images ).ToList();
                if(possibleCleanup.Count == 1)
                {
                    ViewBag.Cleanup = possibleCleanup[0];
                    return View();
                }
            }
            return RedirectToAction("Index", "User");
        }
        [HttpPost]
        [Route("add/photos/cleanup/{id}")]
        public IActionResult ProcessPhoto(int id)
        {
            int? activeId = HttpContext.Session.GetInt32("activeUser");
            if(activeId != null) //Checked to make sure user is actually logged in
            {
                List<CleanupEvent> possibleCleanup = _context.cleanups.Where( c => c.CleanupId == id).ToList();
                if(possibleCleanup.Count == 1 && possibleCleanup[0].UserId == (int)activeId)//Confirm that they went to an existing cleanup event and that they should be the one adding photos
                {
                    //Code to change photo filename, ERIC LOOK HERE
                    return RedirectToAction("AddPhoto", new { id = possibleCleanup[0].CleanupId}); //After new photo added, redirect to photo add page so user can add more (up to 5 max)
                }
            }
            return RedirectToAction("Index", "User");
        }

        [HttpGet]
        [Route("profile/{id}")]
        public IActionResult viewprofile(int id){
            int? activeuser = HttpContext.Session.GetInt32("activeUser");
            if(activeuser != null){
                List<User> active = _context.users.Where(u => u.UserId == id).Include(c => c.CleanupEvent).Include(cr => cr.CreatedCleanups).ToList();;
                if(active.Count < 1){
                    return RedirectToAction("Index", "User");
                }
                ViewBag.active = active[0];
                if(active[0].UserId == activeuser){
                    ViewBag.edit = true; 
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

        // [HttpPost]
        // [Route("live")]
        // public ActionResult live(string data){
        //     Live newmsg = new Live{
        //         Messages = data
        //     };
        //     _context.Add(newmsg);
        //     _context.SaveChanges();
        // }
    }
}