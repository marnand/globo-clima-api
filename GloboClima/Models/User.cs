using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace GloboClima.Models;

[DynamoDBTable("Users")]
public class User
{
    [DynamoDBHashKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [DynamoDBProperty]
    [Required]
    public string Username { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [DynamoDBProperty]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}