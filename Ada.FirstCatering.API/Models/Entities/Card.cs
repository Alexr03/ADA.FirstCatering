using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ada.FirstCatering.API.Models.Entities;

[Table("fc_cards")]
public class Card
{
    [Key]
    [Column("id", TypeName = "varchar(16)")]
    public string Id { get; set; }
    
    [Column("pin", TypeName = "int(4)")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public int Pin { get; set; }

    [Column("balance", TypeName = "decimal(10,2)")]
    public decimal Balance { get; set; } = new(0.00);
    
    [Column("employee_id", TypeName = "int(11)")]
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}