using System.ComponentModel.DataAnnotations;

namespace YourFavECommerce.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string EmailOrUserName { get; set; }
        [DataType(DataType.Password), Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
