using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InventoryOrderingSystem.Models
{
    public class RegistrationModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_]{4,20}$", ErrorMessage = "Username must be 4-20 characters and ocntain only letters, numbers or underscore")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DisplayName("First Name")]
        [StringLength(25, ErrorMessage = "Cannot exceed 25 characters")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        [StringLength(25, ErrorMessage = "Cannot exceed 25 characters")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Password Do Not Match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
