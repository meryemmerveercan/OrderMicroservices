using System.ComponentModel.DataAnnotations;

namespace Order.API.Models
{
	public class Customer
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }

		[EmailAddress]
		public string Email { get; set; }
		public ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();
	}
}
