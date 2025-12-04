namespace YourFavECommerce.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string Comment { get; set; }
        public double Rate { get; set; }

        public string? Img { get; set; }
    }
}
