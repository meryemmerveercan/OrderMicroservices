using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;

namespace Order.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CustomerController : ControllerBase
	{
		private readonly AppDbContext _context;

		public CustomerController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet("customer-list")]
		public List<Customer> GetCustomers()
		{
			return _context.Customers
						   .Include(x => x.CustomerAddresses)
						   .ToList();
		}

		[HttpGet("customer-address-list")]
		public List<CustomerAddress> GetCustomerAddresses()
		{
			return _context.CustomerAddresses
						   .Include(x => x.Customer)
						   .ToList();
		}
	}
}
