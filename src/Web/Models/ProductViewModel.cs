namespace Web.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string PriceTry => Price.ToString("c2");

        public string PictureUri { get; set; }
    }
}
