using System.ComponentModel.DataAnnotations;

namespace ApiCommerce.Models;
public class UsersConnexion
{
    [Required]
    public string Email {get; set;} = string.Empty;
    [Required]
    public string Password {get; set;} = string.Empty;

    public string? Uuid {get; set;} = "";
}