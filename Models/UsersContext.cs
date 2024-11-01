using ApiCommerce.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;
using System.Data.Common;
using BC = BCrypt.Net.BCrypt;
using ApiCommerce.Controllers;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TodoApi.Models;

public class UsersContext : DbContext
{
    private string connectionString = "Server=localhost; User ID=admin; Password=753159852456; Database=DBCommerce";
    private protected string keyToken ="MIICWwIBAAKBgHZO8IQouqjDyY47ZDGdw9jPDVHadgfT1kP3igz5xamdVaYPHaN24UZMeSXjW9sWZzwFVbhOAGrjR0MM6APrlvv5mpy67S/K4q4D7Dvf6QySKFzwMZ99Qk10fK8tLoUlHG3qfk9+85LhL/Rnmd9FD7nz8+cYXFmz5LIaLEQATdyNAgMBAAECgYA9ng2Md34IKbiPGIWthcKb5/LC/+nbV8xPp9xBt9Dn7ybNjy/blC3uJCQwxIJxz/BChXDIxe9XvDnARTeN2yTOKrV6mUfI+VmON5gTD5hMGtWmxEsmTfu3JL0LjDe8Rfdu46w5qjX5jyDwU0ygJPqXJPRmHOQW0WN8oLIaDBxIQQJBAN66qMS2GtcgTqECjnZuuP+qrTKL4JzG+yLLNoyWJbMlF0/HatsmrFq/CkYwA806OTmCkUSm9x6mpX1wHKi4jbECQQCH+yVb67gdghmoNhc5vLgnm/efNnhUh7u07OCL3tE9EBbxZFRs17HftfEcfmtOtoyTBpf9jrOvaGjYxmxXWSedAkByZrHVCCxVHxUEAoomLsz7FTGM6ufd3x6TSomkQGLw1zZYFfe+xOh2W/XtAzCQsz09WuE+v/viVHpgKbuutcyhAkB8o8hXnBVz/rdTxti9FG1b6QstBXmASbXVHbaonkD+DoxpEMSNy5t/6b4qlvn2+T6a2VVhlXbAFhzcbewKmG7FAkEAs8z4Y1uI0Bf6ge4foXZ/2B9/pJpODnp2cbQjHomnXM861B/C+jPW3TJJN2cfbAxhCQT2NhzewaqoYzy7dpYsIQ==";
    public UsersContext(DbContextOptions<UsersContext> options)
        : base(options)
    {
        //using var connection = new MySqlConnection("Default");
    }
    public DbSet<Users> users { get; set; } = null!;
    public DbSet<UsersConnexion> usersConnexions {get; set;} = null!;

    internal async Task<IReadOnlyList<UsersConnexion>> ConnexionUser(UsersConnexion usersConnexions)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();

            command.Parameters.AddWithValue("@email", usersConnexions.email);
            command.CommandText = @"SELECT `email`, `password` FROM `users` WHERE `email` = @email";
            await using var reader = await command.ExecuteReaderAsync();

            var DatausersConnexions = new List<UsersConnexion>();
            while (await reader.ReadAsync())
            {
                var ResultemailUser = reader.GetValue(0).ToString();
                var ResultPasswordUser = reader.GetValue(1).ToString();

                if (ResultemailUser != null && ResultPasswordUser != null)
                {
                    var userConnexion = new UsersConnexion
                    {
                        email = ResultemailUser,
                        password = ResultPasswordUser
                    };
                    DatausersConnexions.Add(userConnexion);
                }
            }

            if (DatausersConnexions.Count > 0)
            {
                Console.WriteLine(DatausersConnexions);
                string passwordUser = "";
                foreach (var data in DatausersConnexions)
                {
                    passwordUser = data.password;
                }
                
                bool verifyPassword = BC.Verify(usersConnexions.password, passwordUser);
                if (verifyPassword)
                {
                    Console.WriteLine("OK");
                    Console.WriteLine(GenerateToken("stephane").ToString());
                }
                else 
                {
                    Console.WriteLine("Password incorrect");
                }
            }
            else 
            {
                Console.WriteLine("Utilisateur incoonue");
            }
            return DatausersConnexions;
            
        }
        catch (System.Exception ex)
        {
            throw new Exception($"Une erreur est survenue lors de l'autentification {ex.Message}");
        }
    }
    internal async Task AddUsersAsync(Users users)
    {
        
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();

            Guid uuid = Guid.NewGuid();
            string passwordHash = BC.HashPassword(users.password);

            command.Parameters.AddWithValue("@uuid", uuid);
            command.Parameters.AddWithValue("@name", users.name);
            command.Parameters.AddWithValue("@last_name", users.last_name);
            command.Parameters.AddWithValue("@email", users.email);
            command.Parameters.AddWithValue("@password", passwordHash);
            
            command.CommandText = @"INSERT INTO `users` (`uuid`, `name`, `last_name`, `email`, `password`) VALUES (@uuid, @name, @last_name, @email, @password);";

            await command.ExecuteNonQueryAsync();
            
        }
        catch (System.Exception ex)
        {
            
            throw new Exception($"Erreur lors de l'ajout de l'utilisateur {ex.Message}");
        }
        
    }

    internal Task<ActionResult<IEnumerable<Users>>> ToListAsync()
    {
        try
        {
            List<Users> users = new List<Users>
            {
                new Users { Id = 1, uuid = "fdgvhcjxwlvbdfb", name = "Stephane", last_name = "Thiebaut", email = "stephanethiebautjob@gmail.com" }
            };
            return Task.FromResult<ActionResult<IEnumerable<Users>>>(users);
        }
        catch (System.Exception)
        {
            throw new NotImplementedException();
        }
    }

    public string GenerateToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyToken));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
}