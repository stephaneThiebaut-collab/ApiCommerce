# API COMMERCE

## Inscription de l'utilisateur

- **Endpoint** : ``` /api/Users/Inscription ```

- **Description** : Ce point d'entrée permet l'inscription des utilisateurs en utilisant le modèle suivant :

> [!Schema]
```cs
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

```cs
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

```cs
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

```cs
command.Parameters.AddWithValue("@email", usersConnexions.email);
command.CommandText = @"SELECT `email`, `password` FROM `users` WHERE `email` = @email";
```

- **Prévention des injections SQL** : La requête utilise des paramètres préparés pour éviter les injections SQL.

>[!VérificationUtilisateur]

```cs
if (DatausersConnexions.Count > 0)

```
Si l'utilisateur est trouvé dans la base de données, le mot de passe est vérifié. Sinon, un message d’erreur est renvoyé.
---

>[!ValidationMotDePasse]

```cs
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

## Base de donnée

### Users

```sql
+-----------+--------------+------+-----+---------+----------------+
| Field     | Type         | Null | Key | Default | Extra          |
+-----------+--------------+------+-----+---------+----------------+
| id        | int          | NO   | PRI | NULL    | auto_increment |
| uuid      | varchar(150) | NO   | UNI | NULL    |                |
| name      | varchar(50)  | NO   |     | NULL    |                |
| last_name | varchar(50)  | NO   |     | NULL    |                |
| email     | varchar(100) | NO   | UNI | NULL    |                |
| password  | varchar(150) | NO   |     | NULL    |                |
+-----------+--------------+------+-----+---------+----------------+
```
>[!Expliquation]

- **Clé Primaire** : Le champ **id** est la clé primaire, garantissant que chaque enregistrement d'utilisateur est unique. Il est configuré avec **auto_increment** pour que la valeur soit générée automatiquement.

- **Unicité** : Les champs **uuid** et **email** sont uniques, ce qui empêche les doublons dans ces colonnes.

- **Nullabilité** : Aucun champ ne permet de valeurs NULL, ce qui signifie que toutes les colonnes doivent être renseignées lors de l'ajout d'un nouvel utilisateur.
---

### Produits

```sql
+---------------+---------------+------+-----+---------+----------------+
| Field         | Type          | Null | Key | Default | Extra          |
+---------------+---------------+------+-----+---------+----------------+
| id            | int           | NO   | PRI | NULL    | auto_increment |
| uuid          | varchar(150)  | NO   | UNI | NULL    |                |
| uuid_user     | varchar(150)  | NO   | MUL | NULL    |                |
| name_produit  | varchar(50)   | NO   |     | NULL    |                |
| total_produit | int           | NO   |     | NULL    |                |
| price_produit | decimal(10,2) | NO   |     | NULL    |                |
| description   | varchar(255)  | NO   |     | NULL    |                |
+---------------+---------------+------+-----+---------+----------------+
```

- **Clé Primaire** : Le champ **id** est la clé primaire de la table, assurant que chaque enregistrement de produit est unique. Ce champ est configuré avec auto_increment pour générer automatiquement un nouvel identifiant.

- **Unicité** : Le champ **uuid** est unique, empêchant ainsi des valeurs dupliquées dans cette colonne

- **Index Multiple** : Le champ **uuid_user** est indexé en tant que clé multiple **(MUL)**, ce qui peut être utile pour les opérations de recherche rapide lorsque cette table est associée à une table d'utilisateurs.

- **Nullabilité** : Aucun champ ne permet les valeurs **NULL**, ce qui signifie que toutes les colonnes doivent être renseignées lors de l'ajout d'un nouveau produit.


```Sql
ALTER TABLE Produit
ADD FOREIGN KEY (uuid_user) REFERENCES users(uuid) ON DELETE CASCADE;
```

- **ALTER TABLE Produit** : Cette partie de la requête indique que l'on souhaite modifier la structure de la table Produit.

- **ADD FOREIGN KEY (uuid_user)** : Cette commande ajoute une contrainte de clé étrangère sur la colonne **uuid_user** de la table Produit. Cela signifie que la valeur de **uuid_user** dans Produit doit correspondre à une valeur existante dans la colonne uuid de la table users.

- **REFERENCES users(uuid)** : Cela définit la référence de la clé étrangère. Ici, la colonne uuid_user de Produit est liée à la colonne **uuid** de la table **users**. En d'autres termes, chaque valeur de **uuid_user** dans Produit doit avoir une correspondance dans **users(uuid)**

- **ON DELETE CASCADE** : Cette option spécifie le comportement lors de la suppression d'un enregistrement dans la table users. Avec **ON DELETE CASCADE**, si un utilisateur est supprimé de la table users, tous les produits associés à cet utilisateur (ayant la même valeur **uuid_user**) seront également automatiquement supprimés de la table Produit. Cela permet de maintenir l'intégrité des données en supprimant les enregistrements orphelins.
---

# Produits

## Ajout d'un Produit à la Base de Données

- **EndPoint** : ``` api/Produit/Create-Produit ```

>[!Schema]

```cs
public class Produit
{
    public string Name_produit {get; set;} = string.Empty;
    public int Total_produit {get; set;}
    public decimal Price_produit {get; set;}
    public string description {get; set;} = string.Empty;
}
```
***Paramètres***

1. **produit**:  Un objet de type Produit qui contient les détails du produit à ajouter, notamment :
    - **Name_produit**: Le nom du produit.
    - **Total_produit**: La quantité disponible du produit.
    - **Price_produit**: Le prix du produit.
    - **Description**: Une description du produit.

### Processus

1. Récupération du Token d'Autorisation
La méthode commence par récupérer le token JWT à partir des en-têtes de la requête HTTP.

```cs
var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
```

2. Vérification du Token

Si le token est présent, la méthode vérifie qu'il est au format correct et commence par le préfixe "Bearer ". Si le token est absent ou mal formé, une exception est levée.

```cs 
if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
{
    throw new Exception("Vous devez être connecté pour effectuer cette action");
}
```
3. Décodage du Token

Le token est décodé pour extraire les informations d'identification de l'utilisateur, notamment son UUID.

```cs
var claims = _tokenController.DecodeToken(token.ToString().Split(" ")[1]);
```

4. Vérification de l'Utilisateur

La méthode vérifie si l'UUID de l'utilisateur est présent dans les claims. Si l'utilisateur est identifié, une connexion à la base de données est établie.

```cs
if (claims.TryGetValue(JwtRegisteredClaimNames.Sub, out var userId))
{
    using var connection = new MySqlConnection(connectionString);
    await connection.OpenAsync();
}
```

5. Vérification de l'Existence de l'Utilisateur

Un **SELECT** est exécuté pour vérifier si l'utilisateur existe dans la table des utilisateurs.

```cs
command.Parameters.AddWithValue("@uuid", userId);
command.CommandText = @"SELECT * FROM `users` WHERE `uuid` = @uuid";
await using var reader = await command.ExecuteReaderAsync();
```

6. Ajout du Produit

Si l'utilisateur est trouvé, un nouvel UUID est généré pour le produit, et un INSERT est exécuté pour ajouter le produit dans la base de données.

```cs
if (reader.HasRows)
{
    var uuidUser = "";

    while (await reader.ReadAsync())
    {
        uuidUser = reader["uuid"].ToString();
    }
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
}
```

7. Gestion des Exceptions

Si une exception se produit à n'importe quel moment, celle-ci est capturée et une nouvelle exception est levée avec un message explicite.

```cs
catch (System.Exception ex)
{
    throw new Exception($"Une erreur est survenue {ex.Message}");
}
```
---

## Méthode: `UpdateProduit`

Cette méthode permet de mettre à jour les détails d'un produit spécifique dans la base de données.

```csharp
public async Task<bool> UpdateProduit(Guid id, Produit produit)
```

**Paramètres**

- **id**: Un **Guid** représentant l'UUID du produit à mettre à jour.
- **produit**: Un objet de type **Produit** contenant les nouvelles valeurs pour les propriétés **Name_produit**, **Total_produit**, **Price_produit**, et **Description**.

**Processus**

- Récupération du Token d'Autorisation
Comme pour l'ajout d'un produit, le token est récupéré et vérifié.

- Vérification de l'Utilisateur
Un **SELECT** est effectué pour vérifier si l'utilisateur est autorisé à modifier le produit en question. Si l'utilisateur n'est pas le propriétaire du produit, une exception est levée.

- Mise à jour du Produit
Si l'utilisateur est autorisé, la méthode exécute une requête **UPDATE** pour mettre à jour les informations du produit dans la base de données.

**Remarques**

La méthode retourne true si la mise à jour a été effectuée avec succès, sinon elle lance une exception.

## Méthode: ```getAllProduit```

Cette méthode permet de récupérer tous les produits de la base de données.

```cs 
public async Task<ActionResult<IEnumerable<InforProduit>>> getAllProduit()
```

**Processus**

- Connexion à la Base de Données
Une connexion à la base de données est établie pour exécuter une requête.

- Récupération des Produits
La méthode exécute une requête **SELECT** pour obtenir tous les produits.

- Création de la Liste de Produits
Chaque produit récupéré est ajouté à une liste d'objets **InforProduit**, qui est ensuite retournée.

## Méthode: ```DeleteProduct```

Cette méthode permet de supprimer un produit de la base de données.

```cs
public async Task<bool> DeleteProduct(string uuid)
```

**Paramètres**
- **uuid**: Un string représentant l'**UUID** du produit à supprimer.

**Processus**

- Récupération du Token d'Autorisation
Comme pour les autres méthodes, la méthode commence par vérifier le token JWT.

- Vérification de l'Utilisateur
Un **SELECT** est effectué pour confirmer que l'utilisateur existe et qu'il est le propriétaire du produit à supprimer.

- Suppression du Produit
Si l'utilisateur est autorisé, la méthode exécute une requête **DELETE** pour retirer le produit de la base de données.

**Remarques**

    La méthode retourne true si la suppression a été effectuée avec succès, sinon elle lance une exception.

**Routes API**

```bash 
api/Produit/Update-produit/:id 
```
**Verbose**: **PUT**

    Met à jour les informations d'un produit existant identifié par :id.

```bash
api/Produit/Delete-Product/:id
```
**Verbose**: **DELETE**
    
Supprime un produit identifié par :id.

```bash 
api/Produit/All-Produit
```
**GET**: Verbose: **GET**
Récupère tous les produits disponibles dans la base de données.