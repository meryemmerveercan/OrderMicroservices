using MassTransit;
using Shared;

namespace Notification.API.Consumers
{
	public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
	{
		public Task Consume(ConsumeContext<OrderCreatedEvent> context)
		{
			// Create a string array with the lines of text
			string customerName = context.Message.CustomerName;
			string customerAddressTitle = context.Message.CustomerAddress;
			string orderId = context.Message.OrderId.ToString();
			string email = context.Message.CustomerEmail;

			string message = "\n" + "To: " + email + " - " + "Dear " + customerName + ", " + "your order with number "
				+ orderId + " is created for your address " + customerAddressTitle;

			// Set a variable to the Documents path.
			string docPath =
			  Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			File.AppendAllText(docPath + "\\notifications.txt", message);

			return Task.CompletedTask;
		}
	}
}
