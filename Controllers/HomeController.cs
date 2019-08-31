using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Auction.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Auction.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private Context _db;
        public HomeController(Context context)
        {
            _db = context;
        }

        [HttpGet("[action]")]
        public IActionResult GetItems()
        {
            ResolveAuction();
            // Fetch list and sort bids.
            List<Item> AllItems = _db.Items
            .Include(i => i.Seller)
            .Include(i => i.Bids)
                .ThenInclude(b => b.User)
            .OrderBy(i => i.End)
            .ToList();
            AllItems.ForEach(i => i.Bids = i.Bids.OrderByDescending(b => b.Ammount).ToList());
            return Ok(JsonConvert.SerializeObject(AllItems, Formatting.Indented, 
                new JsonSerializerSettings 
                    { 
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }
            ));
        }

        [HttpGet("[action]/{ItemId}")]
        public IActionResult GetItem(int? ItemId)
        {
            if(ItemId != null)
            {
                // Fetch list and sort bids.
                List<Item> AllItems = _db.Items
                .Include(i => i.Seller)
                .Include(i => i.Bids)
                    .ThenInclude(b => b.User)
                .OrderBy(i => i.End)
                .ToList();
                AllItems.ForEach(i => i.Bids = i.Bids.OrderByDescending(b => b.Ammount).ToList());
                // Fetch Item.
                Item GetItemById = AllItems.FirstOrDefault(i => i.ItemId == ItemId);
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
        public IActionResult NewItem([FromBody] Item NewItem)
        {
            if(ModelState.IsValid)
            {
                // Add Item to _db.
                _db.Add(NewItem);
                _db.SaveChanges();
                //Create new bid.
                Bid NewBid = new Bid();
                NewBid.UserId = NewItem.ItemId;
                NewBid.ItemId = NewItem.ItemId;
                NewBid.Ammount = NewItem.StartingBid;
                _db.Add(NewBid);
                _db.SaveChanges();
                return Ok(Json(new {ItemId = NewBid.ItemId}));
            }
            return BadRequest(Json(ModelState));
        }

        [HttpPost("[action]")]
        public IActionResult BidItem([FromBody] Bid TryBid)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(Json(ModelState));
            }
            // Fetch User.
            User GetUserById = _db.Users.FirstOrDefault(u => u.UserId == TryBid.UserId);
            // Fetch Item.
            Item GetItemById = _db.Items.FirstOrDefault(a => a.ItemId == TryBid.ItemId);
            // Find Top Bid.
            Bid WinningBid = _db.Bids.Where(b => b.ItemId == TryBid.ItemId).OrderByDescending(i => i.Ammount).First();
            // Check if funds available.
            if(GetUserById.Wallet < TryBid.Ammount)
            {
                ModelState.AddModelError("Ammount", $"Sorry you only have {GetUserById.Wallet} available.");
                return BadRequest(Json(ModelState));
            }
            // Check if bid is higher than previous bid & min.
            if(WinningBid.Ammount >= TryBid.Ammount)
            {
                ModelState.AddModelError("Ammount", "Your bid is not high enough.");
                return BadRequest(Json(ModelState));
            }
            // Create new bid.
            Bid NewBid = new Bid();
            NewBid.UserId = TryBid.UserId;
            NewBid.ItemId = TryBid.ItemId;
            NewBid.Ammount = TryBid.Ammount;
            _db.Add(NewBid);
            _db.SaveChanges();
            // Fetch updated Item.
            Item UpdatedItem = _db.Items
            .Include(i => i.Seller)
            .Include(a => a.Bids)
                .ThenInclude(b => b.User)
            .FirstOrDefault(a => a.ItemId == TryBid.ItemId);
            return Ok(JsonConvert.SerializeObject(UpdatedItem, Formatting.Indented, 
                new JsonSerializerSettings 
                    { 
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }
            ));
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
                    // Fetch Seller.
                    User Seller = _db.Users.FirstOrDefault(u => u.UserId == Item.Seller.UserId);
                    // Fetch Top Bid.
                    Bid WinningBid = _db.Bids.OrderByDescending(i => i.Ammount).First();
                    // Fetch winning buyer.
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
