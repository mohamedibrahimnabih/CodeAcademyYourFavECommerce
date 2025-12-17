namespace YourFavECommerce.ViewModels
{
    public record CreateUserVM(string name, string userName, string email, string phoneNumber, string password, string ConfirmPassword, bool EmailConfirmation, List<string> Roles);
}
