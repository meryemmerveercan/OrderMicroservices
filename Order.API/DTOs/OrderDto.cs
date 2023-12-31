namespace Order.API.DTOs
{
	public class OrderDto
	{
		public int CustomerId { get; set; }
		public int CustomerAddressId { get; set; }
		public List<OrderProductDto> OrderProducts { get; set; }	
	}

	public class OrderProductDto
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
	}

}
