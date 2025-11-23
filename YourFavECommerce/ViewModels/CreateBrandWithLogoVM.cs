namespace YourFavECommerce.ViewModels
{
    public class CreateBrandWithLogoVM
    {
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
        public IFormFile Logo { get; set; } = default!;
    }
}
