using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auction.Models
{
    public class LogReg
    {
        public Login Login {get;set;}
        public User Register {get;set;}
    }
}