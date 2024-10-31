using ApiCommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;
using System.Data.Common;
using BC = BCrypt.Net.BCrypt;

namespace TodoApi.Models;

public class UsersContext : DbContext
{
    private string connectionString = "Server=localhost; User ID=admin; Password=753159852456; Database=DBCommerce";
    public UsersContext(DbContextOptions<UsersContext> options)
        : base(options)
    {
        using var connection = new MySqlConnection("Default");
    }

    public DbSet<Users> users { get; set; } = null!;

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
        //throw new NotImplementedException();
        
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