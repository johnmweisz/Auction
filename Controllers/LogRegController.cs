using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Auction.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Auction.Controllers
{
    [Route("[controller]")]
    public class LogRegController : Controller
    {
        private Context _db;
        public LogRegController(Context context)
        {
            _db = context;
        }
        [HttpPost("[action]")]
        public IActionResult Register([FromBody] User NewUser)
        {
            if(ModelState.IsValid)
            {
                // Check if Email already exists.
                if(_db.Users.Any(u => u.Email == NewUser.Email))
                {
                    ModelState.AddModelError("Register.Email", "already in use, please log in.");
                    return BadRequest(Json(ModelState));
                }
                // Hash password.
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                NewUser.Password = Hasher.HashPassword(NewUser, NewUser.Password);
                _db.Add(NewUser);
                _db.SaveChanges();
                return Ok(JsonConvert.SerializeObject(NewUser, Formatting.Indented, 
                    new JsonSerializerSettings 
                        { 
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }
                ));
            }
            return BadRequest(Json(ModelState));
        }

        [HttpPost("[action]")]
        public IActionResult Login([FromBody] Login TryUser)
        {
            if(ModelState.IsValid)
            {
                // Find user that matches Email provided.
                User FindUserByEmail = _db.Users.FirstOrDefault(u => u.Email == TryUser.Email);
                // Check if email does not exist.
                if(FindUserByEmail == null)
                {
                    ModelState.AddModelError("Login.Email", "does not exist, please register.");
                    return BadRequest(Json(ModelState));
                }
                // Convert string to hash and compare against hashed hassword in the _db.
                var hasher = new PasswordHasher<User>();
                var result = hasher.VerifyHashedPassword(FindUserByEmail, FindUserByEmail.Password, TryUser.Password);
                // If hash matches, add User Id to session.
                if(result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetInt32("UserId", FindUserByEmail.UserId);
                    // Redirect to Main App.
                    return Ok(JsonConvert.SerializeObject(FindUserByEmail, Formatting.Indented, 
                        new JsonSerializerSettings 
                            { 
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            }
                    ));
                }
                // If hash does not match, render form with errors.
                ModelState.AddModelError("Login.Password", "incorrect!");
                return BadRequest(Json(ModelState));
            }
            return BadRequest(Json(ModelState));
        }

    }
}