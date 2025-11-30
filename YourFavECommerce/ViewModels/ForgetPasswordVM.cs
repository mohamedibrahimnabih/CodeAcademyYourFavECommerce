using System.ComponentModel.DataAnnotations;

namespace YourFavECommerce.ViewModels
{
    public class ForgetPasswordVM
    {
        [Required]
        public string EmailOrUserName { get; set; }
    }
}
