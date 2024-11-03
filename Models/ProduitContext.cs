using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiCommerce.Controllers;
using System.IdentityModel.Tokens.Jwt;
using MySqlConnector;
using ApiCommerce.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

public class ProduitContext: DbContext
{
    private protected string connectionString = "Server=localhost; User ID=admin; Password=753159852456; Database=DBCommerce";
    private readonly TokenController _tokenController;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ProduitContext(DbContextOptions<ProduitContext> options, TokenController tokenController, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _tokenController = tokenController;
        _httpContextAccessor = httpContextAccessor;
    }
    public DbSet<Produit> produits {get; set;} = null!;

    public async Task AddProduitDb(Produit produit)
    {
    try
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            
            if (token != null)
            {
                var claims = _tokenController.DecodeToken(token.ToString().Split(" ")[1]);
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    throw new Exception("Vous devez être connecté pour effectuer cette action");
                }
                if (claims.TryGetValue(JwtRegisteredClaimNames.Sub, out var userId))
                {
                    using var connection = new MySqlConnection(connectionString);
                    await connection.OpenAsync();

                    using var command = connection.CreateCommand();

                    command.Parameters.AddWithValue("@uuid", userId);
                    command.CommandText = @"SELECT * FROM `users` WHERE `uuid` = @uuid";
                    await using var reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        var uuidUser = "";

                        while (await reader.ReadAsync())
                        {
                            uuidUser = reader["uuid"].ToString();
                        }
                        await reader.DisposeAsync();
                        command.Parameters.Clear();

                        Guid uuid = Guid.NewGuid();
                        command.Parameters.AddWithValue("@uuid", uuid);
                        command.Parameters.AddWithValue("@uuid_user", uuidUser);
                        command.Parameters.AddWithValue("@name_produit", produit.Name_produit);
                        command.Parameters.AddWithValue("@total_produit", produit.Total_produit);
                        command.Parameters.AddWithValue("@price_produit", produit.Price_produit);
                        command.Parameters.AddWithValue("@description", produit.description);

                        command.CommandText = @"INSERT INTO Produit(uuid, uuid_user, name_produit, total_produit, price_produit, description) VALUES(@uuid, @uuid_user, @name_produit, @total_produit, @price_produit, @description)";
                        command.ExecuteNonQuery();

                        connection.Close();

                    }
                    else
                    {
                        throw new Exception("Utilisateur inconnue");
                    }
                }
                else 
                {
                    throw new Exception("Vous devez être connecter pour effectuer cette action");
                }
            }
        }
        catch (System.Exception ex)
        {
            throw new Exception($"Une erreur est survenue {ex.Message}");
        }
    }

    public async Task<bool> UpdateProduit(Guid id, Produit produit)
    {
        try
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (token != null)
            {
                var claims = _tokenController.DecodeToken(token.ToString().Split(" ")[1]);
                if (claims.TryGetValue(JwtRegisteredClaimNames.Sub, out var userId))
                {
                
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@userId", userId);

                command.CommandText = @"SELECT DISTINCT p.uuid AS uuidProduit, p.uuid_user, p.name_produit, p.total_produit, p.price_produit,p.description FROM Produit p, users u WHERE p.uuid = @id AND p.uuid_user = @userId;";
                
                await using var reader = await command.ExecuteReaderAsync();
                
                    if (reader.HasRows)
                    {
                        await reader.DisposeAsync();
                        command.Parameters.Clear();

                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@name_produit", produit.Name_produit);
                        command.Parameters.AddWithValue("@total_produit", produit.Total_produit);
                        command.Parameters.AddWithValue("@price_produit", produit.Price_produit);
                        command.Parameters.AddWithValue("@description", produit.description);

                        command.CommandText = @" UPDATE Produit  SET name_produit = @name_produit,  total_produit = @total_produit,  price_produit = @price_produit,  description = @description WHERE uuid = @id";
                        var rowsAsAffected = await command.ExecuteNonQueryAsync();

                        connection.Close();
                        
                        return rowsAsAffected > 0;
                        
                    }
                    else
                    {
                        throw new Exception("Vous ne disposez pas des droits n'hessesaire pour effectuer cette action");
                    }
                }
            }
            else
            {
                throw new Exception("Vous devez être connecter pour effectuer cette action");
            }
        return true;
        }
        catch (System.Exception ex)
        {
            throw new Exception($"Une erreur est survenue {ex.Message}");
        }
    }

    public async Task<ActionResult<IEnumerable<InforProduit>>> getAllProduit()
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();

            command.CommandText = @"SELECT * FROM Produit";

            await using var reader = await command.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                List<InforProduit> inforProduits = new List<InforProduit>();
                var uuidProduit = "";
                var uuid_user = "";
                var name_produit = "";
                int total_produit = 0;
                decimal price_produit = 0.0m;
                var description = "";
                while (await reader.ReadAsync())
                {
                    uuidProduit = reader["uuid"].ToString();
                    uuid_user = reader["uuid_user"].ToString();
                    name_produit = reader["name_produit"].ToString();
                    total_produit = (int)reader["total_produit"];
                    price_produit = (decimal)reader["price_produit"];
                    description = reader["description"].ToString();
                    if (uuidProduit != null && uuid_user != null && name_produit != null && description != null)
                    {
                        var produit = new InforProduit
                        {
                            Uuid = uuidProduit,
                            Uuid_user = uuid_user,
                            Name_produit = name_produit,
                            Total_produit = total_produit,
                            Price_produit = price_produit,
                            description = description
                        };
                        inforProduits.Add(produit);
                    }
                }
                connection.Close();
                return inforProduits;
            }
            else
            {
                connection.Close();
                return new List<InforProduit>();
            }
            
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }

    public async Task<bool> DeleteProduct(string uuid){

        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        if (token != null)
        {
            var claims = _tokenController.DecodeToken(token.ToString().Split(" ")[1]);
            if (claims.TryGetValue(JwtRegisteredClaimNames.Sub, out var userId))
            {
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.Parameters.AddWithValue("@uuid", userId);
                command.CommandText = @"SELECT * FROM users WHERE uuid = @uuid";
                
                await using var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    await reader.DisposeAsync();
                    command.Parameters.Clear();


                    command.Parameters.AddWithValue("@uuid", userId);
                    command.Parameters.AddWithValue("@uuidProduit", uuid);
                    command.CommandText = @"SELECT DISTINCT p.uuid AS uuidProduit, p.uuid_user, p.name_produit, p.total_produit, p.price_produit,p.description FROM Produit p, users u WHERE p.uuid = @uuidProduit AND p.uuid_user = @uuid;";
                    await using var readerVerifUserProduit = await command.ExecuteReaderAsync();
                    if (readerVerifUserProduit.HasRows)
                    {
                        await reader.DisposeAsync();
                        command.Parameters.Clear();

                        command.Parameters.AddWithValue("@uuid", userId);
                        command.Parameters.AddWithValue("@uuidProduit", uuid);

                        command.CommandText = @"DELETE FROM Produit WHERE uuid = @uuidProduit AND uuid_user = @uuid";
                        var rowsAsAffected = command.ExecuteNonQuery();
                        return rowsAsAffected > 0;
                    }
                    else 
                    {
                        throw new Exception("Vous ne disposez pas des droits n'essaire pour effectuer cette action");
                    }
                }
                else
                {
                    throw new Exception("Utilisateur inconnue");
                }
            }
            else
            {
                throw new Exception("Vous devez être connecter pour effectuer cette action");
            }
        }
        else 
        {
            throw new Exception("Vous devez être connecter pour effectuer cette action");
        }
    }

}