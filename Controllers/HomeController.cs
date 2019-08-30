using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Auction.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BeltExam.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private Context _db;
        public HomeController(Context context)
        {
            _db = context;
        }
        // Controllers:
        [HttpGet("[action]")]
        public IActionResult Index()
        {
            // Check if logged in.
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId != null)
            {
                return Redirect("/auction");
            }
            return RedirectToAction("LogReg", "LogReg");
        }
        [HttpGet("[action]")]
        public IActionResult GetItems()
        {
            ResolveAuction();
            // Fetch list of all Items.
            List<Item> AllItems = _db.Items
            .Include(a => a.Seller)
            .Include(a => a.Bids)
                .ThenInclude(l => l.User)
            .OrderBy(a => a.End)
            .ToList();
            return Ok(JsonConvert.SerializeObject(AllItems, Formatting.Indented, 
                new JsonSerializerSettings 
                    { 
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }
            ));
        }
        [HttpGet("[action]")]
        public IActionResult GetUser()
        {
            // Check if logged in.
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(ModelState.IsValid && UserId != null)
            {
                // Fetch user and their items.
                User GetUserById = _db.Users
                .Include(u => u.ItemsForSale)
                .FirstOrDefault(u => u.UserId == (int)UserId);
                return Ok(JsonConvert.SerializeObject(GetUserById, Formatting.Indented, 
                    new JsonSerializerSettings 
                        { 
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }
                ));
            }
            return BadRequest();
        }
        [HttpPost("[action]")]
        public IActionResult NewItem([FromBody] Item formobject)
        {
            if(ModelState.IsValid)
            {
                // Add Item to _db.
                _db.Add(formobject);
                _db.SaveChanges();
                //Create new bid.
                Bid NewBid = new Bid();
                NewBid.UserId = formobject.ItemId;
                NewBid.ItemId = formobject.ItemId;
                NewBid.Ammount = formobject.StartingBid;
                _db.Add(NewBid);
                _db.SaveChanges();
                return Ok(Json(new {ItemId = NewBid.ItemId}));
            }
            return BadRequest(Json(ModelState));
        }
        [HttpGet("[action]/{ItemId}")]
        public IActionResult GetItem(int? ItemId)
        {
            if(ItemId != null)
            {
                // Fetch Item.
                Item GetItemById = _db.Items
                .Include(i => i.Seller)
                .Include(a => a.Bids)
                    .ThenInclude(b => b.User)
                .FirstOrDefault(a => a.ItemId == ItemId);
                return Ok(JsonConvert.SerializeObject(GetItemById, Formatting.Indented, 
                    new JsonSerializerSettings 
                        { 
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }
                ));
            }
            return BadRequest();
        }
        [HttpPost("[action]")]
        public IActionResult BidItem([FromBody] Bid formobject)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(Json(ModelState));
            }
            // Fetch User.
            User GetUserById = _db.Users.FirstOrDefault(u => u.UserId == formobject.UserId);
            // Fetch Item.
            Item GetItemById = _db.Items.FirstOrDefault(a => a.ItemId == formobject.ItemId);
            // Find Top Bid.
            Bid WinningBid = _db.Bids.OrderByDescending(i => i.Ammount).First();
            // Check if funds available.
            if(GetUserById.Wallet < formobject.Ammount)
            {
                ModelState.AddModelError("Ammount", $"Sorry you only have {GetUserById.Wallet} available.");
                return BadRequest(Json(ModelState));
            }
            // Check if bid is higher than previous bid & min.
            if(WinningBid.Ammount > formobject.Ammount)
            {
                ModelState.AddModelError("Ammount", "Your bid is not high enough.");
                return BadRequest(Json(ModelState));
            }
            // Create new bid.
            Bid NewBid = new Bid();
            NewBid.UserId = formobject.UserId;
            NewBid.ItemId = formobject.ItemId;
            NewBid.Ammount = formobject.Ammount;
            _db.Add(NewBid);
            _db.SaveChanges();
            // Fetch updated Item.
            Item UpdatedItem = _db.Items
            .Include(i => i.Seller)
            .Include(a => a.Bids)
                .ThenInclude(b => b.User)
            .FirstOrDefault(a => a.ItemId == formobject.ItemId);
            return Ok(JsonConvert.SerializeObject(UpdatedItem, Formatting.Indented, 
                new JsonSerializerSettings 
                    { 
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }
            ));
        }
        [HttpGet("Item/{ItemId}/delete")]
        public IActionResult DeleteItem(int? ItemId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId != null && ItemId != null)
            {
                Item GetItemById = _db.Items.FirstOrDefault(a => a.ItemId == ItemId);
                _db.Remove(GetItemById);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("LogReg", "LogReg");
        }

        public void ResolveAuction()
        {
            // Fetch list of all Items.
            List<Item> AllItems = _db.Items
            .Include(a => a.Seller)
            .Include(a => a.Bids)
                .ThenInclude(l => l.User)
            .OrderBy(a => a.End)
            .ToList();
            // Check if an item has expired.
            foreach(var Item in AllItems)
            {
                if((Item.End - DateTime.Now).TotalSeconds <= 0)
                {
                    // Find Seller.
                    User Seller = _db.Users.FirstOrDefault(u => u.UserId == Item.Seller.UserId);
                    // Find Top Bid.
                    Bid WinningBid = _db.Bids.OrderByDescending(i => i.Ammount).First();
                    // Find winning buyer.
                    User Winner = _db.Users.FirstOrDefault(u => u.UserId == WinningBid.UserId);
                    // Buyer Wallet decreases.
                    Winner.Wallet -= (int)WinningBid.Ammount;
                    // Seller Wallet increases.
                    Seller.Wallet += (int)WinningBid.Ammount;
                    // Remove Item.
                    _db.Remove(Item);
                    // Save Changes
                    _db.SaveChanges();
                }
            }
        }

    }
}
