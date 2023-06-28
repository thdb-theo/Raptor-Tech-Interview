using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MongoDB.Driver;
using MongoDB.Bson;
using Tests;

class Program {
    static void Main(string[] args) {
        if (args.Length >= 1 && args[0] == "test") {
            Test();
            return;
        }
        MongoClient dbClient = new MongoClient("mongodb://localhost:27017");
        var database = dbClient.GetDatabase("raptor");

        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "raptor",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

        Console.WriteLine(" Waiting for orders");

        var consumer = new EventingBasicConsumer(channel);
        var processor = new Processor(consumer, database);

        channel.BasicConsume(queue: "raptor",
                            autoAck: true,
                            consumer: consumer);
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
        
    }
    static void Test() {
        var testCustomer = new TestCustomer();
        testCustomer.RunTests();
        var testOrder = new TestOrder();
        testOrder.RunTests();
        var testReceipt = new TestReceipt();
        testReceipt.RunTests();
        var testStatus = new TestStatus();
        testStatus.RunTests();
    }
}
