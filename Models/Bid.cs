using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Auction.Models
{
    public class Bid
    {
        [Key]
        public int BidId {get;set;}
        [JsonIgnore]
        public int ItemId {get;set;}
        [JsonIgnore]
        public int UserId {get;set;}
        [Required]
        [Display(Name = "Bid")]
        public int? Ammount {get;set;} = null;
        public User User {get;set;}
        public Item Item {get;set;}
    }
    
}