namespace Order.API.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string Barcode { get; set; }
		public string Description { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }

		public void DecreaseQuantity(int amount)
		{
			Quantity -= amount;
		}
	}
}
