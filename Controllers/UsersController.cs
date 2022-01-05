#nullable disable
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ProiectP3_BackendApp.Models;
using Newtonsoft.Json;
using ProiectP3_BackendApp.Trees;

namespace ProiectP3_BackendApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;

        private BTree<User,User> usersTree;

        public UsersController(UserContext context)
        {
            _context = context;
            usersTree = new BTree<User,User>(2);
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/user
        [HttpGet("{user}")]
        public async Task<ActionResult<User>> GetUser(string user)
        {
            //var user = await _context.Users.FindAsync(id);

            await FetchUsersAsync();

            var User = await _context.Users.Where(x=>x.UserName==user).FirstAsync();

            if (User == null)
            {
                return NotFound();
            }

            return usersTree.Search(User).Key;
        }

        private async Task FetchUsersAsync()
        {
            string fileName = $"{System.IO.Directory.GetCurrentDirectory()}\\json\\users.json";
            string jsonString = System.IO.File.ReadAllText(fileName).ToString();
            var x = System.Text.Json.JsonSerializer.Deserialize<User[]>(jsonString);

            foreach (var user in x)
            {
                if(user != null && !_context.Users.Contains(user))
                {
                    usersTree.Insert(user);
                    _context.Users.Add(user);
                }
            }
            await _context.SaveChangesAsync();
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            //Save in json file

            _context.Users.Add(user);

            usersTree.Insert(user);

            await _context.SaveChangesAsync();

            string jsonString = JsonConvert.SerializeObject(_context.Users.ToArray());
            if (jsonString != "")
            {
                string fileName = $"{System.IO.Directory.GetCurrentDirectory()}\\json\\users.json";
                System.IO.File.WriteAllText(fileName, jsonString);
            }

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            usersTree.Delete(user);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

    }
}
