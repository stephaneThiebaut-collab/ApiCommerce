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
    public UsersContext(DbContextOptions<UsersContext> options)
        : base(options)
    {
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

            command.Parameters.AddWithValue("@email", usersConnexions.Email);
            command.CommandText = @"SELECT uuid, email, password FROM `users` WHERE `email` = @email";
            await using var reader = await command.ExecuteReaderAsync();

            var DataUsersConnexions = new List<UsersConnexion>();

            while (await reader.ReadAsync())
            {
                var ResultemailUser = reader["email"].ToString();
                var ResultPasswordUser = reader["password"].ToString();
                var ResultUuidUser = reader["uuid"].ToString();
                
                if (ResultemailUser != null && ResultPasswordUser != null && ResultUuidUser != null)
                {
                    var userConnexion = new UsersConnexion
                    {
                        Email = ResultemailUser,
                        Password = ResultPasswordUser,
                        Uuid = ResultUuidUser
                    };
                    DataUsersConnexions.Add(userConnexion);
                }
            }
            
            return DataUsersConnexions;
        }
        catch (System.Exception ex)
        {
            
            throw new Exception($"Une erreur est survenue veuillez réessayer plus tard  {ex.Message}");
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
}