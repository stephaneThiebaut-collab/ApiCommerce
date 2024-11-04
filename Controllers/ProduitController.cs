using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace ApiCommerce.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]/")]
    [ApiController]
    public class ProduitController: ControllerBase
    {
        private readonly ProduitContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProduitController(ProduitContext context, IHttpContextAccessor httpContextAccessor, TokenController tokenController)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        
        [Authorize]
        [HttpPost("Create-Produit")]
        public async Task<ActionResult<Produit>> CreateProduit(Produit produit)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    return Unauthorized("Vous devez être connecté pour effectuer cette action");
                }
                else
                {
                    await _context.AddProduitDb(produit);
                    return Ok("Votre produit a bien été enregistré");
                }
            }
            catch (System.Exception ex)
            {
                throw new Exception($"Une erreur est survenue {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("Update-produit/{id}")]
        public async Task<ActionResult<Produit>> UpdateProduit(Guid id, [FromBody] Produit produit)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    return Unauthorized("Vous devez être connecté pour effectuer cette action");
                }

                if (ModelState.IsValid)
                {
                    await _context.UpdateProduit(id, produit);
                    
                    var response = new { success = true, message = "Produit modifié" };
                    return Ok(response);
                }
                else
                {
                    return Unauthorized("Les informations entrée sont invalid ou certaines information sont manquante veuillez verifié est réesseyer");
                }
            }
            catch (System.Exception ex)
            {
                
                throw new Exception($"Une erreur est survenue {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("User/All-Produit-User")]
        public async Task<ActionResult<IEnumerable<InforProduit>>> GetProduitUser()
        {
            try
            {
                return await _context.getOneProduitByUser();
            }
            catch (System.Exception)
            {
                
                var err = new { success = false, message = "Un utilisateur existe deja avec cette email" };
                return Unauthorized(err);
            }
        }

        [HttpGet("All-Produit")]
        public async Task<ActionResult<IEnumerable<InforProduit>>> GetAllProduit()
        {
            try
            {
                return await _context.getAllProduit();
            }
            catch (System.Exception ex)
            {
                
                throw new Exception($"Une erreur est survenue {ex.Message}");
            }
        }
        [Authorize]
        [HttpDelete("Delete-Product/{uuid}")]
        public async Task<IActionResult> DeleteProduct(string uuid)
        {
            try
            {
                if (uuid == null)
                {
                    return NotFound();
                }
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    
                    return Unauthorized("Vous devez être connecté pour effectuer cette action");
                }
                await _context.DeleteProduct(uuid);
                var response = new { success = true, message = "Produit modifié" };
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                
                throw new Exception($"Une erreur est survenue {ex.Message}");
            }
        }
    }
}