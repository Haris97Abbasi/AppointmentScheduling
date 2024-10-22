using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppointmentScheduling.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage ="The password must be atleast 6 characters long.")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage ="The passwords don't match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        [DisplayName("Role Name")]
        public string RoleName { get; set; }
    }
}
