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
using Microsoft.AspNetCore.Authorization;
using ApiCommerce.Controllers;
using BC = BCrypt.Net.BCrypt;

namespace ApiCommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersContext _context;
        private readonly TokenController _tokenController;

        public UsersController(UsersContext context, TokenController tokenContext)
        {
            _context = context;
            _tokenController = tokenContext;       
        }

        
        [HttpPost("Inscription")]
        public async Task<ActionResult<Users>> PostUsers(Users users)
        {
            
            try
            {
                await _context.AddUsersAsync(users);
                var response = new { success = true, message = "Inscription réussie" };
                return Ok(response);
            }
            catch (System.Exception)
            {
                var err = new { success = false, message = "Un utilisateur existe deja avec cette email" };
                return Unauthorized(err);
                //throw new Exception($"Erreur lors de l'ajout de l'utilisateur {ex.Message}");
            }
        }

        [HttpPost("Connexion")]
        public async Task<ActionResult<Models.UsersConnexion>> ConnexionUser(UsersConnexion usersConnexion)
        {
            List<ReponseConnexion> reponseConnexions = new List<ReponseConnexion>();
            try
            {
                var dataUsersConnexions = await _context.ConnexionUser(usersConnexion);

                var uuidUser = dataUsersConnexions[0].Uuid;
                if (dataUsersConnexions.Count > 0 && uuidUser != null)
                {
                    string passwordHashUser = dataUsersConnexions[0].Password;
                    string emailUser = dataUsersConnexions[0].Email;

                    bool verifyPassword = BC.Verify(usersConnexion.Password, passwordHashUser);
                    if (verifyPassword)
                    {
                        return Ok(new ReponseConnexion { Message = "Vous etes connecter", Token = _tokenController.GenerateToken(uuidUser).ToString()});
                    }
                    else
                    {
                        return Unauthorized(new ReponseConnexion { Message = "Mot de passe incorrecte", Token = string.Empty});
                    }
                }
                else 
                {
                    return Unauthorized(new ReponseConnexion { Message = "utilisateur inconnue", Token = string.Empty});
                }
            }
            catch (System.Exception ex)
            {
                return Unauthorized(new ReponseConnexion { Message = "Utilisateur inconnue ou mot de passe incorrecte", Token = string.Empty});
                throw new Exception($"Une erreur est survenue lors de la connexion {ex.Message}");
            }
        }
    }
}
