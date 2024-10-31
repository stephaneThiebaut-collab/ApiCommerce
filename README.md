# API COMMERCE

- Inscription user

```bash
/api/Users/Inscription
```

- L'inscription de l'utilisateur ce fait avec le model suivant 

> [!Schema]
> public class Users
> {
>    public int Id {get; set;}
>    public string uuid {get; set;} = string.Empty;
>    [Required]
>    public string name {get; set;} = string.Empty;
>    [Required]
>    public string last_name {get; set;} = string.Empty;
>    [Required]
>    public string email {get; set;} = string.Empty;
>    [Required]
>    public string password {get; set;} = string.Empty;
>}

- l'Id est auto incrementé et gere par la base de donnée Mysql l'uuid est automatique crée par `Guid` est a la contrainte dans la base de donnée d'etre unique,
si un uuid identique survient lors de l'enregistrement d'un utilisateur une erreur sera renvoyer par la base de donnée et toute operation sera abandonnée

- le name, last_name, email, password sont des parametre obligatoire si l'un d'eux manque une erreur sera renvoyer et toute operation abandonnée, l'email est unique
si un email identique deja enregistre sur la base de donnée survient lors d'un enregistrement un message d'erreur sera alors retournée 

- le password est haché par la technologie BCrypt qui encrypte le mot de passe de maniere unique avec aucune clef decryptage connue pour le moment

>[!Requete]
>    command.Parameters.AddWithValue("@uuid", uuid);
>    command.Parameters.AddWithValue("@name", users.name);
>    command.Parameters.AddWithValue("@last_name", users.last_name);
>    command.Parameters.AddWithValue("@email", users.email);
>    command.Parameters.AddWithValue("@password", passwordHash);
>   command.CommandText = @"INSERT INTO `users` (`uuid`, `name`, `last_name`, `email`, `password`) VALUES (@uuid, @name, @last_name, @email, @password);";

- Les parametre de la requete Sql dans les `command.Parameters.AddWithValue()` sont preparé avant la requete pour evite toute eventuelle injection Sql
