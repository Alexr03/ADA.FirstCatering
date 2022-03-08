using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ada.FirstCatering.API.Models;

[Table("fc_employees")]
public class Employee
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("first_name", TypeName = "varchar(50)")]
    public string FirstName { get; set; }
    
    [Column("last_name", TypeName = "varchar(50)")]
    public string LastName { get; set; }
    
    [Column("email", TypeName = "varchar(100)")]
    public string Email { get; set; }
    
    [Column("phone", TypeName = "varchar(20)")]
    public string PhoneNumber { get; set; }
    
    [Column("card_id", TypeName = "varchar(16)")]
    public string CardId { get; set; }
    
    [ForeignKey(nameof(CardId))]
    public Card Card { get; set; } = null!;
}