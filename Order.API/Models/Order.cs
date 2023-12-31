namespace Order.API.Models
{
	public class Order
	{
		public int Id { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int CustomerAddressId { get; set; }
        public CustomerAddress CustomerAddress { get; set; }
        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
	}
}
