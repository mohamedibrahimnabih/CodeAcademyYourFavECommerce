using System.ComponentModel.DataAnnotations;

namespace YourFavECommerce.ViewModels
{
    public class ResendEmailConfirmationVM
    {
        [Required]
        public string EmailOrUserName { get; set; }
    }
}
