using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiCommerce.Models;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiCommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersContext _context;

        public UsersController(UsersContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            return await _context.ToListAsync();
        }

        // GET: api/Users/5
        // [HttpGet("{id}")]
        // public async Task<ActionResult<Users>> GetUsers(int id)
        // {
        //     var users = await _context.Users.FindAsync(id);

        //     if (users == null)
        //     {
        //         return NotFound();
        //     }

        //     return users;
        // }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutUsers(int id, Users users)
        // {
        //     if (id != users.Id)
        //     {
        //         return BadRequest();
        //     }

        //     _context.Entry(users).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!UsersExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }

        //     return NoContent();
        // }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Inscription")]
        public async Task<ActionResult<Users>> PostUsers(Users users)
        {
            
            try
            {
                await _context.AddUsersAsync(users);

                return Ok("Votre inscription a bien été pris en compte");
            }
            catch (System.Exception ex)
            {
                
                throw new Exception($"Erreur lors de l'ajout de l'utilisateur {ex.Message}");
            }
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetUsers", new { id = users.Id }, users);
        }

        [HttpPost("Connexion")]
        public async Task<ActionResult<Models.UsersConnexion>> ConnexionUser(UsersConnexion usersConnexion)
        {
            try
            {
                await _context.ConnexionUser(usersConnexion);
                return usersConnexion;
            }
            catch (System.Exception ex)
            {
                
                throw new Exception($"Une erreur est survenue lors de la connexion {ex.Message}");
            }
        }

        // DELETE: api/Users/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteUsers(int id)
        // {
        //     var users = await _context.Users.FindAsync(id);
        //     if (users == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.Users.Remove(users);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        // private bool UsersExists(int id)
        // {
        //     return _context.Users.Any(e => e.Id == id);
        // }
    }
}
