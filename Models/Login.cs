#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace costumer.Models;

[NotMapped]
public class Login
{
    
    [Required] 
    [Display(Name ="Email")]   
    public string LoginEmail { get; set; }     
}