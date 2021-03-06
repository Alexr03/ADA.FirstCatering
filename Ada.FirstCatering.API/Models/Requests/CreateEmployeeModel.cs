using System.ComponentModel.DataAnnotations;

namespace Ada.FirstCatering.API.Models.Requests;

public class CreateEmployeeModel
{
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string PhoneNumber { get; set; }
}