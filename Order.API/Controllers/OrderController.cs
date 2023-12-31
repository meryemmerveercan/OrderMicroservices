using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.API.DTOs;
using Order.API.Models;
using Order.API.Redis;
using Shared;
using System.Data;
using System.Text.Json;

namespace Order.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IPublishEndpoint _publishEndpoint;
		private readonly ICacheService _cacheService;
		private readonly ILogger<OrderController> _logger;	

		public OrderController(AppDbContext context, IPublishEndpoint publishEndpoint, ICacheService cacheService, ILogger<OrderController> logger)
		{
			_context = context;
			_publishEndpoint = publishEndpoint;
			_cacheService = cacheService;
			_logger = logger;	
		}

		[HttpPost]
		public async Task<IActionResult> Create(OrderDto orderCreate)
		{
			Customer customer = _context.Customers.Where(x => x.Id == orderCreate.CustomerId).FirstOrDefault()!;

			if (customer == null)
			{
				return NotFound("Customer not found, please check the customer list!");
			}
			CustomerAddress customerAddress = _context.CustomerAddresses.Where(x => x.Id == orderCreate.CustomerAddressId).FirstOrDefault()!;

			if (customerAddress == null)
			{
				return NotFound("Customer address not found, please check the customer address list!");
			}

			var newOrder = new Models.Order()
			{
				CustomerId = orderCreate.CustomerId,
				CustomerAddressId = orderCreate.CustomerAddressId,
				CreatedDate = DateTime.UtcNow,
			};

			orderCreate.OrderProducts.ForEach(item =>
			{
				Product product = _context.Products.Where(x => x.Id == item.ProductId).FirstOrDefault()!;

				if (product == null)
				{
					throw new ArgumentException("Product not found, please check the product list!");
				}

				if (product.Quantity < item.Quantity)
				{
					_logger.LogInformation("Product not available, please check the product list!");

					throw new ArgumentException("Product not available, please check the product list!");
				}

				newOrder.OrderProducts.Add(new OrderProduct()
				{
					ProductId = item.ProductId,
					Quantity = item.Quantity,
				});

				product.DecreaseQuantity(item.Quantity);
			});

			await _context.AddAsync(newOrder);
			await _context.SaveChangesAsync();

			Models.Order createdOrder = _context.Orders.Where(x => x.Id == newOrder.Id)
														.Include(x => x.Customer)
														.Include(x => x.CustomerAddress)
														.FirstOrDefault()!;

			var orderCreatedEvent = new OrderCreatedEvent()
			{
				OrderId = createdOrder.Id,
				CustomerName = createdOrder.Customer.Name,
				CustomerAddress = createdOrder.CustomerAddress.Title,
				CustomerEmail = createdOrder.Customer.Email
			};

			await _publishEndpoint.Publish(orderCreatedEvent);
			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			Models.Order order = _context.Orders.Where(x => x.Id == id).FirstOrDefault()!;

			if (order == null)
			{
				return NotFound("Order not found!");
			}

			_context.Orders.Remove(order);
			await _context.SaveChangesAsync();
			return Ok();
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, OrderDto orderUpdate)
		{
			Models.Order orderRecord = _context.Orders.Where(x => x.Id == id).Include(x => x.OrderProducts).FirstOrDefault()!;

			if (orderRecord == null)
			{
				return NotFound("Order not found, please check the order list!");
			}
			Customer customer = _context.Customers.Where(x => x.Id == orderUpdate.CustomerId).FirstOrDefault()!;

			if (customer == null)
			{
				return NotFound("Customer not found, please check the customer list!");
			}
			CustomerAddress customerAddress = _context.CustomerAddresses.Where(x => x.Id == orderUpdate.CustomerAddressId).FirstOrDefault()!;

			if (customerAddress == null)
			{
				return NotFound("Customer address not found, please check the customer address list!");
			}

			orderRecord.CustomerAddressId = orderUpdate.CustomerAddressId;
			orderRecord.OrderProducts.Clear();
			await _context.SaveChangesAsync();

			orderUpdate.OrderProducts.ForEach(item =>
			{
				Product product = _context.Products.Where(x => x.Id == item.ProductId).FirstOrDefault()!;

				if (product == null)
				{
					throw new ArgumentException("Product not found, please check the product list!");
				}

				if (product.Quantity < item.Quantity)
				{
					throw new ArgumentException("Product not available, please check the product list!");
				}

				orderRecord.OrderProducts.Add(new OrderProduct()
				{
					ProductId = item.ProductId,
					Quantity = item.Quantity,
				});

				product.DecreaseQuantity(item.Quantity);
			});

			await _context.SaveChangesAsync();

			return Ok();
		}

		[HttpGet("order-list")]
		public List<Models.Order> GetOrders()
		{
			return _context.Orders
						   .Include(x => x.Customer)
						   .Include(x => x.CustomerAddress)
						   .ToList();
		}

		[HttpGet("product-list")]
		public async Task<List<Product>> GetProducts()
		{
			List<Product> productList;

			var productListCache = _cacheService.GetValueAsync("productList").Result;
			if (productListCache == null)
			{
				productList = _context.Products.ToList();
				var json = JsonSerializer.Serialize(productList);

				await _cacheService.SetValueAsync("productList", json);
			}
			else
			{
				productList = JsonSerializer.Deserialize<List<Product>>(_cacheService.GetValueAsync("productList").Result.ToString());
			}
			return productList;
		}
	}
}
