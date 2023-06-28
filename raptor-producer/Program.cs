using System.Text;
using RabbitMQ.Client;


class Order
{
    public float Price { get; set; }
    public int Id { get; set; }
    public string CustomerEmail { get; set; }
}


class Program {
    static void Main(string[] args) {

        if (args.Length < 2) {
            Console.WriteLine("Usage: dotnet run <price> <email>");
            return;
        }
        var price = float.Parse(args[0]);
        var email = args[1];

        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "raptor",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

        var rnd = new Random();
        var order = new Order { Price = price, Id = rnd.Next(0, 100000), CustomerEmail = email };
        var message = System.Text.Json.JsonSerializer.Serialize(order);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: string.Empty,
                            routingKey: "raptor",
                            basicProperties: null,
                            body: body);
        Console.WriteLine($" [x] Sent {message}");

        Console.WriteLine(" Press [enter] to exit.");
    }
}