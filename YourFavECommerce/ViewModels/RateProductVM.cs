namespace YourFavECommerce.ViewModels
{
    public class RateProductVM
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        public IFormFile? Img { get; set; }
        public int Rate { get; set; }
    }
}
