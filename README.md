# API COMMERCE

## Inscription de l'utilisateur

- **Endpoint** : ``` /api/Users/Inscription ```

- **Description** : Ce point d'entrée permet l'inscription des utilisateurs en utilisant le modèle suivant :

> [!Schema]
```
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
```

**Détails :**

- **Id** : Généré automatiquement et incrémenté par la base de données MySQL.

- **UUID** : Généré automatiquement avec Guid et stocké de manière unique. En cas de duplication, la base de données retourne une erreur et l’opération d’enregistrement est annulée.

- **Champs obligatoires** : `name`, `last_name`, `email`, et `password`. Si l’un de ces champs est manquant, une erreur est renvoyée, et l’opération est abandonnée. L’email doit également être unique : si un email identique existe déjà, un message d’erreur est renvoyé

- **Mot de passe** : Le mot de passe est sécurisé avec l’algorithme de hachage BCrypt, garantissant qu'il est chiffré de manière unique sans clé de décryptage connue.

#### Requête SQL d'insertion

>[!Requete]

```
command.Parameters.AddWithValue("@uuid", uuid);
command.Parameters.AddWithValue("@name", users.name);
command.Parameters.AddWithValue("@last_name", users.last_name);
command.Parameters.AddWithValue("@email", users.email);
command.Parameters.AddWithValue("@password", passwordHash);
command.CommandText = @"INSERT INTO `users` (`uuid`, `name`, `last_name`, `email`, `password`) VALUES (@uuid, @name, @last_name, @email, @password);";

```
Les paramètres sont préparés avant la requête pour éviter toute injection SQL.
---

## Connexion de l'utilisateur


- **Endpoint** : ``` /api/Users/Connexion ```

- **Description** : Ce point d'entrée permet la connexion de l'utilisateur en utilisant le schéma suivant :

> [!Schema]

```
public class Users
{
    [Required]
    public string email { get; set; } = string.Empty;
    [Required]
    public string password { get; set; } = string.Empty;
}
```
**Détails** :

- **Champs obligatoires** : `email` et `password`. Si l'un de ces champs est manquant, une erreur est renvoyée.

- **Recherche de l'utilisateur** : Les informations sont récupérées dans la base de données avec la requête SQL suivante :

>[!RequeteSql]

```
command.Parameters.AddWithValue("@email", usersConnexions.email);
command.CommandText = @"SELECT `email`, `password` FROM `users` WHERE `email` = @email";
```

- **Prévention des injections SQL** : La requête utilise des paramètres préparés pour éviter les injections SQL.

>[!VérificationUtilisateur]

```
if (DatausersConnexions.Count > 0)

```
Si l'utilisateur est trouvé dans la base de données, le mot de passe est vérifié. Sinon, un message d’erreur est renvoyé.
---

>[!ValidationMotDePasse]
```
bool verifyPassword = BC.Verify(usersConnexions.password, passwordUser);
if (verifyPassword)
```
Si le mot de passe haché correspond à celui fourni par l’utilisateur, la connexion est acceptée. Sinon, un message d’erreur est renvoyé.
---

## Génération de Token JWT

Lors de la connexion réussie de l’utilisateur, un token JWT est généré pour authentifier les futures requêtes de l’utilisateur.

```
public string GenerateToken(string username)
```