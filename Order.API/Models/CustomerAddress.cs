﻿namespace Order.API.Models
{
	public class CustomerAddress
	{
		public int Id { get; set; }	
		public string Title { get; set; }
		public string Street { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
