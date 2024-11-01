using System.ComponentModel.DataAnnotations;

namespace ApiCommerce.Models;
public class UsersConnexion
{
    [Required]
    public string email {get; set;} = string.Empty;
    [Required]
    public string password {get; set;} = string.Empty;
}