using System.ComponentModel.DataAnnotations;

namespace ApiCommerce.Models;

public class Users
{
    public int Id {get; set;}
    public string uuid {get; set;} = string.Empty;
    [Required]
    public string name {get; set;} = string.Empty;
    [Required]
    public string last_name {get; set;} = string.Empty;
    [Required]
    public string email {get; set;} = string.Empty;
    [Required]
    public string password {get; set;} = string.Empty;
}