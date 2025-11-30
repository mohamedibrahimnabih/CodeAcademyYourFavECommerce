using System.ComponentModel.DataAnnotations;

namespace YourFavECommerce.ViewModels
{
    public class NewPasswordVM
    {
        public string Id { get; set; }
        [DataType(DataType.Password), Length(8, 32)]
        public string Password { get; set; }
        [Compare(nameof(Password)), DataType(DataType.Password), Length(8, 32)]
        public string ConfirmPassword { get; set; }
    }
}
